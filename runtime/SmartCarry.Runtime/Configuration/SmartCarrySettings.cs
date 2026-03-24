using BepInEx.Configuration;

namespace SmartCarry.Runtime.Configuration;

internal static class SmartCarrySettings
{
    private const int DefaultBaseCapacity = 30;
    private const int DefaultMinimumCapacity = 12;
    private const int DefaultMaximumCapacity = 60;
    private const float DefaultMinimumHealthFactor = 0.65f;
    private const float DefaultMinimumSleepFactor = 0.85f;
    private const float DefaultWoundedCarryFactor = 0.9f;
    private const float DefaultMaleBodyTypeFactor = 1.20f;
    private const float DefaultFemaleBodyTypeFactor = 0.80f;
    private const float DefaultHeightFactorStrength = 0.20f;
    private const float DefaultWeightFactorStrength = 0.30f;
    private const int DefaultPrimeAgeYears = 30;
    private const int DefaultPrimeAgeBandYears = 5;
    private const int DefaultYoungAdultAgeYears = 18;
    private const float DefaultYoungAdultCarryFactor = 0.82f;
    private const int DefaultSeniorAgeYears = 60;
    private const float DefaultSeniorCarryFactor = 0.72f;

    public static DiagnosticLogLevel DiagnosticTraceLevel { get; private set; } = DiagnosticLogLevel.Off;
    public static bool EnableDynamicCarryCapacity { get; private set; } = true;
    public static int DefaultBaseCarryCapacity { get; private set; } = DefaultBaseCapacity;
    public static int MinimumCarryCapacity { get; private set; } = DefaultMinimumCapacity;
    public static int MaximumCarryCapacity { get; private set; } = DefaultMaximumCapacity;
    public static float MinimumHealthFactorAtZeroHealth { get; private set; } = DefaultMinimumHealthFactor;
    public static float MinimumSleepFactorAtZeroSleep { get; private set; } = DefaultMinimumSleepFactor;
    public static float WoundedCarryFactor { get; private set; } = DefaultWoundedCarryFactor;
    public static float MaleBodyTypeFactor { get; private set; } = DefaultMaleBodyTypeFactor;
    public static float FemaleBodyTypeFactor { get; private set; } = DefaultFemaleBodyTypeFactor;
    public static float HeightFactorStrength { get; private set; } = DefaultHeightFactorStrength;
    public static float WeightFactorStrength { get; private set; } = DefaultWeightFactorStrength;
    public static int PrimeAgeYears { get; private set; } = DefaultPrimeAgeYears;
    public static int PrimeAgeBandYears { get; private set; } = DefaultPrimeAgeBandYears;
    public static int YoungAdultAgeYears { get; private set; } = DefaultYoungAdultAgeYears;
    public static float YoungAdultCarryFactor { get; private set; } = DefaultYoungAdultCarryFactor;
    public static int SeniorAgeYears { get; private set; } = DefaultSeniorAgeYears;
    public static float SeniorCarryFactor { get; private set; } = DefaultSeniorCarryFactor;

    public static void Initialize(ConfigFile config)
    {
        DiagnosticTraceLevel = config.Bind(
            "Tracing",
            "DiagnosticTraceLevel",
            DiagnosticLogLevel.Off,
            "Controls SmartCarry diagnostic logging: Off, Error, Info, Trace.").Value;

        EnableDynamicCarryCapacity = config.Bind(
            "Carry",
            "EnableDynamicCarryCapacity",
            true,
            "Enables SmartCarry runtime capacity overrides for worker carry storage.").Value;

        DefaultBaseCarryCapacity = config.Bind(
            "Carry",
            "DefaultBaseCarryCapacity",
            DefaultBaseCapacity,
            "Base carry capacity before per-settler modifiers are applied.").Value;

        MinimumCarryCapacity = config.Bind(
            "Carry",
            "MinimumCarryCapacity",
            DefaultMinimumCapacity,
            "Lower clamp for calculated effective carry capacity.").Value;

        MaximumCarryCapacity = config.Bind(
            "Carry",
            "MaximumCarryCapacity",
            DefaultMaximumCapacity,
            "Upper clamp for calculated effective carry capacity.").Value;

        MinimumHealthFactorAtZeroHealth = config.Bind(
            "Carry",
            "MinimumHealthFactorAtZeroHealth",
            DefaultMinimumHealthFactor,
            "Lower health multiplier used when a settler's health stat reaches zero; full health always maps to 1.0.").Value;

        MinimumSleepFactorAtZeroSleep = config.Bind(
            "Carry",
            "MinimumSleepFactorAtZeroSleep",
            DefaultMinimumSleepFactor,
            "Lower sleep multiplier used when a settler's sleep stat reaches zero; full sleep always maps to 1.0.").Value;

        WoundedCarryFactor = config.Bind(
            "Carry",
            "WoundedCarryFactor",
            DefaultWoundedCarryFactor,
            "Additional multiplier applied when the settler is currently wounded.").Value;

        MaleBodyTypeFactor = config.Bind(
            "Carry",
            "MaleBodyTypeFactor",
            DefaultMaleBodyTypeFactor,
            "Body-type multiplier applied to male settlers before height and weight nudges are applied.").Value;

        FemaleBodyTypeFactor = config.Bind(
            "Carry",
            "FemaleBodyTypeFactor",
            DefaultFemaleBodyTypeFactor,
            "Body-type multiplier applied to female settlers before height and weight nudges are applied.").Value;

        HeightFactorStrength = config.Bind(
            "Carry",
            "HeightFactorStrength",
            DefaultHeightFactorStrength,
            "How strongly the settler's height nudges carry capacity around the neutral height value.").Value;

        WeightFactorStrength = config.Bind(
            "Carry",
            "WeightFactorStrength",
            DefaultWeightFactorStrength,
            "How strongly the settler's current weight nudges carry capacity around the neutral weight value.").Value;

        PrimeAgeYears = config.Bind(
            "Carry",
            "PrimeAgeYears",
            DefaultPrimeAgeYears,
            "Age where settlers are treated as being in their prime carry years.").Value;

        PrimeAgeBandYears = config.Bind(
            "Carry",
            "PrimeAgeBandYears",
            DefaultPrimeAgeBandYears,
            "Half-width of the prime age band around PrimeAgeYears where no age penalty is applied.").Value;

        YoungAdultAgeYears = config.Bind(
            "Carry",
            "YoungAdultAgeYears",
            DefaultYoungAdultAgeYears,
            "Reference young-adult age used for the lower age carry factor.").Value;

        YoungAdultCarryFactor = config.Bind(
            "Carry",
            "YoungAdultCarryFactor",
            DefaultYoungAdultCarryFactor,
            "Carry multiplier applied around the young-adult reference age before settlers reach their prime.").Value;

        SeniorAgeYears = config.Bind(
            "Carry",
            "SeniorAgeYears",
            DefaultSeniorAgeYears,
            "Reference senior age used for the older-age carry factor.").Value;

        SeniorCarryFactor = config.Bind(
            "Carry",
            "SeniorCarryFactor",
            DefaultSeniorCarryFactor,
            "Carry multiplier applied around the senior reference age after settlers leave their prime.").Value;

        if (MinimumCarryCapacity < 1)
        {
            MinimumCarryCapacity = 1;
        }

        if (MaximumCarryCapacity < MinimumCarryCapacity)
        {
            MaximumCarryCapacity = MinimumCarryCapacity;
        }

        if (DefaultBaseCarryCapacity < MinimumCarryCapacity)
        {
            DefaultBaseCarryCapacity = MinimumCarryCapacity;
        }
        else if (DefaultBaseCarryCapacity > MaximumCarryCapacity)
        {
            DefaultBaseCarryCapacity = MaximumCarryCapacity;
        }

        MinimumHealthFactorAtZeroHealth = ClampUnitFactor(MinimumHealthFactorAtZeroHealth);
        MinimumSleepFactorAtZeroSleep = ClampUnitFactor(MinimumSleepFactorAtZeroSleep);
        WoundedCarryFactor = ClampUnitFactor(WoundedCarryFactor);
        MaleBodyTypeFactor = ClampPositiveFactor(MaleBodyTypeFactor, DefaultMaleBodyTypeFactor);
        FemaleBodyTypeFactor = ClampPositiveFactor(FemaleBodyTypeFactor, DefaultFemaleBodyTypeFactor);
        HeightFactorStrength = ClampStrength(HeightFactorStrength);
        WeightFactorStrength = ClampStrength(WeightFactorStrength);
        PrimeAgeYears = ClampAge(PrimeAgeYears, DefaultPrimeAgeYears);
        PrimeAgeBandYears = Math.Clamp(PrimeAgeBandYears, 0, 20);
        YoungAdultAgeYears = ClampAge(YoungAdultAgeYears, DefaultYoungAdultAgeYears);
        SeniorAgeYears = ClampAge(SeniorAgeYears, DefaultSeniorAgeYears);
        YoungAdultCarryFactor = ClampAgeFactor(YoungAdultCarryFactor, DefaultYoungAdultCarryFactor);
        SeniorCarryFactor = ClampAgeFactor(SeniorCarryFactor, DefaultSeniorCarryFactor);

        if (PrimeAgeYears < YoungAdultAgeYears)
        {
            PrimeAgeYears = YoungAdultAgeYears;
        }

        if (SeniorAgeYears < PrimeAgeYears + PrimeAgeBandYears)
        {
            SeniorAgeYears = PrimeAgeYears + PrimeAgeBandYears;
        }
    }

    private static float ClampUnitFactor(float value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
        {
            return 1f;
        }

        return Math.Clamp(value, 0f, 1f);
    }

    private static float ClampPositiveFactor(float value, float fallback)
    {
        if (float.IsNaN(value) || float.IsInfinity(value) || value <= 0f)
        {
            return fallback;
        }

        return Math.Clamp(value, 0.4f, 1.8f);
    }

    private static float ClampStrength(float value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
        {
            return 0f;
        }

        return Math.Clamp(value, 0f, 0.5f);
    }

    private static int ClampAge(int value, int fallback)
    {
        if (value < 0 || value > 120)
        {
            return fallback;
        }

        return value;
    }

    private static float ClampAgeFactor(float value, float fallback)
    {
        if (float.IsNaN(value) || float.IsInfinity(value) || value <= 0f)
        {
            return fallback;
        }

        return Math.Clamp(value, 0.4f, 1f);
    }
}
