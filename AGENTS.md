# SmartCarry Repository Instructions

## Branch and Pull Request Workflow

- Treat `main` as a protected branch when the repository is created.
- Prefer feature branches for isolated work.
- Prefer pull requests for reviewable changes.
- Keep commits focused and behavior-preserving when refactoring.

## Commit Attribution

When a change is materially co-authored, add one or more `Co-authored-by` trailers:

```text
Co-authored-by: Full Name <email@example.com>
```

Example used in this project:

```text
Co-authored-by: Codex (OpenAI) <noreply@openai.com>
```

## Repository Hygiene

- Do not commit decompiled game code, proprietary game assets, or copied game binaries.
- Keep public documentation in English.
- Keep machine-specific settings out of source control.
- Prefer small, test-backed changes over speculative large rewrites.

## Versioning Convention

- Use the same `MAJOR.MINOR.PATCH` version format as the other TigyiMods repositories.
- Do not introduce shortened release versions such as `2.1`; use `2.1.0` instead.
- Keep `PluginInfo.Version`, changelog headings, Git tags, GitHub release titles, and release asset names aligned to the same version string.
- Prefer normal patch/minor bumps to normalize formatting going forward; do not rewrite already-published versions only to change formatting.

## Release Packaging Convention

- Release ZIPs should contain only the installable plugin payload and small user-facing text files.
- For this repository, the expected ZIP contents are:
  - `BepInEx/plugins/SmartCarry.Runtime/SmartCarry.Runtime.dll`
  - `LICENSE.txt`
  - `README.txt`
- `README.txt` in the release asset should be a short install-focused text, not the full repository README.
- Keep the release asset structure stable across versions unless there is a deliberate packaging change.
- Recommended `README.txt` shape:

```text
Smart Carry vX.Y.Z

Install:
1. Install BepInEx 5 for Going Medieval.
2. Extract this archive into your Going Medieval game folder.
3. Verify that SmartCarry.Runtime.dll ends up at:
   BepInEx\\plugins\\SmartCarry.Runtime\\SmartCarry.Runtime.dll
4. Restart the game.

Optional config after first launch:
BepInEx\\config\\hu.tigyi.goingmedieval.smartcarry.runtime.cfg
```

- Recommended GitHub release note shape:

```md
# Smart Carry vX.Y.Z

One-line release summary.

## Highlights

- First user-visible change
- Second user-visible change
- Third user-visible change

## Installation

1. Install BepInEx 5 for Going Medieval.
2. Extract `SmartCarry-vX.Y.Z.zip` into your Going Medieval game folder.
3. Restart the game.

Optional config after first launch:
`BepInEx\\config\\hu.tigyi.goingmedieval.smartcarry.runtime.cfg`
```

## Validation Expectations

- Run `dotnet build` before committing code changes.
- Run `dotnet test` when changing calculator, policy, or other test-covered logic.
- Keep behavior changes explicit in docs and changelog entries.

## Test Standards

- Use xUnit for unit tests.
- Follow the AAA pattern explicitly: Arrange, Act, Assert.
- Prefer method names in the `MethodName_Scenario_ExpectedOutcome` style.
- Use FakeItEasy when isolating collaborators or interface-based dependencies.
- Do not force mocks into pure-function tests.
