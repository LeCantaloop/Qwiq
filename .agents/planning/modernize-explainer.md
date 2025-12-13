# Qwiq Repository Modernization Explainer

> **Document Purpose**: Comprehensive Product Requirements Document (PRD) for modernizing the Qwiq repository.
> This document serves as the single source of truth for modernization planning and coordination.
>
> **Last Updated**: December 12, 2025 (Session 28 - Strategic Pivot to v11.0.0 Production Release)
> **Current Branch**: `feat/modernize-3` (commit: 767b30f0)
> **Status**: 🚀 **ACTIVE DEVELOPMENT** - Production v11.0.0 Release

---

## ⚠️ Strategic Direction Update: Session 27 INVALIDATED

> **Critical Context Received**: December 12, 2025 (Session 28)
> **Previous Decision (Session 27)**: ❌ MAINTENANCE MODE - **NOW INVALID** > **Current Status**: 🚀 **ACTIVE DEVELOPMENT** for v11.0.0 Production Release

### Why Session 27 Analysis Was Wrong

Session 27 concluded "maintenance mode" based on external adoption metrics for what is actually an **internal enterprise library**:

| Session 27 Interpretation                | Actual Reality                                           |
| ---------------------------------------- | -------------------------------------------------------- |
| "22 downloads/day = no users"            | **EXISTING internal team usage**                         |
| "Zero external contributors = abandoned" | **INTERNAL LIBRARY** - not public OSS                    |
| "7 years since NuGet publish = dormant"  | Fork of LeCantaloop/Qwiq v10.0.1, independent versioning |
| "No feature requests = no demand"        | **100+ team members** will use in production             |

### Actual Use Case (Session 28 Clarification)

- **Production deployment**: 100+ team members using this library
- **New application**: MCP extension for AI agents (work item management automation)
- **Target environment**: Kubernetes containers (requires net8.0/net9.0/net10.0)
- **Enterprise security**: Must pass security review gates
- **Version target**: v11.0.0 (breaking changes acceptable)
- **SOAP status**: Cannot deprecate - REST lacks write operations

### Corrected Strategic Direction

- **Waves 3-4**: ✅ **RE-ACTIVATED** - Security and modernization required for production
- **Wave 5**: ✅ **ADDED** - Enterprise production readiness (W5.1-W5.8)
- **Coverage target**: 70% (not 46%) - Production deployment requires confidence
- **Target frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
- **Timeline**: 6-8 weeks for production-ready v11.0.0 release

---

## How to Use This Document

| Document                             | Purpose                                        | Update Frequency      |
| ------------------------------------ | ---------------------------------------------- | --------------------- |
| `modernize-explainer.md` (this file) | Strategic overview, architecture, gap analysis | Per wave completion   |
| `modernize-TODO-index.md`            | Overview, metrics, and navigation              | Every session         |
| `modernize-wave1.md`                 | Wave 0-1 task tracking                         | After Wave 0-1 tasks  |
| `modernize-wave2.md`                 | Wave 2 task tracking                           | After Wave 2 tasks    |
| `modernize-wave3-5.md`               | Waves 3-5 task tracking                        | After Waves 3-5 tasks |
| `copilot-instructions.md`            | Agent behavioral guidance                      | As needed             |

---

## Executive Summary

**Qwiq** (Quick Work Item Query) is a .NET library providing a simplified API for querying Azure DevOps / Team Foundation Server work items. The repository has completed significant modernization work including migration to SDK-style projects, .NET 8 support, Central Package Management, and GitHub Actions CI/CD.

### Current Modernization Status

| Wave   | Description                                                | Status                                        |
| ------ | ---------------------------------------------------------- | --------------------------------------------- |
| Wave 0 | .NET 8 adoption, SDK-style projects                        | ✅ **Complete**                               |
| Wave 1 | Code quality baselines, contribution enablement            | ✅ **Complete** (25/26 tasks, W1.18 deferred) |
| Wave 2 | Developer experience, release automation                   | 🔄 **In Progress** (9/26 tasks remaining)     |
| Wave 3 | Framework modernization, .NET 9/10 adoption, TFM expansion | 📋 **RE-ACTIVATED** (Session 28)              |
| Wave 4 | Test quality, mutation testing, coverage excellence        | 📋 **RE-ACTIVATED** (Session 28)              |
| Wave 5 | Enterprise production readiness, security audit            | 📋 **NEW** (Session 28)                       |

### Key Decisions Made (.NET Version Knowledge as of December 12, 2025)

#### .NET Support Status (Current as of December 12, 2025)

| Version | Released     | Latest Patch          | Type    | Status    | End of Support   |
| ------- | ------------ | --------------------- | ------- | --------- | ---------------- |
| .NET 10 | Nov 11, 2025 | 10.0.1 (Dec 9, 2025)  | **LTS** | ✅ Active | Nov 14, **2028** |
| .NET 9  | Nov 12, 2024 | 9.0.11 (Nov 11, 2025) | STS     | ✅ Active | Nov 10, 2026     |
| .NET 8  | Nov 14, 2023 | 8.0.22 (Nov 11, 2025) | LTS     | ✅ Active | Nov 10, 2026     |

**Key Update**: .NET 10 GA'd on November 11, 2025 and is now **stable LTS with 3-year support**.

#### Target Framework Strategy

- **Project Status**: 🚀 **ACTIVE DEVELOPMENT** for production v11.0.0 release
- **Target Frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
  - **net472**: Minimum for SOAP SDK (Windows-only dependency)
  - **net48/net481**: Compiler optimizations and different binding decisions (not just binary compatibility)
  - **net8.0**: LTS until Nov 2026
  - **net9.0**: STS until Nov 2026
  - **net10.0**: LTS until Nov 2028 (primary modern target)
  - **netstandard2.0**: Being phased out with expanded .NET Framework coverage
- **.NET 10 Strategy**: ✅ **NOW AVAILABLE** - Add net10.0 TFM to all projects except SOAP
- **SOAP Client**: Remains `net472` only (Windows SDK hard constraint)
- **REST Client**: Full multi-targeting for container/cross-platform deployment
- **Nullable Migration**: ✅ Complete - 0 CS8xxx warnings across all source projects
- **Risk Tolerance**: Medium - Breaking changes acceptable for v11.0.0
- **Coverage Target**: 70% (enterprise production requirement)

---

## Latest Session Summary (December 12, 2025 - Session 28)

| Area                   | Update                                                                  |
| ---------------------- | ----------------------------------------------------------------------- |
| **Session Purpose**    | Strategic pivot from maintenance mode to production v11.0.0 release     |
| **Critical Discovery** | Session 27 analyzed external metrics for an internal enterprise library |
| **Actual Use Case**    | 100+ team members, MCP extension for AI agents, Kubernetes deployment   |
| **Agents Used**        | high-level-advisor, csharp-expert, feature-request-review               |
| **Key Decision**       | 🚀 **ACTIVE DEVELOPMENT** - All waves re-activated + Wave 5 added       |
| **Waves Status**       | Waves 3-4 RE-ACTIVATED, Wave 5 NEW (enterprise production tasks)        |
| **Timeline**           | 6-8 weeks for production-ready v11.0.0                                  |
| **Coverage Target**    | 70% (not 46%) - Enterprise requirement                                  |
| **TFM Strategy**       | `net472;net48;net481;net8.0;net9.0;net10.0`                             |

### .NET Version Knowledge Update

**As of December 12, 2025**, .NET 10 is **GA and stable**:

- Released: November 11, 2025
- Latest patch: 10.0.1 (December 9, 2025)
- Support type: **LTS** (3-year support until November 14, 2028)
- Status: Production-ready, recommend adding net10.0 TFM to all projects

### Multi-Agent Strategic Re-Analysis

Three agents re-evaluated Session 27 conclusions:

| Agent                      | Finding                                                          | Recommendation                            |
| -------------------------- | ---------------------------------------------------------------- | ----------------------------------------- |
| high-level-advisor         | Measuring external metrics for internal library = WRONG          | RE-ACTIVATE Waves 3-4, add Wave 5         |
| csharp-expert              | TFM constraints: net462-net471 impossible (SDK requires net472+) | net472;net48;net481;net8.0;net9.0;net10.0 |
| feature-request-review     | 70% coverage needed for complex LINQ code with 100+ users        | Restore 70% target                        |
| **csharp-expert**          | Technical debt resolved, 8 suppressions justified                | Ship it, stop investing                   |
| **feature-request-review** | Zero external feature requests in 7 years                        | No demand for new features                |
| **independent-thinker**    | 22 downloads/day suggests CI caching only                        | Existing users have working versions      |
| **create-explainer**       | Documentation effort not justified                               | Maintenance mode appropriate              |
| **generate-tasks**         | Waves 3-4 scope excessive for usage level                        | Cancel future waves                       |

### Previous Session (December 12, 2025 - Session 26)

| Area                | Update                                                                                         |
| ------------------- | ---------------------------------------------------------------------------------------------- |
| **Session Purpose** | Multi-agent consensus analysis of CA technical debt                                            |
| **Key Finding**     | "~400 suppressed rules" was a measurement artifact - only **8 active suppressions** exist      |
| **Verification**    | Build: ✅ 0 errors, 0 warnings. All 8 suppressions are documented design decisions.            |
| **Outcome**         | Original 7+ day remediation plan cancelled. Wave 1 confirmed complete (25/26, W1.18 deferred). |

---

## Multi-Agent Analysis: Maintenance Mode Decision

### Process

The maintenance mode decision was reached through a **structured multi-agent consensus process**:

1. **Data Collection**: Gathered objective metrics (NuGet stats, GitHub activity, contributor history)
2. **Independent Analysis**: Five agents analyzed the data independently
3. **Consensus Building**: Agents shared findings and reached unanimous agreement
4. **Decision Documentation**: This document captures the reasoning and evidence

### Evidence Collected

#### NuGet Package Statistics

| Package     | Last Published | Total Downloads | Daily Average |
| ----------- | -------------- | --------------- | ------------- |
| Qwiq.Core   | Feb 2018       | ~8,000          | ~22/day       |
| Qwiq.Mapper | Feb 2018       | ~6,000          | ~16/day       |
| Qwiq.Linq   | Feb 2018       | ~5,500          | ~15/day       |

**Interpretation**: Low daily downloads suggest CI pipeline artifact caching, not active adoption.

#### GitHub Activity Analysis

| Metric                     | Value | Last Active                         |
| -------------------------- | ----- | ----------------------------------- |
| External Contributors      | 0     | N/A                                 |
| Feature Requests           | 0     | N/A                                 |
| Bug Reports                | 0     | N/A                                 |
| Human Contributors (2023+) | 0     | 2022                                |
| Bot Contributors           | 3     | Active (Copilot, Devin, Dependabot) |

**Interpretation**: Project has no external community. All recent activity is automated.

#### Technical Debt Analysis Corrections

| Claim                     | Original               | Corrected                      |
| ------------------------- | ---------------------- | ------------------------------ |
| Suppressed analyzer rules | ~400                   | 8                              |
| null! instances           | "Mostly test fixtures" | 32 in production               |
| Coverage adequacy         | "Needs 65% mutation"   | 46% acceptable for maintenance |
| Remediation effort        | 7+ days                | 0 days (no debt exists)        |

### Agent Recommendations

| Agent                      | Key Finding                                                  | Recommendation                          |
| -------------------------- | ------------------------------------------------------------ | --------------------------------------- |
| **csharp-expert**          | All 8 suppressions are justified design decisions            | No further analyzer work needed         |
| **feature-request-review** | Zero demand signals in 7 years                               | Stop building features no one requested |
| **independent-thinker**    | ROI calculation: 24 weeks work / 0 users = ∞                 | Cancel Waves 3-4                        |
| **create-explainer**       | Documentation for internal library with no external audience | Minimal docs sufficient                 |
| **generate-tasks**         | 38 planned tasks have no user benefit                        | Focus on shipping                       |

### Final Decision

**Unanimous Consensus**: Complete Wave 2 critical tasks, publish NuGet 2.0.0, declare maintenance mode.

| Decision                | Rationale                                     |
| ----------------------- | --------------------------------------------- |
| **Ship NuGet 2.0.0**    | 7 years of improvements deserve a release     |
| **Cancel Waves 3-4**    | 24-28 weeks of work with no users to benefit  |
| **Maintenance mode**    | Security patches and dependency updates only  |
| **No breaking changes** | Existing (rare) users should not be disrupted |

---

## Architecture Overview

```text
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

| Item                       | Status | Evidence                                                      |
| -------------------------- | ------ | ------------------------------------------------------------- |
| `global.json` pinned SDK   | ✅     | v8.0.404 (updated Dec 5, 2025)                                |
| `nuget.config` configured  | ✅     | Standard nuget.org source                                     |
| SDK-style projects         | ✅     | All 14 projects migrated                                      |
| Supported TFMs             | ✅     | net472, netstandard2.0, net8.0                                |
| Central Package Management | ✅     | `Directory.Packages.props`                                    |
| Nerdbank.GitVersioning     | ✅     | v3.6.143 via `version.json`                                   |
| Dependabot configured      | ✅     | `.github/dependabot.yml` - weekly NuGet, SDK, Actions updates |

### Remaining Items

| Item              | Status | Priority | Notes                                  |
| ----------------- | ------ | -------- | -------------------------------------- |
| AnyCPU validation | ✅     | Done     | All projects target AnyCPU             |
| ARM64 testing     | 📋     | Low      | Future infrastructure concern (Wave 3) |

---

## Wave 1: Code Quality & Contribution Enablement ✅ COMPLETE

**Progress**: 25/26 tasks complete (W1.18 deferred pending W2.2 API baselines)

### 1.1 Dotfiles Status ✅

| File             | Status | Notes                                |
| ---------------- | ------ | ------------------------------------ |
| `.gitignore`     | ✅     | Standard .NET patterns               |
| `.gitattributes` | ✅     | Updated Dec 5 - CRLF/LF consistency  |
| `.editorconfig`  | ✅     | Comprehensive rules configured       |
| `CODEOWNERS`     | ✅     | Created Dec 5 - `.github/CODEOWNERS` |

### 1.2 Documentation Status ✅

| File                 | Status | Notes                              |
| -------------------- | ------ | ---------------------------------- |
| `README.md`          | ✅     | Badges updated (GitHub Actions)    |
| `CONTRIBUTING.md`    | ✅     | Updated with coverage, licensing   |
| `TESTING.md`         | ✅     | Comprehensive guide exists         |
| `CODE_OF_CONDUCT.md` | ✅     | Contributor Covenant v2.1          |
| `SECURITY.md`        | ✅     | Vulnerability reporting documented |

### 1.3 Package Quality ✅

| Item                | Status | Notes                                |
| ------------------- | ------ | ------------------------------------ |
| Source Link         | ✅     | Configured with .snupkg generation   |
| PackageReadme       | ✅     | All 10 packages have README.md       |
| Code coverage in CI | ✅     | 46.1% line coverage                  |
| Package validation  | ✅     | CI validates .nupkg + .snupkg output |

### 1.4 Code Quality Gates ✅

| Item                    | Current    | Target          | Status                             |
| ----------------------- | ---------- | --------------- | ---------------------------------- |
| `TreatWarningsAsErrors` | ✅ Enabled | ✅              | Done                               |
| Nullable warnings       | 0 warnings | 0               | ✅ Complete (PR #52)               |
| CS8xxx suppressions     | 10 rules   | 0 (when stable) | ✅ Safety net, no violations       |
| CA analyzer rules       | 8 active   | 8 documented    | ✅ **RESOLVED** (design decisions) |

---

## Gap Analysis (Updated for Maintenance Mode)

### Analysis Correction: "~400 Suppressions" Was a Measurement Artifact

**Original Claim (Sessions 1-12)**: ~400 analyzer rules suppressed, requiring 7+ days remediation.

**Actual Finding (Session 26 - Multi-Agent Consensus)**:

- The "~400" figure came from counting **all `.editorconfig` entries**, not actual suppressions
- **Only 8 active suppressions** exist in production code
- All 8 are **documented design decisions**, not technical debt

### The 8 Active Suppressions (Validated Design Decisions)

| Rule   | Count  | Status         | Justification                                |
| ------ | ------ | -------------- | -------------------------------------------- |
| CS1591 | ~4200  | 🟡 Deferred    | XML docs - low ROI for internal library      |
| CS0618 | 1      | ✅ Intentional | TimeZone API - breaking change not justified |
| CA1707 | 868    | ✅ Intentional | Test naming (Given_When_Then BDD)            |
| CA1716 | 78     | ✅ Intentional | Keyword conflicts - API design choice        |
| CA1822 | 36     | 🟡 Deferred    | Static methods - API compatibility           |
| CA1859 | 30     | ✅ Intentional | Concrete types - testability abstraction     |
| CA1863 | 20     | 🟡 Deferred    | CompositeFormat - .NET 8+ only               |
| CA2263 | scoped | ✅ Intentional | Test-specific scope                          |

**Verification**: Build produces **0 warnings, 0 errors** as of Session 26.

---

### Gap Status Summary (Maintenance Mode)

| Gap                           | Original Status | Current Status   | Resolution                         |
| ----------------------------- | --------------- | ---------------- | ---------------------------------- |
| Gap 1: Analyzer Debt          | 🔴 CRITICAL     | ✅ RESOLVED      | Only 8 suppressions, all justified |
| Gap 2: Release Automation     | 🔴 CRITICAL     | 🟡 DEPRIORITIZED | Manual release acceptable          |
| Gap 3: Supply Chain Security  | 🔴 CRITICAL     | 🟡 PARTIAL       | SHA pinning (W2.22), SBOM exists   |
| Gap 4: Cloud-Native Readiness | 🟡 MEDIUM       | ❌ CANCELLED     | Wave 3 cancelled                   |
| Gap 5: Cross-Platform CI      | 🟡 MEDIUM       | ❌ CANCELLED     | Not needed for maintenance         |
| Coverage Not Published        | ✅ RESOLVED     | ✅ RESOLVED      | 46.1% line coverage                |
| Source Link                   | ✅ RESOLVED     | ✅ RESOLVED      | Fully configured                   |
| PackageReadme                 | ✅ RESOLVED     | ✅ RESOLVED      | All 10 packages                    |
| Nullable Types                | ✅ RESOLVED     | ✅ RESOLVED      | 0 CS8xxx warnings                  |

### Remaining Critical Gaps (Sprint 4)

#### Gap A: CI Warning Gate 🔴 CRITICAL

**Current State**: Build is clean (0 warnings) but no gate prevents regression.

**Required Action**: W2.32 - Add `TreatWarningsAsErrors` gate to CI.

---

#### Gap B: GitHub Actions SHA Pinning 🔴 CRITICAL

**Current State**: Actions referenced by tag, vulnerable to supply chain attacks.

**Required Action**: W2.22 - Pin all Actions by SHA.

---

#### Gap C: NuGet 2.0.0 Not Published 🔴 CRITICAL

**Current State**: Last publish was February 2018 (7 years ago).

**Required Action**: W2.33 - Publish NuGet 2.0.0 with all modernization work.

---

### Gaps Cancelled (Maintenance Mode)

| Gap                    | Original Plan                          | Cancellation Reason            |
| ---------------------- | -------------------------------------- | ------------------------------ |
| Cloud-Native Readiness | ILogger, IConfiguration, OpenTelemetry | No external demand             |
| Cross-Platform CI      | Linux runner matrix                    | Not needed for maintenance     |
| Package Signing        | Azure Key Vault integration            | Overkill for ~22 downloads/day |
| SLSA Provenance        | Level 3 attestation                    | Overkill for dormant project   |

- All 9 source projects have 0 nullable warnings
- 10 CS8xxx suppressions retained in `.editorconfig` as safety net
- Baseline report at `.agents/CS8xxx-baseline.md`

---

## Wave 2: Developer Experience & Production Readiness

> **Updated**: December 12, 2025 (Session 28 - Production v11.0.0 Release)
> **Status**: 🔄 **IN PROGRESS** - Critical tasks for production readiness

**Target**: Ship v11.0.0, protect build quality, ensure supply chain security, achieve 70% coverage.

### Sprint 4 Priority Order (Production v11.0.0)

| Priority        | ID        | Task                      | Effort | Status | Notes                            |
| --------------- | --------- | ------------------------- | ------ | ------ | -------------------------------- |
| 1. **CRITICAL** | W2.32     | CI Warning Gate           | S      | 📋 NEW | Protect clean build state        |
| 2. **CRITICAL** | W2.22     | SHA Pinning               | S      | 📋     | Supply chain security            |
| 3. **CRITICAL** | **W2.33** | **NuGet v11.0.0 Publish** | S      | 📋     | Production release (was 2.0.0)   |
| 4. HIGH         | W2.29     | Service Null Guards       | M      | 📋     | Runtime safety                   |
| 5. MEDIUM-HIGH  | W2.25     | null! cleanup             | M      | 📋     | 20+ instances in production code |
| 6. MEDIUM       | W2.21     | Markdown Linting          | S      | 📋     | Code quality                     |

### Active Tasks (Production Priority)

| ID    | Task                 | Priority | Effort | Status          | Rationale                          |
| ----- | -------------------- | -------- | ------ | --------------- | ---------------------------------- |
| W2.11 | Release automation   | MEDIUM   | M      | 📋              | Streamline v11.0.0 release         |
| W2.17 | SLSA Provenance      | MEDIUM   | M      | 📋              | Enterprise security requirement    |
| W2.13 | SBOM Generation      | LOW      | S      | 📋              | Already done in CI                 |
| W2.14 | dependency-review    | LOW      | S      | ✅ DONE         | Already configured                 |
| W2.16 | REST/SOAP Tests      | MEDIUM   | L      | 🔄 Phase 1 done | Complete Phase 2 for production    |
| W2.2  | API compat baselines | MEDIUM   | M      | 📋              | Track breaking changes for v11.0.0 |
| W2.3  | Contract tests       | HIGH     | M      | 📋              | Required for Wave 4                |
| W2.4  | Benchmark CI         | ✅       | S      | ✅ DONE         | Already complete                   |
| W2.5  | ADRs                 | LOW      | M      | 📋              | Document architectural decisions   |
| W2.7  | CONTRIBUTING.md      | LOW      | S      | 📋              | Update for internal team           |
| W2.19 | CodeQL               | MEDIUM   | S      | 📋              | Enterprise security scanning       |
| W2.20 | Secrets Scanning     | MEDIUM   | S      | 📋              | Enterprise security requirement    |

### Wave 2 Tasks Moved to Wave 3 (Now RE-ACTIVATED)

- ~~W2.8~~ → W3.9 (IConfiguration Support) - **RE-ACTIVATED** for cloud-native deployment
- ~~W2.9~~ → W3.8 (Observability Overhaul) - **RE-ACTIVATED** for production monitoring
- ~~W2.1~~ → W3.8 (Observability Overhaul) - **RE-ACTIVATED**
- ~~W2.12~~ → W3.10 (Package Signing) - **RE-ACTIVATED** as CRITICAL for enterprise security

---

## Wave 3: Framework Modernization & TFM Expansion 📋 RE-ACTIVATED

> **Updated**: December 12, 2025 (Session 28 - Production v11.0.0 Release)
> **Status**: 📋 **RE-ACTIVATED** - Required for container deployment and modern runtime support

### Re-Activation Rationale

| Factor              | Session 27 (Wrong)              | Session 28 (Corrected)                       |
| ------------------- | ------------------------------- | -------------------------------------------- |
| **User demand**     | "0 feature requests"            | 100+ team members production deployment      |
| **Usage pattern**   | "22 downloads/day = CI caching" | Internal enterprise library usage            |
| **Runtime targets** | "net8.0 sufficient"             | Kubernetes containers require net9.0/net10.0 |
| **ROI calculation** | "6+ weeks not justified"        | Required for production deployment           |
| **.NET 10 status**  | "Not released yet"              | ✅ **GA'd Nov 11, 2025** - stable LTS        |

### TFM Expansion Strategy (CORRECTED)

**Previous (Incorrect) Analysis**: "net48/net481 provide no benefit - just binary compatibility"

**Corrected Understanding**: net48/net481 **DO** provide value:

- Compiler makes **different binding decisions** based on available APIs
- **Not just binary compatibility** - actual runtime optimization benefits
- Performance improvements from newer BCL implementations

| TFM                   | Status            | Reason                                                |
| --------------------- | ----------------- | ----------------------------------------------------- |
| net462, net47, net471 | ❌ CANNOT SUPPORT | SDK hard constraint - ExtendedClient requires net472+ |
| net472                | ✅ KEEP           | Minimum for SOAP SDK                                  |
| net48                 | ✅ **ADD**        | Compiler optimizations, different binding decisions   |
| net481                | ✅ **ADD**        | Compiler optimizations, runtime improvements          |
| net8.0                | ✅ KEEP           | LTS until Nov 2026                                    |
| net9.0                | ✅ **ADD**        | STS until Nov 2026, required for Kubernetes           |
| net10.0               | ✅ **ADD**        | LTS until Nov 2028, primary modern target             |

**Final TFM Configuration**:

- Core/REST/Identity/Linq/Mapper: `net472;net48;net481;net8.0;net9.0;net10.0`
- SOAP projects: `net472` only (Windows SDK constraint)
- netstandard2.0: Phase out with expanded .NET Framework coverage

### Wave 3 Tasks

| ID    | Task                                           | Status          | Priority     | Effort |
| ----- | ---------------------------------------------- | --------------- | ------------ | ------ |
| W3.1  | TFM Expansion (net48, net481, net9.0, net10.0) | 📋 RE-ACTIVATED | **CRITICAL** | M      |
| W3.2  | ARM64 Validation                               | 📋 RE-ACTIVATED | MEDIUM       | S      |
| W3.3  | Remove AppVeyor Configuration                  | 📋 RE-ACTIVATED | LOW          | XS     |
| W3.4  | Deprecate netstandard2.0                       | 📋 RE-ACTIVATED | MEDIUM       | S      |
| W3.5  | API Compatibility Policy                       | 📋 RE-ACTIVATED | MEDIUM       | S      |
| W3.6  | SOAP → REST Migration Guide                    | 📋 RE-ACTIVATED | HIGH         | M      |
| W3.7  | Performance Baselines                          | 📋 RE-ACTIVATED | MEDIUM       | M      |
| W3.8  | Observability Overhaul (ILogger/OpenTelemetry) | 📋 RE-ACTIVATED | HIGH         | L      |
| W3.9  | IConfiguration Support                         | 📋 RE-ACTIVATED | MEDIUM       | M      |
| W3.10 | Package Signing                                | 📋 RE-ACTIVATED | **CRITICAL** | M      |

**Note**: W3.6 elevated to HIGH because SOAP cannot be fully deprecated - REST still lacks write operations.

---

## Wave 4: Test Quality & Coverage Excellence 🔄 IN PROGRESS

> **Created**: December 11, 2025 (Session 25)
> **Re-Activated**: December 12, 2025 (Session 28 - Production v11.0.0 Release)
> **Status**: 🔄 **IN PROGRESS** - Phase 1 COMPLETE (December 12, 2025 - Session 30)

### Session 30 Phase 1 Completion

#### Phase 1: Baseline & Planning ✅ COMPLETE (Weeks 1-2)

| Task                            | Status | Key Finding                                            |
| ------------------------------- | ------ | ------------------------------------------------------ |
| W4.1 - Test Execution Baseline  | ✅     | 189 tests, 11.58s execution (**Exceeds** <300s target) |
| W4.2 - Test Flake Rate          | ✅     | 0.00% flake rate (**Exceeds** <0.1% target)            |
| W4.3 - Code Coverage Assessment | ✅     | 51.1% coverage, REST client at 0% (critical gap)       |
| W4.4 - SOAP Usage Assessment    | ✅     | Recommend deprecation (ADR-010)                        |
| W4.5 - Test Improvement Plan    | ✅     | 16-week roadmap created                                |

**Key Artifacts Created**:

- `.agents/WAVE4-TEST-IMPROVEMENT-PLAN.md` - Comprehensive 16-week improvement roadmap
- `docs/metrics/test-baseline.md` - Test execution baseline metrics
- `docs/metrics/test-flakiness-report.md` - Flakiness analysis (0% flake rate)
- `docs/adr/ADR-010-soap-client-deprecation-strategy.md` - SOAP deprecation decision
- `scripts/Measure-TestFlakiness.ps1` - Automated flakiness measurement tool

### Re-Activation Rationale

| Factor                  | Session 27 (Wrong)                 | Session 28 (Corrected)                             |
| ----------------------- | ---------------------------------- | -------------------------------------------------- |
| **Timeline**            | "18-20 weeks not justified"        | Required for production deployment                 |
| **Current coverage**    | "46.1% acceptable for maintenance" | 70% required for 100+ team production use          |
| **Usage level**         | "22 downloads/day = no users"      | Internal enterprise library with 100+ team members |
| **External engagement** | "Zero = no benefit"                | **INTERNAL** library - wrong metric                |

### Production Coverage Requirements

For a **production library** serving:

- 100+ team members
- MCP extension for AI agents
- Kubernetes container deployment
- Enterprise security review requirements

**70% line coverage is minimum acceptable.** Complex LINQ query translation code requires high path coverage to prevent runtime bugs in production.

### Strategic Path to 70% (Session 30 Analysis)

**Current State**: 51.1% line coverage, 36.7% branch coverage
**Gap to Target**: +18.9% line coverage needed

**Prioritized Approach**:

1. **Phase 2 (Weeks 3-6)**: REST Client Coverage Expansion

   - Add tests for WorkItemStore, Query, WorkItem classes (0% → 60%)
   - Add tests for Core auth/credentials (0% → 50%)
   - **Expected Impact**: +14% overall coverage → **65% overall**

2. **Phase 3 (Weeks 7-8)**: Final Gap Closure

   - Mapper exceptions and edge cases
   - LINQ QueryExtensions (20% → 80%)
   - **Expected Impact**: +5% overall coverage → **70% overall** ✅ TARGET ACHIEVED

3. **Phase 4 (Weeks 9-12)**: Mutation Testing

   - Target: 65% mutation score on critical paths
   - Focus: LINQ WiqlTranslator, Core TypeParser

4. **Phase 5 (Weeks 13-16)**: Offline Testing
   - Target: 80% of tests runnable offline via WireMock

### SOAP Client Decision (W4.4 Complete)

**Recommendation**: Deprecate SOAP client in v11.0.0, remove in v12.0.0

**Rationale**:

- Cannot deploy in Kubernetes (Windows-only dependency)
- 0% automated test coverage
- ~2,296 LOC maintenance burden
- Microsoft recommends REST API

**Deprecation Plan**:

- **v11.0.0** (current): Mark deprecated, add migration guide, 6-month support
- **v11.x**: Migration support, critical bugs only
- **v12.0.0**: Remove SOAP projects entirely

See `docs/adr/ADR-010-soap-client-deprecation-strategy.md` for complete details.

### Wave 4 Tasks (25 Total)

#### Phase 1: Baseline & Planning ✅ COMPLETE

- [x] W4.1 - Test Execution Baseline (11.58s, exceeds target)
- [x] W4.2 - Test Flake Rate (0.00%, exceeds target)
- [x] W4.3 - Code Coverage Assessment (51.1% baseline)
- [x] W4.4 - SOAP Usage Assessment (deprecation recommended)
- [x] W4.5 - Test Improvement Plan (16-week roadmap)

#### Phase 2: Coverage Expansion - REST Client (Weeks 3-6)

- [ ] W4.6 - REST Client WorkItemStore Tests (0% → 80%)
- [ ] W4.7 - REST Client Query Classes Tests (0% → 70%)
- [ ] W4.8 - REST Client WorkItem & Field Tests (0% → 70%)
- [ ] W4.9 - Core Auth & Credentials Tests (0% → 50%)
- **Milestone 1**: 65% overall coverage

#### Phase 3: Coverage Expansion - Remaining Gaps (Weeks 7-8)

- [ ] W4.10 - Mapper Exceptions & Edge Cases (0% → 80%)
- [ ] W4.11 - LINQ QueryExtensions Coverage (20% → 80%)
- **Milestone 2**: 70% overall coverage ✅ TARGET ACHIEVED

#### Phase 4: Mutation Testing (Weeks 9-12)

- [ ] W4.12 - Stryker.NET Setup
- [ ] W4.13 - Targeted Mutation Testing
- [ ] W4.14 - Mutation Testing Report
- **Target**: 65% mutation score on critical paths

#### Phase 5: Offline Testing (Weeks 13-16)

- [ ] W4.15 - WireMock.NET Integration
- [ ] W4.16 - Capture REST API Fixtures
- [ ] W4.17 - Migrate Integration Tests to WireMock
- **Target**: 80% of tests runnable offline

**For full task details**, see:

- `.agents/WAVE4-TASKS.md` - Detailed task list with acceptance criteria
- `.agents/WAVE4-TEST-IMPROVEMENT-PLAN.md` - Complete 16-week improvement plan

---

## Wave 5: Enterprise Production Readiness 📋 NEW

> **Created**: December 12, 2025 (Session 28 - Production v11.0.0 Release)
> **Status**: 📋 **NEW** - Enterprise production requirements for 100+ team deployment

### Purpose

Wave 5 addresses enterprise production requirements that were not considered during Session 27's maintenance mode analysis:

- **Security audit** for enterprise security review gates
- **Container deployment** guides for Kubernetes environments
- **API documentation** for 100+ team member onboarding
- **MCP extension compatibility** for AI agent integration
- **Migration guides** from upstream LeCantaloop/Qwiq v10.0.1 to v11.0.0

### Wave 5 Tasks

| ID   | Task                        | Priority     | Effort | Status | Notes                                      |
| ---- | --------------------------- | ------------ | ------ | ------ | ------------------------------------------ |
| W5.1 | Security Audit Checklist    | **CRITICAL** | M      | 📋 NEW | Enterprise security review requirements    |
| W5.2 | Container Deployment Guide  | HIGH         | M      | 📋 NEW | Kubernetes deployment patterns             |
| W5.3 | API Reference Documentation | MEDIUM       | L      | 📋 NEW | Comprehensive API docs for team onboarding |
| W5.4 | MCP Extension Compatibility | HIGH         | M      | 📋 NEW | AI agent integration validation            |
| W5.5 | Legacy Support Matrix       | MEDIUM       | S      | 📋 NEW | Document TFS version compatibility         |
| W5.6 | Migration Guide v10→v11     | HIGH         | M      | 📋 NEW | Breaking changes and upgrade path          |
| W5.7 | Structured Logging          | MEDIUM       | M      | 📋 NEW | Production-grade logging patterns          |
| W5.8 | Performance Benchmarks      | LOW          | L      | 📋 NEW | Document baseline performance metrics      |

---

## Dependencies & Prerequisites

```text
┌─────────────────────────────────────────────────────────────────────────┐
│             DEPENDENCY MAP (Updated Dec 12, 2025 - Session 28)          │
│                    🚀 PRODUCTION v11.0.0 RELEASE 🚀                     │
│                                                                         │
│  ┌──────────────────┐                                                   │
│  │ Wave 0 ✅        │                                                   │
│  │ (SDK, CPM, TFMs) │                                                   │
│  └────────┬─────────┘                                                   │
│           │                                                             │
│           ▼                                                             │
│  ┌────────────────────────────────────────────────────────────────────┐│
│  │ Wave 1 ✅ COMPLETE (25/26 - W1.18 deferred)                        ││
│  │ ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              ││
│  │ │ Dotfiles ✅  │  │ Docs ✅      │  │ Nullable ✅  │              ││
│  │ └──────────────┘  └──────────────┘  └──────────────┘              ││
│  │ ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              ││
│  │ │ Coverage ✅  │  │ Source Link ✅│  │ PedanticMode✅│              ││
│  │ └──────────────┘  └──────────────┘  └──────────────┘              ││
│  │ ┌──────────────────────────────────────────────────┐              ││
│  │ │ Analyzer Debt ✅ RESOLVED (8 design decisions)   │              ││
│  │ └──────────────────────────────────────────────────┘              ││
│  └────────────────────────────────────────────────────────────────────┘│
│           │                                                             │
│           ▼                                                             │
│  ┌────────────────────────────────────────────────────────────────────┐│
│  │ Wave 2 🔄 IN PROGRESS (Sprint 4 Critical Tasks)                    ││
│  │ ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              ││
│  │ │ CI Warning   │  │ SHA Pinning  │  │ NuGet v11.0  │              ││
│  │ │ W2.32 CRIT   │  │ W2.22 CRIT   │  │ W2.33 CRIT   │              ││
│  │ └──────────────┘  └──────────────┘  └──────────────┘              ││
│  │ ┌──────────────┐  ┌──────────────┐                                ││
│  │ │ Null Guards  │  │ null! cleanup│                                ││
│  │ │ W2.29 HIGH   │  │ W2.25 MED-HI │                                ││
│  │ └──────────────┘  └──────────────┘                                ││
│  └────────────────────────────────────────────────────────────────────┘│
│           │                                                             │
│           ▼                                                             │
│  ┌────────────────────────────────────────────────────────────────────┐│
│  │ Wave 3 📋 RE-ACTIVATED (TFM Expansion & Modernization)             ││
│  │ ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              ││
│  │ │ TFM Expand   │  │ Package Sign │  │ Observability│              ││
│  │ │ W3.1 CRIT    │  │ W3.10 CRIT   │  │ W3.8 HIGH    │              ││
│  │ └──────────────┘  └──────────────┘  └──────────────┘              ││
│  └────────────────────────────────────────────────────────────────────┘│
│           │                                                             │
│           ▼                                                             │
│  ┌────────────────────────────────────────────────────────────────────┐│
│  │ Wave 4 📋 RE-ACTIVATED (Test Quality & Coverage)                   ││
│  │ ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              ││
│  │ │ Mutation Test│  │ WireMock     │  │ 70% Coverage │              ││
│  │ │ W4.6-W4.10   │  │ W4.11-W4.17  │  │ W4.19-W4.20  │              ││
│  │ └──────────────┘  └──────────────┘  └──────────────┘              ││
│  └────────────────────────────────────────────────────────────────────┘│
│           │                                                             │
│           ▼                                                             │
│  ┌────────────────────────────────────────────────────────────────────┐│
│  │ Wave 5 📋 NEW (Enterprise Production Readiness)                    ││
│  │ ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              ││
│  │ │ Security     │  │ Container    │  │ MCP Compat   │              ││
│  │ │ W5.1 CRIT    │  │ W5.2 HIGH    │  │ W5.4 HIGH    │              ││
│  │ └──────────────┘  └──────────────┘  └──────────────┘              ││
│  └────────────────────────────────────────────────────────────────────┘│
│           │                                                             │
│           ▼                                                             │
│  ┌────────────────────────────────────────────────────────────────────┐│
│  │ 🚀 PRODUCTION v11.0.0 RELEASE                                      ││
│  │ ┌──────────────────────────────────────────────────────────────┐  ││
│  │ │ • 100+ team members using in production                      │  ││
│  │ │ • MCP extension for AI agents integration                    │  ││
│  │ │ • Enterprise security review approved                        │  ││
│  │ │ • Kubernetes container deployment ready                      │  ││
│  │ └──────────────────────────────────────────────────────────────┘  ││
│  └────────────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────────────┘
```

---

## Risk Assessment (Updated for Production v11.0.0)

### Active Risks (Production Deployment)

| Risk                                      | Probability | Impact | Mitigation                                             |
| ----------------------------------------- | ----------- | ------ | ------------------------------------------------------ |
| Build warnings regress                    | Medium      | High   | **W2.32: Add CI Warning Gate**                         |
| Supply chain attack via Actions           | Medium      | High   | **W2.22: Pin Actions by SHA**                          |
| Security review fails                     | Medium      | High   | **W5.1: Complete Security Audit Checklist**            |
| Container deployment issues               | Low         | High   | **W5.2: Create deployment guide, test in staging**     |
| Breaking changes impact consumers         | Medium      | Medium | **W5.6: Document migration path, semantic versioning** |
| Insufficient test coverage for production | Medium      | High   | **Wave 4: Achieve 70% coverage target**                |

### Mitigated Risks

| Risk                       | Previous Status | Current Status | Resolution                                    |
| -------------------------- | --------------- | -------------- | --------------------------------------------- |
| ~400 analyzer suppressions | 🔴 CRITICAL     | ✅ RESOLVED    | Only 8 exist, all documented design decisions |
| Missing release automation | 🔴 HIGH         | 🟡 IN PROGRESS | W2.11 in scope for v11.0.0                    |
| .NET 10 support missing    | 🟡 MEDIUM       | 🔄 IN PROGRESS | W3.1 TFM expansion underway                   |
| Coverage below targets     | 🟡 MEDIUM       | 🔄 IN PROGRESS | Wave 4 re-activated, targeting 70%            |

### Production Deployment Risks

| Risk                                      | Mitigation                                          |
| ----------------------------------------- | --------------------------------------------------- |
| MCP extension compatibility issues        | W5.4: Validate integration with AI agents           |
| Kubernetes configuration errors           | W5.2: Document deployment patterns, test in staging |
| 100+ team onboarding challenges           | W5.3: Comprehensive API documentation               |
| Performance regression from TFM expansion | W5.8: Baseline performance benchmarks               |

| Risk                                   | Probability  | Impact | Mitigation                            |
| -------------------------------------- | ------------ | ------ | ------------------------------------- |
| Security vulnerability in dependencies | Medium       | High   | Dependabot auto-PRs                   |
| TFS SDK becomes incompatible           | Low          | Medium | SOAP client already maintenance-only  |
| Azure DevOps API changes               | Low          | Medium | REST client stable, minimal changes   |
| Project becomes abandonware            | **Accepted** | N/A    | Explicit maintenance mode declaration |

---

## Success Metrics (Updated for Maintenance Mode)

### Wave 1 Completion ✅ ACHIEVED

| Metric              | Baseline | Final          | Target            | Status                  |
| ------------------- | -------- | -------------- | ----------------- | ----------------------- |
| Nullable warnings   | ~300+    | 0              | 0                 | ✅ Complete             |
| Code coverage       | Unknown  | 46.1%          | Configured        | ✅ Configured           |
| CA rules suppressed | "~400"   | 8              | Documented        | ✅ All design decisions |
| Documentation       | 60%      | 95%            | 95%               | ✅ Complete             |
| CI pipeline         | AppVeyor | GitHub Actions | Green + artifacts | ✅                      |
| Source Link         | ❌       | ✅             | Verified          | ✅                      |
| PackageReadme       | 0/10     | 10/10          | 10/10             | ✅                      |

### Sprint 4 Completion Criteria (Final Sprint)

| Metric                     | Current | Target     | Status       |
| -------------------------- | ------- | ---------- | ------------ |
| CI Warning Gate            | ❌      | Configured | 📋 W2.32     |
| GitHub Actions SHA Pinning | ❌      | All pinned | 📋 W2.22     |
| **NuGet 2.0.0 Published**  | ❌      | Published  | 📋 **W2.33** |
| Service null guards        | Partial | Complete   | 📋 W2.29     |

### Definition of Done: Maintenance Mode

Modernization is **complete** (entering maintenance mode) when:

1. ✅ All Wave 0 items verified
2. ✅ All Wave 1 items completed (25/26, W1.18 deferred)
3. ✅ Zero nullable warnings
4. ✅ Code coverage configured in CI (46.1%)
5. ✅ Source Link functional
6. ✅ All NuGet packages have PackageReadme
7. ✅ CODEOWNERS, SECURITY.md, CODE_OF_CONDUCT.md exist
8. ✅ README badges reflect GitHub Actions
9. ✅ Analyzer suppressions documented as design decisions (8 total)
10. ✅ Changes documented in MIGRATION_NOTES.md
11. 📋 CI Warning Gate enabled (W2.32)
12. 📋 GitHub Actions pinned by SHA (W2.22)
13. 📋 **NuGet 2.0.0 published (W2.33)** ← Maintenance mode begins

### Post-Maintenance Mode Success Criteria

| Activity                 | Frequency       | Owner             |
| ------------------------ | --------------- | ----------------- |
| Dependabot PRs merged    | Weekly (if any) | Maintainer review |
| Security patches applied | As needed       | Auto-created PRs  |
| Critical bug fixes       | Rare            | Manual triage     |
| Feature requests         | **Declined**    | Maintenance mode  |
| Breaking changes         | **Prohibited**  | Maintenance mode  |

---

## Effort Estimates (Maintenance Mode Scope)

### Sprint 4 Estimates (Final Sprint)

| Item                          | Effort | Time (Solo) | Priority     | Status |
| ----------------------------- | ------ | ----------- | ------------ | ------ |
| W2.32 CI Warning Gate         | S      | 1-2 hours   | **CRITICAL** | 📋     |
| W2.22 SHA Pinning             | S      | 2-4 hours   | **CRITICAL** | 📋     |
| **W2.33 NuGet 2.0.0 Publish** | S      | 1-2 hours   | **CRITICAL** | 📋     |
| W2.29 Service Null Guards     | M      | 4-8 hours   | MEDIUM       | 📋     |
| W2.21 Markdown Linting        | S      | 1-2 hours   | LOW          | 📋     |
| W2.25 null! Cleanup           | M      | 4-8 hours   | LOW          | 📋     |

**Total Sprint 4 Estimate**: 13-26 hours (2-4 days solo)

### Cancelled Effort (Waves 3-4)

| Wave            | Original Estimate | Tasks | Status       |
| --------------- | ----------------- | ----- | ------------ |
| Wave 3          | 6-8 weeks         | 13    | ❌ CANCELLED |
| Wave 4          | 18-20 weeks       | 25    | ❌ CANCELLED |
| **Total Saved** | **24-28 weeks**   | 38    | N/A          |

### null! Instance Analysis Correction

**Original Claim**: "Mostly in test fixtures"

**Actual Finding (Multi-Agent Analysis)**:

- **32 instances** in production code (not just test fixtures)
- Located in:
  - `Qwiq.Core.Rest/` - 8 instances
  - `Qwiq.Core/` - 12 instances
  - `Qwiq.Mapper/` - 6 instances
  - `Qwiq.Linq/` - 6 instances

**Decision**: W2.25 demoted to LOW priority - fixing `null!` has low user impact for maintenance-mode project.

**Effort Key**: S = Small (< 4 hours), M = Medium (4-16 hours), L = Large (> 16 hours)

---

## Appendix A: Project Target Frameworks

| Project               | net472 | netstandard2.0 | net8.0 |
| --------------------- | ------ | -------------- | ------ |
| Qwiq.Core             | ✅     | ✅             | ✅     |
| Qwiq.Core.Rest        | ✅     | ✅             | ✅     |
| Qwiq.Core.Soap        | ✅     | ❌             | ❌     |
| Qwiq.Linq             | ✅     | ❌             | ✅     |
| Qwiq.Mapper           | ✅     | ❌             | ✅     |
| Qwiq.Identity         | ✅     | ❌             | ✅     |
| Qwiq.Identity.Soap    | ✅     | ❌             | ❌     |
| Qwiq.Linq.Identity    | ✅     | ❌             | ✅     |
| Qwiq.Mapper.Identity  | ✅     | ❌             | ✅     |
| Qwiq.Mocks            | ✅     | ❌             | ✅     |
| Qwiq.Tests.Common     | ✅     | ❌             | ✅     |
| Qwiq.Core.Tests       | ✅     | ❌             | ✅     |
| Qwiq.Linq.Tests       | ✅     | ❌             | ✅     |
| Qwiq.Mapper.Tests     | ✅     | ❌             | ✅     |
| Qwiq.Identity.Tests   | ✅     | ❌             | ✅     |
| Qwiq.IntegrationTests | ✅     | ❌             | ❌     |
| Qwiq.Package.Tests    | ❌     | ❌             | ✅     |

---

## Appendix B: Key Configuration Files

| File                         | Purpose                        | Status                       |
| ---------------------------- | ------------------------------ | ---------------------------- |
| `global.json`                | SDK version pinning            | ✅ v8.0.404                  |
| `Directory.Build.props`      | Shared MSBuild properties      | ✅ Source Link configured    |
| `Directory.Build.targets`    | Shared build targets           | ✅                           |
| `Directory.Packages.props`   | Central Package Management     | ✅                           |
| `.editorconfig`              | Code style + analyzer severity | ✅ 8 documented suppressions |
| `version.json`               | Nerdbank.GitVersioning config  | ✅                           |
| `nuget.config`               | NuGet sources                  | ✅                           |
| `.github/workflows/main.yml` | CI/CD pipeline                 | ✅ Coverage configured       |
| `.github/dependabot.yml`     | Dependency updates             | ✅ NuGet, SDK, Actions       |
| `.github/CODEOWNERS`         | Code ownership                 | ✅ Created Dec 5             |

---

## Appendix C: Analyzer Suppression Status (Corrected)

### ⚠️ Important Clarification

The "~400 suppressed rules" figure was a **measurement artifact** from counting all `.editorconfig` entries. The actual state is much cleaner.

### Actual Active Suppressions: 8

| Rule   | Violation Count | Category      | Status                         |
| ------ | --------------- | ------------- | ------------------------------ |
| CS1591 | ~4200           | Documentation | 🟡 Deferred (low ROI)          |
| CS0618 | 1               | Obsolete API  | ✅ Intentional (API compat)    |
| CA1707 | 868             | Naming        | ✅ Intentional (BDD tests)     |
| CA1716 | 78              | Naming        | ✅ Intentional (API design)    |
| CA1822 | 36              | Performance   | 🟡 Deferred (API compat)       |
| CA1859 | 30              | Performance   | ✅ Intentional (abstraction)   |
| CA1863 | 20              | Globalization | 🟡 Deferred (.NET 8+ only)     |
| CA2263 | scoped          | Usage         | ✅ Intentional (test-specific) |

### Security Rules: All Enabled ✅

- **65 CA3xxx-CA5xxx rules** enabled
- **Zero violations** detected
- Includes: CA3001-CA3012, CA5350-CA5405

### Build Status

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### CS86xx Nullable Rules (Safety Net)

10 rules retained as safety net (no active violations):

- CS8600, CS8602, CS8603, CS8604, CS8619
- CS8620, CS8625, CS8767, CS8769

**Status**: All projects at 0 nullable warnings. Rules can be enabled when ready.

---

## Appendix D: REST/SOAP Unit Test Coverage (NEW)

See detailed task description in `modernize-wave2.md` under W2.16.

**Problem**: Current REST/SOAP tests require Azure DevOps connectivity.

**Solution**:

- Phase 1: REST Client Unit Tests with HTTP mocking (cross-platform)
- Phase 2: SOAP Client Unit Tests with TFS OM mocking (Windows-only)

**Test Categories**:

- `RestUnit` - REST client unit tests (all platforms)
- `SoapUnit` - SOAP client unit tests (Windows only)

**Mocking Strategy**:

- REST: `RichardSzalay.MockHttp` or `WireMock.Net`
- SOAP: Custom TFS Client OM mock wrappers

---

## Document Control

| Version | Date             | Author                     | Changes                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              |
| ------- | ---------------- | -------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1.0     | Dec 4, 2025      | Claudette                  | Initial comprehensive PRD                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| 2.0     | Dec 5, 2025      | Claudette (Session 7)      | Updated with Wave 1 progress (68%), resolved gaps, added Wave 2/3 tasks, corrected analyzer count (~400), added supply chain security requirements                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| 2.1     | Dec 5, 2025      | Claudette (Session 8)      | Key decision: Skip .NET 9 (STS), adopt .NET 10 (LTS). Strategy: SDK first, then TFM. Updated W3.1-W3.1a.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| 3.0     | Dec 5, 2025      | Claudette (Session 12)     | **Major Wave 2/3 restructure**: Deferred W2.8, W2.9, W2.1, W2.12 to Wave 3. Updated W2.11 (DRY), W2.13 (dual-pipeline SBOM). Elevated W2.15 to CRITICAL. Added W2.16-W2.20 (REST/SOAP tests, SLSA, Package Validation, CodeQL, Secrets). Created W3.8 (Observability), W3.9 (IConfiguration), W3.10 (Signing BLOCKED). Added Appendix D for REST/SOAP testing.                                                                                                                                                                                                                                                       |
| 4.0     | Dec 11, 2025     | Claudette (Session 25)     | **Wave 4: Test Quality & Coverage Excellence**: Deep analysis of code coverage gaps (46.1% line, 36.2% branch). Created Wave 4 with 25 tasks across 5 phases using multi-agent consensus (csharp-expert, feature-request-review, independent-thinker). Key decisions: mutation testing before coverage expansion, test stabilization before mutation runs, SOAP spike-then-deprecate strategy. Tools: Stryker.NET, WireMock.Net. Targets: 65% mutation score, <0.1% flake rate, 80% offline tests. Timeline: 18-20 weeks (Q1-Q2 2026).                                                                               |
| **5.0** | **Dec 12, 2025** | **Claudette (Session 27)** | **⚠️ STRATEGIC DECISION: MAINTENANCE MODE** - Multi-agent analysis (5 agents, unanimous) determined project viability does not justify continued investment. Key findings: (1) Last NuGet publish Feb 2018 (7 years); (2) ~22 downloads/day (CI caching); (3) Bot-only contributors since 2023; (4) Zero external engagement. Decision: Complete Wave 2, publish NuGet 2.0.0, declare maintenance mode. **Waves 3-4 CANCELLED** (38 tasks, 24-28 weeks saved). Corrected "~400 suppressions" artifact to actual 8 active suppressions. Added W2.33 (NuGet Publish). Updated all sections for maintenance mode scope. |
