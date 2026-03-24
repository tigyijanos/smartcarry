namespace SmartCarry.Runtime;

/// <summary>
/// Resolves the immutable baseline used for SmartCarry capacity calculations.
/// </summary>
internal static class CarryBaselineCapacityResolver
{
    public static int Resolve(int liveStorageCapacity, int configuredBaseCapacity, bool hasHumanoidOwner)
    {
        var safeConfiguredBase = Math.Max(1, configuredBaseCapacity);
        if (hasHumanoidOwner)
        {
            return safeConfiguredBase;
        }

        return liveStorageCapacity > 0
            ? liveStorageCapacity
            : safeConfiguredBase;
    }
}
