# Retrospective: Missed Package Test Baseline Updates

**Date**: 2025-12-14  
**Session**: Embedded Symbols Implementation  
**Issue**: Package tests failed in CI due to outdated baselines  
**Severity**: Medium (caught by CI, easy fix, but delayed merge)

---

## Executive Summary

During the implementation of switching from portable+snupkg to embedded debug symbols, package test baseline files were not updated, causing CI failure. The root cause was a **process gap** where configuration changes affecting package metadata were not explicitly checked against package test baselines.

**Impact**: ~30 minutes of additional work to diagnose, rebaseline, and document. No production impact (caught in CI).

---

## What Happened (Timeline)

1. **Task assigned**: Switch to embedded symbols based on multi-agent consensus
2. **Implementation phase**: Orchestrator delegated to implementer agent
3. **Changes made**:
   - ✅ Updated `Directory.Build.props` (DebugType=embedded)
   - ✅ Updated `.github/workflows/release.yml` (removed snupkg)
   - ✅ Added `scripts/Validate-PackageSize.ps1`
   - ✅ Updated `README.md`
   - ✅ Created `ADR-012`
4. **Validation performed**:
   - ✅ Build successful (0 errors, 0 warnings)
   - ✅ Package sizes checked (87KB, 92KB - under threshold)
   - ✅ Code review executed
   - ❌ **MISSED**: Package tests not run
5. **CI failure**: Tests failed due to baseline mismatch:
   - Received: `<repository ... branch="refs/heads/copilot/sub-pr-118-again" .../>`
   - Verified: `<repository ... />` (no branch attribute)
6. **Root cause**: DotNet.ReproducibleBuilds now includes branch name in package metadata when building from feature branches
7. **Fix**: Rebaselined using `dotnet verify accept` (manual copy in non-interactive environment)

---

## Root Cause Analysis

### Primary Cause: Process Gap

**The implementing agents did not run package tests as part of the validation workflow.**

### Contributing Factors

#### 1. **Documentation existed but was not consulted**

From `copilot-instructions.md` (lines 641-680):

```markdown
### Package Tests

The `Qwiq.Package.Tests` project validates NuGet package contents using Verify. These tests:

- Require `dotnet pack` to run first (packages must exist)
- Compare package manifests and contents against verified baselines
- Will fail if run without first creating packages

> **⚠️ CRITICAL: When NuGet Package Contents Change**
>
> Whenever ANY change is made that affects NuGet package contents (adding/removing files, changing metadata, etc.), you MUST:

1. **Run PackageTests first** to identify baseline mismatches
2. **Review the test output** to verify changes are expected
3. **Update verified baselines** only after confirming changes are correct
```

**Why wasn't this followed?**

- The implementer agent likely did not review this section
- No checklist item for "configuration changes" triggering package test rebaseline
- The connection between "changing DebugType" → "package metadata changes" → "rebaseline needed" was not made

#### 2. **Agent instructions lacked explicit trigger patterns**

The documentation tells _how_ to rebaseline but doesn't clearly state _when_ to check. It should have said:

> "Configuration changes to `Directory.Build.props` that affect build outputs ALWAYS require package test validation and potential rebaselining."

#### 3. **Validation checklist was incomplete**

The orchestrator's validation included:

- Build success ✅
- Package size check ✅
- Code review ✅

But missed:

- **Run package tests** ❌
- **Verify package manifests** ❌

#### 4. **Non-obvious metadata change**

The change was to `DebugType` and symbol configuration. It's not immediately obvious that this would affect repository metadata in the manifest. The actual cause was:

- DotNet.ReproducibleBuilds package behavior
- Building from a feature branch adds `branch` attribute
- This is expected and correct behavior (for source linking and reproducibility)

---

## Why Existing Documentation Didn't Help

### Documentation Quality: Good ✅

The `copilot-instructions.md` has comprehensive coverage:

- Clear explanation of package tests
- Step-by-step rebaselining instructions
- Warnings about when to rebaseline
- Tool usage examples

### Documentation Discoverability: Poor ❌

**The problem**: The information exists but wasn't surfaced at the right time.

**Missing connection**: No explicit link between:

- "I'm changing build configuration" (agent's current task)
- "I should check package tests" (documented requirement)
- "I should run validation before committing" (general practice)

---

## Lessons Learned

### 1. **Configuration changes are high-risk for package metadata**

Any change to:

- `Directory.Build.props`
- `Directory.Build.targets`
- `Directory.Packages.props`
- Build-related package references (e.g., DotNet.ReproducibleBuilds)

Should **automatically trigger** package test validation.

### 2. **CI is the safety net, not the first test**

Package tests should be run **before** committing, not discovered in CI. The workflow should be:

1. Make change
2. Build
3. **Run affected tests** (including package tests for build config changes)
4. Commit
5. Push
6. CI validates (catches what we missed)

### 3. **Checklists need to be context-aware**

A generic "run tests" checklist item doesn't help if the agent doesn't know which tests to run. We need:

- **Trigger-based checklists**: "If changing X, run tests Y"
- **Validation matrices**: "For config changes: build ✓, unit tests ✓, package tests ✓"

### 4. **Agent instructions need explicit patterns**

Instead of:

> "Package tests validate package contents"

We need:

> "PATTERN: Changes to Directory.Build.props → Run package tests → Rebaseline if metadata changed"

---

## Recommendations

### Immediate Actions (This PR)

- [x] **Fix**: Rebaseline package tests (completed)
- [ ] **Document**: Add this retrospective to .agents/retrospective/
- [ ] **Update**: Add validation reminder to relevant instruction files

### Short-Term Improvements (Next Session)

#### 1. **Update `.github/instructions/msbuild.instructions.md`**

Add section:

```markdown
## Package Test Validation

When modifying Directory.Build.props, Directory.Build.targets, or Directory.Packages.props:

**ALWAYS run package tests before committing:**

\`\`\`bash
dotnet build Qwiq.sln -c Release
dotnet test test/Qwiq.Package.Tests/Qwiq.Package.Tests.csproj --no-build -c Release
\`\`\`

**If package tests fail**, review the diff between .received and .verified files:

- If changes are expected (e.g., metadata updates), rebaseline: `dotnet verify accept -w test/Qwiq.Package.Tests`
- If changes are unexpected, investigate the root cause

**Common triggers for rebaselining**:

- Adding/removing PackageReference
- Changing build configuration (DebugType, IncludeSymbols, etc.)
- Updating DotNet.ReproducibleBuilds or SourceLink packages
- Building from a different branch (branch name appears in metadata)
```

#### 2. **Update `.github/instructions/project.instructions.md`**

Add to validation checklist:

```markdown
## Validation Checklist

Before submitting changes, verify:

- [ ] `dotnet restore Qwiq.sln` succeeds
- [ ] `dotnet build Qwiq.sln -c Release` succeeds with 0 errors
- [ ] No new warnings introduced
- [ ] Tests pass with filters applied
- [ ] **Package tests pass** (if Directory.Build.props or package config changed)
- [ ] Package versions are in `Directory.Packages.props`
- [ ] InternalsVisibleTo entries are correct
```

#### 3. **Create pattern card in `.agents/patterns/`**

Create `.agents/patterns/package-metadata-changes.md`:

```markdown
# Pattern: Package Metadata Changes

## When to Use

Any time you modify:

- Directory.Build.props
- Directory.Build.targets
- Directory.Packages.props
- Build-affecting packages (DotNet.ReproducibleBuilds, SourceLink, etc.)

## Validation Steps

1. Build in Release mode: `dotnet build Qwiq.sln -c Release`
2. Run package tests: `dotnet test test/Qwiq.Package.Tests/Qwiq.Package.Tests.csproj --no-build -c Release`
3. If tests fail, check .received vs .verified files
4. Rebaseline if changes are expected: `dotnet verify accept -w test/Qwiq.Package.Tests`
5. Commit baseline updates with configuration changes

## Common Failures

- Branch name in repository metadata (expected on feature branches)
- Dependency version changes (expected when updating packages)
- File list changes (expected when adding/removing package content)
```

#### 4. **Add to orchestrator instructions**

In `.agents/AGENT-SYSTEM.md` or orchestrator-specific guidance:

```markdown
## Build Configuration Changes

When delegating tasks that modify build configuration:

**Required validations**:

- Standard build/test validation
- **Package test validation** (for metadata changes)
- Size regression checks (if adding content)

**Rebaselining authority**: Implementer or orchestrator can rebaseline package tests if changes are verified as expected.
```

### Long-Term Improvements

#### 1. **Pre-commit hook for package tests**

Add to `.githooks/pre-commit`:

```bash
# Check if Directory.Build.props changed
if git diff --cached --name-only | grep -q "Directory.Build"; then
    echo "⚠️  Build configuration changed. Run package tests before committing."
    echo "   dotnet test test/Qwiq.Package.Tests/Qwiq.Package.Tests.csproj -c Release"
fi
```

#### 2. **CI validation matrix**

Update CI to report:

- Which tests were run
- Which validation steps were skipped
- Suggestions for missing validations

#### 3. **Agent memory/context**

Store pattern cards in agent memory:

- "Configuration change patterns"
- "Validation requirements"
- "Common failure modes"

---

## Success Criteria Going Forward

### For this specific issue

- ✅ Package tests pass
- ✅ Baselines updated
- ⚠️ Documentation updated (in progress)
- ⚠️ Instructions enhanced (recommended)

### For preventing recurrence

- Build configuration changes trigger package test validation
- Agents have clear pattern cards for configuration changes
- Validation checklists are context-aware
- Pre-commit hooks remind about package tests

---

## Conclusion

**Was this preventable?** Yes, with better process documentation.

**Was the documentation missing?** No, it existed but wasn't discovered.

**What failed?** The connection between "task type" and "required validations" was not explicit enough for agents to follow.

**Fix priority**: Medium. Not urgent (CI caught it), but should be addressed to prevent similar issues with other configuration changes.

**Effort to prevent**: Low. Documentation updates + pattern cards.

**Value**: High. Configuration changes are rare but critical; getting validation right matters.

---

## Artifacts

- **Failed CI run**: <https://github.com/rjmurillo/Qwiq/actions/runs/20213522923/job/58022939961#step:8:1>
- **Fix commit**: (to be created with baseline updates)
- **Documentation updates**: (to be created per recommendations)

---

**Retrospective completed**: 2025-12-14  
**Next review**: When implementing recommendations
