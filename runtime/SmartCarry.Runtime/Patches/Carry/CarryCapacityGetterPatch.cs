using HarmonyLib;
using NSMedieval.State;

namespace SmartCarry.Runtime;

/// <summary>
/// Overrides the game's carry-capacity getters so UI and gameplay both see the
/// same derived SmartCarry value.
/// </summary>
[HarmonyPatch]
internal static class CarryCapacityGetterPatch
{
    [HarmonyPatch(typeof(HumanoidInstance), "get_CaravanStorageCapacity")]
    [HarmonyPostfix]
    private static void CaravanStorageCapacityPostfix(HumanoidInstance __instance, ref int __result)
    {
        var originalResult = __result;
        DiagnosticTrace.InfoSample("carry.getter", "HumanoidInstance.get_CaravanStorageCapacity", $"CaravanStorageCapacity getter: humanoid={__instance}, original={originalResult}");
        if (__instance == null || __instance.HasDisposed)
        {
            return;
        }

        if (!CarryCapacityRuntimeApplier.TryGetEffectiveCapacity(__instance, out var effectiveCapacity))
        {
            return;
        }

        __result = effectiveCapacity;
        if (effectiveCapacity != originalResult)
        {
            DiagnosticTrace.Trace("carry.get", $"CaravanStorageCapacity for {__instance}: {originalResult} -> {effectiveCapacity}");
        }
    }

    [HarmonyPatch(typeof(HumanoidInstance), nameof(HumanoidInstance.GetCaravanCarryWeight))]
    [HarmonyPostfix]
    private static void GetCaravanCarryWeightPostfix(HumanoidInstance __instance, ref int __result)
    {
        var originalResult = __result;
        DiagnosticTrace.InfoSample("carry.getter", "HumanoidInstance.GetCaravanCarryWeight", $"GetCaravanCarryWeight getter: humanoid={__instance}, original={originalResult}");
        if (__result <= 0 || __instance == null || __instance.HasDisposed)
        {
            return;
        }

        if (!CarryCapacityRuntimeApplier.TryGetEffectiveCapacity(__instance, out var effectiveCapacity))
        {
            return;
        }

        __result = effectiveCapacity;
        if (effectiveCapacity != originalResult)
        {
            DiagnosticTrace.Trace("carry.get", $"GetCaravanCarryWeight for {__instance}: {originalResult} -> {effectiveCapacity}");
        }
    }
}
