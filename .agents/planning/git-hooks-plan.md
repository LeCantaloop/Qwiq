# Git Hooks Implementation Plan

## Status: Implemented

## Objective

Set up git hooks to prevent committing files that don't pass the documented linters:

1. **markdownlint-cli2** - for markdown files
2. **dotnet format** - for C# files
3. **pprettier (dotnet tool)** - for JSON/YAML files

## Approach

Since `.git/hooks/` is not version controlled, use a `.githooks/` directory in the repository root and configure git to use it via `core.hooksPath`.

### Directory Structure

```text
.githooks/
  pre-commit       # Shell script for Git Bash/WSL/macOS/Linux
```

### Pre-commit Hook Logic

1. Get list of staged files (`git diff --cached --name-only --diff-filter=ACMR`)
2. Filter and run appropriate linter:
   - `.md` files → `npx markdownlint-cli2 --no-globs` (check mode, no --fix)
   - `.cs` files → `dotnet format --verify-no-changes --no-restore --include <files>`
   - `.json`, `.yaml`, `.yml` files → `dotnet pprettier --check`
3. Exit with error if any linter fails

### Installation

Users run once after cloning:

```bash
git config core.hooksPath .githooks
```

## Key Design Decisions

1. **Bash-only approach** - Git for Windows includes Git Bash, so a single bash script works cross-platform
2. **`--no-globs` for markdownlint** - Prevents config file's globs from expanding to check all files
3. **`--no-restore` for dotnet format** - Speeds up execution by skipping package restore
4. **`--include` for dotnet format** - Scopes check to staged files only
5. **Clear error messages** - Each failure includes remediation command

## Agent Review Feedback (Incorporated)

From csharp-pod:

- Architecture validated as sound
- Recommended bash-only approach (adopted)
- Emphasized tool availability checks (implemented)

From csharp-expert:

- Validated `--include` approach for dotnet format
- Recommended `--no-restore` flag (implemented)
- Noted partial staging edge case (documented as low risk)

## Bypass

If users need to bypass temporarily:

```bash
git commit --no-verify
```
