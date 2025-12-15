# QA Review: ADR-011 Portable Debug Symbols with Symbol Packages

**Review Date**: 2025-12-14
**Reviewer**: QA Agent
**ADR Under Review**: `.agents/architecture/ADR-011-portable-symbols-snupkg.md`
**Status**: Accepted (under QA review)

---

## Executive Summary

ADR-011 proposes using portable debug symbols (`.snupkg`) instead of embedded symbols for v11.0.0 NuGet packages. This review evaluates the testability and completeness of the validation criteria from a QA perspective.

---

## 1. Validation Criteria Assessment

### Current Criteria (from ADR-011 Section: Validation > Success Criteria)

| Criterion                                                | Testable | Automated | Gap                            |
| -------------------------------------------------------- | -------- | --------- | ------------------------------ |
| `Directory.Build.props` configured for portable + snupkg | Yes      | Partially | Build validation script exists |
| v11.0.0 packages publish successfully to NuGet.org       | Yes      | No        | Manual verification only       |
| Symbol packages indexed on NuGet.org symbol server       | Yes      | No        | No automated verification      |
| Debugging into QWIQ source works in Visual Studio        | Yes      | No        | Manual only                    |
| Package size is baseline (no embedded symbol bloat)      | Yes      | No        | No size comparison test        |

### Verdict: NEEDS REVISION

**Rationale**: While the ADR identifies correct success criteria, only one is currently automated. The remaining four require manual verification with no regression safety net.

---

## 2. Existing Test Coverage Analysis

### What Already Exists

| Test/Script                    | Location                   | Purpose                                     | Coverage |
| ------------------------------ | -------------------------- | ------------------------------------------- | -------- |
| `Validate-PackageOutput.ps1`   | `build/scripts/`           | Verifies `.nupkg` and `.snupkg` pairs exist | Good     |
| `Verify-SourceLink.ps1`        | `build/scripts/`           | Validates Source Link in PDB files          | Good     |
| `PackageTests.cs`              | `test/Qwiq.Package.Tests/` | Baseline verification of `.nupkg` contents  | Partial  |
| CI workflow `main.yml`         | `.github/workflows/`       | Runs both validation scripts                | Good     |
| Release workflow `release.yml` | `.github/workflows/`       | Publishes both `.nupkg` and `.snupkg`       | Good     |

### Gaps Identified

1. **No `.snupkg` Content Verification**

   - `PackageTests.cs` explicitly skips `.snupkg` files (lines 58-63)
   - Comment references: "pending Verify.Nupkg support. See github.com/MattKotsenas/Verify.Nupkg/issues/38"
   - Risk: Symbol packages could be malformed without detection

2. **No Package Size Baseline**

   - No test compares package size against expected baseline
   - Cannot detect if embedded symbols accidentally get included

3. **No Post-Publish Symbol Server Verification**

   - No automated check that symbols are indexed on `symbols.nuget.org`
   - No smoke test for symbol download functionality

4. **No IDE Debugging Integration Tests**
   - No automated verification that debugging works in Visual Studio, Rider, or VS Code
   - Relies entirely on manual testing

---

## 3. Missing Test Scenarios

### Critical (Must Have Before Release)

| Scenario                              | Description                              | Risk if Missing                 |
| ------------------------------------- | ---------------------------------------- | ------------------------------- |
| Symbol package contains correct PDB   | Verify `.snupkg` contains portable PDB   | Debugging fails silently        |
| PDB type is `portable` not `embedded` | Check `DebugType` in compiled assemblies | Size bloat, unexpected behavior |
| Package size regression test          | Compare `.nupkg` size to baseline        | Undetected embedded symbols     |
| Source Link URLs accessible           | Verify GitHub raw content URLs resolve   | "Source not available" errors   |

### Important (Should Have)

| Scenario                    | Description                                     | Risk if Missing           |
| --------------------------- | ----------------------------------------------- | ------------------------- |
| Visual Studio debugging     | Step into QWIQ code with symbol server          | Primary use case untested |
| JetBrains Rider debugging   | Verify automatic symbol resolution              | Second IDE untested       |
| VS Code debugging           | Verify `CopyDebugSymbolFilesFromPackages` works | Third IDE untested        |
| Corporate firewall scenario | Document workaround for blocked symbol servers  | User confusion            |

### Nice to Have

| Scenario                   | Description                                  | Risk if Missing           |
| -------------------------- | -------------------------------------------- | ------------------------- |
| Symbol server latency      | Measure time to download symbols             | Poor developer experience |
| Multiple version debugging | Debug when multiple QWIQ versions in project | Edge case failures        |
| Offline debugging fallback | Verify Source Link to GitHub works           | No fallback documentation |

---

## 4. Regression Testing Strategy

### Automated Regression Tests (Recommended)

```text
Test Suite: Symbol Package Validation
Location: test/Qwiq.Package.Tests/

1. SymbolPackageTests.cs (NEW)
   - Test_SnupkgContainsPortablePdb()
   - Test_NupkgDoesNotContainPdb()
   - Test_PackageSizeWithinBaseline()
   - Test_DebugTypeIsPortable()

2. SourceLinkTests.cs (NEW)
   - Test_AllSourceLinksResolvable()
   - Test_GitHubRawContentAccessible()
```

### Manual Regression Checklist

```markdown
## Pre-Release Symbol Verification Checklist

### Build Verification

- [ ] `dotnet pack` produces both `.nupkg` and `.snupkg` for all 9 projects
- [ ] `Validate-PackageOutput.ps1` passes
- [ ] `Verify-SourceLink.ps1` passes

### Local Debugging Verification

- [ ] Install package from local feed in test project
- [ ] Set breakpoint in test code calling QWIQ
- [ ] Step into QWIQ source (F11)
- [ ] Verify source code displays correctly

### Post-Publish Verification

- [ ] Packages visible on NuGet.org
- [ ] Symbol packages visible on NuGet.org (check "Symbols" badge)
- [ ] Create new test project with published package
- [ ] Enable NuGet.org symbol server in Visual Studio
- [ ] Verify debugging works with published symbols
```

---

## 5. Rollback Plan Assessment

### Current State: INSUFFICIENT

The ADR does not document a rollback plan. If symbols don't work after publishing v11.0.0:

### Recommended Rollback Plan

#### Immediate Mitigation (Same Day)

1. Publish patch release (v11.0.1) with embedded symbols
2. Update `Directory.Build.props`:

   ```xml
   <DebugType>embedded</DebugType>
   <IncludeSymbols>false</IncludeSymbols>
   ```

3. Remove `<SymbolPackageFormat>snupkg</SymbolPackageFormat>`

#### Alternative: Unlisting

1. Unlist v11.0.0 packages from NuGet.org (does not delete, prevents new installs)
2. Investigate root cause
3. Republish fixed packages

#### Cannot Rollback

- Symbol packages already on `symbols.nuget.org` cannot be removed
- Old symbols remain cached in developer IDEs

### Recommended ADR Addition

Add a "Rollback Strategy" section:

```markdown
## Rollback Strategy

If symbol packages fail validation post-publish:

1. **Detection**: Monitor GitHub issues for debugging complaints within 7 days of release
2. **Triage**: Reproduce issue locally with same package version
3. **Options**:
   - Minor fix: Patch release (v11.0.1) with corrected symbols
   - Major issue: Patch release with embedded symbols (revert to Option 2)
4. **Communication**: Update README with debugging troubleshooting section
```

---

## 6. Specific Issues Found

### Issue 1: Missing DebugType Validation in CI

**Location**: `main.yml` lines 45-79
**Problem**: CI validates Source Link but not `DebugType` property
**Risk**: Could accidentally ship with embedded symbols
**Recommendation**: Add step to verify `DebugType=portable` in compiled assemblies

### Issue 2: No Symbol Package Size Threshold

**Location**: `Validate-PackageOutput.ps1`
**Problem**: Script only checks existence, not size
**Risk**: Bloated packages go undetected
**Recommendation**: Add size threshold check (e.g., fail if `.nupkg` exceeds 2MB)

### Issue 3: Incomplete IDE Coverage in Documentation

**Location**: ADR-011 lines 139-146
**Problem**: VS Code instructions are incomplete ("requires `CopyDebugSymbolFilesFromPackages=true`")
**Risk**: VS Code users cannot debug
**Recommendation**: Add complete `launch.json` configuration example

### Issue 4: No Monitoring for Symbol Server Indexing

**Location**: ADR-011 lines 236-240
**Problem**: "Symbol packages indexed on NuGet.org" has no verification method
**Risk**: Symbols published but not indexed (silent failure)
**Recommendation**: Add post-publish verification script using NuGet API

---

## 7. Recommended Test Additions

### High Priority

1. **Create `test/Qwiq.Package.Tests/SymbolPackageTests.cs`**

   ```csharp
   [Fact]
   public void SnupkgContainsPortablePdb()
   {
       // Extract .snupkg, verify .pdb files present
       // Verify PDB is portable format (check header)
   }

   [Fact]
   public void NupkgDoesNotContainPdb()
   {
       // Extract .nupkg, verify no .pdb files
   }
   ```

2. **Create `build/scripts/Verify-DebugType.ps1`**

   ```powershell
   # Check compiled DLLs have DebugType=portable
   # Fail if any DLL has embedded symbols
   ```

3. **Add size validation to `Validate-PackageOutput.ps1`**

   ```powershell
   $maxNupkgSize = 2MB
   if ($nupkg.Length -gt $maxNupkgSize) {
       Write-Error "Package exceeds size threshold - possible embedded symbols"
   }
   ```

### Medium Priority

1. **Create post-publish verification script**

   ```powershell
   # Verify-SymbolServerIndexing.ps1
   # Query NuGet API for symbol package status
   ```

2. **Add IDE debugging section to README.md**
   - Complete Visual Studio configuration
   - Complete VS Code `launch.json` example
   - Rider (confirm automatic)

---

## 8. Final Verdict

### Verdict: NEEDS REVISION

### Required Changes Before Approval

1. **Add rollback strategy section** to ADR-011
2. **Add DebugType validation** to CI pipeline
3. **Add package size threshold** to validation script
4. **Complete VS Code debugging documentation**

### Recommended Improvements (Not Blocking)

1. Create `SymbolPackageTests.cs` for automated `.snupkg` verification
2. Add post-publish symbol server verification script
3. Create manual testing checklist for release process

### Conditional Approval

ADR-011 can be **ACCEPTED** once items 1-4 are addressed. The decision to use portable symbols with `.snupkg` is sound and aligns with industry practices. The gaps are in validation infrastructure, not the technical decision itself.

---

## Appendix: File References

| File                       | Repository Path                                           |
| -------------------------- | --------------------------------------------------------- |
| ADR-011                    | `.agents/architecture/ADR-011-portable-symbols-snupkg.md` |
| Directory.Build.props      | `Directory.Build.props`                                   |
| Validate-PackageOutput.ps1 | `build/scripts/Validate-PackageOutput.ps1`                |
| Verify-SourceLink.ps1      | `build/scripts/Verify-SourceLink.ps1`                     |
| PackageTests.cs            | `test/Qwiq.Package.Tests/PackageTests.cs`                 |
| main.yml                   | `.github/workflows/main.yml`                              |
| release.yml                | `.github/workflows/release.yml`                           |
