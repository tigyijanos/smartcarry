# Changelog

## Unreleased

- No unreleased changes yet.

## 1.1.0 - 2026-03-24

- Reduced carry getter overhead by reusing a short-lived live profile cache instead of rebuilding the same profile on every hot-path query.
- Removed the duplicate carry profile rebuild from the getter-to-apply path so effective capacity updates no longer compute twice for the same request.
- Reduced avoidable owner-resolution work when establishing baseline capacity for live worker storage.

## 1.0.0 - 2026-03-24

- Added a live dynamic carry-capacity model for Going Medieval settlers.
- Carry capacity now reacts to health, sleep, wounds, body type, age, height, and current weight.
- Patched runtime storage and carry getter queries so gameplay uses the derived capacity.
- Added carry-capacity UI to the selected worker panel and to the character editor preview.
- Added configuration, diagnostics, tests, and developer documentation.
