# Developer Experience Skills

## Skill-DevEx-001

**Statement**: Pre-commit hooks should auto-fix issues then verify, failing only on unfixable errors

**Atomicity**: 97%

**Category**: Developer Experience

**Context**: When implementing linting hooks for developer workflows

**Evidence**: Session Linting Automation - 9 commits processed smoothly after implementation

**Details**:

- Pre-commit hooks should eliminate friction, not create it
- Auto-fix common issues (formatting, language identifiers)
- Only fail on issues that require manual intervention
- Two-layer approach: auto-fix locally, verify in CI

**Hook Philosophy**:

```text
Pre-Commit Hook Flow:

File change detected
       ↓
  Apply fixes (auto-fix)
       ↓
  Re-stage fixed files
       ↓
  Verify unfixable issues
       ↓
  Pass? → Commit allowed
  Fail? → User must fix manually
```

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

**Key Principles**:

1. **Auto-fix First**: Don't make developers fix formatting
2. **Re-stage Files**: Include fixes in the commit
3. **Fail on Real Issues**: Only fail for structural problems
4. **Fast Feedback**: Hooks should complete in seconds
5. **User-Friendly**: Clear error messages for manual fixes

**Benefits**:

- Developers don't waste time on formatting
- CI failures reduced dramatically
- Consistent style across repository
- Linters run without user thinking about it

**Testing Hook**:

```bash
# Test hook behavior
cd test-repo
echo "test" > file.md
git add file.md
git commit -m "test"  # Hook should auto-fix

# Verify files were fixed and committed
git show  # Check if fixes were included
```

---

## Skill-GitHooks-001

**Statement**: Auto-fix hooks must re-stage modified files with `git add` after applying fixes

**Atomicity**: 99%

**Category**: Git-Hooks

**Context**: When implementing auto-fix pre-commit hooks

**Evidence**: Session Linting Automation - Hook explicitly re-stages each fixed file

**Details**:

- Auto-fix modifies files but doesn't stage them
- Without re-staging, fixes aren't included in commit
- Developer sees "hook passed but changes not committed"
- Solution: `git add` modified files after fixing

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

**Detailed Implementation**:

```bash
#!/bin/bash
# .git/hooks/pre-commit - COMPLETE EXAMPLE

set -e

# Get list of staged files
STAGED_FILES=$(git diff --cached --name-only)

# Apply auto-fixes
echo "Running auto-fixes..."
npx markdownlint-cli2 --fix "**/*.md" || true
dotnet format || true

# Re-stage any files that were modified by fixes
echo "Re-staging fixed files..."
git add .

# Now verify no unfixable issues remain
echo "Verifying fixes..."
npx markdownlint-cli2 "**/*.md" || {
  echo "ERROR: Unfixable linting issues found"
  exit 1
}

echo "✓ Pre-commit checks passed"
exit 0
```

**Why This Matters**:

Without re-staging:

1. Hook applies fixes to file.md
2. Hook passes
3. Commit is made
4. file.md in working directory is fixed
5. file.md in commit is NOT fixed
6. CI fails on original file content

With re-staging:

1. Hook applies fixes to file.md
2. `git add` stages the fixed version
3. Commit is made
4. Commit contains fixed file.md
5. CI sees fixed file and passes

**Verification**:

```bash
# After commit, verify fixes were included
git show HEAD:path/to/file.md  # Shows fixed version
git show --stat  # Shows file was modified in commit
```

**Common Mistakes**:

- ❌ Forgetting `git add` after `npx markdownlint-cli2 --fix`
- ❌ Using `git add [specific-files]` instead of `git add .`
- ❌ Not preserving staged changes (use `git diff --cached`)
- ❌ Applying fixes but failing hook (developer confused)

**Best Practice**:

Always verify hook re-stages by:

1. Making a file with linting issues
2. Staging it with `git add`
3. Running pre-commit hook
4. Checking `git diff --cached` shows fixes
