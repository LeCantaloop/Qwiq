# Tasks: Fix CS0006 CI Build Failure - Missing Reference Assembly

## Overview

Fix intermittent CS0006 build failures in GitHub Actions CI caused by race conditions during multi-target framework (MTF) builds. The error manifests as "Metadata file 'obj/Release/ref/Qwiq.Core.dll' could not be found" when reference assemblies are accessed before being fully written.

## Root Cause Analysis

### The Problem

- **Multi-TFM builds** (net472, netstandard2.0, net8.0) use MSBuild's `DispatchToInnerBuilds` for inner-build parallelism
- **`/m:1` limitation**: Only controls solution-level parallelism, NOT inner-build parallelism within multi-targeted projects
- **Reference assembly race**: Reference assemblies (`obj/Release/ref/*.dll`) can be accessed by downstream projects before fully written
- **CI environment**: Slower disk I/O in CI runners exacerbates timing issues, causing intermittent CS0006 errors

### The Solution

**Two-part fix implemented:**

1. **Disable inner-build parallelism** (in `Directory.Build.props` and `Directory.Build.rsp`):

```xml
<PropertyGroup>
  <BuildInParallel>false</BuildInParallel>
  <MSBuildBuildInParallel>false</MSBuildBuildInParallel>
  <ProduceReferenceAssembly>false</ProduceReferenceAssembly>
</PropertyGroup>
```

1. **Add .NET Framework reference assemblies for Linux** (in `Directory.Build.props`):

```xml
<ItemGroup Condition="'$(TargetFramework)' == 'net472'">
  <PackageReference Include="Microsoft.NETFramework.ReferenceAssemblies" PrivateAssets="All" />
</ItemGroup>
```

This allows building net472 targets on Linux/macOS without the Windows SDK installed.
See: <https://github.com/microsoft/dotnet-framework-reference-assemblies>

## Current Situation

- **Fix implemented**: Changes are in local `Directory.Build.props`, `Directory.Build.rsp`, `Directory.Packages.props`
- **Original source**: Fix from commit `f5c68d02` on `origin/devin/1764355026-net8-sdk-migration` branch
- **Current branch**: `copilot/sub-pr-65` (tracking `origin/copilot/sub-pr-65`)
- **Status**: Changes need to be committed and pushed
- **CI Run**: 20109171974 failed with CS0006 error

## Success Criteria

1. Fix committed and pushed to `copilot/sub-pr-65`
2. CI build passes without CS0006 errors on both Windows AND Linux
3. Build time increase documented (expected: ~10-20% slower on CI)
4. Linux can now build net472 targets (previously skipped)
5. Solutions Repository updated in copilot-instructions.md
6. Local validation confirms behavior matches CI

## Relevant Files

- `Directory.Build.props` - MSBuild property configuration (parallelism fix + reference assemblies)
- `Directory.Build.rsp` - MSBuild response file (CLI defaults)
- `Directory.Packages.props` - Central Package Management (reference assemblies version)
- `.github/workflows/main.yml` - CI workflow configuration
- `.github/copilot-instructions.md` - Solutions Repository documentation
- `artifacts/logs/build.binlog` - Binary build logs for diagnosis

## Phase 1: High-Level Tasks

The following high-level tasks break down the work needed to complete this fix:

---

## Tasks

- [ ] **Task 1: Commit and Push the CS0006 Fix**

  - Verify uncommitted changes in `Directory.Build.props`
  - Commit with conventional commit message
  - Push to `origin/copilot/sub-pr-65`

- [ ] **Task 2: Verify Fix Application in CI Environment**

  - Download binlog from failing CI run
  - Analyze binlog to confirm MSBuild properties
  - Verify `ContinuousIntegrationBuild=true` is set

- [ ] **Task 3: Local CI-Equivalent Validation**

  - Reproduce CI build conditions locally
  - Verify fix prevents CS0006 errors
  - Measure build time impact

- [ ] **Task 4: CI Workflow Enhancement (Optional)**

  - Evaluate if `dotnet clean` step needed before build
  - Consider cache invalidation for obj/bin folders
  - Add diagnostic logging for build parallelism settings

- [ ] **Task 5: Documentation and Knowledge Capture**
  - Update Solutions Repository in copilot-instructions.md
  - Document build time tradeoffs
  - Add binlog analysis notes

---

## Task 1: Commit and Push the CS0006 Fix

**Objective**: Commit the local changes to `Directory.Build.props` and push to the remote branch so CI can apply the fix.

### Prerequisites

- Uncommitted changes in `Directory.Build.props` (confirmed present)
- Clean working directory otherwise

### Sub-tasks

- [ ] **1.1: Review uncommitted changes**

  - Run `git diff Directory.Build.props` to review changes
  - Verify the added PropertyGroup contains:
    - `BuildInParallel=false`
    - `MSBuildBuildInParallel=false`
    - `ProduceReferenceAssembly=false`
    - Condition: `'$(ContinuousIntegrationBuild)' == 'true'`
  - Confirm comment block explains the race condition

- [ ] **1.2: Stage and commit changes**

  - Run `git add Directory.Build.props`
  - Commit with message: `ci: disable inner-build parallelism to fix CS0006 race conditions`
  - Body text:

    ```text
    Fixes intermittent CS0006 errors in CI by:
    - Disabling BuildInParallel for multi-TFM inner builds
    - Disabling reference assembly generation on CI

    The /m:1 flag only limits solution-level parallelism, not
    DispatchToInnerBuilds parallelism within projects. This change
    prevents reference assemblies from being accessed before fully written.

    Tradeoff: ~10-20% slower CI builds for stability.

    Refs: #65
    ```

- [ ] **1.3: Push to remote**

  - Run `git push origin copilot/sub-pr-65`
  - Verify push succeeds
  - Confirm commit appears in GitHub UI

- [ ] **1.4: Verify branch state**
  - Run `git status` to confirm no uncommitted changes
  - Run `git log -1 --oneline` to confirm commit
  - Check GitHub Actions tab for new CI run

### Expected Outcome

- Commit appears on `origin/copilot/sub-pr-65`
- CI workflow triggers automatically
- Changes ready for CI validation

---

## Task 2: Verify Fix Application in CI Environment

**Objective**: Confirm that the MSBuild properties are actually being applied during CI builds by analyzing build logs.

### Prerequisites

- CI run has started (after Task 1.3 push)
- Access to GitHub Actions logs
- (Optional) binlog artifact downloaded

### Sub-tasks

- [ ] **2.1: Monitor new CI run**

  - Go to GitHub Actions tab
  - Find CI run triggered by latest commit
  - Wait for build to complete (or fail)
  - Note run ID for reference

- [ ] **2.2: Download and analyze binlog (if available)**

  - Download `build.binlog` from artifacts
  - Open in MSBuild Structured Log Viewer (<https://msbuildlog.com/>)
  - Search for properties:
    - `BuildInParallel`
    - `MSBuildBuildInParallel`
    - `ProduceReferenceAssembly`
    - `ContinuousIntegrationBuild`
  - Verify all are set correctly during Windows build

- [ ] **2.3: Check build logs for CS0006 errors**

  - Search logs for "CS0006"
  - Search logs for "Qwiq.Core.dll' could not be found"
  - If errors exist, note which project and TFM

- [ ] **2.4: Verify build time impact**

  - Compare new build duration to previous runs
  - Expected: 10-20% increase due to serial inner builds
  - Document actual increase in task notes

- [ ] **2.5: Check both Windows and Linux builds**
  - Verify Windows build succeeds (most likely to hit CS0006)
  - Verify Linux build unaffected (already avoids CS0006 due to fewer TFMs)

### Expected Outcome

- binlog confirms properties applied
- No CS0006 errors in logs
- Build completes successfully on both platforms
- Build time impact documented

### Troubleshooting

If CS0006 still occurs:

- Check if `Directory.Build.props` is being imported (search binlog for "Directory.Build.props")
- Verify `ContinuousIntegrationBuild` is `true` (should be set in workflow)
- Check for project-level overrides of these properties
- Proceed to Task 4 for additional mitigation

---

## Task 3: Local CI-Equivalent Validation

**Objective**: Reproduce CI build conditions locally to validate the fix before relying solely on CI feedback loops.

### Prerequisites

- All changes committed (Task 1 complete)
- Windows development machine (for full TFM coverage)
- Clean working directory

### Sub-tasks

- [ ] **3.1: Clean local build artifacts**

  - Run `git clean -xdf` to remove all untracked files (CAUTION: saves your work first!)
  - OR manually: `Remove-Item -Recurse -Force artifacts, src/**/obj, src/**/bin, test/**/obj, test/**/bin`
  - Run `dotnet clean Qwiq.sln`

- [ ] **3.2: Reproduce CI build command (Windows)**

  - Run exact CI command with CI properties:

    ```powershell
    dotnet build Qwiq.sln -c Release --no-restore /t:Build,Pack /p:ContinuousIntegrationBuild=true /m:1 /nodeReuse:false /bl:./artifacts/logs/build-local-ci.binlog
    ```

  - Watch for CS0006 errors
  - Note build duration

- [ ] **3.3: Analyze local binlog**

  - Open `build-local-ci.binlog` in MSBuild Structured Log Viewer
  - Verify properties applied:
    - `ContinuousIntegrationBuild=true`
    - `BuildInParallel=false`
    - `ProduceReferenceAssembly=false`
  - Check "Messages" node for parallelism indicators

- [ ] **3.4: Run without CI properties (control test)**

  - Clean again: `dotnet clean`
  - Run local build WITHOUT CI properties:

    ```powershell
    dotnet build Qwiq.sln -c Release /m:1 /bl:./artifacts/logs/build-local-dev.binlog
    ```

  - Compare binlog: properties should be default values
  - Confirm `ProduceReferenceAssembly=true` (default) in dev build

- [ ] **3.5: Stress test for race conditions**
  - Run CI-equivalent build 5 times in succession
  - Clean between each: `dotnet clean`
  - Watch for ANY CS0006 errors
  - Document results: pass rate, timing variance

### Expected Outcome

- Local CI-equivalent build passes 5/5 times
- Properties correctly applied only when `ContinuousIntegrationBuild=true`
- No CS0006 errors in any run
- Build time ~10-20% slower than dev build

### Troubleshooting

If CS0006 occurs locally:

- Check if `Directory.Build.props` is imported (search binlog)
- Try adding `/p:BuildInParallel=false` explicitly to command
- Check for project-level `<BuildInParallel>true</BuildInParallel>` overrides
- Consider more aggressive serialization (see Task 4)

---

## Task 4: CI Workflow Enhancement (Optional)

**Objective**: Evaluate and implement additional CI workflow changes to prevent CS0006 recurrence or improve diagnosis.

### Prerequisites

- Task 2 complete (CI validation results available)
- Task 3 complete (local validation results available)

### Sub-tasks

- [ ] **4.1: Evaluate `dotnet clean` necessity**

  - **Context**: GitHub Actions uses fresh runners, caching is opt-in
  - Check workflow for cache actions (e.g., `actions/cache`)
  - **Decision point**:
    - If caching present: Add `dotnet clean` before build
    - If NO caching: Skip this (clean state guaranteed)
  - Document decision rationale

- [ ] **4.2: Consider obj/bin cache invalidation**

  - IF caching is used, check cache key includes:
    - `Directory.Packages.props` hash
    - `Directory.Build.props` hash
    - All `*.csproj` hashes
  - Update cache key if missing critical files

- [ ] **4.3: Add diagnostic logging**

  - Option A: Add diagnostic step before build:

    ```yaml
    - name: Show MSBuild properties
      run: dotnet msbuild Qwiq.sln /t:Restore /p:ContinuousIntegrationBuild=true /pp:./artifacts/logs/preprocessed.xml
    ```

  - Option B: Use `/v:diag` for one-time verbose build:

    ```yaml
    /bl:./artifacts/logs/build.binlog /v:diag > ./artifacts/logs/build.log
    ```

  - Commit whichever option provides value for future debugging

- [ ] **4.4: Consider RestoreUseStaticGraphEvaluation**

  - **Context**: Static graph restore can avoid some dependency race conditions
  - Test locally with: `/p:RestoreUseStaticGraphEvaluation=true`
  - If beneficial, add to `Directory.Build.props` for CI
  - Document outcome

- [ ] **4.5: Evaluate /restore flag**
  - **Current**: Separate `dotnet restore` then `dotnet build --no-restore`
  - **Alternative**: Single `dotnet build` (implicit restore)
  - Test locally which is more stable
  - Document findings, update workflow if needed

### Expected Outcome

- Workflow enhanced with any valuable changes
- Diagnostic capabilities improved for future issues
- OR: Documented reasons for NOT adding each enhancement

### Decision Criteria

Only implement changes that:

1. Measurably improve reliability OR
2. Significantly improve debuggability OR
3. Are zero-cost (no performance impact)

---

## Task 5: Documentation and Knowledge Capture

**Objective**: Update repository documentation with the CS0006 fix, build insights, and lessons learned for future maintainers.

### Prerequisites

- All previous tasks complete
- CI passing consistently
- Local validation results documented

### Sub-tasks

- [ ] **5.1: Update Solutions Repository**

  - Open `.github/copilot-instructions.md`
  - Navigate to `## Solutions Repository` section
  - Add new entry under "Build Debugging" table:

    ```markdown
    | CS0006 in multi-TFM CI builds | Disable inner-build parallelism: BuildInParallel=false, ProduceReferenceAssembly=false | 98% |
    ```

  - Add detailed notes if pattern differs from description

- [ ] **5.2: Document build time tradeoffs**

  - In copilot-instructions.md, add note in "Build Commands" section:

    ```markdown
    **CI Build Performance**: CI builds use `/p:ContinuousIntegrationBuild=true` which:

    - Disables inner-build parallelism (BuildInParallel=false)
    - Disables reference assembly generation (ProduceReferenceAssembly=false)
    - Results in ~10-20% slower builds but prevents CS0006 race conditions
    - Local dev builds are unaffected (parallelism enabled)
    ```

- [ ] **5.3: Update TESTING.md (if exists)**

  - Check if `TESTING.md` or similar exists
  - If so, add section on reproducing CI builds locally
  - Include the exact command from Task 3.2

- [ ] **5.4: Consider ADR (Architecture Decision Record)**

  - **Decision point**: Is this significant enough for an ADR?
  - Criteria: Affects build architecture, has tradeoffs, future maintainers need context
  - **If YES**: Create `docs/adr/NNNN-disable-ci-build-parallelism.md`
  - Include:
    - Context: CS0006 race conditions
    - Decision: Disable inner parallelism on CI
    - Consequences: Slower builds, more stable
    - Alternatives considered: /restore, static graph, explicit ordering

- [ ] **5.5: Update session notes**

  - Create or update `.agents/sessions/2025-12-10-cs0006-fix.md`
  - Document:
    - Root cause analysis
    - Fix implementation
    - Validation results
    - Build time measurements
    - Lessons learned

- [ ] **5.6: Close the loop on PR #65**
  - If PR #65 is related to this issue, add comment with summary
  - Link to this task list
  - Note CI stability improvement

### Expected Outcome

- Future maintainers understand why build parallelism is disabled
- Pattern documented for similar issues
- Build time tradeoffs clearly communicated
- Knowledge preserved in multiple formats (copilot-instructions, ADR, session notes)

---

## Notes

### Build Parallelism Hierarchy

MSBuild has multiple levels of parallelism that can interact:

1. **Solution-level parallelism** (`/m:N`): Controls concurrent project builds
2. **Inner-build parallelism** (`BuildInParallel`, `MSBuildBuildInParallel`): Controls concurrent TFM builds within multi-targeted projects
3. **Task-level parallelism**: Individual tasks (like Csc) may parallelize internally

**Key Insight**: `/m:1` only affects level 1, NOT level 2. This fix targets level 2.

### Reference Assembly Race Condition

Reference assemblies (`obj/*/ref/*.dll`) enable faster incremental compilation:

- **Producer**: Project A builds, emits `ref/A.dll` (contains only public API metadata)
- **Consumer**: Project B compiles against `ref/A.dll` instead of full `bin/A.dll`
- **Race**: If B starts before A finishes writing `ref/A.dll` → CS0006

**Why CI is affected more than local**:

- CI: Slower disk I/O, timing more variable
- Local: Faster SSD, timing more consistent

### Why Not ProduceReferenceAssembly=false Everywhere?

- **Dev builds**: Ref assemblies speed up incremental compilation significantly
- **CI builds**: Full rebuild every time, ref assemblies provide minimal benefit
- **Tradeoff**: CI stability > CI speed, Dev speed > Dev stability risk

### Alternatives Considered (and why not chosen)

1. **Explicit project dependencies**: Too brittle, defeats multi-TFM benefits
2. **Single-targeted builds**: Loses framework-specific testing coverage
3. **Longer timeouts/retries**: Masks problem, doesn't fix root cause
4. **Static graph restore**: Helps restore phase, not compile phase

### Key Files to Watch

If CS0006 recurs, check these for overrides:

- Individual `*.csproj` files (project-level overrides)
- `Directory.Build.targets` (late-binding property sets)
- Workflow YAML (CLI argument overrides)

### Related Issues

- Linux CI build fix (Task list: `TASKS-fix-linux-ci-build.md`)
- Package testing (requires successful build)
- SDK migration (where fix originated: `f5c68d02`)

### Testing Checklist

Before marking complete, verify:

- ✅ Windows CI build passes
- ✅ Linux CI build passes (unaffected, but verify)
- ✅ Local CI-equivalent build passes 5/5 times
- ✅ Dev build still uses ref assemblies (verify `ProduceReferenceAssembly=true` locally)
- ✅ Build time increase documented
- ✅ No CS0006 errors in logs
- ✅ Documentation updated

---

## Implementation Log

_Document progress here as you work through tasks. Include timestamps, decisions, and unexpected findings._

### [YYYY-MM-DD HH:MM] - Task X.Y: Description

- Action taken
- Result
- Next steps
