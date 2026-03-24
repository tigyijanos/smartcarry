using NSMedieval.State;
using NSMedieval.StatsSystem;
using NSMedieval;
using HarmonyLib;

namespace SmartCarry.Runtime;

/// <summary>
/// Reads the live creature stats that feed the carry-capacity model.
/// </summary>
internal static class CreatureCarrySignalsReader
{
    public static CarrySignalSnapshot Read(CreatureBase creature, int baselineCapacity)
    {
        var healthNormalized = ReadNormalizedStat(creature, StatType.Health);
        var sleepNormalized = ReadNormalizedStat(creature, StatType.Sleep);
        var isWounded = ReadIsWounded(creature);
        var bodyType = BodyType.None;
        var ageYears = 0;
        var height = 1f;
        var weightCoefficient = 1f;

        if (creature is HumanoidInstance humanoid)
        {
            bodyType = ReadBodyType(humanoid);
            ageYears = ReadAgeYears(humanoid);
            height = ReadHeight(humanoid);
            weightCoefficient = ReadWeightCoefficient(humanoid);
        }

        return new CarrySignalSnapshot(
            baseCapacity: baselineCapacity,
            normalizedHealth: healthNormalized,
            normalizedSleep: sleepNormalized,
            isWounded: isWounded,
            bodyType: bodyType,
            ageYears: ageYears,
            height: height,
            weightCoefficient: weightCoefficient,
            sourceDescription: $"health={healthNormalized:0.00}, sleep={sleepNormalized:0.00}, wounded={isWounded}, body={bodyType}, age={ageYears}, height={height:0.00}, weight={weightCoefficient:0.00}");
    }

    private static float ReadNormalizedStat(CreatureBase creature, StatType statType)
    {
        try
        {
            var stat = creature.GetStat(statType);
            if (stat == null)
            {
                return 1f;
            }

            var maximum = stat.Max;
            if (maximum <= 0f)
            {
                return 1f;
            }

            return Math.Clamp(stat.Current / maximum, 0f, 1f);
        }
        catch (Exception exception)
        {
            DiagnosticTrace.InfoSample(
                "carry.runtime",
                $"CreatureCarrySignalsReader.ReadNormalizedStat.{statType}",
                $"Failed to read {statType} for {creature}: {exception.GetType().Name}");
            return 1f;
        }
    }

    private static bool ReadIsWounded(CreatureBase creature)
    {
        try
        {
            return creature.IsWounded;
        }
        catch (Exception exception)
        {
            DiagnosticTrace.InfoSample(
                "carry.runtime",
                "CreatureCarrySignalsReader.ReadIsWounded",
                $"Failed to read IsWounded for {creature}: {exception.GetType().Name}");
            return false;
        }
    }

    private static BodyType ReadBodyType(HumanoidInstance humanoid)
    {
        try
        {
            return humanoid.Info?.BodyType ?? BodyType.None;
        }
        catch (Exception exception)
        {
            DiagnosticTrace.InfoSample(
                "carry.runtime",
                "CreatureCarrySignalsReader.ReadBodyType",
                $"Failed to read BodyType for {humanoid}: {exception.GetType().Name}");
            return BodyType.None;
        }
    }

    private static float ReadHeight(HumanoidInstance humanoid)
    {
        try
        {
            return humanoid.Info?.Height ?? 1f;
        }
        catch (Exception exception)
        {
            DiagnosticTrace.InfoSample(
                "carry.runtime",
                "CreatureCarrySignalsReader.ReadHeight",
                $"Failed to read Height for {humanoid}: {exception.GetType().Name}");
            return 1f;
        }
    }

    private static float ReadWeightCoefficient(HumanoidInstance humanoid)
    {
        try
        {
            return humanoid.Info?.GetWeight() ?? 1f;
        }
        catch (Exception exception)
        {
            DiagnosticTrace.InfoSample(
                "carry.runtime",
                "CreatureCarrySignalsReader.ReadWeightCoefficient",
                $"Failed to read WeightCoefficient for {humanoid}: {exception.GetType().Name}");
            return 1f;
        }
    }

    private static int ReadAgeYears(HumanoidInstance humanoid)
    {
        try
        {
            var humanoidTraverse = Traverse.Create(humanoid);
            var directAge = TryReadInt(humanoidTraverse, "Age");
            if (directAge > 0)
            {
                return directAge;
            }

            var info = humanoid.Info;
            if (info == null)
            {
                return 0;
            }

            var infoTraverse = Traverse.Create(info);
            return TryReadInt(infoTraverse, "Age");
        }
        catch (Exception exception)
        {
            DiagnosticTrace.InfoSample(
                "carry.runtime",
                "CreatureCarrySignalsReader.ReadAgeYears",
                $"Failed to read Age for {humanoid}: {exception.GetType().Name}");
            return 0;
        }
    }

    private static int TryReadInt(Traverse traverse, string memberName)
    {
        try
        {
            var propertyValue = traverse.Property(memberName).GetValue();
            if (propertyValue != null)
            {
                return ConvertToInt(propertyValue);
            }
        }
        catch
        {
        }

        try
        {
            var fieldValue = traverse.Field(memberName).GetValue();
            if (fieldValue != null)
            {
                return ConvertToInt(fieldValue);
            }
        }
        catch
        {
        }

        return 0;
    }

    private static int ConvertToInt(object value)
    {
        return value switch
        {
            int intValue => intValue,
            float floatValue => (int)MathF.Round(floatValue, MidpointRounding.AwayFromZero),
            double doubleValue => (int)Math.Round(doubleValue, MidpointRounding.AwayFromZero),
            _ => 0
        };
    }
}
