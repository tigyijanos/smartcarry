using HarmonyLib;
using NSMedieval.UI;

namespace SmartCarry.Runtime;

[HarmonyPatch(typeof(CharactersView), "OnWorkerGeneration")]
internal static class CharactersViewGenerationPatch
{
    private static void Postfix(CharactersView __instance, bool workersGenerating)
    {
        DiagnosticTrace.InfoSample(
            "carry.ui.flow",
            $"CharactersView.OnWorkerGeneration.{workersGenerating}",
            $"OnWorkerGeneration generating={workersGenerating}, active={__instance?.isActiveAndEnabled}");
        if (__instance != null)
        {
            CharacterEditorGenerationState.SetGenerating(__instance, workersGenerating);
            if (!workersGenerating)
            {
                CharacterStatsEditCarryCapacityPatch.RefreshActiveViews("OnWorkerGeneration.False");
            }
        }
    }
}
