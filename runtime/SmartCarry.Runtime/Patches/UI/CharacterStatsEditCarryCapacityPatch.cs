using System.Runtime.CompilerServices;
using HarmonyLib;
using NSMedieval.Controllers;
using NSMedieval.State;
using NSMedieval.UI;
using TMPro;
using UnityEngine;

namespace SmartCarry.Runtime;

[HarmonyPatch]
internal static class CharacterStatsEditCarryCapacityPatch
{
    private static readonly ConditionalWeakTable<CharacterStatsEdit, PatchState> StateByView = new();
    private static readonly HashSet<CharacterStatsEdit> ActiveViews = new();
    private static readonly AccessTools.FieldRef<CharacterStatsEdit, CharactersView> CharactersViewRef =
        AccessTools.FieldRefAccess<CharacterStatsEdit, CharactersView>("charactersView");
    private static readonly AccessTools.FieldRef<CharacterStatsEdit, TMP_Text> SkillsTitleRef =
        AccessTools.FieldRefAccess<CharacterStatsEdit, TMP_Text>("skillsTitle");
    private static readonly AccessTools.FieldRef<CharacterStatsEdit, EditableInputGroupLayoutItemView> AgeEditViewRef =
        AccessTools.FieldRefAccess<CharacterStatsEdit, EditableInputGroupLayoutItemView>("ageEditView");
    private static readonly AccessTools.FieldRef<CharacterStatsEdit, EditableInputGroupLayoutItemView> HeightEditViewRef =
        AccessTools.FieldRefAccess<CharacterStatsEdit, EditableInputGroupLayoutItemView>("heightEditView");
    private static readonly AccessTools.FieldRef<CharacterStatsEdit, EditableInputGroupLayoutItemView> WeightEditViewRef =
        AccessTools.FieldRefAccess<CharacterStatsEdit, EditableInputGroupLayoutItemView>("weightEditView");

    [HarmonyPatch(typeof(CharacterStatsEdit), "OnEnable")]
    [HarmonyPostfix]
    private static void OnEnablePostfix(CharacterStatsEdit __instance)
    {
        ActiveViews.Add(__instance);
        Refresh(__instance, "OnEnable");
    }

    [HarmonyPatch(typeof(CharacterStatsEdit), "OnSelectedWorkerChanged")]
    [HarmonyPostfix]
    private static void OnSelectedWorkerChangedPostfix(CharacterStatsEdit __instance)
    {
        Refresh(__instance, "OnSelectedWorkerChanged");
    }

    [HarmonyPatch(typeof(CharacterStatsEdit), "OnEditModeEnabled")]
    [HarmonyPostfix]
    private static void OnEditModeEnabledPostfix(CharacterStatsEdit __instance, bool enabled)
    {
        if (enabled)
        {
            Refresh(__instance, "OnEditModeEnabled");
        }
    }

    [HarmonyPatch(typeof(CharacterStatsEdit), "OnDisable")]
    [HarmonyPostfix]
    private static void OnDisablePostfix(CharacterStatsEdit __instance)
    {
        ActiveViews.Remove(__instance);
        var state = StateByView.GetOrCreateValue(__instance);
        var template = SkillsTitleRef(__instance);
        if (template != null && state.OriginalSkillsTitle != null)
        {
            template.SetText(state.OriginalSkillsTitle, true);
        }
    }

    [HarmonyPatch(typeof(CharacterEditController), "NotifyCharacterUpdated")]
    [HarmonyPostfix]
    private static void NotifyCharacterUpdatedPostfix()
    {
        RefreshActiveViews("NotifyCharacterUpdated");
    }

    [HarmonyPatch(typeof(CharacterEditController), "ModifyAge")]
    [HarmonyPostfix]
    private static void ModifyAgePostfix()
    {
        RefreshActiveViews("ModifyAge");
    }

    [HarmonyPatch(typeof(CharacterEditController), "SetAge")]
    [HarmonyPostfix]
    private static void ControllerSetAgePostfix()
    {
        RefreshActiveViews("Controller.SetAge");
    }

    [HarmonyPatch(typeof(CharacterStatsEdit), "SetWeight")]
    [HarmonyPostfix]
    private static void SetWeightPostfix(CharacterStatsEdit __instance)
    {
        Refresh(__instance, "SetWeight");
    }

    [HarmonyPatch(typeof(CharacterStatsEdit), "SetHeight")]
    [HarmonyPostfix]
    private static void SetHeightPostfix(CharacterStatsEdit __instance)
    {
        Refresh(__instance, "SetHeight");
    }

    [HarmonyPatch(typeof(EditableInputGroupLayoutItemView), "OnInputCallback")]
    [HarmonyPostfix]
    private static void OnInputCallbackPostfix(EditableInputGroupLayoutItemView __instance, string s)
    {
        if (TryResolveView(__instance, out var owner, out var editKind))
        {
            Refresh(owner, $"{editKind}.OnInput:{s}");
        }
    }

    [HarmonyPatch(typeof(EditableInputGroupLayoutItemView), "OnButtonCallback")]
    [HarmonyPostfix]
    private static void OnButtonCallbackPostfix(EditableInputGroupLayoutItemView __instance, int value)
    {
        if (TryResolveView(__instance, out var owner, out var editKind))
        {
            Refresh(owner, $"{editKind}.OnButton:{value}");
        }
    }

    private static void Refresh(CharacterStatsEdit __instance, string source)
    {
        if (__instance == null || !__instance.isActiveAndEnabled)
        {
            return;
        }

        var charactersView = CharactersViewRef(__instance);
        if (CharacterEditorGenerationState.IsGenerating(charactersView))
        {
            DiagnosticTrace.InfoSample("carry.ui.stats", "CharacterStatsEdit.Generating", $"Refresh skipped during generation, source={source}");
            return;
        }

        var state = StateByView.GetOrCreateValue(__instance);
        if (state.IsUpdating)
        {
            DiagnosticTrace.InfoSample("carry.ui.stats", "CharacterStatsEdit.Reentrant", $"Refresh skipped reentrant, source={source}");
            return;
        }

        var editController = Traverse.Create(__instance).Property("EditController").GetValue();
        var humanoid = editController == null
            ? null
            : Traverse.Create(editController).Property("SelectedHumanoid").GetValue<HumanoidInstance>();
        if (humanoid == null || humanoid.HasDisposed)
        {
            HideLabel(state);
            DiagnosticTrace.InfoSample("carry.ui.stats", "CharacterStatsEdit.NoHumanoid", $"Refresh skipped no humanoid, source={source}");
            return;
        }

        if (!CarryCapacityRuntimeApplier.TryGetPreviewProfile(humanoid, out var profile) || profile == null)
        {
            HideLabel(state);
            DiagnosticTrace.InfoSample("carry.ui.stats", "CharacterStatsEdit.NoPreviewProfile", $"Refresh skipped no preview profile for {humanoid}, source={source}");
            return;
        }

        var template = SkillsTitleRef(__instance);
        if (template == null)
        {
            DiagnosticTrace.InfoSample("carry.ui.stats", "CharacterStatsEdit.NoTemplate", $"Refresh skipped no skillsTitle template, source={source}");
            return;
        }

        var originalTitle = state.OriginalSkillsTitle ?? template.text;
        state.OriginalSkillsTitle = originalTitle;
        var carryLine = CarryCapacityUiText.BuildCarryInfoLine(profile, "kg");
        var nextText = string.Concat(originalTitle, "  |  ", carryLine);
        if (string.Equals(template.text, nextText, StringComparison.Ordinal))
        {
            return;
        }

        state.IsUpdating = true;
        try
        {
            template.SetText(nextText, true);
            DiagnosticTrace.InfoSample(
                "carry.ui.stats",
                "CharacterStatsEdit.Updated",
                $"Updated preview carry for {humanoid}, source={source}, line='{carryLine}', age={TryReadAgeYears(humanoid)?.ToString() ?? "-"}, height={TryReadHeight(humanoid)?.ToString("0.00") ?? "-"}, weight={TryReadWeight(humanoid)?.ToString("0.00") ?? "-"}");
        }
        finally
        {
            state.IsUpdating = false;
        }
    }

    private static void HideLabel(PatchState state)
    {
        _ = state;
    }

    internal static void RefreshActiveViews(string source)
    {
        foreach (var view in ActiveViews.ToArray())
        {
            Refresh(view, source);
        }
    }

    private static bool TryResolveView(
        EditableInputGroupLayoutItemView? editView,
        out CharacterStatsEdit owner,
        out string editKind)
    {
        foreach (var activeView in ActiveViews)
        {
            if (ReferenceEquals(AgeEditViewRef(activeView), editView))
            {
                owner = activeView;
                editKind = "Age";
                return true;
            }

            if (ReferenceEquals(HeightEditViewRef(activeView), editView))
            {
                owner = activeView;
                editKind = "Height";
                return true;
            }

            if (ReferenceEquals(WeightEditViewRef(activeView), editView))
            {
                owner = activeView;
                editKind = "Weight";
                return true;
            }
        }

        owner = null!;
        editKind = string.Empty;
        return false;
    }

    private static int? TryReadAgeYears(HumanoidInstance humanoid)
    {
        return humanoid?.Info == null
            ? null
            : (int?)Mathf.RoundToInt(humanoid.Info.Age);
    }

    private static float? TryReadHeight(HumanoidInstance humanoid)
    {
        return humanoid?.Info?.Height;
    }

    private static float? TryReadWeight(HumanoidInstance humanoid)
    {
        return humanoid?.Info?.GetWeight();
    }

    private sealed class PatchState
    {
        public string? OriginalSkillsTitle { get; set; }

        public bool IsUpdating { get; set; }
    }

}
