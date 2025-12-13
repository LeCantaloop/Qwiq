# Wave 4: Test Quality Improvement Plan

**Status**: Active  
**Owner**: Engineering Team  
**Timeline**: 16 weeks  
**Target Release**: v11.0.0 (Production deployment for 100+ team members)

---

## Executive Summary

This plan outlines the path to production-grade test quality for Qwiq v11.0.0, addressing code coverage, test reliability, and testing infrastructure improvements.

**Current State** (2025-12-12 Baseline):

- **Code Coverage**: 51.1% line, 36.7% branch (Target: 70%)
- **Test Flake Rate**: 0.00% (Target: <0.1%) ✅ **Exceeds target**
- **Test Execution Time**: 11.58s for 189 tests (Target: <300s) ✅ **Exceeds target**
- **SOAP Client**: 0% coverage, cannot deploy in Kubernetes → **Deprecate**

**Gap to Target**: +18.9% line coverage needed

**Strategic Path**: Focus on REST client testing (0% → 60%) + Core auth/credentials (0% → 50%)

---

## Phase 1: Baseline & Planning ✅ COMPLETE

**Duration**: Weeks 1-2  
**Status**: Complete (2025-12-12, Session 30)

### Completed Tasks

| Task                                | Status | Key Finding                                      |
| ----------------------------------- | ------ | ------------------------------------------------ |
| **W4.1** - Test Execution Baseline  | ✅     | 189 tests, 11.58s execution (well under target)  |
| **W4.2** - Test Flake Rate          | ✅     | 0.00% flake rate (10/10 iterations passed)       |
| **W4.3** - Code Coverage Assessment | ✅     | 51.1% coverage, REST client at 0% (critical gap) |
| **W4.4** - SOAP Usage Assessment    | ✅     | Recommend deprecation (ADR-010)                  |
| **W4.5** - Test Improvement Plan    | ✅     | This document                                    |

### Key Artifacts Created

- `docs/metrics/test-baseline.md` - Test execution baseline
- `docs/metrics/test-flakiness-report.md` - Flakiness analysis (0% flake rate)
- `docs/adr/ADR-010-soap-client-deprecation-strategy.md` - SOAP deprecation decision
- `.agents/WAVE4-TEST-IMPROVEMENT-PLAN.md` - This plan
- `scripts/Measure-TestFlakiness.ps1` - Automated flakiness measurement

---

## Phase 2: Coverage Expansion - REST Client (Weeks 3-6)

**Goal**: Increase coverage from 51.1% → 65%+ by adding REST client tests  
**Priority**: 🔴 CRITICAL - Highest ROI toward 70% target

### W4.6: REST Client - WorkItemStore Tests

**Effort**: L (8 hours)  
**Target Coverage**: WorkItemStore.cs (0% → 80%)

**Tests to Add**:

- [ ] `Given_WorkItemStore_created_with_valid_options`
  - Then_can_query_work_items
  - Then_can_get_work_item_by_id
  - Then_can_get_multiple_work_items
- [ ] `Given_WorkItemStore_with_invalid_credentials`
  - Then_throws_authentication_exception
- [ ] `Given_WorkItemStore_querying_nonexistent_work_item`
  - Then_returns_null
- [ ] `Given_WorkItemStore_with_batch_read`
  - Then_retrieves_work_items_efficiently

**Dependencies**: WireMock fixtures for REST API responses (W4.11+)

---

### W4.7: REST Client - Query Classes Tests

**Effort**: M (6 hours)  
**Target Coverage**: Query.cs, QueryDefinition.cs, QueryFolder.cs (0% → 70%)

**Tests to Add**:

- [ ] `Given_Query_with_WIQL_string`
  - Then_executes_query_and_returns_work_items
- [ ] `Given_QueryDefinition_from_server`
  - Then_loads_WIQL_correctly
  - Then_preserves_query_metadata
- [ ] `Given_QueryFolder_hierarchy`
  - Then_enumerates_child_folders
  - Then_enumerates_child_queries

**Approach**: Use `MockWorkItemStore` for isolation, WireMock for integration scenarios

---

### W4.8: REST Client - WorkItem & Field Tests

**Effort**: M (6 hours)  
**Target Coverage**: WorkItem.cs, Field.cs, FieldCollection.cs (0% → 70%)

**Tests to Add**:

- [ ] `Given_WorkItem_loaded_from_REST`
  - Then_fields_are_accessible
  - Then_revisions_are_enumerable
  - Then_links_are_accessible
- [ ] `Given_Field_with_various_types`
  - Then_string_values_handled
  - Then_numeric_values_handled
  - Then_datetime_values_handled
  - Then_identity_values_handled

---

### W4.9: Core - Authentication & Credentials Tests

**Effort**: M (5 hours)  
**Target Coverage**: AuthenticationOptions.cs, CredentialsFactory.cs (0% → 50%)

**Tests to Add**:

- [ ] `Given_AuthenticationOptions_with_PAT`
  - Then_creates_network_credential
- [ ] `Given_AuthenticationOptions_with_Windows_auth`
  - Then_uses_default_credentials
- [ ] `Given_CredentialsFactory_with_invalid_type`
  - Then_throws_appropriate_exception

**Note**: Focus on unit tests for credential creation logic, not actual authentication flows

---

### Coverage Milestone 1

**Target**: 65% overall coverage after W4.6-W4.9  
**Verification**: Run `dotnet test --settings coverage.runsettings` and check `artifacts/coverage/Summary.txt`

Expected breakdown:

- Qwiq.Client.Rest: 0% → 60%
- Qwiq.Core: 51.2% → 60%
- Overall: 51.1% → 65%

---

## Phase 3: Coverage Expansion - Remaining Gaps (Weeks 7-8)

**Goal**: Close remaining gaps to achieve 70% target

### W4.10: Mapper - Exception & Edge Cases

**Effort**: S (3 hours)  
**Target Coverage**: AttributeMapException.cs, PropertyMap.cs (0% → 80%)

**Tests to Add**:

- [ ] `Given_AttributeMapper_with_invalid_field_name`
  - Then_throws_AttributeMapException_with_details
- [ ] `Given_PropertyMap_with_null_work_item`
  - Then_handles_gracefully

---

### W4.11: LINQ - QueryExtensions Coverage

**Effort**: S (3 hours)  
**Target Coverage**: QueryExtensions.cs (20% → 80%)

**Tests to Add**:

- [ ] `Given_Query_with_AsOf_expression`
  - Then_generates_correct_WIQL
- [ ] `Given_Query_with_WasEver_expression`
  - Then_generates_correct_WIQL
- [ ] `Given_Query_with_InGroup_expression`
  - Then_generates_correct_WIQL

---

### Coverage Milestone 2

**Target**: 70% overall coverage after W4.10-W4.11  
**Verification**: Final coverage report

Expected breakdown:

- Qwiq.Client.Rest: 60%
- Qwiq.Linq: 90.9% → 95%
- Qwiq.Mapper: 73.9% → 85%
- Qwiq.Core: 60%
- **Overall: 70%** ✅

---

## Phase 4: Mutation Testing (Weeks 9-12)

**Goal**: Achieve 65% mutation score on critical paths

### W4.12: Stryker.NET Setup

**Effort**: S (2 hours)

- [ ] Add Stryker.NET to `.config/dotnet-tools.json`
- [ ] Create `stryker-config.json` with baseline configuration
- [ ] Run initial mutation test on Qwiq.Core
- [ ] Document baseline mutation score

---

### W4.13: Targeted Mutation Testing

**Effort**: M (6 hours per project)

Focus areas (in priority order):

1. **Qwiq.Linq.WiqlTranslator** (CRITICAL - query translation logic)
2. **Qwiq.Core.TypeParser** (HIGH - type conversion)
3. **Qwiq.Mapper.AttributeMapperStrategy** (MEDIUM - object mapping)

**Per Project**:

- [ ] Run Stryker mutation testing
- [ ] Analyze surviving mutants
- [ ] Add tests to kill high-value mutants
- [ ] Re-run until 65% mutation score achieved

---

### W4.14: Mutation Testing Report

**Effort**: S (2 hours)

- [ ] Generate HTML mutation reports
- [ ] Document mutation score per project
- [ ] Identify patterns in surviving mutants
- [ ] Create mutation testing guidelines for future development

---

## Phase 5: Offline Testing (Weeks 13-16)

**Goal**: 80% of tests runnable offline (WireMock migration)

### W4.15: WireMock.NET Integration

**Effort**: M (5 hours)

- [ ] Add WireMock.NET dependency
- [ ] Create WireMockFixture base class
- [ ] Implement recording helper
- [ ] Document WireMock usage patterns

**Reference**: ADR-008 (WireMock Offline REST Testing)

---

### W4.16: Capture REST API Fixtures

**Effort**: L (8 hours)

**Common Scenarios to Record**:

- [ ] Get work item by ID (various types: Bug, Task, User Story)
- [ ] Query work items by WIQL
- [ ] Get work item revisions
- [ ] Get work item links
- [ ] Get field definitions
- [ ] Get projects

**Process**:

1. Run integration tests with WireMock in recording mode
2. Authenticate with Azure DevOps sandbox
3. Capture HTTP traffic to JSON files
4. Review and sanitize fixtures (remove credentials)
5. Commit fixtures to repository

---

### W4.17: Migrate Integration Tests to WireMock

**Effort**: L (10 hours)

- [ ] Convert REST integration tests to use WireMock fixtures
- [ ] Verify tests pass offline (no network access)
- [ ] Update test categories (remove `REST` tag, keep as unit tests)
- [ ] Document WireMock test patterns

**Target**: 80% of tests run offline (only SOAP integration tests remain online)

---

## SOAP Client Strategy

**Decision**: Deprecate SOAP client (ADR-010)

### Actions Required

**v11.0.0** (Current Release):

- [ ] Add `<PackageDeprecated>true</PackageDeprecated>` to SOAP NuGet metadata
- [ ] Add `[Obsolete]` attributes to SOAP public APIs
- [ ] Create `docs/SOAP-TO-REST-MIGRATION.md` guide
- [ ] Update README.md to recommend REST client

**v11.x** (6-month support window):

- [ ] Monitor SOAP package downloads
- [ ] Provide migration support
- [ ] Critical bug fixes only

**v12.0.0** (Breaking change release):

- [ ] Remove `Qwiq.Client.Soap` project
- [ ] Remove `Qwiq.Identity.Soap` project
- [ ] Update migration documentation

**Rationale**: SOAP client cannot be deployed in Kubernetes (primary deployment target), has 0% test coverage, and Microsoft recommends REST API.

---

## Success Metrics & Tracking

### Primary Metrics

| Metric                  | Baseline | Target | Current | Status            |
| ----------------------- | -------- | ------ | ------- | ----------------- |
| **Line Coverage**       | 51.1%    | 70%    | 51.1%   | 🔴 In Progress    |
| **Branch Coverage**     | 36.7%    | 60%    | 36.7%   | 🔴 In Progress    |
| **Flake Rate**          | 0.00%    | <0.1%  | 0.00%   | ✅ Exceeds Target |
| **Test Execution Time** | 11.58s   | <300s  | 11.58s  | ✅ Exceeds Target |
| **Mutation Score**      | TBD      | 65%    | TBD     | 📋 Pending        |
| **Offline Tests**       | ~10%     | 80%    | ~10%    | 📋 Pending        |

### Secondary Metrics

- **Test Count**: 189 → 350+ (coverage expansion)
- **Integration Test Count**: Reduce from ~30 to ~6 (WireMock migration)
- **SOAP Test Count**: Exclude from CI (deprecated)

---

## Test Patterns & Best Practices

### Established Patterns (Keep Using)

✅ **ContextSpecification Pattern**:

```csharp
[TestClass]
public class Given_WorkItemStore_with_valid_options : ContextSpecification
{
    private IWorkItemStore _sut;

    public override void Given()
    {
        var options = new AuthenticationOptions(/* ... */);
        _sut = WorkItemStoreFactory.Default.Create(options);
    }

    [TestMethod]
    public void Then_can_query_work_items()
    {
        var results = _sut.Query("SELECT [System.Id] FROM WorkItems");
        results.ShouldNotBeEmpty();
    }
}
```

✅ **Mock Usage**:

- Use `MockWorkItemStore`, `MockWorkItem`, `MockRevision` from `Qwiq.Mocks`
- Use `MockIdentityManagementService` for identity resolution tests

✅ **Shouldly Assertions**:

- `result.ShouldBe(expected)`
- `collection.ShouldNotBeEmpty()`
- `value.ShouldBeNull()`

### New Patterns (Add for Coverage)

🆕 **WireMock for REST Testing** (Phase 5):

```csharp
public class Given_REST_WorkItem_from_fixture : WireMockContextSpecification
{
    protected override string FixtureName => "work-item-123.json";

    [TestMethod]
    public void Then_loads_fields_correctly()
    {
        var workItem = _store.GetWorkItem(123);
        workItem.Title.ShouldBe("Expected Title");
    }
}
```

🆕 **Mutation Testing Focus**:

- Add boundary condition tests (off-by-one errors)
- Test null handling explicitly
- Test arithmetic edge cases
- Test boolean logic branches

---

## Risk Management

### High Risks

| Risk                                          | Impact    | Mitigation                                                               |
| --------------------------------------------- | --------- | ------------------------------------------------------------------------ |
| **REST coverage takes longer than estimated** | 🔴 HIGH   | Start with highest-value tests (WorkItemStore), defer nice-to-have tests |
| **WireMock fixtures are brittle**             | 🟡 MEDIUM | Keep fixtures minimal, focus on common scenarios only                    |
| **Mutation testing reveals deeper issues**    | 🟡 MEDIUM | Budget extra time for mutation test fixes, prioritize critical paths     |

### Medium Risks

| Risk                                        | Impact    | Mitigation                                            |
| ------------------------------------------- | --------- | ----------------------------------------------------- |
| **SOAP users resist deprecation**           | 🟡 MEDIUM | Provide clear migration guide, 6-month support window |
| **Integration tests still needed for SOAP** | 🟡 MEDIUM | Accept SOAP tests as manual/excluded from CI          |

---

## Timeline & Milestones

### Week 1-2: ✅ Baseline (COMPLETE)

- W4.1: Test execution baseline
- W4.2: Flake rate measurement
- W4.3: Coverage assessment
- W4.4: SOAP usage assessment
- W4.5: This improvement plan

### Week 3-6: Coverage Expansion - REST

- W4.6: WorkItemStore tests → 65% coverage milestone
- W4.7: Query classes tests
- W4.8: WorkItem & Field tests
- W4.9: Authentication tests

**Milestone**: 65% overall coverage

### Week 7-8: Coverage Expansion - Final Push

- W4.10: Mapper exceptions
- W4.11: LINQ QueryExtensions

**Milestone**: 70% overall coverage ✅ **Production Target Achieved**

### Week 9-12: Mutation Testing

- W4.12: Stryker.NET setup
- W4.13: Targeted mutation testing
- W4.14: Mutation reports

**Milestone**: 65% mutation score

### Week 13-16: Offline Testing

- W4.15: WireMock integration
- W4.16: Capture fixtures
- W4.17: Migrate integration tests

**Milestone**: 80% tests runnable offline

---

## Deliverables

### Documentation

- [x] Test execution baseline report
- [x] Flakiness measurement report
- [x] SOAP deprecation ADR
- [x] This improvement plan
- [ ] SOAP→REST migration guide
- [ ] WireMock usage guide
- [ ] Mutation testing guidelines

### Code

- [ ] 161+ new tests (189 → 350+)
- [ ] WireMock fixtures for REST API
- [ ] Stryker.NET configuration
- [ ] SOAP deprecation annotations

### Metrics

- [ ] 70% line coverage
- [ ] 65% mutation score
- [ ] 80% offline test capability
- [ ] 0% flake rate (maintain)

---

## Approval & Sign-off

**Prepared by**: Copilot Agent (Session 30)  
**Reviewed by**: _Pending_  
**Approved by**: _Pending_  
**Date**: 2025-12-12

---

## Related Documents

- [Wave 4 Task List](../.agents/WAVE4-TASKS.md)
- [Test Execution Baseline](metrics/test-baseline.md)
- [Flakiness Report](metrics/test-flakiness-report.md)
- [ADR-010: SOAP Deprecation](adr/ADR-010-soap-client-deprecation-strategy.md)
- [ADR-007: REST Client Testability](adr/ADR-007-rest-client-testability.md)
- [ADR-008: WireMock Offline Testing](adr/ADR-008-wiremock-offline-rest-testing.md)
