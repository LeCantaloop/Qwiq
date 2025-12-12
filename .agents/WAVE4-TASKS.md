# Wave 4: Test Quality & Coverage Excellence - Task List

**Timeline**: 16 weeks  
**Success Metrics**:
- 65% mutation score on REST core paths
- <0.1% flake rate
- <5 minutes test execution time
- 80% of tests runnable offline

---

## Phase 1: Baseline & Planning (Weeks 1-2)

#### W4.1 Collect Test Execution Baseline Metrics 📋 PLANNED
- [ ] **Task**: Measure current test execution time across all test projects
- **Effort**: S (3 hours)
- **Priority**: Critical
- **Dependencies**: None
- **File(s)**:
  - `.github/workflows/main.yml` (add timing metrics)
  - `docs/metrics/test-baseline.md` (create)
- **Acceptance Criteria**:
  - [x] Execution time measured per test project (Qwiq.Core.Tests, Qwiq.Linq.Tests, etc.)
  - [x] Baseline metrics documented in `docs/metrics/test-baseline.md`
  - [x] GitHub Actions workflow captures timing data
  - [x] Total test execution time baseline established (<5min target)

---

#### W4.2 Measure Test Flake Rate 📋 PLANNED
- [ ] **Task**: Run test suite 50+ times to identify flaky tests
- **Effort**: M (6 hours)
- **Priority**: High
- **Dependencies**: W4.1
- **File(s)**:
  - `scripts/Measure-TestFlakiness.ps1` (create)
  - `docs/metrics/test-flakiness-report.md` (create)
- **Acceptance Criteria**:
  - [x] PowerShell script runs test suite 50+ iterations
  - [x] Script captures pass/fail/skip counts per test
  - [x] Flaky tests identified (fail >0% but <100% of runs)
  - [x] Flakiness report documents failure rates and patterns
  - [x] Baseline flake rate calculated (<0.1% target)

---

#### W4.3 Assess Current Code Coverage 📋 PLANNED
- [ ] **Task**: Generate code coverage baseline using existing coverage.runsettings
- **Effort**: S (2 hours)
- **Priority**: High
- **Dependencies**: W4.1
- **File(s)**:
  - `coverage.runsettings` (verify configuration)
  - `.github/workflows/main.yml` (add coverage reporting)
  - `docs/metrics/coverage-baseline.md` (create)
- **Acceptance Criteria**:
  - [x] Coverage report generated for all production projects
  - [x] Line coverage % documented per project
  - [x] Branch coverage % documented per project
  - [x] Coverage gaps identified in Qwiq.Core.Rest, Qwiq.Linq
  - [x] Baseline documented in `docs/metrics/coverage-baseline.md`

---

#### W4.4 SOAP Client Usage Assessment 📋 PLANNED
- [ ] **Task**: Analyze SOAP client usage to inform migration/deprecation strategy
- **Effort**: M (5 hours)
- **Priority**: Medium
- **Dependencies**: None
- **File(s)**:
  - `src/Qwiq.Core.Soap/**/*.cs` (analyze)
  - `test/Qwiq.Integration.Tests/**/*.cs` (identify SOAP tests)
  - `docs/adr/0XXX-soap-deprecation-strategy.md` (create)
- **Acceptance Criteria**:
  - [x] SOAP-specific code identified (Qwiq.Core.Soap, Qwiq.Identity.Soap)
  - [x] SOAP test coverage analyzed (net472 only tests)
  - [x] Public API surface documented for SOAP client
  - [x] Recommendation produced: Deprecate, Maintain, or Enhance
  - [x] ADR created documenting decision rationale

---

#### W4.5 Create Test Quality Improvement Plan 📋 PLANNED
- [ ] **Task**: Synthesize baseline metrics into actionable improvement plan
- **Effort**: S (3 hours)
- **Priority**: High
- **Dependencies**: W4.1, W4.2, W4.3, W4.4
- **File(s)**:
  - `docs/WAVE4-TEST-IMPROVEMENT-PLAN.md` (create)
- **Acceptance Criteria**:
  - [x] Plan prioritizes specific tests for mutation testing
  - [x] Plan identifies specific flaky tests to fix
  - [x] Plan lists integration tests to migrate to WireMock
  - [x] Plan includes timeline for achieving 65% mutation score
  - [x] Plan addresses SOAP test strategy based on W4.4 findings

---

## Phase 2: Mutation Testing Setup (Weeks 3-4)

#### W4.6 Add Stryker.NET to Project 📋 PLANNED
- [ ] **Task**: Install Stryker.NET mutation testing framework
- **Effort**: S (2 hours)
- **Priority**: Critical
- **Dependencies**: W4.5
- **File(s)**:
  - `.config/dotnet-tools.json` (update)
  - `stryker-config.json` (create at repo root)
- **Acceptance Criteria**:
  - [x] Stryker.NET added to dotnet tool manifest
  - [x] `dotnet tool restore` succeeds
  - [x] Basic stryker-config.json created with threshold-break: 0
  - [x] Stryker runs successfully: `dotnet stryker`
  - [x] Initial mutation score baseline captured

---

#### W4.7 Configure Stryker for Qwiq.Core 📋 PLANNED
- [ ] **Task**: Create targeted Stryker configuration for Qwiq.Core mutation testing
- **Effort**: M (4 hours)
- **Priority**: High
- **Dependencies**: W4.6
- **File(s)**:
  - `stryker-config.json` (update)
  - `test/Qwiq.Core.Tests/stryker-config.json` (create project-specific config)
- **Acceptance Criteria**:
  - [x] Stryker configured to mutate Qwiq.Core project only
  - [x] Test project Qwiq.Core.Tests specified
  - [x] Mutation operators configured (arithmetic, equality, logical, string)
  - [x] Timeout settings tuned for typical test execution
  - [x] HTML report generation enabled
  - [x] Mutation run completes in <10 minutes

---

#### W4.8 Configure Stryker for Qwiq.Core.Rest 📋 PLANNED
- [ ] **Task**: Create targeted Stryker configuration for REST client mutation testing
- **Effort**: M (4 hours)
- **Priority**: High
- **Dependencies**: W4.6
- **File(s)**:
  - `test/Qwiq.Core.Tests/stryker-config-rest.json` (create)
- **Acceptance Criteria**:
  - [x] Stryker configured to mutate Qwiq.Core.Rest project only
  - [x] Integration tests excluded from mutation testing
  - [x] REST-specific mutation thresholds configured
  - [x] Mutation run completes in <15 minutes
  - [x] Baseline mutation score documented (target 40-50% initially)

---

#### W4.9 Create Mutation Testing GitHub Workflow 📋 PLANNED
- [ ] **Task**: Add CI workflow for automated mutation testing
- **Effort**: M (5 hours)
- **Priority**: Medium
- **Dependencies**: W4.7, W4.8
- **File(s)**:
  - `.github/workflows/mutation-testing.yml` (create)
- **Acceptance Criteria**:
  - [x] Workflow runs Stryker on Qwiq.Core
  - [x] Workflow runs Stryker on Qwiq.Core.Rest
  - [x] Mutation reports uploaded as artifacts
  - [x] Workflow fails if mutation score drops below threshold
  - [x] Workflow scheduled weekly (not on every PR)
  - [x] Workflow uses `windows-latest` runner (net472 support)

---

#### W4.10 Analyze Initial Mutation Testing Results 📋 PLANNED
- [ ] **Task**: Review mutation testing output and identify weak test assertions
- **Effort**: M (8 hours)
- **Priority**: High
- **Dependencies**: W4.7, W4.8, W4.9
- **File(s)**:
  - `docs/metrics/mutation-testing-baseline.md` (create)
  - `test/Qwiq.Core.Tests/**/*.cs` (analyze test quality)
- **Acceptance Criteria**:
  - [x] All survived mutants reviewed and categorized
  - [x] Weak assertions identified (e.g., no asserts, only null checks)
  - [x] Missing test cases identified (uncovered branches)
  - [x] At least 10 specific test improvements documented
  - [x] Baseline mutation scores documented per project

---

## Phase 3: WireMock Integration Tests (Weeks 5-8)

#### W4.11 Add WireMock.Net Dependency 📋 PLANNED
- [ ] **Task**: Install WireMock.Net for HTTP mocking in integration tests
- **Effort**: S (2 hours)
- **Priority**: Critical
- **Dependencies**: W4.5
- **File(s)**:
  - `Directory.Packages.props` (add WireMock.Net version)
  - `test/Qwiq.Integration.Tests/Qwiq.IntegrationTests.csproj` (add PackageReference)
- **Acceptance Criteria**:
  - [x] WireMock.Net package added (latest stable version)
  - [x] Central Package Management used for version
  - [x] Test project builds successfully with WireMock reference
  - [x] No dependency conflicts with existing packages

---

#### W4.12 Create WireMockFixture Base Class 📋 PLANNED
- [ ] **Task**: Implement reusable WireMock test fixture for integration tests
- **Effort**: M (6 hours)
- **Priority**: Critical
- **Dependencies**: W4.11
- **File(s)**:
  - `test/Qwiq.Integration.Tests/Infrastructure/WireMockFixture.cs` (create)
  - `test/Qwiq.Integration.Tests/Infrastructure/WireMockTestBase.cs` (create)
- **Acceptance Criteria**:
  - [x] WireMockFixture starts/stops WireMock server
  - [x] Fixture loads recordings from `WireMockRecordings/` directory
  - [x] Fixture configures server port, SSL, and headers
  - [x] WireMockTestBase provides helper methods for test setup
  - [x] Fixture implements IDisposable for proper cleanup
  - [x] Example test demonstrates usage

---

#### W4.13 Implement WireMock Recording Helper 📋 PLANNED
- [ ] **Task**: Create utility to load and replay WireMock recordings
- **Effort**: M (5 hours)
- **Priority**: High
- **Dependencies**: W4.12
- **File(s)**:
  - `test/Qwiq.Integration.Tests/Infrastructure/WireMockRecordingHelper.cs` (create)
  - `scripts/Load-WireMockRecording.ps1` (enhance existing)
- **Acceptance Criteria**:
  - [x] Helper loads JSON mappings from recording directory
  - [x] Helper validates recording files before loading
  - [x] Helper supports filtering recordings by scenario/tag
  - [x] Helper provides fluent API for test configuration
  - [x] Documentation added for creating/using recordings

---

#### W4.14 Capture WireMock Recordings for Common Scenarios 📋 PLANNED
- [ ] **Task**: Record HTTP traffic for standard Azure DevOps operations
- **Effort**: L (12 hours)
- **Priority**: High
- **Dependencies**: W4.13
- **File(s)**:
  - `scripts/Capture-WireMockTraffic.ps1` (use existing)
  - `WireMockRecordings/BasicWorkItemQuery/` (create)
  - `WireMockRecordings/HierarchyTraversal/` (create)
  - `WireMockRecordings/IdentityResolution/` (create)
  - `WireMockRecordings/LinkRetrieval/` (create)
- **Acceptance Criteria**:
  - [x] Recording captured for basic work item query (ID lookup)
  - [x] Recording captured for WIQL query execution
  - [x] Recording captured for work item hierarchy traversal
  - [x] Recording captured for identity resolution
  - [x] Recording captured for link retrieval
  - [x] Each recording includes request/response pairs in JSON
  - [x] Recordings sanitized (no PATs, no PII)

---

#### W4.15 Migrate REST Integration Tests to WireMock 📋 PLANNED
- [ ] **Task**: Convert brittle REST integration tests to use WireMock recordings
- **Effort**: L (16 hours)
- **Priority**: High
- **Dependencies**: W4.14
- **File(s)**:
  - `test/Qwiq.Integration.Tests/Rest/**/*.cs` (migrate tests)
  - `test/Qwiq.Integration.Tests/Infrastructure/RestTestCategories.cs` (update)
- **Acceptance Criteria**:
  - [x] At least 10 integration tests migrated to WireMock
  - [x] Migrated tests pass without live Azure DevOps connection
  - [x] Migrated tests run in <30 seconds (fast feedback)
  - [x] Original live tests preserved with TestCategory=LiveIntegration
  - [x] New tests use TestCategory=OfflineIntegration
  - [x] Migration pattern documented for other contributors

---

#### W4.16 Create WireMock Tests for LINQ Provider 📋 PLANNED
- [ ] **Task**: Add offline integration tests for LINQ-to-WIQL translation
- **Effort**: M (8 hours)
- **Priority**: Medium
- **Dependencies**: W4.15
- **File(s)**:
  - `test/Qwiq.Linq.Tests/Integration/WireMockLinqTests.cs` (create)
  - `WireMockRecordings/LinqQueries/` (create)
- **Acceptance Criteria**:
  - [x] WireMock recordings captured for common LINQ queries
  - [x] Tests cover Where, OrderBy, Select operators
  - [x] Tests cover WIQL-specific extensions (AsOf, WasEver)
  - [x] Tests verify correct WIQL translation
  - [x] Tests verify correct result deserialization
  - [x] Tests run offline without external dependencies

---

#### W4.17 Validate 80% Offline Test Coverage 📋 PLANNED
- [ ] **Task**: Verify majority of tests can run without live connections
- **Effort**: S (3 hours)
- **Priority**: Medium
- **Dependencies**: W4.15, W4.16
- **File(s)**:
  - `.github/workflows/main.yml` (add offline test run)
  - `docs/metrics/offline-test-coverage.md` (create)
- **Acceptance Criteria**:
  - [x] Test suite runs with no network connectivity
  - [x] At least 80% of tests pass offline
  - [x] Only tests with TestCategory=LiveIntegration require network
  - [x] CI workflow runs offline tests separately from live tests
  - [x] Offline test execution time <3 minutes

---

## Phase 4: Test Quality Improvements (Weeks 9-12)

#### W4.18 Fix Identified Flaky Tests 📋 PLANNED
- [ ] **Task**: Eliminate flaky tests identified in W4.2
- **Effort**: L (16 hours)
- **Priority**: Critical
- **Dependencies**: W4.2, W4.5
- **File(s)**:
  - `test/Qwiq.Core.Tests/**/*.cs` (fix flaky tests)
  - `test/Qwiq.Integration.Tests/**/*.cs` (fix flaky tests)
- **Acceptance Criteria**:
  - [x] All tests with >1% flake rate fixed or removed
  - [x] Root causes documented (timing, state pollution, etc.)
  - [x] Fixes applied (add retries, isolate state, add waits)
  - [x] 50-run validation confirms <0.1% flake rate
  - [x] Flake rate improvement documented

---

#### W4.19 Improve Test Assertions Based on Mutation Testing 📋 PLANNED
- [ ] **Task**: Strengthen test assertions to kill more mutants
- **Effort**: L (20 hours)
- **Priority**: High
- **Dependencies**: W4.10
- **File(s)**:
  - `test/Qwiq.Core.Tests/**/*.cs` (enhance assertions)
  - `test/Qwiq.Core.Tests/**/*.cs` (add missing test cases)
- **Acceptance Criteria**:
  - [x] At least 10 weak assertions strengthened
  - [x] Missing test cases added for uncovered branches
  - [x] Mutation score improved by at least 10 percentage points
  - [x] All changes include comments explaining mutation testing rationale
  - [x] Tests remain maintainable and readable

---

#### W4.20 Achieve 65% Mutation Score on Qwiq.Core.Rest 📋 PLANNED
- [ ] **Task**: Iterate on test improvements until target mutation score reached
- **Effort**: L (24 hours)
- **Priority**: High
- **Dependencies**: W4.19
- **File(s)**:
  - `test/Qwiq.Core.Tests/**/*.cs` (continue improvements)
  - `stryker-config.json` (update thresholds)
- **Acceptance Criteria**:
  - [x] Mutation score ≥65% on Qwiq.Core.Rest core paths
  - [x] Mutation testing workflow passes in CI
  - [x] No mutations survive in critical paths (auth, query execution)
  - [x] Mutation testing report documented
  - [x] Future mutation score threshold enforced in CI

---

#### W4.21 Optimize Test Execution Time 📋 PLANNED
- [ ] **Task**: Reduce total test execution time to <5 minutes
- **Effort**: M (8 hours)
- **Priority**: Medium
- **Dependencies**: W4.17, W4.18
- **File(s)**:
  - `coverage.runsettings` (optimize settings)
  - `.github/workflows/main.yml` (parallel test execution)
  - `test/**/*.csproj` (review test configuration)
- **Acceptance Criteria**:
  - [x] Test execution time reduced to <5 minutes in CI
  - [x] Parallel test execution configured where safe
  - [x] Slow tests identified and optimized or isolated
  - [x] No degradation in test reliability
  - [x] Timing improvements documented

---

## Phase 5: Documentation & Knowledge Transfer (Weeks 13-16)

#### W4.22 Update TESTING.md with Comprehensive Guide 📋 PLANNED
- [ ] **Task**: Document all test practices, patterns, and tooling
- **Effort**: M (8 hours)
- **Priority**: High
- **Dependencies**: W4.17, W4.20, W4.21
- **File(s)**:
  - `TESTING.md` (major update)
- **Acceptance Criteria**:
  - [x] Document test execution commands (offline, live, mutation)
  - [x] Document WireMock recording/playback process
  - [x] Document mutation testing workflow
  - [x] Document flake prevention best practices
  - [x] Document test categorization strategy
  - [x] Include troubleshooting section for common issues
  - [x] Add examples of well-written tests

---

#### W4.23 Create ADR for Mutation Testing Strategy 📋 PLANNED
- [ ] **Task**: Document mutation testing decisions and rationale
- **Effort**: S (3 hours)
- **Priority**: Medium
- **Dependencies**: W4.20
- **File(s)**:
  - `docs/adr/0XXX-mutation-testing-strategy.md` (create)
- **Acceptance Criteria**:
  - [x] ADR documents why mutation testing was adopted
  - [x] ADR explains Stryker.NET selection rationale
  - [x] ADR justifies 65% mutation score target
  - [x] ADR describes mutation testing workflow
  - [x] ADR lists considered alternatives
  - [x] ADR follows existing ADR template

---

#### W4.24 Create ADR for WireMock Integration Testing 📋 PLANNED
- [ ] **Task**: Document WireMock decisions and offline testing strategy
- **Effort**: S (3 hours)
- **Priority**: Medium
- **Dependencies**: W4.17
- **File(s)**:
  - `docs/adr/0XXX-wiremock-integration-testing.md` (create)
- **Acceptance Criteria**:
  - [x] ADR documents why WireMock was adopted
  - [x] ADR explains offline-first testing strategy
  - [x] ADR describes recording capture process
  - [x] ADR justifies 80% offline test coverage target
  - [x] ADR addresses security concerns (PAT sanitization)
  - [x] ADR follows existing ADR template

---

#### W4.25 Add Definition of Done Checklist for Tests 📋 PLANNED
- [ ] **Task**: Create checklist ensuring all PRs meet test quality standards
- **Effort**: S (2 hours)
- **Priority**: Medium
- **Dependencies**: W4.22, W4.23, W4.24
- **File(s)**:
  - `.github/PULL_REQUEST_TEMPLATE.md` (update)
  - `CONTRIBUTING.md` (update with test requirements)
- **Acceptance Criteria**:
  - [x] PR template includes test quality checklist
  - [x] Checklist enforces mutation testing on new code
  - [x] Checklist enforces offline test capability where applicable
  - [x] Checklist requires test categorization
  - [x] Checklist references TESTING.md for guidance
  - [x] CONTRIBUTING.md updated with test expectations

---

## Summary

**Total Tasks**: 25  
**Total Effort**: ~185 hours (estimated)  
**Timeline**: 16 weeks

**Priority Breakdown**:
- Critical: 6 tasks
- High: 12 tasks
- Medium: 7 tasks

**Effort Breakdown**:
- Small (S): 9 tasks (~28 hours)
- Medium (M): 10 tasks (~67 hours)
- Large (L): 6 tasks (~90 hours)

**Key Milestones**:
- Week 2: Baseline metrics complete, improvement plan finalized
- Week 4: Mutation testing operational, initial scores captured
- Week 8: WireMock infrastructure complete, 80% offline tests
- Week 12: 65% mutation score achieved, <0.1% flake rate
- Week 16: Documentation complete, knowledge transferred
