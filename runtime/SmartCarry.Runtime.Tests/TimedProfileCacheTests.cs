namespace SmartCarry.Runtime.Tests;

public sealed class TimedProfileCacheTests
{
    [Fact]
    public void CanReuse_WhenBaselineMatchesAndEntryIsFresh_ReturnsTrue()
    {
        // Arrange
        var entry = TimedProfileCache.Create(baselineCapacity: 30, now: 10d, lifetimeSeconds: 0.25d);

        // Act
        var canReuse = TimedProfileCache.CanReuse(entry, baselineCapacity: 30, now: 10.1d);

        // Assert
        Assert.True(canReuse);
    }

    [Fact]
    public void CanReuse_WhenBaselineChanges_ReturnsFalse()
    {
        // Arrange
        var entry = TimedProfileCache.Create(baselineCapacity: 30, now: 10d, lifetimeSeconds: 0.25d);

        // Act
        var canReuse = TimedProfileCache.CanReuse(entry, baselineCapacity: 31, now: 10.1d);

        // Assert
        Assert.False(canReuse);
    }

    [Fact]
    public void CanReuse_WhenEntryExpired_ReturnsFalse()
    {
        // Arrange
        var entry = TimedProfileCache.Create(baselineCapacity: 30, now: 10d, lifetimeSeconds: 0.25d);

        // Act
        var canReuse = TimedProfileCache.CanReuse(entry, baselineCapacity: 30, now: 10.5d);

        // Assert
        Assert.False(canReuse);
    }
}
