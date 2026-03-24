# SmartCarry: Going Medieval Mod

SmartCarry is an unofficial carry-capacity mod for Going Medieval.

## What It Does

SmartCarry stops treating every settler like they should carry the same amount.

Right now a settler's effective carry capacity is based on:
- their normal game carry capacity
- current health
- current sleep stat
- whether they are wounded
- body type
- age
- height
- current weight

Current live model:

```text
effective carry
  = base carry
  x health modifier
  x sleep modifier
  x wound modifier
  x body modifier
  x age modifier
  x height modifier
  x weight modifier
  -> clamped min/max
```

What this means in practice:
- healthier settlers carry more
- exhausted settlers carry less
- wounded settlers carry a bit less
- body type shifts the baseline up or down
- settlers near their prime years carry more; older settlers taper off
- taller settlers get a clearer bump, shorter settlers a clearer penalty
- heavier settlers get a clearer bump, lighter settlers a clearer penalty
- the selected worker details panel shows the live carry value next to weight
- the character editor preview also shows the current carry value
- trait-based modifiers are not wired yet

## Install

1. Install BepInEx 5 into the game directory.
2. Extract the release archive into your Going Medieval game directory.
3. Verify that the DLL ends up here:

```text
<Going Medieval>\BepInEx\plugins\SmartCarry.Runtime\
```

4. Restart the game.

If you want to build from source instead, see [docs/development-setup.md](docs/development-setup.md).

## Configuration

- The mod writes its settings to:

```text
<Going Medieval>\BepInEx\config\hu.tigyi.goingmedieval.smartcarry.runtime.cfg
```

- Current settings control the live carry-capacity model:

```ini
[Carry]
EnableDynamicCarryCapacity = true
DefaultBaseCarryCapacity = 30
MinimumCarryCapacity = 12
MaximumCarryCapacity = 60
MinimumHealthFactorAtZeroHealth = 0.65
MinimumSleepFactorAtZeroSleep = 0.85
WoundedCarryFactor = 0.9
MaleBodyTypeFactor = 1.20
FemaleBodyTypeFactor = 0.80
HeightFactorStrength = 0.20
WeightFactorStrength = 0.30
PrimeAgeYears = 30
PrimeAgeBandYears = 5
YoungAdultAgeYears = 18
YoungAdultCarryFactor = 0.82
SeniorAgeYears = 60
SeniorCarryFactor = 0.72

[Tracing]
DiagnosticTraceLevel = Off
```

- `DefaultBaseCarryCapacity` is only a fallback.
- In normal use, SmartCarry starts from the settler's observed game carry capacity and modifies that.

## Unofficial

This project is not official and is not affiliated with, endorsed by, or supported by the game developers or publisher.

## Disclaimer

- This mod changes Going Medieval runtime behaviour and can cause unexpected behaviour, errors, or mod conflicts.
- Use it at your own risk.
- Back up your saves before testing new versions.
- The software is provided without warranty; see [LICENSE](LICENSE).

## Author

- [tigyijanos](https://github.com/tigyijanos)

## Dependencies

- [Going Medieval](https://mythwright.com/games/going-medieval)
- [BepInEx 5](https://docs.bepinex.dev/)
- [Windows](https://www.microsoft.com/en-us/windows/)

## Legal

- This workspace's own code is licensed under [MIT](LICENSE).
- Third-party dependencies and platform terms are listed in [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).
- This workspace is source-only.
- It does not include game binaries, copied game assets, or decompiled game source.
- You need your own licensed local game installation to build the project.

## Documentation

- Changelog: [CHANGELOG.md](CHANGELOG.md)
- Developer guide: [docs/developer-guide.md](docs/developer-guide.md)
- Development setup: [docs/development-setup.md](docs/development-setup.md)
- Runtime flow: [docs/architecture-flow.md](docs/architecture-flow.md)
- Carry mental map: [docs/carry-capacity-mental-map.md](docs/carry-capacity-mental-map.md)
- Feasibility notes: [docs/smart-carry-feasibility.md](docs/smart-carry-feasibility.md)
- Nexus listing text: [docs/nexus-listing.md](docs/nexus-listing.md)
