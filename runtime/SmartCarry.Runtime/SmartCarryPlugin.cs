using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SmartCarry.Runtime.Configuration;
using UnityEngine;

namespace SmartCarry.Runtime;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
[BepInProcess("Going Medieval.exe")]
public sealed class SmartCarryPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger { get; private set; } = null!;
    private Harmony? harmony;

    private void Awake()
    {
        Logger = base.Logger;
        SmartCarrySettings.Initialize(Config);
        DiagnosticTrace.Configure(SmartCarrySettings.DiagnosticTraceLevel);
        DiagnosticTrace.StartSession();
        harmony = new Harmony(PluginInfo.Guid);

        Logger.LogInfo($"{PluginInfo.Name} initializing on Unity {Application.unityVersion}.");
        Logger.LogInfo($"Diagnostic trace level: {DiagnosticTrace.CurrentLevel}");
        Logger.LogInfo($"Carry settings: enabled={SmartCarrySettings.EnableDynamicCarryCapacity}, fallbackBase={SmartCarrySettings.DefaultBaseCarryCapacity}, min={SmartCarrySettings.MinimumCarryCapacity}, max={SmartCarrySettings.MaximumCarryCapacity}");
        Logger.LogInfo($"Carry modifiers: healthFloor={SmartCarrySettings.MinimumHealthFactorAtZeroHealth:0.00}, sleepFloor={SmartCarrySettings.MinimumSleepFactorAtZeroSleep:0.00}, wounded={SmartCarrySettings.WoundedCarryFactor:0.00}");
        Logger.LogInfo($"Carry physique: male={SmartCarrySettings.MaleBodyTypeFactor:0.00}, female={SmartCarrySettings.FemaleBodyTypeFactor:0.00}, heightStrength={SmartCarrySettings.HeightFactorStrength:0.00}, weightStrength={SmartCarrySettings.WeightFactorStrength:0.00}");
        Logger.LogInfo($"Carry age: youngAge={SmartCarrySettings.YoungAdultAgeYears}, youngFactor={SmartCarrySettings.YoungAdultCarryFactor:0.00}, prime={SmartCarrySettings.PrimeAgeYears}±{SmartCarrySettings.PrimeAgeBandYears}, seniorAge={SmartCarrySettings.SeniorAgeYears}, seniorFactor={SmartCarrySettings.SeniorCarryFactor:0.00}");

        try
        {
            harmony.PatchAll();
            Logger.LogInfo($"Harmony patched methods: {harmony.GetPatchedMethods().Count()}");
            Logger.LogInfo($"{PluginInfo.Name} loaded.");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to apply Harmony patches: {ex}");
            DiagnosticTrace.Error("bootstrap.error", ex.ToString());
            throw;
        }
    }
}
