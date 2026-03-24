# Nexus Listing

## Suggested Title

`SmartCarry - Dynamic Settler Carry Capacity`

## Short Description

`Dynamic carry capacity for Going Medieval settlers based on health, sleep, wounds, body type, age, height, and weight.`

## Full Description

### Description

SmartCarry improves how carry capacity works in Going Medieval.

Instead of giving every settler the same flat carry limit, the mod derives an effective carry capacity from the settler's current state and physique. Healthier and better-rested settlers can carry more. Wounded settlers carry less. Body type, age, height, and weight also affect the final result.

The goal is simple: make carry strength feel more believable while still staying readable, configurable, and compatible with the rest of the game's logistics systems.

### Installation Instructions

1. Install BepInEx 5 for Going Medieval.
2. Download the SmartCarry release archive.
3. Extract the archive into your Going Medieval game folder.
4. Make sure the DLL ends up here:

```text
<Going Medieval>\BepInEx\plugins\SmartCarry.Runtime\SmartCarry.Runtime.dll
```

5. Start or restart the game.

Optional:
After first launch, the config file will be created here:

```text
<Going Medieval>\BepInEx\config\hu.tigyi.goingmedieval.smartcarry.runtime.cfg
```

### Main Features

- Dynamic carry capacity instead of a flat value for every settler
- Carry capacity reacts to health, sleep, wounds, body type, age, height, and current weight
- Live runtime integration so gameplay uses the calculated capacity
- Selected worker UI shows the current effective carry value
- Character editor preview shows the current carry value while editing
- Configurable factors and bounds through BepInEx config

### Requirements

- Going Medieval
- BepInEx 5
- Windows

Notes:

- This is an unofficial mod.
- This mod changes runtime behaviour and may conflict with other mods that patch the same systems.
- Backing up saves before testing new mod versions is recommended.

### Shout Outs

- Thanks to the Going Medieval developers.
- Thanks to the BepInEx and Harmony communities.
- Thanks to players testing preview UI edge cases and reporting carry-model issues.
