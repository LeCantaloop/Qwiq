# Session 36: Git Hooks for Linting Enforcement

**Date**: 2025-12-13
**Duration**: ~30 minutes
**Focus**: Developer tooling - pre-commit hooks

## Objective

Set up git hooks to prevent committing files that don't pass the documented linters.

## What Was Done

### 1. Planning Phase

- Created implementation plan at `.agents/plans/git-hooks-plan.md`
- Evaluated approaches: Husky, lefthook, custom scripts
- Chose `.githooks/` directory with `core.hooksPath` configuration

### 2. Agent Review

Consulted specialized agents for architecture review:

**csharp-pod feedback**:

- Architecture validated as sound
- Recommended bash-only approach (adopted)
- Emphasized tool availability checks (implemented)

**csharp-expert feedback**:

- Validated `--include` approach for dotnet format
- Recommended `--no-restore` flag (implemented)
- Noted partial staging edge case (documented as low risk)

### 3. Implementation

Created `.githooks/pre-commit` bash script that:

- Gets staged files via `git diff --cached --name-only --diff-filter=ACMR`
- Runs `markdownlint-cli2 --no-globs` on staged `.md` files
- Runs `dotnet format --verify-no-changes --no-restore --include` on staged `.cs` files
- Runs `dotnet pprettier --check` on staged `.json`/`.yaml`/`.yml` files
- Provides colored output and actionable error messages
- Checks tool availability before running

### 4. Documentation

Added "Git Hooks" section to `CONTRIBUTING.md` with:

- Setup instructions (`git config core.hooksPath .githooks`)
- How it works (staged files only, fast)
- Bypass instructions (`git commit --no-verify`)

## Key Design Decisions

| Decision                         | Rationale                                                    |
| -------------------------------- | ------------------------------------------------------------ |
| Bash-only (no PowerShell)        | Git for Windows includes Git Bash; single script to maintain |
| `--no-globs` for markdownlint    | Prevents config's globs from checking all files              |
| `--no-restore` for dotnet format | Speeds up execution (assumes restore done during dev)        |
| `--include` for dotnet format    | Scopes check to staged files only                            |
| Block on missing tools           | Graceful degradation defeats purpose of hooks                |

## Files Changed

| File                              | Change                                 |
| --------------------------------- | -------------------------------------- |
| `.githooks/pre-commit`            | **Created** - 176 lines                |
| `CONTRIBUTING.md`                 | **Modified** - Added Git Hooks section |
| `.agents/plans/git-hooks-plan.md` | **Created** - Implementation plan      |

## Commits

```text
ccef65a5 chore: add pre-commit hooks for linting enforcement
```

## Testing

- Hook tested with staged markdown files - passed
- Hook tested with lint violations - correctly blocked commit
- Hook tested with mixed file types - all linters run appropriately

## Next Steps

- Monitor hook performance on real commits
- Consider adding commit-msg hook for conventional commits enforcement
- May add husky-style auto-setup via post-checkout hook in future
