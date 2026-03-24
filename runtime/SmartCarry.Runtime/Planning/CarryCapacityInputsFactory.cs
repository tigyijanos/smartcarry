using NSMedieval;
using SmartCarry.Runtime.Configuration;

namespace SmartCarry.Runtime;

/// <summary>
/// Builds normalized calculator inputs from runtime signals and current settings.
/// </summary>
internal static class CarryCapacityInputsFactory
{
    private const float NeutralNormalizedPhysiqueValue = 1f;
    private const float NormalizedHeightHalfRange = 0.08f;
    private const float NormalizedWeightHalfRange = 0.12f;
    private const float NeutralRawHeightValue = 170f;
    private const float RawHeightHalfRange = 15f;
    private const float NeutralRawWeightValue = 75f;
    private const float RawWeightHalfRange = 15f;

    public static CarryCapacityInputs Create(CarrySignalSnapshot snapshot)
    {
        var minimumHealthFactor = SmartCarrySettings.MinimumHealthFactorAtZeroHealth;
        var minimumSleepFactor = SmartCarrySettings.MinimumSleepFactorAtZeroSleep;

        var healthFactor = InterpolateToOne(minimumHealthFactor, snapshot.NormalizedHealth);
        var sleepFactor = InterpolateToOne(minimumSleepFactor, snapshot.NormalizedSleep);
        var injuryFactor = snapshot.IsWounded
            ? SmartCarrySettings.WoundedCarryFactor
            : 1f;
        var ageFactor = BuildAgeFactor(snapshot.AgeYears);
        var frameFactor = BuildFrameFactor(snapshot);

        return new CarryCapacityInputs(
            baseCapacity: snapshot.BaseCapacity,
            healthFactor: healthFactor,
            injuryFactor: injuryFactor,
            ageFactor: ageFactor,
            traitFactor: 1f,
            frameFactor: frameFactor,
            fatigueFactor: sleepFactor,
            minimumCapacity: SmartCarrySettings.MinimumCarryCapacity,
            maximumCapacity: SmartCarrySettings.MaximumCarryCapacity);
    }

    private static float BuildFrameFactor(CarrySignalSnapshot snapshot)
    {
        var bodyFactor = snapshot.BodyType switch
        {
            BodyType.Male => SmartCarrySettings.MaleBodyTypeFactor,
            BodyType.Female => SmartCarrySettings.FemaleBodyTypeFactor,
            _ => 1f
        };

        var heightFactor = BuildHeightFactor(snapshot.Height, SmartCarrySettings.HeightFactorStrength);
        var weightFactor = BuildWeightFactor(snapshot.WeightCoefficient, SmartCarrySettings.WeightFactorStrength);
        return bodyFactor * heightFactor * weightFactor;
    }

    private static float BuildAgeFactor(int ageYears)
    {
        if (ageYears <= 0)
        {
            return 1f;
        }

        var primeStartAge = Math.Max(SmartCarrySettings.YoungAdultAgeYears, SmartCarrySettings.PrimeAgeYears - SmartCarrySettings.PrimeAgeBandYears);
        var primeEndAge = Math.Max(primeStartAge, SmartCarrySettings.PrimeAgeYears + SmartCarrySettings.PrimeAgeBandYears);

        if (ageYears <= primeStartAge)
        {
            return InterpolateBetweenAges(
                ageYears,
                SmartCarrySettings.YoungAdultAgeYears,
                SmartCarrySettings.YoungAdultCarryFactor,
                primeStartAge,
                1f);
        }

        if (ageYears <= primeEndAge)
        {
            return 1f;
        }

        return InterpolateBetweenAges(
            ageYears,
            primeEndAge,
            1f,
            SmartCarrySettings.SeniorAgeYears,
            SmartCarrySettings.SeniorCarryFactor);
    }

    private static float BuildHeightFactor(float value, float strength)
    {
        var (center, halfRange) = value > 10f
            ? (NeutralRawHeightValue, RawHeightHalfRange)
            : (NeutralNormalizedPhysiqueValue, NormalizedHeightHalfRange);
        return BuildPhysiqueFactor(value, strength, center, halfRange);
    }

    private static float BuildWeightFactor(float value, float strength)
    {
        var (center, halfRange) = value > 5f
            ? (NeutralRawWeightValue, RawWeightHalfRange)
            : (NeutralNormalizedPhysiqueValue, NormalizedWeightHalfRange);
        return BuildPhysiqueFactor(value, strength, center, halfRange);
    }

    private static float BuildPhysiqueFactor(float value, float strength, float center, float halfRange)
    {
        var clampedStrength = ClampStrength(strength);
        if (clampedStrength <= 0f)
        {
            return 1f;
        }

        var normalizedDelta = NormalizeSigned(value, center, halfRange);
        return 1f + (normalizedDelta * clampedStrength);
    }

    private static float InterpolateBetweenAges(int ageYears, int fromAge, float fromFactor, int toAge, float toFactor)
    {
        if (toAge <= fromAge)
        {
            return toFactor;
        }

        var normalized = Math.Clamp((ageYears - fromAge) / (float)(toAge - fromAge), 0f, 1f);
        return fromFactor + ((toFactor - fromFactor) * normalized);
    }

    private static float InterpolateToOne(float minimumFactor, float normalizedValue)
    {
        var clampedNormalizedValue = Clamp01(normalizedValue);
        var clampedMinimumFactor = Clamp01(minimumFactor);
        return clampedMinimumFactor + ((1f - clampedMinimumFactor) * clampedNormalizedValue);
    }

    private static float NormalizeSigned(float value, float center, float halfRange)
    {
        if (float.IsNaN(value) || float.IsInfinity(value) || halfRange <= 0f)
        {
            return 0f;
        }

        return Math.Clamp((value - center) / halfRange, -1f, 1f);
    }

    private static float Clamp01(float value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
        {
            return 1f;
        }

        return Math.Clamp(value, 0f, 1f);
    }

    private static float ClampStrength(float value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
        {
            return 0f;
        }

        return Math.Clamp(value, 0f, 0.25f);
    }
}
