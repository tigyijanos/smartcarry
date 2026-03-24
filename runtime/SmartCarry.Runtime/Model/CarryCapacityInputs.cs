namespace SmartCarry.Runtime;

/// <summary>
/// Normalized per-settler inputs used to derive an effective carry capacity.
/// </summary>
internal sealed class CarryCapacityInputs
{
    public CarryCapacityInputs(
        int baseCapacity,
        float healthFactor,
        float injuryFactor,
        float ageFactor,
        float traitFactor,
        float frameFactor,
        float fatigueFactor,
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

    public int MinimumCapacity { get; }

    public int MaximumCapacity { get; }
}
