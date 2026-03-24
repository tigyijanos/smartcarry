using NSMedieval;

namespace SmartCarry.Runtime;

/// <summary>
/// Raw, normalized runtime signals that influence one settler's carry capacity.
/// </summary>
internal sealed class CarrySignalSnapshot
{
    public CarrySignalSnapshot(
        int baseCapacity,
        float normalizedHealth,
        float normalizedSleep,
        bool isWounded,
        BodyType bodyType,
        int ageYears,
        float height,
        float weightCoefficient,
        string? sourceDescription = null)
    {
        BaseCapacity = baseCapacity;
        NormalizedHealth = normalizedHealth;
        NormalizedSleep = normalizedSleep;
        IsWounded = isWounded;
        BodyType = bodyType;
        AgeYears = ageYears;
        Height = height;
        WeightCoefficient = weightCoefficient;
        SourceDescription = sourceDescription;
    }

    public int BaseCapacity { get; }

    public float NormalizedHealth { get; }

    public float NormalizedSleep { get; }

    public bool IsWounded { get; }

    public BodyType BodyType { get; }

    public int AgeYears { get; }

    public float Height { get; }

    public float WeightCoefficient { get; }

    public string? SourceDescription { get; }
}
