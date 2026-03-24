using System.Reflection;
using HarmonyLib;
using NSMedieval.State;
using NSMedieval.UI;

namespace SmartCarry.Runtime;

[HarmonyPatch]
internal static class WorkerInventoryCarryCapacityPatch
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        var parameterless = AccessTools.Method(typeof(WorkerInventoryExtraPanel), "StorageUpdated", Type.EmptyTypes);
        if (parameterless != null)
        {
            yield return parameterless;
        }

        var resourceCountMethod = AccessTools.Method(typeof(WorkerInventoryExtraPanel), "StorageUpdated", new[] { typeof(SimpleResourceCount) });
        if (resourceCountMethod != null)
        {
            yield return resourceCountMethod;
        }
    }

    [HarmonyPrefix]
    private static void Prefix(WorkerInventoryExtraPanel __instance)
    {
        if (__instance == null)
        {
            return;
        }

        var humanoid = Traverse.Create(__instance).Property("Humanoid").GetValue<HumanoidInstance>();
        if (humanoid == null || humanoid.HasDisposed || humanoid.Storage == null || humanoid.Storage.HasDisposed)
        {
            return;
        }

        CarryCapacityRuntimeApplier.ApplyIfNeeded(humanoid.Storage);

        DiagnosticTrace.Trace(
            "carry.ui",
            $"WorkerInventoryExtraPanel prepared for {humanoid}: current={humanoid.Storage.GetResourceWeight():0.#}, max={humanoid.Storage.StorageBase.Capacity}");
    }
}
