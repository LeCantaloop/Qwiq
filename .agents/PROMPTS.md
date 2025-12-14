# Copilot Agent Prompts

> **Purpose**: Self-contained prompts for Copilot agent sessions. Each prompt includes
> all context needed - no prior knowledge required. Simply copy/paste one task prompt
> into a GitHub Copilot comment to start a session.
>
> **Target**: Production v11.0.0 Release for 100+ team members
> **Timeline**: 6-8 weeks
> **Branch**: `chore/modernize-4`

---

## Quick Reference: Current Sprint Priorities

| Tier       | Task ID | Description            | Status                 |
| ---------- | ------- | ---------------------- | ---------------------- |
| 1 CRITICAL | W2.22   | SHA Digest Pinning     | 📋 Planned             |
| 1 CRITICAL | W3.10   | Package Signing        | ⏸️ BLOCKED (Key Vault) |
| 1 CRITICAL | W5.1    | Security Audit         | 📋 Planned             |
| 2 HIGH     | W2.29   | Null Guards            | 📋 Planned             |
| 2 HIGH     | W5.2    | Container Docs         | 📋 Planned             |
| 3 MEDIUM   | W2.33   | NuGet v11.0.0 Publish  | 📋 Planned             |
| 3 MEDIUM   | W5.6    | Migration Guide v10→11 | 📋 Planned             |

### Recently Completed (prompts removed)

- ✅ **W2.32** - CI Warning Gate (Session 32, 2025-12-12)
- ✅ **W3.1** - TFM Expansion (Session 30, 2025-12-12)
- 🔄 **W4.1** - Code Coverage (In Progress, 2025-12-13) - See `.agents/W4-COVERAGE-PLAN.md`

## Documentation File Structure

The modernization TODO was split into multiple files for AI agent readability:

| File                                                        | Content                     |
| ----------------------------------------------------------- | --------------------------- |
| [modernize-TODO-index.md](planning/modernize-TODO-index.md) | Index, metrics, session log |
| [modernize-wave1.md](planning/modernize-wave1.md)           | Wave 0 + Wave 1 tasks       |
| [modernize-wave2.md](planning/modernize-wave2.md)           | Wave 2 tasks                |
| [modernize-wave3-5.md](planning/modernize-wave3-5.md)       | Wave 3-5 tasks              |

---

## Tier 1 CRITICAL Tasks

### W2.22 - SHA Digest Pinning

- **Priority**: CRITICAL (Tier 1)
- **Effort**: M (2-4 hours)
- **Description**: Pin all GitHub Actions to SHA digests to prevent supply chain attacks.

**Copy/paste prompt:**

```text
# QWIQ Modernization Session - Task W2.22: SHA Digest Pinning

**CRITICAL**: Only terminate your turn when you are sure the problem is solved and all TODO items are checked off. **Continue working until the task is truly and completely solved.**

## Session Setup (MANDATORY - Do First)
1. Read `.agents/AGENT-INSTRUCTIONS.md` - operational protocol
2. Read `.agents/HANDOFF.md` - previous session context
3. Read `.agents/modernize-TODO.md` - task details and current state
4. Verify git state is clean: run `git status` (stop if not clean)
5. Verify branch: run `git branch --show-current` (expected: `chore/modernize-4`; switch/create if needed)
6. Create session log in `.agents/sessions/` using today's date and next number (example: `.agents/sessions/2025-12-12-session-01.md`)
7. Baseline verification build (required before changes): `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`

## Project Context
- Repository: Qwiq - .NET library for Azure DevOps work item queries
- Branch: `chore/modernize-4`
- Target: Production v11.0.0 for 100+ team members
- Build Status: 0 errors, 0 warnings (MUST maintain)
- Test Status: 208 passed, 1 skipped

## Task: W2.22 - Pin GitHub Actions to SHA Digests
GitHub Actions using tag-based versions (v4, v5) are vulnerable to supply chain
attacks. Pin all actions to immutable SHA digests for security.

### Requirements
1. Pin ALL GitHub Actions in ALL workflows to SHA digests
2. Add version comment after each SHA for maintainability
3. Ensure Renovate/Dependabot can update pinned versions

### Files to Review/Modify
- `.github/workflows/main.yml`
- `.github/workflows/release.yml`
- `.github/workflows/dependency-review.yml`
- `.github/workflows/codeql.yml`
- `.github/workflows/dependabot-auto-approve.yml`
- `.github/workflows/dependabot-auto-merge.yml`
- Any other `.github/workflows/*.yml` files

### Pattern
# BEFORE (vulnerable)
- uses: actions/checkout@v4

# AFTER (secure)
- uses: actions/checkout@11bd71901bbe5b1630ceea73d27597364c9af683 # v4.2.2

### Acceptance Criteria
- [ ] All actions pinned to SHA in all workflow files
- [ ] Version comments added for maintainability
- [ ] Renovate config updated to handle SHA updates
- [ ] All workflows still pass

### Build Commands
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

## Session Finalization (MANDATORY - Before Ending)
1. Check off W2.22 in `.agents/modernize-TODO.md`
2. Complete session log with: what was done, decisions made, challenges, files changed
3. Update `.agents/HANDOFF.md` with: current state, what's completed, what's next
4. Lint: `npx markdownlint-cli2 --fix "**/*.md"`, `dotnet format`, `dotnet pprettier --write .`
5. Stage and commit: `git add .agents/ && git add -f .agents/sessions/*.md && git commit -m "docs: complete W2.22 SHA digest pinning"`
6. Verify build passes: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`
7. Verify tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`

CRITICAL: The next session has ZERO context except checked-in documentation.
```

---

### W5.1 - Security Audit Checklist

- **Priority**: CRITICAL (Tier 1)
- **Effort**: S-M (2-4 hours)
- **Description**: Create comprehensive security audit checklist for enterprise review.

**Copy/paste prompt:**

```text
# QWIQ Modernization Session - Task W5.1: Security Audit Checklist

**CRITICAL**: Only terminate your turn when you are sure the problem is solved and all TODO items are checked off. **Continue working until the task is truly and completely solved.**

## Session Setup (MANDATORY - Do First)
1. Read `.agents/AGENT-INSTRUCTIONS.md` - operational protocol
2. Read `.agents/HANDOFF.md` - previous session context
3. Read `.agents/modernize-TODO.md` - task details and current state
4. Verify git state is clean: run `git status` (stop if not clean)
5. Verify branch: run `git branch --show-current` (expected: `chore/modernize-4`; switch/create if needed)
6. Create session log in `.agents/sessions/` using today's date and next number (example: `.agents/sessions/2025-12-12-session-01.md`)
7. Baseline verification build (required before changes): `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`

## Project Context
- Repository: Qwiq - .NET library for Azure DevOps work item queries
- Branch: `chore/modernize-4`
- Target: Production v11.0.0 for 100+ team members
- Build Status: 0 errors, 0 warnings (MUST maintain)
- Test Status: 208 passed, 1 skipped

## Task: W5.1 - Create Security Audit Checklist
Production deployment for 100+ team members requires passing enterprise security
review. Create comprehensive checklist documenting all security controls.

### Dependencies
- W2.22 (SHA Pinning)
- W3.10 (Package Signing - blocked)

### Requirements
Create documentation covering:
1. Supply chain security (SHA pinning, SLSA provenance, SBOM)
2. Code analysis (CodeQL, DevSkim, Gitleaks)
3. Dependency management (vulnerability scanning, license compliance)
4. Secret management procedures
5. Package signing status and plans

### Files to Create
- `docs/security/audit-checklist.md` - Comprehensive security checklist
- Update `SECURITY.md` with compliance summary

### Current Security Controls (Already Implemented)
- ✅ CodeQL integrated (security-extended queries)
- ✅ DevSkim scanning active
- ✅ Gitleaks secret scanning
- ✅ Dependency review with license blocking
- ✅ SLSA Provenance Level 3 in release workflow
- ✅ SBOM generation (dual-pipeline)
- ⏸️ Package signing (BLOCKED - needs Azure Key Vault)

### Acceptance Criteria
- [ ] Security checklist covers all enterprise requirements
- [ ] Supply chain security documented
- [ ] Dependency vulnerability process documented
- [ ] Secret management procedures documented
- [ ] Checklist is self-contained (readable without other docs)

### Build Commands
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

## Session Finalization (MANDATORY - Before Ending)
1. Check off W5.1 in `.agents/modernize-TODO.md`
2. Complete session log with: what was done, decisions made, challenges, files changed
3. Update `.agents/HANDOFF.md` with: current state, what's completed, what's next
4. Lint: `npx markdownlint-cli2 --fix "**/*.md"`, `dotnet format`, `dotnet pprettier --write .`
5. Stage and commit: `git add .agents/ && git add -f .agents/sessions/*.md && git commit -m "docs: complete W5.1 security audit checklist"`
6. Verify build passes: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`
7. Verify tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`

CRITICAL: The next session has ZERO context except checked-in documentation.
```

---

## Tier 2 HIGH Tasks

### W2.29 - Service Resolution Null Guards

- **Priority**: HIGH (Tier 2)
- **Effort**: M (2-4 hours)
- **Description**: Add defensive null checks for service resolution to prevent NullReferenceException.

**Copy/paste prompt:**

```text
# QWIQ Modernization Session - Task W2.29: Service Resolution Null Guards

**CRITICAL**: Only terminate your turn when you are sure the problem is solved and all TODO items are checked off. **Continue working until the task is truly and completely solved.**

## Session Setup (MANDATORY - Do First)
1. Read `.agents/AGENT-INSTRUCTIONS.md` - operational protocol
2. Read `.agents/HANDOFF.md` - previous session context
3. Read `.agents/modernize-TODO.md` - task details and current state
4. Verify git state is clean: run `git status` (stop if not clean)
5. Verify branch: run `git branch --show-current` (expected: `chore/modernize-4`; switch/create if needed)
6. Create session log in `.agents/sessions/` using today's date and next number (example: `.agents/sessions/2025-12-12-session-01.md`)
7. Baseline verification build (required before changes): `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`

## Project Context
- Repository: Qwiq - .NET library for Azure DevOps work item queries
- Branch: `chore/modernize-4`
- Target: Production v11.0.0 for 100+ team members
- Build Status: 0 errors, 0 warnings (MUST maintain)
- Test Status: 208 passed, 1 skipped

## Task: W2.29 - Add Null Guards for Service Resolution
Service resolution can return null without guards, causing NullReferenceException
at runtime. Add defensive null checks for production safety.

### Requirements
1. Identify all service resolution points that can return null
2. Add null guards with descriptive ArgumentNullException
3. Follow existing null guard patterns in codebase

### Pattern to Follow
// Correct pattern (from csharp.instructions.md)
public void Method(SomeType parameter)
{
    if (parameter == null) throw new ArgumentNullException(nameof(parameter));
    // ... rest of method
}

// For constructor base calls
public MyClass(IService service)
    : base(service?.Property ?? throw new ArgumentNullException(nameof(service)))
{
}

### Files to Review
- `src/Qwiq.Core/` - Core service resolution
- `src/Qwiq.Core.Rest/` - REST client factory
- `src/Qwiq.Mapper/` - Mapper service resolution

### Known Locations (from codebase analysis)
- Service resolution in factories
- Dependency injection entry points
- Interface implementations that accept nullable parameters

### Acceptance Criteria
- [ ] All service resolution points have null guards
- [ ] Build passes with 0 warnings
- [ ] Tests pass
- [ ] No breaking API changes (guards throw, not return null)

### Build Commands
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

## Session Finalization (MANDATORY - Before Ending)
1. Check off W2.29 in `.agents/modernize-TODO.md`
2. Complete session log with: what was done, decisions made, challenges, files changed
3. Update `.agents/HANDOFF.md` with: current state, what's completed, what's next
4. Lint: `npx markdownlint-cli2 --fix "**/*.md"`, `dotnet format`, `dotnet pprettier --write .`
5. Stage and commit: `git add .agents/ && git add -f .agents/sessions/*.md && git commit -m "fix: complete W2.29 service resolution null guards"`
6. Verify build passes: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`
7. Verify tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`

CRITICAL: The next session has ZERO context except checked-in documentation.
```

---

### W5.2 - Container Deployment Guide

- **Priority**: HIGH (Tier 2)
- **Effort**: M (4-6 hours)
- **Description**: Create documentation for containerizing applications using Qwiq.

**Copy/paste prompt:**

```text
# QWIQ Modernization Session - Task W5.2: Container Deployment Guide

**CRITICAL**: Only terminate your turn when you are sure the problem is solved and all TODO items are checked off. **Continue working until the task is truly and completely solved.**

## Session Setup (MANDATORY - Do First)
1. Read `.agents/AGENT-INSTRUCTIONS.md` - operational protocol
2. Read `.agents/HANDOFF.md` - previous session context
3. Read `.agents/modernize-TODO.md` - task details and current state
4. Verify git state is clean: run `git status` (stop if not clean)
5. Verify branch: run `git branch --show-current` (expected: `chore/modernize-4`; switch/create if needed)
6. Create session log in `.agents/sessions/` using today's date and next number (example: `.agents/sessions/2025-12-12-session-01.md`)
7. Baseline verification build (required before changes): `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`

## Project Context
- Repository: Qwiq - .NET library for Azure DevOps work item queries
- Branch: `chore/modernize-4`
- Target: Production v11.0.0 for 100+ team members
- Build Status: 0 errors, 0 warnings (MUST maintain)
- Test Status: 208 passed, 1 skipped

## Task: W5.2 - Create Container Deployment Documentation
Production deployment targets Kubernetes. Create comprehensive documentation
for containerizing applications using Qwiq.

### Dependencies
- W3.1 (TFM Expansion to net8.0/net9.0)

### Requirements
Create documentation for:
1. Docker image build instructions
2. Kubernetes deployment YAML examples
3. Resource requirements and recommendations
4. Environment variable configuration
5. Health check patterns (if applicable)

### Files to Create
- `docs/deployment/kubernetes.md` - Kubernetes deployment guide
- `docs/deployment/docker.md` - Docker containerization guide
- `samples/Dockerfile` - Example Dockerfile

### Key Considerations
- REST client is container-ready (no Windows dependencies)
- SOAP client is Windows-only (net472, cannot containerize)
- Document which features work in containers

### Acceptance Criteria
- [ ] Docker build instructions documented
- [ ] Kubernetes deployment YAML examples provided
- [ ] Resource requirements documented
- [ ] Environment configuration guide complete
- [ ] Clear indication of SOAP limitations in containers

### Build Commands
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

## Session Finalization (MANDATORY - Before Ending)
1. Check off W5.2 in `.agents/modernize-TODO.md`
2. Complete session log with: what was done, decisions made, challenges, files changed
3. Update `.agents/HANDOFF.md` with: current state, what's completed, what's next
4. Lint: `npx markdownlint-cli2 --fix "**/*.md"`, `dotnet format`, `dotnet pprettier --write .`
5. Stage and commit: `git add .agents/ && git add -f .agents/sessions/*.md && git commit -m "docs: complete W5.2 container deployment guide"`
6. Verify build passes: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`
7. Verify tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`

CRITICAL: The next session has ZERO context except checked-in documentation.
```

---

### W3.8 - Observability Overhaul

- **Priority**: HIGH (Tier 2)
- **Effort**: L (8-16 hours)
- **Description**: Add ILogger and OpenTelemetry support for distributed tracing.

**Copy/paste prompt:**

```text
# QWIQ Modernization Session - Task W3.8: Observability Overhaul

**CRITICAL**: Only terminate your turn when you are sure the problem is solved and all TODO items are checked off. **Continue working until the task is truly and completely solved.**

## Session Setup (MANDATORY - Do First)
1. Read `.agents/AGENT-INSTRUCTIONS.md` - operational protocol
2. Read `.agents/HANDOFF.md` - previous session context
3. Read `.agents/modernize-TODO.md` - task details and current state
4. Verify git state is clean: run `git status` (stop if not clean)
5. Verify branch: run `git branch --show-current` (expected: `chore/modernize-4`; switch/create if needed)
6. Create session log in `.agents/sessions/` using today's date and next number (example: `.agents/sessions/2025-12-12-session-01.md`)
7. Baseline verification build (required before changes): `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`

## Project Context
- Repository: Qwiq - .NET library for Azure DevOps work item queries
- Branch: `chore/modernize-4`
- Target: Production v11.0.0 for 100+ team members
- Build Status: 0 errors, 0 warnings (MUST maintain)
- Test Status: 208 passed, 1 skipped

## Task: W3.8 - Add ILogger and OpenTelemetry Support
Modern .NET applications expect ILogger integration and OpenTelemetry for
distributed tracing. Add observability hooks without breaking existing code.

### Dependencies
- W3.1 (TFM Expansion)

### Requirements
1. Add ILogger integration for all major operations
2. Add OpenTelemetry activity tracing for queries
3. Structured logging with correlation IDs
4. Backward compatible (existing code without logging still works)

### Files to Create/Modify
- `src/Qwiq.Core/Logging/` - Logging abstractions
- `src/Qwiq.Core/Extensions/ServiceCollectionExtensions.cs` - DI extensions
- Update query execution paths with activity spans

### Design Considerations
- Use Microsoft.Extensions.Logging.Abstractions for ILogger
- Use System.Diagnostics.ActivitySource for OpenTelemetry
- Make logging optional (null logger by default)
- Follow existing patterns in ASP.NET Core libraries

### Acceptance Criteria
- [ ] ILogger integration for major operations
- [ ] OpenTelemetry ActivitySource for query tracing
- [ ] Structured log events with query context
- [ ] No breaking changes to existing API
- [ ] Tests cover logging paths

### Build Commands
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

## Session Finalization (MANDATORY - Before Ending)
1. Check off W3.8 in `.agents/modernize-TODO.md`
2. Complete session log with: what was done, decisions made, challenges, files changed
3. Update `.agents/HANDOFF.md` with: current state, what's completed, what's next
4. Lint: `npx markdownlint-cli2 --fix "**/*.md"`, `dotnet format`, `dotnet pprettier --write .`
5. Stage and commit: `git add .agents/ && git add -f .agents/sessions/*.md && git commit -m "feat: complete W3.8 observability overhaul"`
6. Verify build passes: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`
7. Verify tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`

CRITICAL: The next session has ZERO context except checked-in documentation.
```

---

### W4.1 - Achieve 70% Code Coverage (IN PROGRESS)

- **Priority**: HIGH (Tier 2)
- **Effort**: XL (16-32 hours)
- **Description**: Increase test coverage from 46.1% to 70% for enterprise requirements.
- **Current Progress**: See `.agents/W4-COVERAGE-PLAN.md` and `.agents/sessions/2025-12-13-session-w4-coverage.md`
- **Coverage**: Qwiq.Core 45.2%, 285+ tests passing

**Copy/paste prompt:**

```text
# QWIQ Modernization Session - Task W4.1: Achieve 70% Code Coverage

**CRITICAL**: Only terminate your turn when you are sure the problem is solved and all TODO items are checked off. **Continue working until the task is truly and completely solved.**

## Session Setup (MANDATORY - Do First)
1. Read `.agents/AGENT-INSTRUCTIONS.md` - operational protocol
2. Read `.agents/HANDOFF.md` - previous session context
3. Read `.agents/modernize-TODO.md` - task details and current state
4. Verify git state is clean: run `git status` (stop if not clean)
5. Verify branch: run `git branch --show-current` (expected: `chore/modernize-4`; switch/create if needed)
6. Create session log in `.agents/sessions/` using today's date and next number (example: `.agents/sessions/2025-12-12-session-01.md`)
7. Baseline verification build (required before changes): `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`

## Project Context
- Repository: Qwiq - .NET library for Azure DevOps work item queries
- Branch: `chore/modernize-4`
- Target: Production v11.0.0 for 100+ team members
- Build Status: 0 errors, 0 warnings (MUST maintain)
- Test Status: 208 passed, 1 skipped

## Task: W4.1 - Increase Test Coverage to 70%
Production deployment for 100+ team members requires high test confidence.
Current coverage is 46.1%, target is 70% for enterprise requirements.

### Current State
- Coverage: 46.1%
- Tests: 208 passed, 1 skipped
- Test framework: MSTest with Shouldly assertions

### Priority Areas for Coverage
1. **LINQ Provider** (highest complexity):
   - WiqlTranslator.cs (397 LOC, 10+ expression handlers)
   - QueryRewriter.cs (160 LOC, ExpressionVisitor)
   - PartialEvaluator.cs (inner classes)
2. **REST Client**: HTTP path coverage
3. **Mapper**: Field mapping edge cases

### Test Patterns
Follow existing patterns in `test/Qwiq.Tests.Common/`:
- Use `ContextSpecification` base class (Given/When/Then)
- Use `MockWorkItem`, `MockRevision` from Qwiq.Mocks
- Use Shouldly for assertions

### Coverage Commands
# Run with coverage
dotnet test Qwiq.sln --collect:"XPlat Code Coverage" --settings coverage.runsettings

# Generate HTML report (requires reportgenerator tool)
reportgenerator -reports:artifacts/TestResults/**/coverage.cobertura.xml -targetdir:artifacts/coverage

### Acceptance Criteria
- [ ] Overall coverage ≥ 70%
- [ ] LINQ provider coverage ≥ 80% (complexity requires more)
- [ ] All expression visitor paths tested
- [ ] No decrease in existing coverage

### Build Commands
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

## Session Finalization (MANDATORY - Before Ending)
1. Check off W4.1 in `.agents/modernize-TODO.md`
2. Complete session log with: what was done, decisions made, challenges, files changed
3. Update `.agents/HANDOFF.md` with: current state, what's completed, what's next
4. Lint: `npx markdownlint-cli2 --fix "**/*.md"`, `dotnet format`, `dotnet pprettier --write .`
5. Stage and commit: `git add .agents/ && git add -f .agents/sessions/*.md && git commit -m "test: complete W4.1 code coverage improvements"`
6. Update coverage badge in README
7. Verify build passes: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`
8. Verify tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`

CRITICAL: The next session has ZERO context except checked-in documentation.
```

---

## Tier 3 MEDIUM Tasks

### W2.33 - NuGet v11.0.0 Publish

- **Priority**: MEDIUM (Tier 3) - after security tasks complete
- **Effort**: M (4-6 hours)
- **Description**: First NuGet release in 7 years! Publish v11.0.0 packages.

**Copy/paste prompt:**

```text
# QWIQ Modernization Session - Task W2.33: NuGet v11.0.0 Publish

**CRITICAL**: Only terminate your turn when you are sure the problem is solved and all TODO items are checked off. **Continue working until the task is truly and completely solved.**

## Session Setup (MANDATORY - Do First)
1. Read `.agents/AGENT-INSTRUCTIONS.md` - operational protocol
2. Read `.agents/HANDOFF.md` - previous session context
3. Read `.agents/modernize-TODO.md` - task details and current state
4. Verify git state is clean: run `git status` (stop if not clean)
5. Verify branch: run `git branch --show-current` (expected: `chore/modernize-4`; switch/create if needed)
6. Create session log in `.agents/sessions/` using today's date and next number (example: `.agents/sessions/2025-12-12-session-01.md`)
7. Baseline verification build (required before changes): `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`

## Project Context
- Repository: Qwiq - .NET library for Azure DevOps work item queries
- Branch: `chore/modernize-4`
- Target: Production v11.0.0 for 100+ team members
- Build Status: 0 errors, 0 warnings (MUST maintain)
- Test Status: 208 passed, 1 skipped

## Task: W2.33 - Publish NuGet v11.0.0
First NuGet release in 7 years! This is a fork of LeCantaloop/Qwiq v10.0.1.
Version 11.0.0 indicates breaking changes from the fork.

### Dependencies
- W2.32 (CI Gate)
- W2.22 (SHA Pinning)
- W3.1 (TFM)

### Pre-Release Checklist
Before publishing, verify:
- [ ] W2.32 CI Warning Gate enabled
- [ ] W2.22 All actions SHA-pinned
- [ ] W3.1 TFM expansion complete
- [ ] All tests pass
- [ ] SLSA provenance configured in release workflow
- [ ] SBOM generation working

### Release Process
1. Create release tag following semantic versioning
2. Trigger release workflow
3. Verify NuGet packages have:
   - Correct version (11.0.0)
   - All TFMs included
   - SLSA attestation
   - SBOM attached

### Files to Verify
- `.github/workflows/release.yml` - Release workflow
- `version.json` - Nerdbank.GitVersioning config
- `Directory.Build.props` - Package metadata

### Acceptance Criteria
- [ ] v11.0.0 packages published to NuGet.org
- [ ] All packages have SLSA provenance
- [ ] SBOM attached to GitHub release
- [ ] Release notes document breaking changes from v10

### Build Commands
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

## Session Finalization (MANDATORY - Before Ending)
1. Check off W2.33 in `.agents/modernize-TODO.md`
2. Complete session log with: what was done, decisions made, challenges, files changed
3. Update `.agents/HANDOFF.md` with: current state, what's completed, what's next
4. Update README.md with new version badge
5. Stage and commit: `git add .agents/ && git add -f .agents/sessions/*.md && git commit -m "chore: complete W2.33 NuGet v11.0.0 publish"`
6. Verify build passes: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`
7. Verify tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`

CRITICAL: The next session has ZERO context except checked-in documentation.
```

---

### W5.6 - Migration Guide v10→v11

- **Priority**: MEDIUM (Tier 3)
- **Effort**: M (4-6 hours)
- **Description**: Create migration guide for users upgrading from v10.x to v11.0.0.

**Copy/paste prompt:**

```text
# QWIQ Modernization Session - Task W5.6: Migration Guide v10→v11

**CRITICAL**: Only terminate your turn when you are sure the problem is solved and all TODO items are checked off. **Continue working until the task is truly and completely solved.**

## Session Setup (MANDATORY - Do First)
1. Read `.agents/AGENT-INSTRUCTIONS.md` - operational protocol
2. Read `.agents/HANDOFF.md` - previous session context
3. Read `.agents/modernize-TODO.md` - task details and current state
4. Verify git state is clean: run `git status` (stop if not clean)
5. Verify branch: run `git branch --show-current` (expected: `chore/modernize-4`; switch/create if needed)
6. Create session log in `.agents/sessions/` using today's date and next number (example: `.agents/sessions/2025-12-12-session-01.md`)
7. Baseline verification build (required before changes): `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`

## Project Context
- Repository: Qwiq - .NET library for Azure DevOps work item queries
- Branch: `chore/modernize-4`
- Target: Production v11.0.0 for 100+ team members
- Build Status: 0 errors, 0 warnings (MUST maintain)
- Test Status: 208 passed, 1 skipped

## Task: W5.6 - Create Migration Guide from v10 to v11
Users upgrading from LeCantaloop/Qwiq v10.x need clear migration path to v11.0.0.
Document all breaking changes and provide step-by-step guide.

### Dependencies
- W2.33 (v11.0.0 Publish)

### Requirements
Document:
1. All breaking changes from v10 to v11
2. Namespace changes (if any)
3. API changes with before/after examples
4. TFM changes and implications
5. Step-by-step migration checklist

### Files to Create
- `docs/migration/v10-to-v11.md` - Detailed migration guide
- `MIGRATION.md` - Root-level quick reference

### Breaking Changes to Document
- TFM changes (added net8.0+, potentially removed netstandard2.0)
- Any API signature changes
- Nullable reference type annotations
- Package dependencies changes

### Acceptance Criteria
- [ ] All breaking changes documented with examples
- [ ] Before/after code samples provided
- [ ] Step-by-step migration checklist
- [ ] Document is self-contained (no prior knowledge needed)

### Build Commands
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

## Session Finalization (MANDATORY - Before Ending)
1. Check off W5.6 in `.agents/modernize-TODO.md`
2. Complete session log with: what was done, decisions made, challenges, files changed
3. Update `.agents/HANDOFF.md` with: current state, what's completed, what's next
4. Lint: `npx markdownlint-cli2 --fix "**/*.md"`, `dotnet format`, `dotnet pprettier --write .`
5. Stage and commit: `git add .agents/ && git add -f .agents/sessions/*.md && git commit -m "docs: complete W5.6 migration guide v10 to v11"`
6. Verify build passes: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`
7. Verify tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`

CRITICAL: The next session has ZERO context except checked-in documentation.
```

---

### W3.9 - IConfiguration Support

- **Priority**: MEDIUM (Tier 3)
- **Effort**: M (4-8 hours)
- **Description**: Add configuration binding support for connection options.

**Copy/paste prompt:**

```text
# QWIQ Modernization Session - Task W3.9: IConfiguration Support

**CRITICAL**: Only terminate your turn when you are sure the problem is solved and all TODO items are checked off. **Continue working until the task is truly and completely solved.**

## Session Setup (MANDATORY - Do First)
1. Read `.agents/AGENT-INSTRUCTIONS.md` - operational protocol
2. Read `.agents/HANDOFF.md` - previous session context
3. Read `.agents/modernize-TODO.md` - task details and current state
4. Verify git state is clean: run `git status` (stop if not clean)
5. Verify branch: run `git branch --show-current` (expected: `chore/modernize-4`; switch/create if needed)
6. Create session log in `.agents/sessions/` using today's date and next number (example: `.agents/sessions/2025-12-12-session-01.md`)
7. Baseline verification build (required before changes): `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`

## Project Context
- Repository: Qwiq - .NET library for Azure DevOps work item queries
- Branch: `chore/modernize-4`
- Target: Production v11.0.0 for 100+ team members
- Build Status: 0 errors, 0 warnings (MUST maintain)
- Test Status: 208 passed, 1 skipped

## Task: W3.9 - Add IConfiguration Support
Modern .NET applications use IConfiguration for settings. Add configuration
binding support for connection options.

### Dependencies
- W3.8 (Observability)

### Requirements
1. Add configuration binding for AuthenticationOptions
2. Support appsettings.json configuration pattern
3. Backward compatible (manual construction still works)

### Example Configuration Pattern
{
  "Qwiq": {
    "Uri": "https://dev.azure.com/myorg",
    "AuthenticationType": "PersonalAccessToken",
    "Project": "MyProject"
  }
}

### Files to Create/Modify
- `src/Qwiq.Core/Configuration/` - Configuration models
- `src/Qwiq.Core/Extensions/` - Configuration extension methods

### Acceptance Criteria
- [ ] AuthenticationOptions bindable from IConfiguration
- [ ] Example configuration in documentation
- [ ] Backward compatible with manual construction
- [ ] Tests cover configuration binding

### Build Commands
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

## Session Finalization (MANDATORY - Before Ending)
1. Check off W3.9 in `.agents/modernize-TODO.md`
2. Complete session log with: what was done, decisions made, challenges, files changed
3. Update `.agents/HANDOFF.md` with: current state, what's completed, what's next
4. Lint: `npx markdownlint-cli2 --fix "**/*.md"`, `dotnet format`, `dotnet pprettier --write .`
5. Stage and commit: `git add .agents/ && git add -f .agents/sessions/*.md && git commit -m "feat: complete W3.9 IConfiguration support"`
6. Verify build passes: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`
7. Verify tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`

CRITICAL: The next session has ZERO context except checked-in documentation.
```

---

### W2.3 - Contract Tests

- **Priority**: LOW (Tier 3)
- **Effort**: M (4-8 hours)
- **Description**: Create tests verifying REST/SOAP client parity.

**Copy/paste prompt:**

```text
# QWIQ Modernization Session - Task W2.3: Contract Tests

**CRITICAL**: Only terminate your turn when you are sure the problem is solved and all TODO items are checked off. **Continue working until the task is truly and completely solved.**

## Session Setup (MANDATORY - Do First)
1. Read `.agents/AGENT-INSTRUCTIONS.md` - operational protocol
2. Read `.agents/HANDOFF.md` - previous session context
3. Read `.agents/modernize-TODO.md` - task details and current state
4. Verify git state is clean: run `git status` (stop if not clean)
5. Verify branch: run `git branch --show-current` (expected: `chore/modernize-4`; switch/create if needed)
6. Create session log in `.agents/sessions/` using today's date and next number (example: `.agents/sessions/2025-12-12-session-01.md`)
7. Baseline verification build (required before changes): `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`

## Project Context
- Repository: Qwiq - .NET library for Azure DevOps work item queries
- Branch: `chore/modernize-4`
- Target: Production v11.0.0 for 100+ team members
- Build Status: 0 errors, 0 warnings (MUST maintain)
- Test Status: 208 passed, 1 skipped

## Task: W2.3 - Create Contract Tests for REST/SOAP Parity
REST and SOAP clients should return equivalent data for the same queries.
Contract tests verify this parity.

### Requirements
1. Create tests that execute same query on both REST and SOAP
2. Compare results for equivalence (not necessarily identical)
3. Document known differences

### Known Differences (from codebase analysis)
- REST returns System.AreaLevel1-7 and System.IterationLevel1-7 fields
- SOAP does not return these fields

### Files to Create
- `test/Qwiq.Integration.Tests/ContractTests/` - Parity tests

### Acceptance Criteria
- [ ] Contract tests verify field parity
- [ ] Known differences documented
- [ ] Tests can run against sandbox environment

### Build Commands
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

## Session Finalization (MANDATORY - Before Ending)
1. Check off W2.3 in `.agents/modernize-TODO.md`
2. Complete session log with: what was done, decisions made, challenges, files changed
3. Update `.agents/HANDOFF.md` with: current state, what's completed, what's next
4. Lint: `npx markdownlint-cli2 --fix "**/*.md"`, `dotnet format`, `dotnet pprettier --write .`
5. Stage and commit: `git add .agents/ && git add -f .agents/sessions/*.md && git commit -m "test: complete W2.3 contract tests"`
6. Verify build passes: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`
7. Verify tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`

CRITICAL: The next session has ZERO context except checked-in documentation.
```

---

### W2.7 - Update CONTRIBUTING.md

- **Priority**: MEDIUM (Tier 3)
- **Effort**: S (1-2 hours)
- **Description**: Update contribution guidelines for modern SDK-style development.

**Copy/paste prompt:**

```text
# QWIQ Modernization Session - Task W2.7: Update CONTRIBUTING.md

**CRITICAL**: Only terminate your turn when you are sure the problem is solved and all TODO items are checked off. **Continue working until the task is truly and completely solved.**

## Session Setup (MANDATORY - Do First)
1. Read `.agents/AGENT-INSTRUCTIONS.md` - operational protocol
2. Read `.agents/HANDOFF.md` - previous session context
3. Read `.agents/modernize-TODO.md` - task details and current state
4. Verify git state is clean: run `git status` (stop if not clean)
5. Verify branch: run `git branch --show-current` (expected: `chore/modernize-4`; switch/create if needed)
6. Create session log in `.agents/sessions/` using today's date and next number (example: `.agents/sessions/2025-12-12-session-01.md`)
7. Baseline verification build (required before changes): `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`

## Project Context
- Repository: Qwiq - .NET library for Azure DevOps work item queries
- Branch: `chore/modernize-4`
- Target: Production v11.0.0 for 100+ team members
- Build Status: 0 errors, 0 warnings (MUST maintain)
- Test Status: 208 passed, 1 skipped

## Task: W2.7 - Update CONTRIBUTING.md
Contribution guidelines need updating to reflect modern development workflow
and SDK-style project structure.

### Requirements
Update documentation to include:
1. Modern build commands (dotnet CLI)
2. Test execution with category filters
3. Conventional commit format
4. Code style guidelines (nullable types, etc.)

### Files to Modify
- `CONTRIBUTING.md` - Main contribution guide

### Acceptance Criteria
- [ ] Build instructions current
- [ ] Test instructions with filters
- [ ] Commit format documented
- [ ] Code style requirements documented

### Build Commands
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

## Session Finalization (MANDATORY - Before Ending)
1. Check off W2.7 in `.agents/modernize-TODO.md`
2. Complete session log with: what was done, decisions made, challenges, files changed
3. Update `.agents/HANDOFF.md` with: current state, what's completed, what's next
4. Lint: `npx markdownlint-cli2 --fix "**/*.md"`, `dotnet format`, `dotnet pprettier --write .`
5. Stage and commit: `git add .agents/ && git add -f .agents/sessions/*.md && git commit -m "docs: complete W2.7 update CONTRIBUTING.md"`
6. Verify build passes: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`
7. Verify tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`

CRITICAL: The next session has ZERO context except checked-in documentation.
```

---

## Troubleshooting Prompts

### Context Recovery

- **When to use**: Agent has lost track of what was being done.

**Copy/paste prompt:**

```text
# Context Recovery Protocol

You have lost context. Follow these steps to recover:

## Step 1: Read Core Documentation (in this order)
1. `.agents/AGENT-INSTRUCTIONS.md` - Process instructions
2. `.agents/HANDOFF.md` - Previous session context
3. `.agents/planning/modernize-TODO-index.md` - Overview and navigation
4. Then read the appropriate wave file based on your task:
   - `.agents/planning/modernize-wave1.md` - Wave 0-1 tasks
   - `.agents/planning/modernize-wave2.md` - Wave 2 tasks
   - `.agents/planning/modernize-wave3-5.md` - Wave 3-5 tasks

## Step 2: Review Recent Session Logs
Check `.agents/sessions/` for recent logs (most recent first).

## Step 3: Check Git State
git status
git log --oneline -5

## Step 4: Resume Work
Continue with current sprint priorities from planning/modernize-TODO-index.md.

## Build Commands for Verification
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
```

---

### Documentation Reminder

- **When to use**: Agent is ending session without updating docs.

**Copy/paste prompt:**

```text
# STOP - Documentation Updates Are Mandatory

Complete these steps immediately before ending the session:

## 1. Update Task Tracking
- [ ] Check off ALL completed tasks in the appropriate wave file:
  - `.agents/planning/modernize-wave1.md` - Wave 0-1 tasks
  - `.agents/planning/modernize-wave2.md` - Wave 2 tasks
  - `.agents/planning/modernize-wave3-5.md` - Wave 3-5 tasks
- [ ] Update metrics in `.agents/planning/modernize-TODO-index.md`

## 2. Complete Session Log
Update your session log in `.agents/sessions/` (example: `.agents/sessions/2025-12-12-session-01.md`) with:
- What was done
- Decisions made and rationale
- Challenges and resolutions
- Files changed and commits made

## 3. Update Handoff Document
Update `.agents/HANDOFF.md` with:
- Current state (build/test status)
- What was completed this session
- What's next for following session
- Any blockers or concerns

## 4. Commit Documentation
git add .agents/
git add -f .agents/sessions/*.md
git commit -m "docs: update session documentation"

## Critical Reminder
The next session has ZERO context except checked-in documentation.
Make documentation complete enough for any agent to continue.
```

---

### Build/Test Failure Recovery

- **When to use**: Build or tests are failing unexpectedly.

**Copy/paste prompt:**

```text
# Build/Test Failure Recovery Protocol

## Step 1: Identify the Error
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false 2>&1 | Select-String "error"

## Step 2: Check Recent Commits
git log --oneline -5

## Step 3: If Needed, Revert Last Change
git revert HEAD --no-edit

## Step 4: Diagnose with Warnings Allowed
dotnet build Qwiq.sln -c Release /p:PedanticMode=false /m:1 /nodeReuse:false

## Step 5: Document Issue
Add findings to session log at `.agents/sessions/<YYYY-MM-DD>-session-<NN>.md`

## CRITICAL
Build must pass with 0 warnings before any commit.
Tests must pass before marking any task complete.
```

---

### Package Test Failure Recovery

- **When to use**: Package tests (Verify baseline) are failing.

**Copy/paste prompt:**

```text
# Package Test Failure Recovery (Verify Baseline Mismatch)

When NuGet package contents change, baselines need updating.

## Step 1: Run Package Tests to See Diff
dotnet test test/Qwiq.Package.Tests/Qwiq.Package.Tests.csproj -c Release

## Step 2: Review Changes
Compare .received.* files vs .verified.* files in test/Qwiq.Package.Tests/

## Step 3: If Changes Are Expected, Accept New Baselines
dotnet verify accept -w test/Qwiq.Package.Tests

## Step 4: Commit Updated Baselines
git add test/Qwiq.Package.Tests/*.verified.*
git commit -m "test: update package test baselines"

## Note
Only accept baselines after confirming the package content changes are intentional.
```

---

## BLOCKED Tasks Reference

### W3.10 - Package Signing (⏸️ BLOCKED)

- **Status**: ⏸️ BLOCKED - requires Azure Key Vault setup by maintainer
- **DO NOT attempt until maintainer confirms Key Vault is ready**

**Reference information (not a session prompt):**

```text
# Task: W3.10 - NuGet Package Signing

## Status: ⏸️ BLOCKED

## Blocker
Requires Azure Key Vault setup by repository maintainer.

## Prerequisites (for maintainer)
1. Create Azure Key Vault
2. Create code signing certificate
3. Store certificate in Key Vault
4. Create GitHub secret with Key Vault access
5. Update release workflow with signing step

## When Unblocked
Once prerequisites are met, the task involves:
1. Add NuGet signing step to release workflow
2. Configure certificate from Azure Key Vault
3. Verify signed packages

DO NOT attempt this task until maintainer confirms Key Vault is ready.
```

---

## Universal Session Start Prompt

````markdown
# QWIQ Modernization Session

Read the contents of the `.agents` directory before starting work:

1. **FIRST**: Read `.agents/AGENT-INSTRUCTIONS.md` - operational protocol
2. **SECOND**: Read `.agents/HANDOFF.md` - previous session context
3. **THIRD**: Read `.agents/planning/modernize-TODO-index.md` - task details and current state

## Project Context

- **Repository**: Qwiq - .NET library for Azure DevOps work item queries
- **Branch**: `develop`
- **Target**: Production v11.0.0 for 100+ team members
- **Timeline**: 6-8 weeks
- **Build Status**: 0 errors, 0 warnings (MUST maintain)
- **Test Status**: 208 passed, 1 skipped
- **Code Coverage**: 46.1% (target: 70% for enterprise)

## Session Protocol

1. Create session log: `.agents/sessions/YYYY-MM-DD-session-NN.md`
2. Complete pre-flight checklist from AGENT-INSTRUCTIONS.md
3. Identify the next task. Delegate that to the orchestrate agent.
4. Create a new branch if needed (naming: `chore/modernize-<wave>-<task>`)
5. Work incrementally with small, conventional commits
6. Check off tasks in planning/modernize-TODO-index.md and related planning/modernize-wave\*.md files as completed
7. Update session log with decisions and challenges
8. Before completing, use the retrospective agent

## Build Commands

```powershell
# Build
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false

# Test
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

# Lint
dotnet pprettier --write . && dotnet format
```

Continue with tasks from modernize-TODO.md based on current sprint priorities.
````

## Universal Session End Prompt

Use this prompt before ending ANY session:

```text
# Session Finalization Checklist

Before ending, complete ALL mandatory steps:

## 1. Documentation Updates
- [ ] All completed tasks checked off in the appropriate wave file:
  - `.agents/planning/modernize-wave1.md` - Wave 0-1 tasks
  - `.agents/planning/modernize-wave2.md` - Wave 2 tasks
  - `.agents/planning/modernize-wave3-5.md` - Wave 3-5 tasks
- [ ] Update metrics in `.agents/planning/modernize-TODO-index.md`
- [ ] Session log complete at `.agents/sessions/YYYY-MM-DD-session-NN.md`:
  - What was done for each task
  - Decisions made and rationale
  - Challenges encountered and resolutions
  - Files changed and commits made
- [ ] `.agents/HANDOFF.md` updated with:
  - Current state (build/test status)
  - What was completed this session
  - What's next for following session
  - Any blockers or concerns
- [ ] retrospective agent run with all participants and findings documented
  - [ ] Commit documentation:
    - `git add .agents/`
    - `git add -f .agents/sessions/*.md`
    - `git commit -m "docs: update session documentation"`
    - `git add .serena/memories/`
    - `git commit -m "docs: update agent memories"`

## 2. Linting (run BEFORE committing)
- [ ] Fix markdown: `npx markdownlint-cli2 --fix "**/*.md"`
- [ ] Fix C# formatting: `dotnet format`
- [ ] Fix general formatting: `dotnet pprettier --write .`

## 3. Git Operations
- [ ] Stage all documentation: `git add .agents/`
- [ ] Force-add session logs: `git add -f .agents/sessions/*.md`
- [ ] Commit with conventional message

## 4. Verification
- [ ] Build passes: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`
- [ ] Tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`

## Critical Reminder
The next session has ZERO context except checked-in documentation.
Make documentation complete enough for any agent to continue.
```

---

## Document Control

| Version | Date       | Changes                                                                           |
| ------- | ---------- | --------------------------------------------------------------------------------- |
| 1.0     | 2025-12-06 | Initial prompts                                                                   |
| 2.0     | 2025-12-12 | Complete rewrite: tier-based prompts, zero-context format                         |
| 3.0     | 2025-12-12 | Self-contained prompts: each task includes full start/end protocol for copy/paste |
| 3.1     | 2025-12-12 | Added branch/clean checks, baseline build step, and forced add of session logs    |
| 3.2     | 2025-12-12 | Restored Universal Session End Prompt for agent handoff continuity                |
| 3.3     | 2025-12-12 | Updated for split TODO files: added file structure table, updated all references  |
| 3.4     | 2025-12-13 | Removed completed task prompts: W2.32 (CI Warning Gate), W3.1 (TFM Expansion)     |
| 3.5     | 2025-12-13 | Updated linting commands: use `npx markdownlint-cli2 --fix` and `dotnet format`   |
| 3.6     | 2025-12-13 | Updated W4.1 status as IN PROGRESS with coverage plan reference                   |
