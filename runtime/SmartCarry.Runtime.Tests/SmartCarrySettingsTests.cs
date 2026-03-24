namespace SmartCarry.Runtime.Tests;

public sealed class SmartCarrySettingsTests
{
    [Fact]
    public void Initialize_ClampsMaximumToMinimum_WhenConfiguredMaximumIsLower()
    {
        // Arrange
        var configPath = Path.Combine(Path.GetTempPath(), $"smartcarry-tests-{Guid.NewGuid():N}.cfg");
        try
        {
            var config = new BepInEx.Configuration.ConfigFile(configPath, true);
            config.Bind("Carry", "EnableDynamicCarryCapacity", true).Value = true;
            config.Bind("Carry", "DefaultBaseCarryCapacity", 60).Value = 60;
            config.Bind("Carry", "MinimumCarryCapacity", 50).Value = 50;
            config.Bind("Carry", "MaximumCarryCapacity", 20).Value = 20;
            config.Save();

            // Act
            SmartCarry.Runtime.Configuration.SmartCarrySettings.Initialize(config);

            // Assert
            Assert.Equal(50, SmartCarry.Runtime.Configuration.SmartCarrySettings.MinimumCarryCapacity);
            Assert.Equal(50, SmartCarry.Runtime.Configuration.SmartCarrySettings.MaximumCarryCapacity);
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
    public void Initialize_ClampsCarryFactorsIntoUnitRange()
    {
        // Arrange
        var configPath = Path.Combine(Path.GetTempPath(), $"smartcarry-tests-{Guid.NewGuid():N}.cfg");
        try
        {
            var config = new BepInEx.Configuration.ConfigFile(configPath, true);
            config.Bind("Carry", "EnableDynamicCarryCapacity", true).Value = true;
            config.Bind("Carry", "MinimumHealthFactorAtZeroHealth", -2f).Value = -2f;
            config.Bind("Carry", "MinimumSleepFactorAtZeroSleep", 5f).Value = 5f;
            config.Bind("Carry", "WoundedCarryFactor", float.NaN).Value = float.NaN;
            config.Save();

            // Act
            SmartCarry.Runtime.Configuration.SmartCarrySettings.Initialize(config);

            // Assert
            Assert.Equal(0f, SmartCarry.Runtime.Configuration.SmartCarrySettings.MinimumHealthFactorAtZeroHealth);
            Assert.Equal(1f, SmartCarry.Runtime.Configuration.SmartCarrySettings.MinimumSleepFactorAtZeroSleep);
            Assert.Equal(1f, SmartCarry.Runtime.Configuration.SmartCarrySettings.WoundedCarryFactor);
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
    public void Initialize_ClampsPhysiqueFactorsIntoSafeRanges()
    {
        // Arrange
        var configPath = Path.Combine(Path.GetTempPath(), $"smartcarry-tests-{Guid.NewGuid():N}.cfg");
        try
        {
            var config = new BepInEx.Configuration.ConfigFile(configPath, true);
            config.Bind("Carry", "MaleBodyTypeFactor", -5f).Value = -5f;
            config.Bind("Carry", "FemaleBodyTypeFactor", 10f).Value = 10f;
            config.Bind("Carry", "HeightFactorStrength", -1f).Value = -1f;
            config.Bind("Carry", "WeightFactorStrength", 1f).Value = 1f;
            config.Bind("Carry", "PrimeAgeYears", 150).Value = 150;
            config.Bind("Carry", "PrimeAgeBandYears", 50).Value = 50;
            config.Bind("Carry", "YoungAdultAgeYears", -5).Value = -5;
            config.Bind("Carry", "YoungAdultCarryFactor", -1f).Value = -1f;
            config.Bind("Carry", "SeniorAgeYears", 10).Value = 10;
            config.Bind("Carry", "SeniorCarryFactor", 5f).Value = 5f;

            // Act
            SmartCarry.Runtime.Configuration.SmartCarrySettings.Initialize(config);

            // Assert
            Assert.Equal(1.20f, SmartCarry.Runtime.Configuration.SmartCarrySettings.MaleBodyTypeFactor, 3);
            Assert.Equal(1.8f, SmartCarry.Runtime.Configuration.SmartCarrySettings.FemaleBodyTypeFactor, 3);
            Assert.Equal(0f, SmartCarry.Runtime.Configuration.SmartCarrySettings.HeightFactorStrength, 3);
            Assert.Equal(0.5f, SmartCarry.Runtime.Configuration.SmartCarrySettings.WeightFactorStrength, 3);
            Assert.Equal(30, SmartCarry.Runtime.Configuration.SmartCarrySettings.PrimeAgeYears);
            Assert.Equal(20, SmartCarry.Runtime.Configuration.SmartCarrySettings.PrimeAgeBandYears);
            Assert.Equal(18, SmartCarry.Runtime.Configuration.SmartCarrySettings.YoungAdultAgeYears);
            Assert.Equal(0.82f, SmartCarry.Runtime.Configuration.SmartCarrySettings.YoungAdultCarryFactor, 3);
            Assert.Equal(50, SmartCarry.Runtime.Configuration.SmartCarrySettings.SeniorAgeYears);
            Assert.Equal(1f, SmartCarry.Runtime.Configuration.SmartCarrySettings.SeniorCarryFactor, 3);
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
