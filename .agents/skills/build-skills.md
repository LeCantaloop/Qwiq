# Build & CI Skills

## Skill-Build-001

**Statement**: Use `/p:ContinuousIntegrationBuild=true /p:UseSharedCompilation=false /m:1 /nodeReuse:false` for local builds to match CI analyzer strictness

**Atomicity**: 96%

**Category**: Build

**Context**: Before pushing changes that passed local builds

**Evidence**: Session 39 - Fixed CA1711, CA1001, CA1861 errors that passed locally but failed CI

**Details**:

- Local builds without CI flags don't enable strict analyzer rules
- CI pipeline enables these flags by default
- Mismatch causes "works locally, fails in CI" situations
- Always use full CI build command before pushing

**Application Example**:

```powershell
# WRONG - may pass but CI will fail
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false

# CORRECT - matches CI behavior
dotnet build Qwiq.sln -c Release /p:ContinuousIntegrationBuild=true /p:UseSharedCompilation=false /m:1 /nodeReuse:false
```

**Common Errors Caught**:

- CA1711: Identifier contains type name (e.g., classes ending in "Collection")
- CA1001: Types that own disposable fields
- CA1861: Avoid passing initialization data to const arrays

---

## Skill-Build-002

**Statement**: Set `BuildInParallel=false` and `ProduceReferenceAssembly=false` for Windows multi-framework builds

**Atomicity**: 93%

**Category**: Build

**Context**: When experiencing CS0006 "metadata file not found" errors in multi-targeting

**Evidence**: Session CS0006-fix - Directory.Build.props lines 88-96

**Details**:

- Inner-build parallelism causes reference assembly conflicts on Windows
- Multi-targeting builds (.NET Framework + .NET Core) need special handling
- Error manifests as "The metadata file X cannot be found"
- Infrastructure fix in Directory.Build.props prevents race conditions

**Application**:

Add to Directory.Build.props:

```xml
<!-- Workaround for Windows multi-framework builds -->
<PropertyGroup Condition="'$(OS)' == 'Windows_NT'">
  <BuildInParallel>false</BuildInParallel>
  <ProduceReferenceAssembly>false</ProduceReferenceAssembly>
</PropertyGroup>
```

**When to Apply**:

- Multi-targeting projects (.NET Framework + .NET 6+)
- Windows CI agents experiencing CS0006 errors
- Full solution builds with net472 + modern framework targets

---

## Skill-CI-001

**Statement**: CI should verify lint rules without auto-fix to catch commits bypassing pre-commit hooks

**Atomicity**: 95%

**Category**: CI

**Context**: When configuring CI pipeline for markdown-heavy repositories

**Evidence**: Session Linting Automation - Added check-only step to main.yml

**Details**:

- Pre-commit hooks auto-fix issues locally (developer experience)
- CI should verify rules WITHOUT auto-fixing (catch bypasses)
- Prevents developers from bypassing local hooks
- Two-layer approach: auto-fix locally, verify in CI

**Implementation Pattern**:

```yaml
# In CI workflow (main.yml)
- name: Verify markdown linting
  run: |
    npm install -g markdownlint-cli2
    # Use --no-fix to catch issues
    npx markdownlint-cli2 "**/*.md"
  env:
    SKIP_AUTOFIX: 1
```

**Contrast with Local**:

```bash
# Local pre-commit hook - AUTO-FIXES
npx markdownlint-cli2 --fix "**/*.md"

# CI verification - CHECK ONLY
npx markdownlint-cli2 "**/*.md"
```

**Benefits**:

- Local: Eliminates friction, auto-fixes common issues
- CI: Catches intentional bypass attempts
- Clear separation of concerns

---

## Skill-CI-002

**Statement**: CI must validate all files (`**/*.md`), not just changes, as final safety net

**Atomicity**: 92%

**Category**: CI

**Context**: When configuring CI lint checks for repositories with incremental local hooks

**Evidence**: Session markdown-lint-incident 2025-12-14 - 321 errors caught by CI that hooks missed

**Details**:

- Pre-commit hooks validate only **staged files** for performance
- Files committed before hooks enabled are never locally validated
- Files committed with `--no-verify` bypass local validation entirely
- CI must scan entire repository to catch accumulated issues

**Implementation Pattern**:

```yaml
# In CI workflow (lint.yml)
- name: Lint markdown
  run: |
    # Validate ALL files, not just changed ones
    npx markdownlint-cli2 "**/*.md"
```

**Contrast with Local Hook**:

```bash
# Local hook - staged files only (fast)
STAGED_FILES=$(git diff --cached --name-only --diff-filter=ACMR)
npx markdownlint-cli2 --fix $STAGED_FILES

# CI - all files (comprehensive)
npx markdownlint-cli2 "**/*.md"
```

**Gap Analysis**:

| Scenario                  | Hook Catches | CI Catches |
| ------------------------- | ------------ | ---------- |
| Staged files with errors  | ✅           | ✅         |
| Pre-existing files        | ❌           | ✅         |
| Files via `--no-verify`   | ❌           | ✅         |
| Files from other branches | ❌           | ✅         |

**Application**:

- Accept the hook/CI scope difference (hooks are staged-only by design)
- Run baseline validation when first enabling hooks: `npx markdownlint-cli2 "**/*.md"`
- Document this gap in CONTRIBUTING.md

---

## Skill-CI-003

**Statement**: Pre-commit hooks validating only staged files miss pre-existing issues; run baseline validation when enabling hooks

**Atomicity**: 88%

**Category**: CI

**Context**: When enabling git hooks on an existing codebase

**Evidence**: Session markdown-lint-incident 2025-12-14 - 321 errors accumulated before CI detected them

**Details**:

- Git hooks that check only staged files have a blind spot
- Files that existed before hook enablement are never validated
- This creates a "technical debt" of unvalidated files
- Solution: Run full repository scan before enabling incremental hooks

**Pattern**:

```bash
# WRONG - enables hook without baseline
git config core.hooksPath .githooks

# CORRECT - run baseline first
npx markdownlint-cli2 "**/*.md"     # Fix any existing issues
npx markdownlint-cli2 --fix "**/*.md"  # Auto-fix what can be fixed
git add . && git commit -m "chore: fix baseline lint errors"
git config core.hooksPath .githooks  # Now enable hooks
```

**Checklist When Enabling Hooks**:

1. [ ] Run full repository lint check
2. [ ] Fix or suppress existing issues
3. [ ] Commit fixes before enabling hooks
4. [ ] Enable hooks after baseline is clean
5. [ ] Document hook limitations in CONTRIBUTING.md

**Timeline Example** (from incident):

```text
02:18:53 - Hook added (ccef65a5)
   ↓
~22 markdown commits made
   ↓
02:47:07 - CI catches 321 errors (Run 20201556849)
   ↓
19:06:43 - All errors fixed (6a7d623e)
```

**Prevention**: Always run baseline validation before enabling incremental hooks
