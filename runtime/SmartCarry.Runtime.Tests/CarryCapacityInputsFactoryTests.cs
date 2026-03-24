namespace SmartCarry.Runtime.Tests;

public sealed class CarryCapacityInputsFactoryTests
{
    [Fact]
    public void Create_InterpolatesHealthAndSleepFloors_AndAppliesWoundPenalty()
    {
        // Arrange
        var configPath = Path.Combine(Path.GetTempPath(), $"smartcarry-inputs-{Guid.NewGuid():N}.cfg");
        try
        {
            var config = new BepInEx.Configuration.ConfigFile(configPath, true);
            config.Bind("Carry", "DefaultBaseCarryCapacity", 60).Value = 60;
            config.Bind("Carry", "MinimumCarryCapacity", 40).Value = 40;
            config.Bind("Carry", "MaximumCarryCapacity", 90).Value = 90;
            config.Bind("Carry", "MinimumHealthFactorAtZeroHealth", 0.6f).Value = 0.6f;
            config.Bind("Carry", "MinimumSleepFactorAtZeroSleep", 0.8f).Value = 0.8f;
            config.Bind("Carry", "WoundedCarryFactor", 0.9f).Value = 0.9f;
            config.Bind("Carry", "MaleBodyTypeFactor", 1.20f).Value = 1.20f;
            config.Bind("Carry", "FemaleBodyTypeFactor", 0.80f).Value = 0.80f;
            config.Bind("Carry", "HeightFactorStrength", 0.20f).Value = 0.20f;
            config.Bind("Carry", "WeightFactorStrength", 0.30f).Value = 0.30f;
            config.Bind("Carry", "PrimeAgeYears", 30).Value = 30;
            config.Bind("Carry", "PrimeAgeBandYears", 5).Value = 5;
            config.Bind("Carry", "YoungAdultAgeYears", 18).Value = 18;
            config.Bind("Carry", "YoungAdultCarryFactor", 0.82f).Value = 0.82f;
            config.Bind("Carry", "SeniorAgeYears", 60).Value = 60;
            config.Bind("Carry", "SeniorCarryFactor", 0.72f).Value = 0.72f;
            SmartCarry.Runtime.Configuration.SmartCarrySettings.Initialize(config);

            var snapshot = new CarrySignalSnapshot(
                baseCapacity: 60,
                normalizedHealth: 0.5f,
                normalizedSleep: 0.25f,
                isWounded: true,
                bodyType: NSMedieval.BodyType.Male,
                ageYears: 30,
                height: 1.1f,
                weightCoefficient: 1.15f);

            // Act
            var result = CarryCapacityInputsFactory.Create(snapshot);

            // Assert
            Assert.Equal(0.8f, result.HealthFactor, 3);
            Assert.Equal(0.85f, result.FatigueFactor, 3);
            Assert.Equal(0.9f, result.InjuryFactor, 3);
            Assert.Equal(1f, result.AgeFactor, 3);
            Assert.Equal(1f, result.TraitFactor);
            Assert.Equal(1.8f, result.FrameFactor, 3);
        }
        finally
        {
            if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }
        }
    }

    [Fact]
    public void Create_ClampsInvalidNormalizedSignals()
    {
        // Arrange
        var configPath = Path.Combine(Path.GetTempPath(), $"smartcarry-inputs-{Guid.NewGuid():N}.cfg");
        try
        {
            var config = new BepInEx.Configuration.ConfigFile(configPath, true);
            config.Bind("Carry", "MinimumHealthFactorAtZeroHealth", 0.65f).Value = 0.65f;
            config.Bind("Carry", "MinimumSleepFactorAtZeroSleep", 0.85f).Value = 0.85f;
            config.Bind("Carry", "WoundedCarryFactor", 0.9f).Value = 0.9f;
            config.Bind("Carry", "HeightFactorStrength", float.NaN).Value = float.NaN;
            config.Bind("Carry", "WeightFactorStrength", 0.05f).Value = 0.05f;
            config.Bind("Carry", "YoungAdultCarryFactor", 0.82f).Value = 0.82f;
            config.Bind("Carry", "SeniorCarryFactor", 0.72f).Value = 0.72f;
            SmartCarry.Runtime.Configuration.SmartCarrySettings.Initialize(config);

            var snapshot = new CarrySignalSnapshot(
                baseCapacity: 60,
                normalizedHealth: -1f,
                normalizedSleep: float.NaN,
                isWounded: false,
                bodyType: NSMedieval.BodyType.None,
                ageYears: 0,
                height: float.NaN,
                weightCoefficient: -1f);

            // Act
            var result = CarryCapacityInputsFactory.Create(snapshot);

            // Assert
            Assert.Equal(0.65f, result.HealthFactor, 3);
            Assert.Equal(1f, result.FatigueFactor, 3);
            Assert.Equal(1f, result.InjuryFactor);
            Assert.Equal(1f, result.AgeFactor, 3);
            Assert.Equal(0.95f, result.FrameFactor, 3);
        }
        finally
        {
            if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }
        }
    }

    [Fact]
    public void Create_AppliesFemaleBodyTypeAndLowerPhysiqueSignals()
    {
        // Arrange
        var configPath = Path.Combine(Path.GetTempPath(), $"smartcarry-inputs-{Guid.NewGuid():N}.cfg");
        try
        {
            var config = new BepInEx.Configuration.ConfigFile(configPath, true);
            config.Bind("Carry", "MaleBodyTypeFactor", 1.20f).Value = 1.20f;
            config.Bind("Carry", "FemaleBodyTypeFactor", 0.80f).Value = 0.80f;
            config.Bind("Carry", "HeightFactorStrength", 0.20f).Value = 0.20f;
            config.Bind("Carry", "WeightFactorStrength", 0.30f).Value = 0.30f;
            config.Bind("Carry", "PrimeAgeYears", 30).Value = 30;
            config.Bind("Carry", "PrimeAgeBandYears", 5).Value = 5;
            config.Bind("Carry", "YoungAdultAgeYears", 18).Value = 18;
            config.Bind("Carry", "YoungAdultCarryFactor", 0.82f).Value = 0.82f;
            config.Bind("Carry", "SeniorAgeYears", 60).Value = 60;
            config.Bind("Carry", "SeniorCarryFactor", 0.72f).Value = 0.72f;
            SmartCarry.Runtime.Configuration.SmartCarrySettings.Initialize(config);

            var snapshot = new CarrySignalSnapshot(
                baseCapacity: 60,
                normalizedHealth: 1f,
                normalizedSleep: 1f,
                isWounded: false,
                bodyType: NSMedieval.BodyType.Female,
                ageYears: 50,
                height: 0.8f,
                weightCoefficient: 0.8f);

            // Act
            var result = CarryCapacityInputsFactory.Create(snapshot);

            // Assert
            Assert.Equal(0.832f, result.AgeFactor, 3);
            Assert.Equal(0.48f, result.FrameFactor, 3);
        }
        finally
        {
            if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }
        }
    }

    [Fact]
    public void Create_AppliesAgePenaltyAfterPrimeYears()
    {
        var configPath = Path.Combine(Path.GetTempPath(), $"smartcarry-inputs-{Guid.NewGuid():N}.cfg");
        try
        {
            var config = new BepInEx.Configuration.ConfigFile(configPath, true);
            config.Bind("Carry", "PrimeAgeYears", 30).Value = 30;
            config.Bind("Carry", "PrimeAgeBandYears", 5).Value = 5;
            config.Bind("Carry", "YoungAdultAgeYears", 18).Value = 18;
            config.Bind("Carry", "YoungAdultCarryFactor", 0.82f).Value = 0.82f;
            config.Bind("Carry", "SeniorAgeYears", 60).Value = 60;
            config.Bind("Carry", "SeniorCarryFactor", 0.72f).Value = 0.72f;
            SmartCarry.Runtime.Configuration.SmartCarrySettings.Initialize(config);

            var primeSnapshot = new CarrySignalSnapshot(
                baseCapacity: 30,
                normalizedHealth: 1f,
                normalizedSleep: 1f,
                isWounded: false,
                bodyType: NSMedieval.BodyType.None,
                ageYears: 30,
                height: 1f,
                weightCoefficient: 1f);
            var olderSnapshot = new CarrySignalSnapshot(
                baseCapacity: 30,
                normalizedHealth: 1f,
                normalizedSleep: 1f,
                isWounded: false,
                bodyType: NSMedieval.BodyType.None,
                ageYears: 50,
                height: 1f,
                weightCoefficient: 1f);

            var prime = CarryCapacityInputsFactory.Create(primeSnapshot);
            var older = CarryCapacityInputsFactory.Create(olderSnapshot);

            Assert.Equal(1f, prime.AgeFactor, 3);
            Assert.Equal(0.832f, older.AgeFactor, 3);
            Assert.True(older.AgeFactor < prime.AgeFactor);
        }
        finally
        {
            if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }
        }
    }
}
