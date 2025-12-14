# Developer Experience & Git Hooks Skills

## Skill-DevEx-001

**Entity Type**: Skill
**Statement**: Pre-commit hooks should auto-fix issues then verify, failing only on unfixable errors
**Atomicity**: 97%
**Category**: Developer Experience
**Context**: When implementing linting hooks for developer workflows
**Evidence**: Session Linting Automation - 9 commits processed smoothly after implementation
**Tag**: helpful
**Impact**: 9
**Validated**: 2

**Summary**: Pre-commit hooks should eliminate friction, not create it. Auto-fix common issues (formatting, language identifiers). Only fail on issues that require manual intervention. Two-layer approach: auto-fix locally, verify in CI.

**Implementation Pattern**:

```bash
#!/bin/bash
# .git/hooks/pre-commit

set -e

# Auto-fix markdown
npx markdownlint-cli2 --fix "**/*.md"

# Auto-fix C#
dotnet format

# Re-stage fixed files
git add .

# Verify any remaining issues would fail CI
echo "Verifying no unfixable issues..."
npx markdownlint-cli2 "**/*.md" || {
  echo "ERROR: Fix markdown issues manually"
  exit 1
}

echo "Pre-commit checks passed"
exit 0
```

---

## Skill-GitHooks-001

**Entity Type**: Skill
**Statement**: Auto-fix hooks must re-stage modified files with `git add` after applying fixes
**Atomicity**: 99%
**Category**: Git-Hooks
**Context**: When implementing auto-fix pre-commit hooks
**Evidence**: Session Linting Automation - Hook explicitly re-stages each fixed file
**Tag**: helpful
**Impact**: 10
**Validated**: 1

**Summary**: Auto-fix modifies files but doesn't stage them. Without re-staging, fixes aren't included in commit. Developer sees "hook passed but changes not committed". Solution: `git add` modified files after fixing.

**Critical Pattern**:

```bash
#!/bin/bash
# WRONG - fixes applied but not staged
npx markdownlint-cli2 --fix "**/*.md"
dotnet format
# Missing: git add to re-stage!
exit 0

# CORRECT - fixes are staged
npx markdownlint-cli2 --fix "**/*.md"
dotnet format
git add .  # Re-stage all fixes!
exit 0
```
