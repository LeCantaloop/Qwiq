# Retrospective: Package Test Baseline Failure

**Date**: 2025-12-14  
**Issue**: PR #125 - Package tests failing in CI after baseline update  
**Workflow Run**: [20214813678](https://github.com/rjmurillo/Qwiq/actions/runs/20214813678/job/58027100102)

## What Happened

The package tests failed in CI with 8 out of 11 tests failing, despite all tests passing locally before the commit.

### Root Cause

The reproducible builds feature (from `DotNet.ReproducibleBuilds` package) adds a `branch` attribute to the `<repository>` element in NuGet package manifests for **ALL** packages. However, only 2 out of 10 baseline files were updated with this attribute.

**Packages Updated** (2):

- `Qwiq.Client.Soap`
- `Qwiq.Identity.Soap`

**Packages Missing Update** (8):

- `Qwiq.Client.Rest`
- `Qwiq.Core`
- `Qwiq.Identity`
- `Qwiq.Linq`
- `Qwiq.Linq.Identity`
- `Qwiq.Mapper`
- `Qwiq.Mapper.Identity`
- `Qwiq.Mocks`

## Why It Happened

### Incomplete Test Execution

The package tests were run locally, but only after building with a targeted subset of projects. The full solution build (which generates ALL packages) was not run before the final commit.

**What was done**:

```bash
# Build and test cycle was incomplete
dotnet build -c Release  # Built all packages
# ... made changes ...
dotnet test test/Qwiq.Package.Tests # Only tested without rebuilding all packages
```

**What should have been done**:

```bash
# Complete build-test cycle
dotnet build -c Release  # Build all packages
dotnet test test/Qwiq.Package.Tests --no-build  # Test all baselines
# Review ALL .received files
# Update ALL affected baselines
# Retest to confirm
```

### Incomplete Baseline Review

When the initial package tests were run, only the two SOAP packages generated `.received` files because:

1. The test was run after partial builds
2. Only packages that had been recently built were tested
3. The other packages' `.nupkg` files were from an earlier build that didn't have the branch attribute

## Impact

- CI build failed on Windows platform
- 8 package baseline tests failed
- Wasted CI resources (4 minutes of Windows runner time)
- Delayed PR merge

## Prevention Strategies

### 1. Complete Build-Test Cycle (MANDATORY)

**Always follow this sequence for package test baseline updates:**

```bash
# 1. Clean build
dotnet clean
rm -rf artifacts/

# 2. Full build with packages
dotnet build Qwiq.sln -c Release

# 3. Run package tests
dotnet test test/Qwiq.Package.Tests/Qwiq.Package.Tests.csproj -c Release --no-build

# 4. Review ALL .received files
ls test/Qwiq.Package.Tests/*.received.*

# 5. Verify changes match expectations for ALL packages
# Compare each .received file with its .verified counterpart

# 6. Update baselines
dotnet verify accept -w test/Qwiq.Package.Tests
# OR manual copy for all files

# 7. Re-run tests to confirm
dotnet test test/Qwiq.Package.Tests/Qwiq.Package.Tests.csproj -c Release --no-build
```

### 2. Systematic Baseline Verification

When updating baselines after infrastructure changes (like reproducible builds):

1. **Identify ALL affected packages** - Don't assume only some packages are affected
2. **Review the actual changes** - Use `diff` or `dotnet verify review` to see what changed
3. **Verify consistency** - Ensure all packages have the same structural changes (e.g., all have branch attribute, not just some)
4. **Check for patterns** - If 2 packages have a new attribute, likely all packages should have it

### 3. Pre-Commit Validation Checklist

Before committing package baseline updates:

- [ ] Clean build completed (`dotnet clean && rm -rf artifacts/`)
- [ ] All packages built (`dotnet build -c Release`)
- [ ] Package tests run (`dotnet test ... --no-build`)
- [ ] ALL .received files reviewed (not just the ones that changed locally)
- [ ] Count of updated baselines matches count of packages (10 packages = 10 baseline updates)
- [ ] Re-test after baseline update shows 0 failures
- [ ] Git diff reviewed to confirm only expected files changed

### 4. CI Smoke Test Pattern

For infrastructure changes affecting all packages:

```yaml
# Add a smoke test job that runs before merge
- name: Validate Package Tests
  run: |
    dotnet clean
    dotnet build -c Release
    dotnet test test/Qwiq.Package.Tests -c Release --no-build
    # Fail if ANY .received files exist (baselines out of date)
    if (Test-Path test/Qwiq.Package.Tests/*.received.*) {
      Get-ChildItem test/Qwiq.Package.Tests/*.received.* | ForEach-Object { Write-Host "::error::$($_.Name)" }
      exit 1
    }
```

## Action Items

### Immediate (Fix Current Issue)

- [x] Create this retrospective document
- [ ] Update all 8 missing baseline files with branch attribute
- [ ] Re-run full test suite to confirm
- [ ] Document the fix in commit message

### Short-Term (Prevent Recurrence)

- [ ] Add package baseline validation checklist to `.github/instructions/`
- [ ] Update `copilot-instructions.md` with package test best practices
- [ ] Consider adding a GitHub Action check for orphaned .received files

### Long-Term (Process Improvement)

- [ ] Add pre-commit hook that warns if .received files exist in package tests
- [ ] Create a script that automates the "clean build + test + verify" cycle
- [ ] Document common pitfalls in TESTING.md

## Key Learnings

1. **Infrastructure changes affect ALL packages** - When a build feature changes (like reproducible builds), expect all packages to be affected, not just a subset
2. **Local success ≠ CI success** - Running tests locally without a complete build cycle can miss issues that CI will catch
3. **Verify counts** - If there are N packages, expect N baseline updates for infrastructure changes
4. **Trust but verify** - Even if 2 tests pass locally, verify that all tests will pass before committing
5. **The scrubber works correctly** - The branch scrubber added in commit 90e5a15 is working as designed; the issue was incomplete baseline updates

## References

- Initial baseline update: commit 6354cdf
- Scrubber implementation: commit 90e5a15
- Failed CI run: <https://github.com/rjmurillo/Qwiq/actions/runs/20214813678/job/58027100102>
- Package test instructions: `.github/copilot-instructions.md` lines 749-815
