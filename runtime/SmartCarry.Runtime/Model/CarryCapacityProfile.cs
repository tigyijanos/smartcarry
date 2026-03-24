namespace SmartCarry.Runtime;

/// <summary>
/// Result of the carry-capacity calculation together with the factors that produced it.
/// </summary>
internal sealed class CarryCapacityProfile
{
    public CarryCapacityProfile(
        int baseCapacity,
        float healthFactor,
        float injuryFactor,
        float ageFactor,
        float traitFactor,
        float frameFactor,
        float fatigueFactor,
        float rawCapacity,
        int effectiveCapacity,
        int minimumCapacity,
        int maximumCapacity)
    {
        BaseCapacity = baseCapacity;
        HealthFactor = healthFactor;
        InjuryFactor = injuryFactor;
        AgeFactor = ageFactor;
        TraitFactor = traitFactor;
        FrameFactor = frameFactor;
        FatigueFactor = fatigueFactor;
        RawCapacity = rawCapacity;
        EffectiveCapacity = effectiveCapacity;
        MinimumCapacity = minimumCapacity;
        MaximumCapacity = maximumCapacity;
    }

    public int BaseCapacity { get; }

    public float HealthFactor { get; }

    public float InjuryFactor { get; }

    public float AgeFactor { get; }

    public float TraitFactor { get; }

    public float FrameFactor { get; }

    public float FatigueFactor { get; }

    public float RawCapacity { get; }

    public int EffectiveCapacity { get; }

    public int MinimumCapacity { get; }

    public int MaximumCapacity { get; }
}
