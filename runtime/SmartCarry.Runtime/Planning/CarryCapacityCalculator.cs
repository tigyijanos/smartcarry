namespace SmartCarry.Runtime;

/// <summary>
/// Pure calculator for the derived effective carry capacity.
/// </summary>
internal static class CarryCapacityCalculator
{
    public static CarryCapacityProfile Calculate(CarryCapacityInputs inputs)
    {
        var safeBaseCapacity = Math.Max(1, inputs.BaseCapacity);
        var minimumCapacity = Math.Max(1, inputs.MinimumCapacity);
        var maximumCapacity = Math.Max(minimumCapacity, inputs.MaximumCapacity);

        var rawCapacity = safeBaseCapacity *
            ClampFactor(inputs.HealthFactor) *
            ClampFactor(inputs.InjuryFactor) *
            ClampFactor(inputs.AgeFactor) *
            ClampFactor(inputs.TraitFactor) *
            ClampFactor(inputs.FrameFactor) *
            ClampFactor(inputs.FatigueFactor);

        var effectiveCapacity = (int)MathF.Round(rawCapacity, MidpointRounding.AwayFromZero);
        if (effectiveCapacity < minimumCapacity)
        {
            effectiveCapacity = minimumCapacity;
        }
        else if (effectiveCapacity > maximumCapacity)
        {
            effectiveCapacity = maximumCapacity;
        }

        return new CarryCapacityProfile(
            safeBaseCapacity,
            ClampFactor(inputs.HealthFactor),
            ClampFactor(inputs.InjuryFactor),
            ClampFactor(inputs.AgeFactor),
            ClampFactor(inputs.TraitFactor),
            ClampFactor(inputs.FrameFactor),
            ClampFactor(inputs.FatigueFactor),
            rawCapacity,
            effectiveCapacity,
            minimumCapacity,
            maximumCapacity);
    }

    private static float ClampFactor(float value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
        {
            return 1f;
        }

        return MathF.Max(0f, value);
    }
}
