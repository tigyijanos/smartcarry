using HarmonyLib;
using NSMedieval.State;
using NSMedieval.UI;

namespace SmartCarry.Runtime;

[HarmonyPatch(typeof(WorkerDetailsView), nameof(WorkerDetailsView.SetPseudonymTitle))]
internal static class WorkerDetailsCarryCapacityPatch
{
    private static void Postfix(WorkerDetailsView __instance, HumanoidInstance humanoid)
    {
        if (__instance?.WeightLabel == null ||
            humanoid == null ||
            humanoid.HasDisposed ||
            !CarryCapacityRuntimeApplier.TryGetCurrentProfile(humanoid, out var profile) ||
            profile == null)
        {
            return;
        }

        __instance.WeightLabel.text = CarryCapacityUiText.AppendCarryLabel(__instance.WeightLabel.text, profile);
    }
}
