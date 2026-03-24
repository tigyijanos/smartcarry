using HarmonyLib;
using NSMedieval.Components;

namespace SmartCarry.Runtime;

/// <summary>
/// Overrides worker carry free space using the derived SmartCarry profile.
/// </summary>
[HarmonyPatch]
internal static class CarryCapacityStoragePatch
{
    [HarmonyPatch(typeof(Storage), nameof(Storage.GetFreeSpace))]
    [HarmonyPostfix]
    private static void GetFreeSpacePostfix(Storage __instance, ref float __result)
    {
        var originalResult = __result;
        DiagnosticTrace.InfoSample("carry.storage", "Storage.GetFreeSpace", $"GetFreeSpace: storage={__instance}, original={originalResult:0.##}");
        if (!CarryCapacityRuntimeApplier.TryGetFreeSpace(__instance, out var freeSpace))
        {
            return;
        }

        __result = freeSpace;
        if (MathF.Abs(freeSpace - originalResult) > 0.01f)
        {
            DiagnosticTrace.Trace("carry.free", $"GetFreeSpace override: {originalResult:0.##} -> {freeSpace:0.##}");
        }
    }
}
