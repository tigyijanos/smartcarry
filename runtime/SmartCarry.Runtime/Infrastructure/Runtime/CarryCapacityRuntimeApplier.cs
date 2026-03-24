using System.Diagnostics;
using System.Runtime.CompilerServices;
using NSMedieval.Components;
using NSMedieval.State;
using SmartCarry.Runtime.Configuration;

namespace SmartCarry.Runtime;

/// <summary>
/// Applies the derived carry capacity to live worker storage instances on demand.
/// </summary>
internal static class CarryCapacityRuntimeApplier
{
    private const double LiveProfileCacheLifetimeSeconds = 0.25d;

    private static readonly ConditionalWeakTable<Storage, BaselineCapacityEntry> BaselineCapacityByStorage = new();
    private static readonly ConditionalWeakTable<HumanoidInstance, LiveProfileCacheEntry> LiveProfileCacheByHumanoid = new();
    private static readonly ConditionalWeakTable<HumanoidInstance, ProfileTraceEntry> TracedProfilesByHumanoid = new();

    [ThreadStatic]
    private static bool isApplying;

    public static bool TryGetCurrentProfile(HumanoidInstance humanoid, out CarryCapacityProfile? profile)
    {
        profile = null;
        if (!IsLiveWorldHumanoid(humanoid))
        {
            DiagnosticTrace.InfoSample("carry.runtime", "TryGetCurrentProfile.NotLiveWorld", $"TryGetCurrentProfile skipped: not live-world humanoid {humanoid}");
            return false;
        }

        if (!TryBuildProfile(humanoid, out var snapshot, out _, out var builtProfile))
        {
            DiagnosticTrace.InfoSample("carry.runtime", "TryGetCurrentProfile.BuildFailed", $"TryGetCurrentProfile build failed for {humanoid}; trying UI fallback");
            if (!TryBuildUiFallbackProfile(humanoid, out snapshot, out _, out builtProfile))
            {
                DiagnosticTrace.InfoSample("carry.runtime", "TryGetCurrentProfile.UiFallbackFailed", $"TryGetCurrentProfile UI fallback failed for {humanoid}");
                return false;
            }
        }

        profile = builtProfile;
        TraceProfile(humanoid, snapshot, builtProfile!, "profile");
        return true;
    }

    public static bool TryGetPreviewProfile(HumanoidInstance humanoid, out CarryCapacityProfile? profile)
    {
        return TryGetPreviewProfile(humanoid, null, null, null, out profile);
    }

    public static bool TryGetPreviewProfile(
        HumanoidInstance humanoid,
        int? ageYearsOverride,
        float? heightOverride,
        float? weightCoefficientOverride,
        out CarryCapacityProfile? profile)
    {
        profile = null;
        if (!TryBuildUiFallbackProfile(humanoid, out var snapshot, out _, out var builtProfile) || builtProfile == null || snapshot == null)
        {
            DiagnosticTrace.InfoSample("carry.runtime", "TryGetPreviewProfile.BuildFailed", $"TryGetPreviewProfile build failed for {humanoid}");
            return false;
        }

        var effectiveSnapshot = ApplyPreviewOverrides(snapshot, ageYearsOverride, heightOverride, weightCoefficientOverride);
        var inputs = CarryCapacityInputsFactory.Create(effectiveSnapshot);
        profile = CarryCapacityCalculator.Calculate(inputs);
        TraceProfile(humanoid, effectiveSnapshot, profile, "preview");
        return true;
    }

    public static bool TryGetEffectiveCapacity(HumanoidInstance humanoid, out int effectiveCapacity)
    {
        effectiveCapacity = 0;
        if (!IsLiveWorldHumanoid(humanoid))
        {
            DiagnosticTrace.InfoSample("carry.runtime", "TryGetEffectiveCapacity.NotLiveWorld", $"TryGetEffectiveCapacity skipped: not live-world humanoid {humanoid}");
            return false;
        }

        if (!TryBuildProfile(humanoid, out var snapshot, out _, out var profile) || profile == null)
        {
            DiagnosticTrace.InfoSample("carry.runtime", "TryGetEffectiveCapacity.BuildFailed", $"TryGetEffectiveCapacity build failed for {humanoid}");
            return false;
        }

        TraceProfile(humanoid, snapshot, profile, "capacity");
        ApplyCapacityIfNeeded(humanoid.Storage, snapshot, profile);
        effectiveCapacity = profile.EffectiveCapacity;
        return true;
    }

    public static bool TryGetFreeSpace(Storage storage, out float freeSpace)
    {
        freeSpace = 0f;
        if (storage == null || storage.HasDisposed)
        {
            DiagnosticTrace.InfoSample("carry.runtime", "TryGetFreeSpace.InvalidStorage", $"TryGetFreeSpace skipped: invalid storage {storage}");
            return false;
        }

        var humanoid = CarryStorageOwnerResolver.Resolve(storage);
        if (humanoid == null || humanoid.HasDisposed)
        {
            DiagnosticTrace.InfoSample("carry.runtime", "TryGetFreeSpace.NoOwner", $"TryGetFreeSpace skipped: no humanoid owner for storage {storage}");
            return false;
        }

        if (!TryBuildProfile(humanoid, storage, out _, out _, out var profile) || profile == null)
        {
            DiagnosticTrace.InfoSample("carry.runtime", "TryGetFreeSpace.BuildFailed", $"TryGetFreeSpace build failed for {humanoid}");
            return false;
        }

        TraceProfile(humanoid, null, profile, "free");
        var currentWeight = storage.GetResourceWeight();
        freeSpace = MathF.Max(0f, profile.EffectiveCapacity - currentWeight);
        return true;
    }

    public static void ApplyIfNeeded(Storage storage)
    {
        if (isApplying ||
            !SmartCarrySettings.EnableDynamicCarryCapacity ||
            storage == null ||
            storage.HasDisposed)
        {
            DiagnosticTrace.InfoSample("carry.runtime", "ApplyIfNeeded.EarlyExit", $"ApplyIfNeeded early exit: applying={isApplying}, enabled={SmartCarrySettings.EnableDynamicCarryCapacity}, storage={storage}, disposed={storage?.HasDisposed}");
            return;
        }

        var humanoid = CarryStorageOwnerResolver.Resolve(storage);
        if (humanoid == null)
        {
            DiagnosticTrace.InfoSample("carry.runtime", "ApplyIfNeeded.NoOwner", $"ApplyIfNeeded skipped: no owner for storage {storage}");
            return;
        }

        if (!TryBuildProfile(humanoid, storage, out var snapshot, out _, out var profile))
        {
            DiagnosticTrace.InfoSample("carry.runtime", "ApplyIfNeeded.BuildFailed", $"ApplyIfNeeded build failed for {humanoid}");
            return;
        }

        ApplyCapacityIfNeeded(storage, snapshot, profile);
    }

    private static bool TryBuildProfile(
        HumanoidInstance humanoid,
        out CarrySignalSnapshot? snapshot,
        out CarryCapacityInputs? inputs,
        out CarryCapacityProfile? profile)
    {
        return TryBuildProfile(humanoid, humanoid?.Storage, out snapshot, out inputs, out profile);
    }

    private static bool TryBuildProfile(
        HumanoidInstance? humanoid,
        Storage? storage,
        out CarrySignalSnapshot? snapshot,
        out CarryCapacityInputs? inputs,
        out CarryCapacityProfile? profile)
    {
        snapshot = null;
        inputs = null;
        profile = null;

        if (!SmartCarrySettings.EnableDynamicCarryCapacity ||
            humanoid == null ||
            humanoid.HasDisposed ||
            storage == null ||
            storage.HasDisposed)
        {
            return false;
        }

        var baselineCapacity = GetBaselineCapacity(storage, hasHumanoidOwner: true);
        if (TryGetCachedProfile(humanoid, baselineCapacity, out snapshot, out inputs, out profile))
        {
            return true;
        }

        snapshot = CreatureCarrySignalsReader.Read(humanoid, baselineCapacity);
        inputs = CarryCapacityInputsFactory.Create(snapshot);
        profile = CarryCapacityCalculator.Calculate(inputs);
        StoreCachedProfile(humanoid, baselineCapacity, snapshot, inputs, profile);
        return true;
    }

    private static bool TryBuildUiFallbackProfile(
        HumanoidInstance? humanoid,
        out CarrySignalSnapshot? snapshot,
        out CarryCapacityInputs? inputs,
        out CarryCapacityProfile? profile)
    {
        snapshot = null;
        inputs = null;
        profile = null;

        if (!SmartCarrySettings.EnableDynamicCarryCapacity ||
            humanoid == null ||
            humanoid.HasDisposed)
        {
            return false;
        }

        var existingCapacity = humanoid.Storage != null && !humanoid.Storage.HasDisposed
            ? humanoid.Storage.StorageBase.Capacity
            : 0;
        var baselineCapacity = CarryBaselineCapacityResolver.Resolve(
            existingCapacity,
            SmartCarrySettings.DefaultBaseCarryCapacity,
            hasHumanoidOwner: true);
        try
        {
            snapshot = CreatureCarrySignalsReader.Read(humanoid, baselineCapacity);
            inputs = CarryCapacityInputsFactory.Create(snapshot);
            profile = CarryCapacityCalculator.Calculate(inputs);
            return true;
        }
        catch (Exception exception)
        {
            DiagnosticTrace.Error(
                "carry.runtime",
                $"TryBuildUiFallbackProfile failed for {humanoid}: {exception.GetType().Name}: {exception.Message}");
            snapshot = null;
            inputs = null;
            profile = null;
            return false;
        }
    }

    private static int GetBaselineCapacity(Storage storage, bool hasHumanoidOwner = false)
    {
        var entry = BaselineCapacityByStorage.GetOrCreateValue(storage);
        if (!entry.Initialized)
        {
            entry.Capacity = CarryBaselineCapacityResolver.Resolve(
                storage.StorageBase.Capacity,
                SmartCarrySettings.DefaultBaseCarryCapacity,
                hasHumanoidOwner);
            entry.Initialized = true;
        }

        return entry.Capacity;
    }

    private static bool TryGetCachedProfile(
        HumanoidInstance humanoid,
        int baselineCapacity,
        out CarrySignalSnapshot? snapshot,
        out CarryCapacityInputs? inputs,
        out CarryCapacityProfile? profile)
    {
        snapshot = null;
        inputs = null;
        profile = null;

        if (!LiveProfileCacheByHumanoid.TryGetValue(humanoid, out var entry) ||
            !entry.Initialized ||
            !TimedProfileCache.CanReuse(entry.Cache, baselineCapacity, GetNowSeconds()) ||
            entry.Snapshot == null ||
            entry.Inputs == null ||
            entry.Profile == null)
        {
            return false;
        }

        snapshot = entry.Snapshot;
        inputs = entry.Inputs;
        profile = entry.Profile;
        return true;
    }

    private static void StoreCachedProfile(
        HumanoidInstance humanoid,
        int baselineCapacity,
        CarrySignalSnapshot snapshot,
        CarryCapacityInputs inputs,
        CarryCapacityProfile profile)
    {
        var entry = LiveProfileCacheByHumanoid.GetOrCreateValue(humanoid);
        entry.Cache = TimedProfileCache.Create(baselineCapacity, GetNowSeconds(), LiveProfileCacheLifetimeSeconds);
        entry.Snapshot = snapshot;
        entry.Inputs = inputs;
        entry.Profile = profile;
        entry.Initialized = true;
    }

    private static void ApplyCapacityIfNeeded(
        Storage storage,
        CarrySignalSnapshot? snapshot,
        CarryCapacityProfile? profile)
    {
        var safeSnapshot = snapshot!;
        var safeProfile = profile!;
        var currentCapacity = storage.StorageBase.Capacity;
        if (currentCapacity == safeProfile.EffectiveCapacity)
        {
            DiagnosticTrace.InfoSample("carry.runtime", "ApplyIfNeeded.NoChange", $"ApplyIfNeeded no change: capacity={currentCapacity}");
            return;
        }

        isApplying = true;
        try
        {
            storage.SetStorageCapacity(safeProfile.EffectiveCapacity);
            DiagnosticTrace.Trace(
                "carry.apply",
                $"Applied carry capacity: {currentCapacity} -> {safeProfile.EffectiveCapacity}, base={safeSnapshot.BaseCapacity}, health={safeSnapshot.NormalizedHealth:0.00}, sleep={safeSnapshot.NormalizedSleep:0.00}, wounded={safeSnapshot.IsWounded}, body={safeSnapshot.BodyType}, height={safeSnapshot.Height:0.00}, weight={safeSnapshot.WeightCoefficient:0.00}, frame={safeProfile.FrameFactor:0.000}");
        }
        finally
        {
            isApplying = false;
        }
    }

    private static double GetNowSeconds()
    {
        return Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
    }

    private static CarrySignalSnapshot ApplyPreviewOverrides(
        CarrySignalSnapshot snapshot,
        int? ageYearsOverride,
        float? heightOverride,
        float? weightCoefficientOverride)
    {
        var ageYears = ageYearsOverride.GetValueOrDefault(snapshot.AgeYears);
        var height = heightOverride.GetValueOrDefault(snapshot.Height);
        var weightCoefficient = weightCoefficientOverride.GetValueOrDefault(snapshot.WeightCoefficient);

        if (ageYears == snapshot.AgeYears &&
            Math.Abs(height - snapshot.Height) < 0.0001f &&
            Math.Abs(weightCoefficient - snapshot.WeightCoefficient) < 0.0001f)
        {
            return snapshot;
        }

        return new CarrySignalSnapshot(
            baseCapacity: snapshot.BaseCapacity,
            normalizedHealth: snapshot.NormalizedHealth,
            normalizedSleep: snapshot.NormalizedSleep,
            isWounded: snapshot.IsWounded,
            bodyType: snapshot.BodyType,
            ageYears: ageYears,
            height: height,
            weightCoefficient: weightCoefficient,
            sourceDescription: $"{snapshot.SourceDescription}, previewOverride(age={ageYears}, height={height:0.00}, weight={weightCoefficient:0.00})");
    }

    private static bool IsLiveWorldHumanoid(HumanoidInstance? humanoid)
    {
        if (humanoid == null ||
            humanoid.HasDisposed ||
            humanoid.Storage == null ||
            humanoid.Storage.HasDisposed)
        {
            DiagnosticTrace.InfoSample("carry.runtime", "IsLiveWorldHumanoid.Invalid", $"IsLiveWorldHumanoid=false invalid humanoid={humanoid}, disposed={humanoid?.HasDisposed}, storage={humanoid?.Storage}, storageDisposed={humanoid?.Storage?.HasDisposed}");
            return false;
        }

        var resolved = CarryStorageOwnerResolver.Resolve(humanoid.Storage);
        var isLiveWorld = ReferenceEquals(resolved, humanoid);
        if (!isLiveWorld)
        {
            DiagnosticTrace.InfoSample("carry.runtime", "IsLiveWorldHumanoid.NoResolveMatch", $"IsLiveWorldHumanoid=false resolve mismatch humanoid={humanoid}, resolved={resolved}");
        }

        return isLiveWorld;
    }

    private static void TraceProfile(
        HumanoidInstance humanoid,
        CarrySignalSnapshot? snapshot,
        CarryCapacityProfile profile,
        string reason)
    {
        if (DiagnosticTrace.CurrentLevel < DiagnosticLogLevel.Trace)
        {
            return;
        }

        var entry = TracedProfilesByHumanoid.GetOrCreateValue(humanoid);
        if (entry.Capacity == profile.EffectiveCapacity &&
            string.Equals(entry.Reason, reason, StringComparison.Ordinal) &&
            string.Equals(entry.SourceDescription, snapshot?.SourceDescription, StringComparison.Ordinal))
        {
            return;
        }

        entry.Capacity = profile.EffectiveCapacity;
        entry.Reason = reason;
        entry.SourceDescription = snapshot?.SourceDescription;

        DiagnosticTrace.Trace(
            "carry.profile",
            $"reason={reason}, humanoid={humanoid}, effective={profile.EffectiveCapacity}, raw={profile.RawCapacity:0.##}, base={profile.BaseCapacity}, health={profile.HealthFactor:0.00}, fatigue={profile.FatigueFactor:0.00}, injury={profile.InjuryFactor:0.00}, age={profile.AgeFactor:0.00}, frame={profile.FrameFactor:0.000}, source=[{snapshot?.SourceDescription ?? "<none>"}]");
    }

    private sealed class BaselineCapacityEntry
    {
        public int Capacity { get; set; }

        public bool Initialized { get; set; }
    }

    private sealed class LiveProfileCacheEntry
    {
        public TimedProfileCache.Entry Cache { get; set; }

        public CarrySignalSnapshot? Snapshot { get; set; }

        public CarryCapacityInputs? Inputs { get; set; }

        public CarryCapacityProfile? Profile { get; set; }

        public bool Initialized { get; set; }
    }

    private sealed class ProfileTraceEntry
    {
        public int Capacity { get; set; }

        public string? Reason { get; set; }

        public string? SourceDescription { get; set; }
    }
}
