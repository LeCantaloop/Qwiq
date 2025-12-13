# QWIQ Modernization - Wave 2: Infrastructure & Quality

> **Navigation**: [📋 Index](modernize-TODO-index.md) | [Wave 1](modernize-wave1.md) | **Wave 2** | [Waves 3-5](modernize-wave3-5.md)
>
> **Purpose**: This file contains Wave 2 tasks for infrastructure, supply chain security, testing, and code quality.
>
> **Token Budget**: ~15,000 tokens (AI agent friendly)

---

## Quick Reference

| Phase | Focus                 | Tasks                   | Status             |
| ----- | --------------------- | ----------------------- | ------------------ |
| 2A    | Release Automation    | W2.11                   | ✅ Complete        |
| 2B    | Supply Chain Security | W2.13-W2.17             | ✅ Complete        |
| 2C    | Testing Enhancements  | W2.16, W2.18, W2.2-W2.4 | ✅ Mostly Complete |
| 2D    | Security Hardening    | W2.19-W2.20             | ✅ Complete        |
| 2E    | Documentation         | W2.5, W2.7              | 🟡 Partial         |
| 2F    | Code Quality (PR #65) | W2.21-W2.33             | 🔄 In Progress     |

---

## Phase 2A: Release Automation

### W2.11 Create Release Workflow ✅ COMPLETE

- [x] **Task**: Automate NuGet publishing with GitHub Release integration
- **Effort**: M (1 day)
- **Priority**: **CRITICAL**
- **Dependencies**: W1.19 (Build quality gates)
- **File**: `.github/workflows/release.yml`
- **Completed**: 2025-12-06 (Session 17 - Phase 2A)

**Implementation** (`.github/workflows/release.yml`):

```yaml
name: Release

on:
  workflow_dispatch:
  release:
    types:
      - published
      - edited
      - prereleased
      - released
  push:
    tags:
      - "v*"

permissions:
  contents: write
  packages: read
  actions: read

jobs:
  build:
    uses: ./.github/workflows/main.yml

  publish:
    needs: build
    runs-on: windows-latest
    environment:
      name: production-nuget
      url: https://nuget.org/packages/Qwiq
    steps:
      - name: Download packages
        uses: actions/download-artifact@v4
        with:
          name: packages-windows-latest
          path: packages

      - name: Publish to NuGet
        shell: pwsh
        run: |
          foreach ($file in (Get-ChildItem ./packages -Recurse -Include *.nupkg)) {
            dotnet nuget push $file --api-key "${{ secrets.NUGET_API_KEY }}" --source https://api.nuget.org/v3/index.json --skip-duplicate
          }
          foreach ($file in (Get-ChildItem ./packages -Recurse -Include *.snupkg)) {
            dotnet nuget push $file --api-key "${{ secrets.NUGET_API_KEY }}" --source https://api.nuget.org/v3/index.json --skip-duplicate
          }

      - name: Create GitHub Release
        uses: softprops/action-gh-release@v1
        with:
          files: packages/**/*.nupkg
          generate_release_notes: true
```

- **Acceptance Criteria**:
  - [x] ~~Composite action created~~ → Used `workflow_call` instead (better DRY approach)
  - [x] `release.yml` workflow uses `workflow_call` to reuse main.yml
  - [ ] NuGet API key stored as repository secret (manual step - requires repo admin)
  - [x] Version tags (`v*`) trigger releases
  - [x] GitHub Release created with auto-generated changelog
  - [x] `--skip-duplicate` prevents re-publish errors
  - [x] Environment approval gate for production-nuget

**Commits**: 815354e9 (feat(ci): add release workflow for NuGet publishing)

---

## Phase 2B: Supply Chain Security (CRITICAL)

### W2.13 Generate SBOM (Dual Pipeline) ✅ COMPLETE

- [x] **Task**: Generate Software Bill of Materials in BOTH build and release pipelines
- **Effort**: S (2-4 hours)
- **Priority**: **HIGH**
- **Dependencies**: W2.11
- **Files**: `.github/workflows/main.yml`, `.github/workflows/release.yml`
- **Completed**: 2025-12-06 (Session 18 - Phase 2B)
- **Commit**: `e569bb5`

**Rationale**: "We don't release often and want to make sure SBOM is always running"

**Design**:

1. **Build Pipeline**: Generate SBOM for validation (catches issues early)
2. **Release Pipeline**: Generate authoritative SBOM attached to GitHub Release

**Implementation - Build Pipeline**:

```yaml
- name: Generate SBOM (validation)
  uses: microsoft/sbom-tool@v4.1.4
  with:
    buildDropPath: ./artifacts/packages
    outputPath: ./artifacts/sbom
    packageName: Qwiq
    packageVersion: ${{ github.run_number }}
    manifestDirPath: ./artifacts/sbom

- name: Upload SBOM artifact
  uses: actions/upload-artifact@v4
  with:
    name: sbom-validation
    path: ./artifacts/sbom/
```

**Compliance Notes**: SPDX 2.2+ format meets NTIA Minimum Elements and Executive Order 14028 compliance.

- **Acceptance Criteria**:
  - [x] SBOM generated in build pipeline (validation)
  - [x] SBOM generated in release pipeline (authoritative)
  - [x] SPDX format with full dependency graph
  - [x] SBOM attached to GitHub Release
  - [x] Dependencies accurately listed including transitive

---

### W2.14 Add Dependency Review Action ✅ COMPLETE

- [x] **Task**: Block PRs that introduce vulnerable dependencies
- **Effort**: S (1-2 hours)
- **Priority**: **HIGH** (elevated from Medium)
- **Dependencies**: None
- **File**: `.github/workflows/main.yml`
- **Completed**: 2025-12-06 (Session 18 - Phase 2B)
- **Commit**: `5222a66`

**License Policy Rationale**:

| License      | Status   | Rationale                                               |
| ------------ | -------- | ------------------------------------------------------- |
| GPL-2.0      | ❌ Deny  | Copyleft: requires derivative works to be GPL-licensed  |
| GPL-3.0      | ❌ Deny  | Stronger copyleft with patent provisions                |
| AGPL-3.0     | ❌ Deny  | Network copyleft: even SaaS usage triggers requirements |
| LGPL-3.0     | ❌ Deny  | "Lesser" GPL still requires source disclosure           |
| MIT          | ✅ Allow | Permissive: Qwiq's own license                          |
| Apache-2.0   | ✅ Allow | Permissive with patent grant                            |
| BSD-3-Clause | ✅ Allow | Permissive with non-endorsement clause                  |
| 0BSD         | ✅ Allow | Public domain equivalent                                |

- **Acceptance Criteria**:
  - [x] Dependency review runs on all PRs
  - [x] Vulnerable dependencies blocked (moderate+ severity)
  - [x] License violations detected and blocked
  - [x] PR comments show dependency summary
  - [x] License policy documented in CONTRIBUTING.md

---

### W2.15 Pin GitHub Actions by SHA ✅ COMPLETE

- [x] **Task**: Use SHA-pinned action versions for supply chain security
- **Effort**: S (1-2 hours) ⏱️ Actual: ~30 minutes
- **Priority**: **CRITICAL** (elevated from Medium)
- **Dependencies**: None
- **Files**: `.github/dependabot.yml`, `renovate.json`
- **Completed**: 2025-12-06 (Session 16)

**Why Critical**: Supply chain attack vector (tag poisoning), SLSA Level 3 requirement.

**Implementation Approach**: Configured automation tools to handle SHA pinning:

- **Renovate** will automatically convert action version tags to SHA pins via `helpers:pinGitHubActionDigests` preset
- **Dependabot** configured as complementary tool for dependency management

- **Acceptance Criteria**:
  - [x] Dependabot configured with enhanced settings
  - [x] Renovate configured with `helpers:pinGitHubActionDigests` preset
  - [x] NuGet dependencies tracked by both tools
  - [x] .NET SDK updates configured
  - [ ] Actions will be pinned to SHAs automatically when Renovate sends first PR

**Commit**: 65c1a6b

---

### W2.17 SLSA Provenance Generation ✅ COMPLETE

- [x] **Task**: Generate cryptographic build provenance for supply chain security
- **Effort**: M (1 day)
- **Priority**: **CRITICAL**
- **Dependencies**: W2.11
- **File**: `.github/workflows/release.yml`
- **Completed**: 2025-12-06 (Session 18 - Phase 2B)
- **Commit**: `c4077d5`

**Implementation**:

```yaml
permissions:
  id-token: write
  contents: read
  actions: read

jobs:
  provenance:
    needs: build
    uses: slsa-framework/slsa-github-generator/.github/workflows/generator_generic_slsa3.yml@v2.0.0
    with:
      base64-subjects: "${{ needs.build.outputs.hashes }}"
      provenance-name: "qwiq-provenance.intoto.jsonl"
      upload-assets: true
```

- **Acceptance Criteria**:
  - [x] SLSA provenance generated for releases
  - [x] Provenance attached to GitHub Release
  - [x] Verification instructions documented

---

## Phase 2C: Testing Enhancements

### W2.16 REST/SOAP Unit Test Coverage

- [x] **Phase 1 (REST Offline)**: WireMock-based REST tests - **COMPLETE** 2025-12-12
- [x] **Phase 2 (SOAP Offline)**: SOAP unit tests (Windows-only, Moq-based) - **COMPLETE** 2025-12-10
- **Effort**: L (2-3 weeks total) ⏱️ Actual: Phase 1 (1 week), Phase 2 (1 day)
- **Priority**: **HIGH**
- **Files**: `test/Qwiq.WireMock.Tests/`, `test/Qwiq.Integration.Tests/Soap/`

**Phase 1 Outcome (REST offline)**:

- WireMock.Net + captured ADO traffic via Fiddler HAR → `scripts/Convert-HarToWireMock.ps1`
- **Moved to dedicated project**: `test/Qwiq.WireMock.Tests/` targeting net8.0/net9.0/net10.0
- Test suite: 9 tests (category `WireMock`)
- ADR: `docs/adr/008-wiremock-offline-rest-testing.md`

**Phase 2 Outcome (SOAP offline)**:

- Windows-only (net472, TFS Client OM)
- Uses Moq 4.16.0 + Moq.Analyzers 0.4.0
- Test suite: 13 tests (category `SoapUnit`)
- **Total**: 22 offline unit tests

- **Acceptance Criteria**:
  - [x] Phase 1: REST offline tests pass without Azure DevOps
  - [x] Phase 2: SOAP offline tests created with Moq
  - [ ] Phase 2 Validation: SOAP tests verified on Windows CI
  - [ ] CI updated with conditional SOAP execution

---

### W2.18 Enable Package Validation ✅ COMPLETE

- [x] **Task**: Detect breaking API changes automatically
- **Effort**: S (4 hours) ⏱️ Actual: ~45 minutes
- **Priority**: **HIGH**
- **Files**: All 9 packable project .csproj files
- **Completed**: 2025-12-06 (Session 16)
- **Commit**: 91c3244

**Implementation**:

```xml
<PropertyGroup>
  <EnablePackageValidation>true</EnablePackageValidation>
  <EnableStrictModeForCompatibleTfms>true</EnableStrictModeForCompatibleTfms>
  <EnableStrictModeForCompatibleFrameworksInPackage>true</EnableStrictModeForCompatibleFrameworksInPackage>
</PropertyGroup>
```

- **Acceptance Criteria**:
  - [x] Package validation enabled for all 9 packable projects
  - [x] Strict mode configured for TFM and framework compatibility
  - [x] Breaking changes will fail build (once baseline is set)
  - [ ] Baseline version to be set after next release

---

### W2.2 Create API Compatibility Baselines ✅ COMPLETE

- [x] **Task**: Establish API surface baselines for breaking change detection
- **Effort**: M (4-8 hours) ⏱️ Actual: ~4 hours
- **Priority**: **CRITICAL**
- **Dependencies**: W2.18
- **Files**: `Directory.Packages.props`, per-project PublicAPI files
- **Completed**: 2025-12-06 (Sessions 14-15)

**Implementation Complete**:

1. ✅ Added analyzer package to all 9 packable projects
2. ✅ Created PublicAPI.Shipped.txt and PublicAPI.Unshipped.txt files
3. ✅ Populated Unshipped.txt files with 1,268 API entries
4. ✅ Created framework-specific files for net472 polyfill types
5. ✅ Build passes with 0 RS00xx warnings

**API Entry Summary**: 1,268 total entries across 9 projects (911 in Qwiq.Core alone)

**Commits**: 9bb975c, 11c5f689, eed357c0, 6836de38, 2d068aa9

---

### W2.3 Add Contract Tests for REST/SOAP Parity

- [ ] **Task**: Create shared specification tests
- **Effort**: M (2-3 days)
- **Priority**: Low
- **Dependencies**: W2.16

---

### W2.4 Benchmark CI Integration ✅ COMPLETE

- [x] **Task**: Run benchmarks in CI (compile-only validation)
- **Effort**: S (< 1 hour actual)
- **Priority**: Low
- **Completed**: 2025-12-06 (Session 20 - Phase 2C)

---

## Phase 2D: Security Hardening

### W2.19 CodeQL Advanced Security ✅ COMPLETE

- [x] **Task**: Add advanced code scanning with CodeQL integrated into main build
- **Effort**: S (2 hours) ⏱️ Actual: ~30 minutes
- **Priority**: Medium
- **File**: `.github/workflows/main.yml` (integrated, not separate workflow)
- **Completed**: 2025-12-11 (Session Phase 2D)

**Design Decision**: Integrate CodeQL into the main build workflow to avoid duplicate builds.

- **Acceptance Criteria**:
  - [x] CodeQL integrated into main.yml
  - [x] Security-extended queries enabled
  - [x] Same build configuration as regular CI
  - [x] Weekly scheduled deep scan (Monday 2:30 AM UTC)

---

### W2.20 Secrets Scanning ✅ COMPLETE

- [x] **Task**: Add pre-commit secrets scanning
- **Effort**: S (1 hour) ⏱️ Actual: ~15 minutes
- **Priority**: Medium
- **File**: `.github/workflows/secrets.yml`
- **Completed**: 2025-12-11 (Session Phase 2D)

**Implementation**: Chose Gitleaks for automated scanning in CI/CD.

- **Acceptance Criteria**:
  - [x] Secret scanning enabled (Gitleaks)
  - [x] Historical scan enabled (fetch-depth: 0)
  - [x] Runs on push and pull_request events
  - [ ] No secrets detected in repository (will be verified after first run)

---

## Phase 2E: Documentation

### W2.5 Create Architecture Decision Records ✅ COMPLETE

- [x] **Task**: Document key architectural decisions
- **Effort**: M (1 day) ⏱️ Actual: ~3 hours
- **Priority**: **HIGH**
- **Location**: `docs/adr/`
- **Completed**: 2025-12-06 (Session 14)
- **Commit**: 28af61c

**Topics documented** (6 ADRs):

- ADR-001: Factory pattern for WorkItemStore
- ADR-002: Interface-first design
- ADR-003: REST vs SOAP client strategy
- ADR-004: Multi-targeting approach
- ADR-005: Central Package Management adoption
- ADR-006: Nullable reference types migration strategy

---

### W2.7 Update CONTRIBUTING.md

- [ ] **Task**: Modernize contribution guidelines
- **Effort**: S (2-4 hours)
- **Priority**: Medium
- **Dependencies**: W1.4, W1.5, W1.6

---

## Phase 2F: Code Quality & Security Hardening (PR #65 Bot Feedback)

### W2.21 Add Markdown Linting Configuration

- [ ] **Task**: Add `.prettierrc` and `.markdownlint-cli2.yaml`
- **Effort**: S (1-2 hours)
- **Priority**: **LOW** - Demoted (vanity metric, no active documentation audience)
- **Files**: `.prettierrc`, `.markdownlint-cli2.yaml`, `.github/workflows/main.yml`

---

### W2.22 Pin GitHub Actions to SHA Digests 🔴

- [ ] **Task**: Convert all version tags to commit SHA pins
- **Effort**: S (2-3 hours) ⚠️ Expert review: allow extra time for digest lookup
- **Priority**: **CRITICAL** - Supply chain attack prevention
- **Dependencies**: W2.15 (Dependabot/Renovate configured)
- **Files**: All `.github/workflows/*.yml` files

**Actions to Pin**: actions/checkout, actions/setup-dotnet, actions/upload-artifact, softprops/action-gh-release, github/codeql-action, gitleaks/gitleaks-action, slsa-framework/slsa-github-generator

---

### W2.23 Standardize Artifact Upload to v5

- [ ] **Task**: Update all `upload-artifact` and `download-artifact` to v5
- **Effort**: S (30 minutes)
- **Priority**: Medium

---

### W2.24 Add PowerShell Parameter Metadata

- [ ] **Task**: Add `[Parameter()]` attributes to all PowerShell scripts
- **Effort**: M (6-8 hours) ⚠️ Expert review: metadata ripple effects
- **Priority**: Medium
- **Files**: `scripts/*.ps1`, `build/scripts/*.ps1`

---

### W2.25 Null-Forgiving Operator Defensive Checks

- [ ] **Task**: Replace `!` null-forgiving operators with defensive null checks
- **Effort**: M (8-12 hours) ⚠️ Expert review: TDD requirement
- **Priority**: **LOW** - Demoted (32 instances, 0 bug reports)
- **Note**: Defer most work. Only fix if specific issues arise.
- **Files**: `test/Qwiq.Integration.Tests/WireMock/*.cs`

---

### W2.26 Remove Unused Code (Cleanup)

- [ ] **Task**: Remove unused fields and imports
- **Effort**: S (1-2 hours) ⚠️ Trace usage across TFMs
- **Priority**: Low
- **Files**: `test/Qwiq.Mocks/MockTfsConnectionFactory.cs`

---

### W2.27 Extend JSON Escaping for Control Characters

- [ ] **Task**: Add tab and control character escaping to `EscapeJson` method
- **Effort**: S (3-4 hours) ⚠️ Requires audit + fuzzing/unit tests
- **Priority**: Medium
- **File**: `scripts/Convert-HarToWireMock.ps1`

---

### W2.28 Fix Test Proxy Restoration

- [ ] **Task**: Restore original `WebRequest.DefaultWebProxy` instead of null
- **Effort**: S (15 minutes)
- **Priority**: Low
- **File**: `test/Qwiq.Integration.Tests/WireMock/RecordingTests.cs`

---

### W2.29 Service Resolution Null Guards

- [ ] **Task**: Add null checks for `GetService<T>()` calls
- **Effort**: S (1-2 hours) ⚠️ Guard placement affects constructor contracts
- **Priority**: **HIGH** - Null safety
- **File**: `src/Qwiq.Core/Extensions.cs`

---

### W2.30 Secrets Workflow Runner Documentation ✅ RESOLVED

- [x] **Task**: Document runner selection rationale
- **Status**: ✅ RESOLVED - No action needed, current setup is correct

**Runner Selection Policy** (clarified):

- **Preferred**: `ubuntu-latest` (Linux) - faster startup, lower cost
- **Use Windows when**: Building net472 targets (avoids mono)

---

### W2.31 Fix SLSA Verification Documentation

- [ ] **Task**: Correct wget/curl commands in SLSA verification instructions
- **Effort**: S (15 minutes)
- **Priority**: Low
- **File**: `docs/SLSA-VERIFICATION.md`

---

### W2.32 Add CI Warning Gate ✅ COMPLETE

- [x] **Task**: Add CI step to fail build if warnings exceed threshold
- **Effort**: S (1-2 hours)
- **Priority**: **CRITICAL** - Prevents regression of clean build state
- **Dependencies**: W1.19 (PedanticMode)
- **File**: `.github/workflows/main.yml`, `build/targets/codeanalysis/CodeAnalysis.targets`

**Implementation**: Uses PedanticMode - CI builds set `/p:ContinuousIntegrationBuild=true` which activates PedanticMode automatically.

**Verification (Session 2025-12-12)**:

- PedanticMode=true on CI: ✅ Confirmed
- TreatWarningsAsErrors=true on CI: ✅ Confirmed
- Build passes with 0 warnings: ✅ Confirmed
- Local builds allow warnings: ✅ Confirmed

---

### W2.33 NuGet 2.0.0 Publish 🔴

- [ ] **Task**: Publish first NuGet release in 7 years, declare maintenance mode
- **Effort**: S (2-4 hours)
- **Priority**: **CRITICAL** - Release milestone before maintenance mode
- **Dependencies**: W2.32 (CI Warning Gate), W2.22 (SHA Pinning)
- **Files**: `.github/workflows/release.yml`, `README.md`, GitHub Release

**Pre-Release Checklist**:

1. [ ] All CRITICAL Wave 2 tasks complete (W2.32, W2.22)
2. [ ] CI passing on develop branch
3. [ ] Version set to 2.0.0 via version.json
4. [ ] CHANGELOG/release notes drafted
5. [ ] README.md updated with maintenance mode notice

**NuGet.org Expectations**:

- 10 packages published (Core, Rest, Soap, Linq, Mapper, Identity, etc.)
- Symbol packages (.snupkg) included
- SLSA provenance attached to GitHub Release
- SBOM attached to GitHub Release

---

> **Next**: [Waves 3-5](modernize-wave3-5.md) - Framework Modernization & Enterprise Production
