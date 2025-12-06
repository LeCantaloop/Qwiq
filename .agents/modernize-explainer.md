# Qwiq Repository Modernization Explainer

> **Document Purpose**: Comprehensive Product Requirements Document (PRD) for modernizing the Qwiq repository.
> This document serves as the single source of truth for modernization planning and coordination.
>
> **Last Updated**: December 5, 2025 (Session 10 - Handoff Preparation)
> **Status**: Active Planning

---

## How to Use This Document

| Document | Purpose | Update Frequency |
|----------|---------|------------------|
| `modernize-explainer.md` (this file) | Strategic overview, architecture, gap analysis | Per wave completion |
| `modernize-TODO.md` | Task-level tracking, session logs | Every session |
| `copilot-instructions.md` | Agent behavioral guidance | As needed |

---

## Executive Summary

**Qwiq** (Quick Work Item Query) is a .NET library providing a simplified API for querying Azure DevOps / Team Foundation Server work items. The repository has already completed significant modernization work including migration to SDK-style projects, .NET 8 support, Central Package Management, and GitHub Actions CI/CD.

### Current Modernization Status

| Wave | Description | Status |
|------|-------------|--------|
| Wave 0 | .NET 8 adoption, SDK-style projects | ✅ **Complete** |
| Wave 1 | Code quality baselines, contribution enablement | 🔄 **In Progress** (18/27 tasks) |
| Wave 2 | Developer experience, observability, release automation | 📋 **Planned** |
| Wave 3 | Framework modernization, .NET 10 adoption | 📋 **Future** |

### Key Decisions Made

- **Target Frameworks**: Maintain `net472`, `netstandard2.0`, `net8.0` for maximum compatibility
- **.NET 10 Strategy**: Skip .NET 9 (STS), adopt .NET 10 (LTS) - SDK upgrade first, then TFM
- **SOAP Client**: Maintenance-only mode (bug fixes only, no new features)
- **REST Client**: Active development, cross-platform focus
- **Nullable Migration**: ✅ Complete - 0 CS8xxx warnings across all source projects
- **Risk Tolerance**: Low - each change must be independently revertible

---

## Latest Session Summary (December 5, 2025 - Session 10)

| Area | Update |
|------|--------|
| **Session Purpose** | Documentation cleanup and handoff preparation for next session |
| **Verification** | Build: ✅ 0 errors. Tests: ✅ 196 passed. Git: Clean working tree on `feat/modernize-2`. |
| **Corrections Made** | Fixed Quick Reference table (Wave 1: 18/27, Wave 2: 14 tasks). Added missing Session 7 log entry. |
| **Next Steps** | Continue Phase 1D (W1.15A-W1.18 Analyzer Debt), or start W2.11 (Release Automation). |

> **Where to look next:** Continue using the `.agents` folder as the single source of truth for modernization planning. No mirrors exist elsewhere in the repository.

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────┐
│                         Application Layer                            │
│                    (Consumer Applications)                           │
└─────────────────────────────────────────────────────────────────────┘
                                   │
                                   ▼
┌─────────────────────────────────────────────────────────────────────┐
│                       Integration Layer                              │
│  ┌────────────────┐  ┌─────────────────┐  ┌──────────────────┐     │
│  │ Qwiq.Linq     │  │ Qwiq.Mapper     │  │ Qwiq.Identity    │     │
│  │ .Identity     │  │ .Identity       │  │ .Soap            │     │
│  └───────┬───────┘  └────────┬────────┘  └────────┬─────────┘     │
└──────────┼───────────────────┼───────────────────┼──────────────────┘
           │                   │                   │
           ▼                   ▼                   ▼
┌─────────────────────────────────────────────────────────────────────┐
│                        Extension Layer                               │
│  ┌────────────────┐  ┌─────────────────┐  ┌──────────────────┐     │
│  │ Qwiq.Linq     │  │ Qwiq.Mapper     │  │ Qwiq.Identity    │     │
│  │ LINQ→WIQL     │  │ Object Mapping  │  │ Identity Mgmt    │     │
│  └───────┬───────┘  └────────┬────────┘  └────────┬─────────┘     │
└──────────┼───────────────────┼───────────────────┼──────────────────┘
           │                   │                   │
           └───────────────────┼───────────────────┘
                               ▼
┌─────────────────────────────────────────────────────────────────────┐
│                          Client Layer                                │
│  ┌─────────────────────────────┐  ┌────────────────────────────┐   │
│  │ Qwiq.Core.Rest              │  │ Qwiq.Core.Soap             │   │
│  │ net472/netstandard2.0/net8.0│  │ net472 only (Windows)      │   │
│  │ ✅ Active Development       │  │ 🔧 Maintenance Only        │   │
│  └──────────────┬──────────────┘  └─────────────┬──────────────┘   │
└─────────────────┼───────────────────────────────┼───────────────────┘
                  │                               │
                  ▼                               ▼
┌─────────────────────────────────────────────────────────────────────┐
│                           Core Layer                                 │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ Qwiq.Core - Interfaces: IWorkItem, IWorkItemStore, IQuery   │   │
│  │ net472 / netstandard2.0 / net8.0                            │   │
│  └─────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────────────┐
│                       External Systems                               │
│  ┌─────────────────────────┐  ┌────────────────────────────────┐   │
│  │ Azure DevOps Services   │  │ TFS On-Premises                │   │
│  │ (REST API)              │  │ (SOAP API)                     │   │
│  └─────────────────────────┘  └────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Wave 0: .NET 8 Adoption ✅ COMPLETE

### Status Summary

| Item | Status | Evidence |
|------|--------|----------|
| `global.json` pinned SDK | ✅ | v8.0.404 (updated Dec 5, 2025) |
| `nuget.config` configured | ✅ | Standard nuget.org source |
| SDK-style projects | ✅ | All 14 projects migrated |
| Supported TFMs | ✅ | net472, netstandard2.0, net8.0 |
| Central Package Management | ✅ | `Directory.Packages.props` |
| Nerdbank.GitVersioning | ✅ | v3.6.143 via `version.json` |
| Dependabot configured | ✅ | `.github/dependabot.yml` - weekly NuGet, SDK, Actions updates |

### Remaining Items

| Item | Status | Priority | Notes |
|------|--------|----------|-------|
| AnyCPU validation | ✅ | Done | All projects target AnyCPU |
| ARM64 testing | 📋 | Low | Future infrastructure concern (Wave 3) |

---

## Wave 1: Code Quality & Contribution Enablement 🔄 IN PROGRESS

**Progress**: 17/25 tasks complete (Phase 1A-1C done, 1D-1E remaining)

### 1.1 Dotfiles Status

| File | Status | Notes |
|------|--------|-------|
| `.gitignore` | ✅ | Standard .NET patterns |
| `.gitattributes` | ✅ | Updated Dec 5 - CRLF/LF consistency |
| `.editorconfig` | ✅ | Comprehensive rules configured |
| `CODEOWNERS` | ✅ | Created Dec 5 - `.github/CODEOWNERS` |

### 1.2 Documentation Status

| File | Status | Notes |
|------|--------|-------|
| `README.md` | ✅ | Badges updated (GitHub Actions) |
| `CONTRIBUTING.md` | 🔄 | Needs ADRs, PedanticMode docs |
| `TESTING.md` | ✅ | Comprehensive guide exists |
| `CODE_OF_CONDUCT.md` | ✅ | Contributor Covenant v2.1 |
| `SECURITY.md` | ✅ | Vulnerability reporting documented |

### 1.3 Package Quality

| Item | Status | Notes |
|------|--------|-------|
| Source Link | ✅ | Configured with .snupkg generation |
| PackageReadme | ✅ | All 10 packages have README.md |
| Code coverage in CI | ✅ | Configured (baseline TBD) |
| Package validation | ✅ | CI validates .nupkg + .snupkg output |

### 1.4 Code Quality Gates

| Item | Current | Target | Status |
|------|---------|--------|--------|
| `TreatWarningsAsErrors` | ✅ Enabled | ✅ | Done |
| Nullable warnings | 0 warnings | 0 | ✅ Complete (PR #52) |
| CS8xxx suppressions | 10 rules | 0 (when stable) | 🟡 Retained as safety net |
| CA analyzer rules | ~400 suppressed | <50 priority | 🔴 Phase 1D next |

---

## Wave 1 Remaining Gaps

### Gap 1: Analyzer Technical Debt (~400 Rules Suppressed) 🔴

**Current State**: Significantly more suppressions than initially estimated.

**Actual Suppression Counts** (verified):
```
Total suppressions: ~400 rules
├── CA1xxx (Design):       ~135
├── CA2xxx (Reliability):   ~66
├── CA3xxx-CA5xxx (Security): ~65
├── IDE (Code Style):      ~107
└── CS (Compiler):          ~27
```

**Phased Enablement Plan**:
| Priority | Category | Rules | Impact |
|----------|----------|-------|--------|
| P0 | Security | CA2100, CA5350, CA5351, CA3075 | Immediate security audit |
| P1 | Reliability | CA2000, CA1062, CA2007 | Dispose patterns, null validation |
| P2 | Performance | CA1822, CA1826, CA1845, CA1852 | Allocation reduction |
| P3 | Design | CA1000-CA1065 | API quality (after API compat) |

---

### Gap 2: Release Automation 🔴 CRITICAL (NEW)

**Current State**: No automated NuGet publishing pipeline.

**Required Changes**:
1. Create `release.yml` workflow triggered by tags
2. Implement automated changelog generation
3. Configure NuGet.org publishing with `--skip-duplicate`
4. Add GitHub Release creation with package links

---

### Gap 3: Supply Chain Security 🔴 CRITICAL (NEW)

**Current State**: Missing SBOM generation and package signing.

**Required Changes**:
1. Generate SPDX/CycloneDX SBOMs for packages
2. Implement NuGet package signing with code signing certificate
3. Add dependency-review-action to PRs
4. Pin GitHub Actions by SHA

---

### Gap 4: Cloud-Native Readiness 🟡 (NEW)

**Current State**: Library works but lacks cloud-native integrations.

**Required Changes**:
1. Add `Microsoft.Extensions.Configuration` support for credentials
2. Migrate from `System.Diagnostics.Trace` to `ILogger<T>`
3. Create Azure Functions / App Service deployment samples
4. Document Key Vault integration patterns

---

### Gap 5: Cross-Platform CI 🟡 (NEW)

**Current State**: CI only runs on Windows.

**Required Changes**:
1. Add Linux runner to CI matrix
2. Skip SOAP projects on Linux (net472 dependency)
3. Validate REST client cross-platform promise
4. Ensure path handling works on both platforms

---

### ~~Gap 1: Code Coverage Not Published in CI~~ ✅ RESOLVED

**Resolution**: Coverage collection configured in CI (Dec 5, 2025).
- `--collect:"XPlat Code Coverage"` added to test command
- Coverage reports uploaded as artifacts
- Baseline establishment pending first CI run

---

### ~~Gap 2: Source Link Not Configured~~ ✅ RESOLVED

**Resolution**: Source Link fully configured (Dec 5, 2025).
- `Microsoft.SourceLink.GitHub` 8.0.0 added
- Portable PDBs for Release builds
- Symbol packages (.snupkg) generated for all packages
- CI validation step verifies Source Link

---

### ~~Gap 3: PackageReadme Not Authored~~ ✅ RESOLVED

**Resolution**: All 10 packages have comprehensive READMEs (Dec 5, 2025).
- Created in `docs/package-readme/`
- Configured `PackageReadmeFile` in all packable projects
- Package tests verify README inclusion

---

### ~~Gap 4: Nullable Reference Types~~ ✅ RESOLVED

**Resolution**: 0 CS8xxx warnings achieved (PR #52, Dec 5, 2025).
- All 9 source projects have 0 nullable warnings
- 10 CS8xxx suppressions retained in `.editorconfig` as safety net
- Baseline report at `.agents/CS8xxx-baseline.md`
---

## Wave 2: Library Excellence 📋 PLANNED

**Target**: Establish production-grade library patterns and developer experience.

| ID | Task | Priority | Effort | Status |
|----|------|----------|--------|--------|
| W2.1 | Enable AnalysisLevel=latest | High | S | 📋 |
| W2.2 | Create API compatibility baselines | High | M | 📋 |
| W2.3 | Add contract tests for public API | Medium | M | 📋 |
| W2.4 | Implement property-based testing (FsCheck) | Medium | M | 📋 |
| W2.5 | Add performance benchmarks (BenchmarkDotNet) | Medium | M | 📋 |
| W2.6 | Migrate to record types (DTOs) | Low | L | 📋 |
| W2.7 | Add pattern matching to type checking | Low | M | 📋 |
| W2.8 | Add IConfiguration support for credentials | High | M | 📋 |
| W2.9 | Migrate Trace to ILogger<T> | Medium | L | 📋 |
| W2.10 | Add Azure Functions sample | Low | S | 📋 |
| W2.11 | Create release.yml automation | **Critical** | M | 📋 |
| W2.12 | Implement NuGet package signing | High | M | 📋 |
| W2.13 | Generate SBOM (SPDX/CycloneDX) | High | S | 📋 |
| W2.14 | Add dependency-review-action | Medium | S | 📋 |
| W2.15 | Pin GitHub Actions by SHA | Medium | S | 📋 |

---

## Wave 3: Long-Term Excellence 📋 FUTURE

**Target**: Prepare for future .NET versions and SOAP deprecation.

| ID | Task | Priority | Effort | Status |
|----|------|----------|--------|--------|
| W3.1 | OpenTelemetry basic tracing | Medium | M | 📋 |
| W3.2 | .NET 10 SDK upgrade (when available) | Medium | S | 📋 |
| W3.2a | Add net10.0 TFM | Medium | M | 📋 |
| W3.3 | ARM64 testing infrastructure | Low | S | 📋 |
| W3.4 | SOAP client deprecation plan | Low | L | 📋 |
| W3.5 | API compatibility policy document | Medium | S | 📋 |
| W3.6 | SOAP → REST migration guide | Medium | M | 📋 |
| W3.7 | Performance baseline establishment | Low | M | 📋 |

---

## Dependencies & Prerequisites

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        DEPENDENCY MAP (Updated Dec 5, 2025)             │
│                                                                         │
│  ┌──────────────────┐                                                   │
│  │ Wave 0 ✅        │                                                   │
│  │ (SDK, CPM, TFMs) │                                                   │
│  └────────┬─────────┘                                                   │
│           │                                                             │
│           ▼                                                             │
│  ┌────────────────────────────────────────────────────────────────────┐│
│  │ Wave 1 (68% Complete)                                              ││
│  │ ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              ││
│  │ │ Dotfiles ✅  │  │ Docs ✅      │  │ Nullable ✅  │              ││
│  │ └──────────────┘  └──────────────┘  └──────────────┘              ││
│  │ ┌──────────────┐  ┌──────────────┐                                 ││
│  │ │ Coverage ✅  │  │ Source Link ✅│                                ││
│  │ └──────────────┘  └──────────────┘                                 ││
│  │ ┌──────────────────────────────────────────────────┐              ││
│  │ │ CA Analyzer Debt (~400 rules) 🔴 IN PROGRESS     │              ││
│  │ └──────────────────────────────────────────────────┘              ││
│  └────────────────────────────────────────────────────────────────────┘│
│           │                                                             │
│           ▼                                                             │
│  ┌────────────────────────────────────────────────────────────────────┐│
│  │ Wave 2 (Planned)                                                   ││
│  │ ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              ││
│  │ │ API Compat   │  │ IConfiguration│ │ Release Auto │              ││
│  │ └──────────────┘  └──────────────┘  └──────────────┘              ││
│  │ ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              ││
│  │ │ SBOM Gen     │  │ Pkg Signing  │  │ ILogger<T>   │              ││
│  │ └──────────────┘  └──────────────┘  └──────────────┘              ││
│  └────────────────────────────────────────────────────────────────────┘│
│           │                                                             │
│           ▼                                                             │
│  ┌────────────────────────────────────────────────────────────────────┐│
│  │ Wave 3 (Future)                                                    ││
│  │ ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              ││
│  │ │ .NET 10 LTS  │  │ OpenTelemetry│  │ SOAP Deprec  │              ││
│  │ └──────────────┘  └──────────────┘  └──────────────┘              ││
│  └────────────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────────────┘
```

---

## Risk Assessment

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Analyzer changes break consumers | Medium | High | Phased rollout with API compat baselines |
| TFS SDK updates break SOAP client | Low | Medium | Pin versions, maintenance-only mode |
| Code coverage gates block PRs | Low | Medium | Start with warnings, graduate to gates |
| Missing release automation causes manual errors | **High** | **High** | **Priority: Create release.yml** |
| Supply chain vulnerabilities | Medium | High | Add SBOM, dependency-review-action |
| .NET 10 introduces breaking changes | Low | Medium | Skip .NET 9 (STS), adopt .NET 10 (LTS) |

---

## Success Metrics

### Wave 1 Completion Criteria (Updated)

| Metric | Baseline | Current | Target | Status |
|--------|----------|---------|--------|--------|
| Nullable warnings | ~300+ | 0 | 0 | ✅ Complete |
| Code coverage | Unknown | Configured | Published | ✅ Configured |
| CA rules suppressed | ~400 | ~400 | <50 priority | 🔴 Phase 1D |
| Documentation | 60% | 95% | 100% | 🟡 CONTRIBUTING |
| CI pipeline | AppVeyor | GitHub Actions | Green + artifacts | ✅ |
| Source Link | ❌ | ✅ | Verified | ✅ |
| PackageReadme | 0/10 | 10/10 | 10/10 | ✅ |

### Definition of Done (Updated)

Modernization is **complete** when:

1. ✅ All Wave 0 items verified
2. 🔄 All Wave 1 items completed (68% done)
3. ✅ Zero nullable warnings
4. ✅ Code coverage configured in CI
5. ✅ Source Link functional
6. ✅ All NuGet packages have PackageReadme
7. ✅ CODEOWNERS, SECURITY.md, CODE_OF_CONDUCT.md exist
8. ✅ README badges reflect GitHub Actions
9. 🔴 Priority CA rules enabled (P0-P2)
10. ✅ Changes documented in MIGRATION_NOTES.md
11. 📋 **NEW**: Release automation pipeline (Wave 2)
12. 📋 **NEW**: SBOM generation (Wave 2)

---

## Effort Estimates (Updated)

### Wave 1 Remaining (Phase 1D-1E)

| Item | Effort | Time (Solo) | Priority |
|------|--------|-------------|----------|
| W1.15 P0 Security Rules | M | 4-8 hours | **Critical** |
| W1.15 P1 Reliability Rules | M | 8-16 hours | High |
| W1.16 P2 Performance Rules | M | 8-16 hours | Medium |
| W1.17-W1.18 Design Rules | L | 1-2 weeks | Low |
| W1.24 Cross-Platform CI | S | 2-4 hours | Medium |

### Wave 2 Estimates

| Item | Effort | Time (Solo) | Priority |
|------|--------|-------------|----------|
| W2.11 Release Automation | M | 1-2 days | **Critical** |
| W2.13 SBOM Generation | S | 2-4 hours | High |
| W2.8 IConfiguration | M | 1-2 days | High |
| W2.2 API Compat Baselines | M | 4-8 hours | High |
| W2.12 Package Signing | M | 1 day | High |
| W2.9 ILogger Migration | L | 1-2 weeks | Medium |

**Effort Key**: S = Small (< 4 hours), M = Medium (4-16 hours), L = Large (> 16 hours)

---

## Appendix A: Project Target Frameworks

| Project | net472 | netstandard2.0 | net8.0 |
|---------|--------|----------------|--------|
| Qwiq.Core | ✅ | ✅ | ✅ |
| Qwiq.Core.Rest | ✅ | ✅ | ✅ |
| Qwiq.Core.Soap | ✅ | ❌ | ❌ |
| Qwiq.Linq | ✅ | ❌ | ✅ |
| Qwiq.Mapper | ✅ | ❌ | ✅ |
| Qwiq.Identity | ✅ | ❌ | ✅ |
| Qwiq.Identity.Soap | ✅ | ❌ | ❌ |
| Qwiq.Linq.Identity | ✅ | ❌ | ✅ |
| Qwiq.Mapper.Identity | ✅ | ❌ | ✅ |
| Qwiq.Mocks | ✅ | ❌ | ✅ |
| Qwiq.Tests.Common | ✅ | ❌ | ✅ |
| Qwiq.Core.Tests | ✅ | ❌ | ✅ |
| Qwiq.Linq.Tests | ✅ | ❌ | ✅ |
| Qwiq.Mapper.Tests | ✅ | ❌ | ✅ |
| Qwiq.Identity.Tests | ✅ | ❌ | ✅ |
| Qwiq.IntegrationTests | ✅ | ❌ | ❌ |
| Qwiq.Package.Tests | ❌ | ❌ | ✅ |

---

## Appendix B: Key Configuration Files

| File | Purpose | Status |
|------|---------|--------|
| `global.json` | SDK version pinning | ✅ v8.0.404 |
| `Directory.Build.props` | Shared MSBuild properties | ✅ Source Link configured |
| `Directory.Build.targets` | Shared build targets | ✅ |
| `Directory.Packages.props` | Central Package Management | ✅ |
| `.editorconfig` | Code style + analyzer severity | 🔴 ~400 suppressions |
| `version.json` | Nerdbank.GitVersioning config | ✅ |
| `nuget.config` | NuGet sources | ✅ |
| `.github/workflows/main.yml` | CI/CD pipeline | ✅ Coverage configured |
| `.github/dependabot.yml` | Dependency updates | ✅ NuGet, SDK, Actions |
| `.github/CODEOWNERS` | Code ownership | ✅ Created Dec 5 |

---

## Appendix C: Suppressed Analyzer Categories (Actual Counts)

### CS86xx (Nullable) - 10 rules (safety net only)
- CS8600, CS8602, CS8603, CS8604, CS8619
- CS8620, CS8625, CS8767, CS8769

### CA1xxx (Design) - ~135 rules suppressed
### CA2xxx (Reliability) - ~66 rules suppressed
### CA3xxx-CA5xxx (Security) - ~65 rules suppressed
### IDE0xxx (Style) - ~107 rules suppressed
### CS (Compiler) - ~27 rules suppressed

**Total**: ~400 analyzer rules currently suppressed

---

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | Dec 4, 2025 | Claudette | Initial comprehensive PRD |
| 2.0 | Dec 5, 2025 | Claudette (Session 7) | Updated with Wave 1 progress (68%), resolved gaps, added Wave 2/3 tasks, corrected analyzer count (~400), added supply chain security requirements |
| 2.1 | Dec 5, 2025 | Claudette (Session 8) | Key decision: Skip .NET 9 (STS), adopt .NET 10 (LTS). Strategy: SDK first, then TFM. Updated W3.1-W3.1a. |
