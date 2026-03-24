namespace SmartCarry.Runtime.Tests;

public sealed class CarryBaselineCapacityResolverTests
{
    [Fact]
    public void Resolve_ReturnsConfiguredBase_ForHumanoidCarryStorage()
    {
        // Arrange
        const int liveStorageCapacity = 60;
        const int configuredBaseCapacity = 30;

        // Act
        var result = CarryBaselineCapacityResolver.Resolve(liveStorageCapacity, configuredBaseCapacity, hasHumanoidOwner: true);

        // Assert
        Assert.Equal(30, result);
    }

    [Fact]
    public void Resolve_ReturnsLiveStorageCapacity_ForNonHumanoidStorage()
    {
        // Arrange
        const int liveStorageCapacity = 80;
        const int configuredBaseCapacity = 30;

        // Act
        var result = CarryBaselineCapacityResolver.Resolve(liveStorageCapacity, configuredBaseCapacity, hasHumanoidOwner: false);

        // Assert
        Assert.Equal(80, result);
    }

    [Fact]
    public void Resolve_FallsBackToConfiguredBase_WhenNonHumanoidStorageCapacityIsMissing()
    {
        // Arrange
        const int liveStorageCapacity = 0;
        const int configuredBaseCapacity = 30;

        // Act
        var result = CarryBaselineCapacityResolver.Resolve(liveStorageCapacity, configuredBaseCapacity, hasHumanoidOwner: false);

        // Assert
        Assert.Equal(30, result);
    }
}
