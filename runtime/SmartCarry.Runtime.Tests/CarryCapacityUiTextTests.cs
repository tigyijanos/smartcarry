namespace SmartCarry.Runtime.Tests;

public sealed class CarryCapacityUiTextTests
{
    [Fact]
    public void AppendCarryLabel_AppendsCarryCapacityToExistingText()
    {
        // Arrange
        var profile = new CarryCapacityProfile(
            baseCapacity: 60,
            healthFactor: 1f,
            injuryFactor: 1f,
            ageFactor: 1f,
            traitFactor: 1f,
            frameFactor: 1f,
            fatigueFactor: 1f,
            rawCapacity: 63f,
            effectiveCapacity: 63,
            minimumCapacity: 40,
            maximumCapacity: 90);

        // Act
        var result = CarryCapacityUiText.AppendCarryLabel("60", profile);

        // Assert
        Assert.Equal("60 / carry 63", result);
    }

    [Fact]
    public void AppendCarryLabel_ReplacesExistingCarrySuffix()
    {
        // Arrange
        var profile = new CarryCapacityProfile(
            baseCapacity: 60,
            healthFactor: 1f,
            injuryFactor: 1f,
            ageFactor: 1f,
            traitFactor: 1f,
            frameFactor: 1f,
            fatigueFactor: 1f,
            rawCapacity: 57f,
            effectiveCapacity: 57,
            minimumCapacity: 40,
            maximumCapacity: 90);

        // Act
        var result = CarryCapacityUiText.AppendCarryLabel("60 / carry 57", profile);

        // Assert
        Assert.Equal("60 / carry 57", result);
    }

    [Fact]
    public void BuildCarryTooltip_ReturnsCarryOnlyTooltip()
    {
        // Arrange
        var profile = new CarryCapacityProfile(
            baseCapacity: 60,
            healthFactor: 1f,
            injuryFactor: 1f,
            ageFactor: 1f,
            traitFactor: 1f,
            frameFactor: 1f,
            fatigueFactor: 1f,
            rawCapacity: 54f,
            effectiveCapacity: 54,
            minimumCapacity: 40,
            maximumCapacity: 90);

        // Act
        var result = CarryCapacityUiText.BuildCarryTooltip(profile);

        // Assert
        Assert.Equal("Carry 54", result);
    }

    [Fact]
    public void BuildMassCarriedText_FormatsCurrentAndMaximumCarry()
    {
        // Act
        var result = CarryCapacityUiText.BuildMassCarriedText(65f, 72);

        // Assert
        Assert.Equal("65/72", result);
    }

    [Fact]
    public void AppendCarryMassLabel_AppendsCarryCapacityWithUnit()
    {
        // Arrange
        var profile = new CarryCapacityProfile(
            baseCapacity: 30,
            healthFactor: 1f,
            injuryFactor: 1f,
            ageFactor: 1f,
            traitFactor: 1f,
            frameFactor: 1.25f,
            fatigueFactor: 1f,
            rawCapacity: 37.5f,
            effectiveCapacity: 38,
            minimumCapacity: 12,
            maximumCapacity: 60);

        // Act
        var result = CarryCapacityUiText.AppendCarryMassLabel("22 kg", profile, "kg");

        // Assert
        Assert.Equal("22 kg / carry 38 kg", result);
    }

    [Fact]
    public void UpsertCarryInfoLine_ReplacesExistingCarryLine()
    {
        // Arrange
        var profile = new CarryCapacityProfile(
            baseCapacity: 30,
            healthFactor: 1f,
            injuryFactor: 1f,
            ageFactor: 1f,
            traitFactor: 1f,
            frameFactor: 1.25f,
            fatigueFactor: 1f,
            rawCapacity: 37.5f,
            effectiveCapacity: 38,
            minimumCapacity: 12,
            maximumCapacity: 60);
        var infos = new List<string> { "Age 24", "Carry 31 kg", "Height 176 cm" };

        // Act
        CarryCapacityUiText.UpsertCarryInfoLine(infos, profile, "kg");

        // Assert
        Assert.Equal(new[] { "Age 24", "Carry 38 kg", "Height 176 cm" }, infos);
    }
}
