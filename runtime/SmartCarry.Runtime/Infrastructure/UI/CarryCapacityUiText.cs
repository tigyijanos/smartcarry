namespace SmartCarry.Runtime;

/// <summary>
/// Formats SmartCarry values for small UI surfaces without duplicating model logic.
/// </summary>
internal static class CarryCapacityUiText
{
    private const string CarryMarker = " / carry ";
    private const string CarryInfoPrefix = "Carry ";

    public static string AppendCarryLabel(string? existingText, CarryCapacityProfile profile)
    {
        var baseText = StripCarryLabel(existingText);
        return $"{baseText}{CarryMarker}{profile.EffectiveCapacity}";
    }

    public static string AppendCarryMassLabel(string? existingText, CarryCapacityProfile profile, string massUnit)
    {
        var baseText = StripCarryLabel(existingText);
        return $"{baseText}{CarryMarker}{profile.EffectiveCapacity} {massUnit}";
    }

    public static string BuildCarryTooltip(CarryCapacityProfile profile)
    {
        return $"Carry {profile.EffectiveCapacity}";
    }

    public static string BuildCarryInfoLine(CarryCapacityProfile profile, string massUnit)
    {
        return $"{CarryInfoPrefix}{profile.EffectiveCapacity} {massUnit}";
    }

    public static string BuildMassCarriedText(float currentWeight, int effectiveCapacity)
    {
        return $"{FormatMass(currentWeight)}/{effectiveCapacity}";
    }

    public static void UpsertCarryInfoLine(IList<string> infos, CarryCapacityProfile profile, string massUnit)
    {
        var carryLine = $"{CarryInfoPrefix}{profile.EffectiveCapacity} {massUnit}";
        for (var index = 0; index < infos.Count; index++)
        {
            if (infos[index].StartsWith(CarryInfoPrefix, StringComparison.OrdinalIgnoreCase))
            {
                infos[index] = carryLine;
                return;
            }
        }

        infos.Add(carryLine);
    }

    private static string StripCarryLabel(string? existingText)
    {
        if (string.IsNullOrWhiteSpace(existingText))
        {
            return "Weight";
        }

        var markerIndex = existingText.IndexOf(CarryMarker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex < 0)
        {
            return existingText;
        }

        return existingText[..markerIndex];
    }

    private static string FormatMass(float value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
        {
            return "0";
        }

        return value.ToString("0.#");
    }
}
