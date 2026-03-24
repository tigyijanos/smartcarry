# Developer Guide

## Purpose

SmartCarry is intended to own one thing: how a settler's effective carry capacity is derived.

The hauling, construction, and storage systems should consume the resulting capacity, but they should not each invent their own carry-capacity logic.

## Current Shape

- `SmartCarryPlugin`: plugin bootstrap and config load
- `SmartCarrySettings`: configurable baseline carry-capacity bounds
- `CarryStorageOwnerResolver`: maps a worker carry storage back to its owning humanoid
- `CreatureCarrySignalsReader`: reads live health, sleep, wound, body, age, height, and weight state
- `CarryCapacityInputsFactory`: turns runtime signals into calculator inputs
- `CarryCapacityInputs`: normalized inputs for one settler
- `CarryCapacityProfile`: calculated factors and final capacity breakdown
- `CarryCapacityCalculator`: pure calculator for the effective carry capacity
- `CarryCapacityRuntimeApplier`: applies the derived result back to live worker storage
- `CarryCapacityStoragePatch`: keeps runtime capacity in sync before storage queries run
- `WorkerDetailsCarryCapacityPatch` / `WorkerInventoryCarryCapacityPatch`: surface the live result in gameplay UI
- `CharacterStatsEditCarryCapacityPatch`: shows the preview carry result in the character editor

## Runtime Flow

1. A storage query hits the mod patch.
2. The patch resolves whether that storage belongs to a worker carry inventory.
3. The mod reads the worker's current `Health`, `Sleep`, `IsWounded`, `BodyType`, `Age`, `Height`, and current `Weight` signals.
4. `CarryCapacityInputsFactory` builds normalized calculator inputs.
5. `CarryCapacityCalculator` derives the effective carry capacity.
6. The live storage capacity is updated before the original game query continues.

## Current Model

```text
base carry
  x health factor
  x sleep factor
  x wound factor
  x body type factor
  x age factor
  x height factor
  x weight factor
  -> clamped effective carry
```

## What Is Still Missing

- No trait-based modifier is implemented yet.
- No compatibility layer exists yet for other gameplay mods.
