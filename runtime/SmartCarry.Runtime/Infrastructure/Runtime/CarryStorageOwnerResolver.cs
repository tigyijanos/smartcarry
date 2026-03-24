using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using NSEipix.Base;
using NSMedieval.Manager;
using NSMedieval.State;

namespace SmartCarry.Runtime;

/// <summary>
/// Maps a worker carry storage instance back to its owning humanoid.
/// </summary>
internal static class CarryStorageOwnerResolver
{
    private static readonly PropertyInfo? CreaturesProperty =
        AccessTools.Property(typeof(CreatureManager), "Creatures");

    private static readonly FieldInfo? CreaturesField =
        AccessTools.Field(typeof(CreatureManager), "creatures");

    private static readonly ConditionalWeakTable<NSMedieval.Components.Storage, OwnerEntry> OwnerByStorage = new();

    public static HumanoidInstance? Resolve(NSMedieval.Components.Storage storage)
    {
        if (storage == null || storage.HasDisposed)
        {
            DiagnosticTrace.InfoSample("carry.resolve", "Resolve.InvalidStorage", $"Resolve skipped: invalid storage {storage}");
            return null;
        }

        if (OwnerByStorage.TryGetValue(storage, out var cached) &&
            cached.Humanoid != null &&
            !cached.Humanoid.HasDisposed &&
            ReferenceEquals(cached.Humanoid.Storage, storage))
        {
            DiagnosticTrace.InfoSample("carry.resolve", "Resolve.CacheHit", $"Resolve cache hit: storage={storage}, humanoid={cached.Humanoid}");
            return cached.Humanoid;
        }

        OwnerByStorage.Remove(storage);

        var manager = MonoSingleton<CreatureManager>.Instance;
        if (manager == null)
        {
            DiagnosticTrace.InfoSample("carry.resolve", "Resolve.NoManager", $"Resolve skipped: CreatureManager missing for storage {storage}");
            return null;
        }

        foreach (var creature in GetCreatureEnumerable(manager).OfType<HumanoidInstance>())
        {
            if (creature == null ||
                creature.HasDisposed ||
                creature.Storage == null ||
                !ReferenceEquals(creature.Storage, storage))
            {
                continue;
            }

            OwnerByStorage.Add(storage, new OwnerEntry(creature));
            DiagnosticTrace.InfoSample("carry.resolve", "Resolve.WorldMatch", $"Resolve world match: storage={storage}, humanoid={creature}");
            return creature;
        }

        DiagnosticTrace.InfoSample("carry.resolve", "Resolve.NoMatch", $"Resolve no match: storage={storage}");
        return null;
    }

    private static IEnumerable GetCreatureEnumerable(CreatureManager manager)
    {
        if (CreaturesProperty?.GetValue(manager) is IEnumerable creaturesByProperty)
        {
            return creaturesByProperty;
        }

        if (CreaturesField?.GetValue(manager) is IEnumerable creaturesByField)
        {
            return creaturesByField;
        }

        return Array.Empty<object>();
    }

    private sealed class OwnerEntry
    {
        public OwnerEntry(HumanoidInstance humanoid)
        {
            Humanoid = humanoid;
        }

        public HumanoidInstance Humanoid { get; }
    }
}
