# Session Log: SBOM Tool Fix

**Date**: December 6, 2025
**Phase**: Phase 2B - Supply Chain Security (SBOM Fix)
**Branch**: `copilot/sub-pr-65`

---

## Summary

Fixed SBOM generation in GitHub Actions workflows. The `microsoft/sbom-tool` GitHub Action is a container action that only works on Linux runners, causing failures on Windows. Refactored to use the .NET tool approach and DRYed out the workflows.

---

## Tasks Completed

### 1. Diagnosed SBOM Tool Failure (GitHub Actions Run 19987665367)

**Problem**: The `microsoft/sbom-tool@v4.1.4` GitHub Action failed with:

- `Container action is only supported on Linux`
- `Unexpected input(s) 'buildDropPath', 'buildComponentPath'...`

**Root Cause**: The sbom-tool GitHub Action is a Docker container action that only runs on Linux. Windows runners cannot execute container actions.

### 2. Implemented .NET Tool Solution

**Solution**: Install `Microsoft.Sbom.DotNetTool` as a local .NET tool and invoke via CLI.

**Changes**:

1. Added `microsoft.sbom.dotnettool` v4.1.4 to `.config/dotnet-tools.json`
2. Updated workflows to use `dotnet sbom-tool generate` instead of the container action
3. Tool is restored via `dotnet tool restore` (already in build pipeline)

### 3. Fixed SBOM Version and Cross-Platform Support

**Changes**:

- Use `nbgv` to get the correct NuGetPackageVersion for SBOM
- Run SBOM generation on both Windows and Linux (cross-platform goal)
- Create output directory before running sbom-tool (`-m` flag requires existing directory)

### 4. Standardized Shell Usage

**Changes**:

- Changed all `shell: bash` to `shell: pwsh` in release.yml for consistency
- Converted bash commands to PowerShell equivalents:
  - `sha256sum`/`find` → `Get-FileHash`/`Get-ChildItem`
  - `mkdir -p` → `New-Item -ItemType Directory -Force`

### 5. DRYed Out Workflows

**Before**: release.yml had a separate `sbom` job (36 lines) that duplicated:

- Checkout
- Setup .NET SDK
- Restore .NET tools
- SBOM generation

**After**:

- main.yml generates SBOM and uploads as `sbom-${{ matrix.os }}` artifact
- release.yml downloads `sbom-windows-latest` artifact from build job
- Removed 36 lines of duplicate code

---

## Decisions Made

| Decision                                  | Rationale                                                                               |
| ----------------------------------------- | --------------------------------------------------------------------------------------- |
| Use .NET tool instead of container action | Container actions only work on Linux; .NET tool is cross-platform                       |
| Add sbom-tool to local tool manifest      | Consistent with other tools (nbgv, reportgenerator); restored via `dotnet tool restore` |
| Use nbgv version for SBOM                 | Ensures SBOM version matches NuGet package version                                      |
| Run SBOM on both OS                       | Cross-platform goal; Linux SBOM validates different dependency detection                |
| DRY out SBOM generation                   | Single source of truth; reduces maintenance burden                                      |
| Use pwsh shell everywhere                 | Consistency across workflows; PowerShell is cross-platform                              |

---

## Challenges Encountered

### 1. Container Action Limitation

- **Issue**: `microsoft/sbom-tool` is a Docker container action
- **Resolution**: Use .NET tool approach instead

### 2. ManifestDirPath Must Exist

- **Issue**: `sbom-tool generate -m ./path` fails if directory doesn't exist
- **Resolution**: Create directory before running: `New-Item -ItemType Directory -Path ./artifacts/sbom -Force`

### 3. Hash Computation for SLSA

- **Issue**: Bash `sha256sum` not available on Windows
- **Resolution**: Use PowerShell `Get-FileHash` with proper formatting for SLSA provenance

---

## Files Changed

| File                            | Change                                                             |
| ------------------------------- | ------------------------------------------------------------------ |
| `.config/dotnet-tools.json`     | Added `microsoft.sbom.dotnettool` v4.1.4                           |
| `.github/workflows/main.yml`    | Use .NET tool for SBOM, nbgv version, cross-platform               |
| `.github/workflows/release.yml` | Removed duplicate sbom job, use pwsh shell, download sbom artifact |
| `.agents/modernize-TODO.md`     | Updated SBOM tool version in examples                              |

---

## Commits Made

1. `0bbc269e` - fix(ci): use sbom-tool .NET tool instead of container action
2. `f15b63a8` - fix(ci): add sbom-tool to local tool manifest
3. `66c636aa` - fix(ci): use nbgv version for SBOM and run on all platforms
4. `91c04624` - fix(ci): create SBOM output directory before generation
5. `e931efd8` - refactor(ci): use pwsh shell consistently in release workflow
6. `236a2891` - refactor(ci): DRY out SBOM generation between main and release workflows

---

## Verification

```powershell
# Build passes
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
# Build succeeded in 17.9s

# Tests pass
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
# Test summary: total: 196, failed: 0, succeeded: 196, skipped: 0

# SBOM generation tested locally
dotnet sbom-tool generate -b ./src -bc ./ -pn Qwiq -pv "10.0.65-gcb60fa2721" -ps "Qwiq Contributors" -nsb https://github.com/rjmurillo/Qwiq -m ./artifacts/sbom
# Generated 4.4MB manifest.spdx.json with 378 components detected
```

---

## Next Steps

1. **Monitor CI**: Verify GitHub Actions run succeeds with the new SBOM approach
2. **Phase 2C**: Continue with Testing Enhancements (W2.16 - REST/SOAP Unit Tests)

---

## Notes for Next Session

- SBOM tool is now in `.config/dotnet-tools.json` - restored automatically
- SBOM artifact name is `sbom-${{ matrix.os }}` (e.g., `sbom-windows-latest`)
- Release workflow downloads SBOM from build job, doesn't regenerate
- All shells in workflows are now `pwsh` for consistency
