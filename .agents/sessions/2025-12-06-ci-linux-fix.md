# Session: Fix Linux CI Build Failure

**Date**: December 6, 2025
**Branch**: `copilot/sub-pr-58-another-one`
**GitHub Actions Run**: 19983820834 (failed)
**Status**: ✅ Fix committed and pushed

---

## Problem Summary

GitHub Actions run 19983820834 failed on both Windows and Ubuntu runners with the error:

```text
The file '/home/runner/work/Qwiq/Qwiq/src/Qwiq.Core/bin/Release/net472/Qwiq.Core.dll' to be packed was not found on disk.
```

## Root Cause Analysis

The workflow was using `/t:Build,Pack` on Linux, which forces an **outer-build Pack step** that expects ALL target frameworks (including `net472`). On Linux, only `net8.0` can be built because `net472` requires the Windows/.NET Framework.

### Technical Details

| Build Approach                | Pack Behavior                     | Linux Compatibility          |
| ----------------------------- | --------------------------------- | ---------------------------- |
| `GeneratePackageOnBuild=true` | Pack runs inside each inner build | ✅ Respects OS TFM filtering |
| `/t:Build,Pack`               | Pack runs as outer build step     | ❌ Expects all TFMs          |

When using `dotnet build` with `GeneratePackageOnBuild=true` (default), Pack runs inside each inner build and respects OS-level TFM filtering. When using `/t:Build,Pack`, Pack runs as an outer build step that expects all TFMs to have been built.

## Solution Applied

### Before (Complex, Failing)

```yaml
# Linux: Build each project individually to avoid quoting issues
dotnet build src/Qwiq.Core/Qwiq.Core.csproj -c Release --no-restore /t:Build,Pack ...
dotnet build src/Qwiq.Core.Rest/Qwiq.Client.Rest.csproj -c Release --no-restore /t:Build,Pack ...
# ... 13 more individual project builds
```

### After (Simple, Working)

```yaml
if [ "${{ matrix.os }}" = "windows-latest" ]; then
  # Windows: Full build with packing for all TFMs
  dotnet build Qwiq.sln -c Release --no-restore /t:Build,Pack /p:ContinuousIntegrationBuild=true /m:1 /nodeReuse:false /bl:./artifacts/logs/build.binlog
else
  # Linux: Build solution without packing (net472 cannot be built on Linux)
  dotnet build Qwiq.sln -c Release --no-restore /p:GeneratePackageOnBuild=false /p:ContinuousIntegrationBuild=true /m:1 /nodeReuse:false /bl:./artifacts/logs/build.binlog
fi
```

### Test Step Simplification

```yaml
# Before: Separate per-project test commands for Linux
# After: Single solution-level test for both platforms
run: dotnet test Qwiq.sln -c Release --no-build --filter "${{ env.TEST_FILTER }}" ...
```

## Validation

| Check                                                    | Result                                      |
| -------------------------------------------------------- | ------------------------------------------- |
| Windows build with `/t:Build,Pack`                       | ✅ All packages created                     |
| Linux-style build with `/p:GeneratePackageOnBuild=false` | ✅ Build succeeds                           |
| Unit tests                                               | ✅ 196 tests pass (108 + 10 + 16 + 34 + 28) |

## Files Changed

- `.github/workflows/main.yml` - Simplified build and test steps
- `.agents/TASKS-fix-linux-ci-build.md` - Task tracking document

## Commits

### Commit 1: Simplify cross-platform build

```text
fix(ci): simplify cross-platform build by skipping pack on Linux

Root cause: /t:Build,Pack forces outer-build Pack step that expects
ALL target frameworks. On Linux, net472 cannot be built, so Pack
failed looking for missing binaries.

Solution:
- Linux: Use solution-level build with /p:GeneratePackageOnBuild=false
- Windows: Keep /t:Build,Pack for full packaging of all TFMs
- Test step: Simplified to single solution-level command

Fixes: GitHub Actions run 19983820834
```

### Commit 2: Fix MSBuild syntax for bash

```text
fix(ci): use dash syntax for MSBuild args in bash

The forward slash syntax (/t:, /p:, /m:, /bl:) can be misinterpreted
by bash on Windows. Use dash syntax (-t:, -p:, -m:, -bl:) which is
cross-platform compatible and works in both bash and PowerShell.
```

## Key Learnings

1. **MSBuild inner vs outer builds**: Multi-targeting creates inner builds per TFM. Pack behavior differs based on where it runs.
2. **OS-level TFM filtering**: The SDK automatically skips Windows-only TFMs on Linux during inner builds, but outer Pack doesn't respect this.
3. **Simplicity wins**: Solution-level builds are simpler and more maintainable than per-project builds.

## Next Steps

1. Monitor the CI run triggered by this push
2. If successful, merge PR #64 to `feat/modernize-2`
3. Continue with Wave 1 modernization tasks (W1.22-W1.24)
