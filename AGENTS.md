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
