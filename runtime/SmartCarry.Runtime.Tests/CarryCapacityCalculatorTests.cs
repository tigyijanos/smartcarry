namespace SmartCarry.Runtime.Tests;

public sealed class CarryCapacityCalculatorTests
{
    [Fact]
    public void Calculate_ReturnsDerivedCapacity_WhenAllFactorsAreApplied()
    {
        // Arrange
        var inputs = new CarryCapacityInputs(
            baseCapacity: 60,
            healthFactor: 1.0f,
            injuryFactor: 0.9f,
            ageFactor: 1.0f,
            traitFactor: 1.1f,
            frameFactor: 1.05f,
            fatigueFactor: 0.95f,
            minimumCapacity: 40,
            maximumCapacity: 90);

        // Act
        var result = CarryCapacityCalculator.Calculate(inputs);

        // Assert
        Assert.Equal(59, result.EffectiveCapacity);
        Assert.Equal(59.2515f, result.RawCapacity, 3);
    }

    [Fact]
    public void Calculate_ClampsToMinimum_WhenFactorsDriveCapacityBelowMinimum()
    {
        // Arrange
        var inputs = new CarryCapacityInputs(
            baseCapacity: 60,
            healthFactor: 0.4f,
            injuryFactor: 0.5f,
            ageFactor: 1.0f,
            traitFactor: 1.0f,
            frameFactor: 1.0f,
            fatigueFactor: 1.0f,
            minimumCapacity: 40,
            maximumCapacity: 90);

        // Act
        var result = CarryCapacityCalculator.Calculate(inputs);

        // Assert
        Assert.Equal(40, result.EffectiveCapacity);
    }

    [Fact]
    public void Calculate_ClampsToMaximum_WhenFactorsDriveCapacityAboveMaximum()
    {
        // Arrange
        var inputs = new CarryCapacityInputs(
            baseCapacity: 60,
            healthFactor: 1.2f,
            injuryFactor: 1.1f,
            ageFactor: 1.0f,
            traitFactor: 1.15f,
            frameFactor: 1.1f,
            fatigueFactor: 1.0f,
            minimumCapacity: 40,
            maximumCapacity: 90);

        // Act
        var result = CarryCapacityCalculator.Calculate(inputs);

        // Assert
        Assert.Equal(90, result.EffectiveCapacity);
    }

    [Fact]
    public void Calculate_NormalizesInvalidFactors_WhenFactorsAreNaNOrNegative()
    {
        // Arrange
        var inputs = new CarryCapacityInputs(
            baseCapacity: 60,
            healthFactor: float.NaN,
            injuryFactor: -1f,
            ageFactor: 1.0f,
            traitFactor: 1.0f,
            frameFactor: 1.0f,
            fatigueFactor: 1.0f,
            minimumCapacity: 20,
            maximumCapacity: 90);

        // Act
        var result = CarryCapacityCalculator.Calculate(inputs);

        // Assert
        Assert.Equal(20, result.EffectiveCapacity);
        Assert.Equal(1f, result.HealthFactor);
        Assert.Equal(0f, result.InjuryFactor);
    }
}
