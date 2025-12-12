# Explainer: REST/SOAP Unit Test Coverage for CI

> **Document Purpose**: Product Requirements Document (PRD) for implementing unit test coverage for REST and SOAP client logic without requiring Azure DevOps connectivity.
>
> **Last Updated**: December 6, 2025
> **Status**: Planning
> **Priority**: Phase 1 (REST) - High, Phase 2 (SOAP) - Medium

---

## Table of Contents

1. [Introduction/Overview](#introductionoverview)
2. [Goals](#goals)
3. [Non-Goals (Out of Scope)](#non-goals-out-of-scope)
4. [User Stories](#user-stories)
5. [Problem Statement](#problem-statement)
6. [Current State Analysis](#current-state-analysis)
7. [Functional Requirements](#functional-requirements)
8. [Technical Approach](#technical-approach)
9. [Implementation Phases](#implementation-phases)
10. [Testing Strategy](#testing-strategy)
11. [CI/CD Integration](#cicd-integration)
12. [Dependencies and Prerequisites](#dependencies-and-prerequisites)
13. [Risks and Mitigations](#risks-and-mitigations)
14. [Success Criteria](#success-criteria)
15. [Estimated Effort](#estimated-effort)
16. [Open Questions](#open-questions)

---

## Introduction/Overview

The Qwiq library provides two client implementations (REST and SOAP) for querying Azure DevOps / TFS work items. Currently, all tests for these clients require live Azure DevOps connectivity, making them unsuitable for CI environments and creating friction for contributors who lack Azure DevOps access.

This explainer defines a plan to implement **unit test coverage** for REST and SOAP client logic that runs in CI without external dependencies. The solution will use mocking to isolate client behavior from the underlying SDKs (`Microsoft.VisualStudio.Services.Client` for REST, `Microsoft.TeamFoundationServer.ExtendedClient` for SOAP), enabling fast, reliable validation of adapter logic in continuous integration pipelines.

---

## Goals

1. **Enable Local Development**: Contributors can run and pass all unit tests without Azure DevOps credentials or connectivity
2. **Accelerate CI Feedback**: CI pipelines validate REST/SOAP logic without external dependencies, reducing build times and increasing reliability
3. **Increase Test Coverage**: Critical paths (query execution, work item retrieval, field access patterns, authentication flow) have automated unit test validation
4. **Maintain Test Quality**: Tests focus on behavior/output validation (state testing) rather than SDK call verification
5. **Cross-Platform REST Tests**: REST client unit tests run on all platforms (Windows, Linux, macOS)
6. **Document Test Patterns**: Establish clear patterns for testing thin adapters wrapping external SDKs

---

## Non-Goals (Out of Scope)

1. **Refactoring Existing Architecture**: Work within the current "thin adapter" design pattern without major restructuring
2. **Integration Test Replacement**: Unit tests complement (not replace) existing integration tests that validate real Azure DevOps connectivity
3. **Code Coverage Percentage Targets**: Focus on critical paths rather than arbitrary coverage metrics
4. **SOAP Priority**: Phase 2 (SOAP) is lower priority and deferred until Phase 1 (REST) is complete

## Implementation Decision: Real Captured Traffic (ADR-008)

**Note**: The original requirement specified "synthetic test data only" (no recorded responses). However, during implementation, manual stub creation failed due to Azure DevOps SDK's proprietary JSON serialization requirements, particularly for `IdentityDescriptor` which requires exact string format `"Microsoft.IdentityModel.Claims.ClaimsIdentity;..."` rather than object format.

**Decision**: Use real captured Azure DevOps HTTP traffic (via Fiddler HAR capture) converted to WireMock stubs. This approach was documented in ADR-008 and successfully implemented.

**Rationale**:
- Manual synthetic stubs failed due to serialization format mismatches
- Real captured traffic ensures authentic API response formats
- Captured stubs are deterministic and can be updated by recapturing traffic
- No sensitive data: stubs contain only test work item data from sandbox environment

See: `docs/adr/008-wiremock-offline-rest-testing.md` for full decision rationale.
6. **Testing Framework Changes**: Continue using existing Moq, Shouldly, and ContextSpecification patterns

---

## User Stories

### Primary Users

**As a contributor without Azure DevOps access**, I want to run all unit tests locally so that I can validate my changes before submitting a pull request.

**As a CI pipeline**, I want to validate REST/SOAP logic without external dependencies so that builds are fast, reliable, and independent of external service availability.

**As a maintainer**, I want comprehensive unit tests for critical paths so that I can confidently review pull requests and catch regressions early.

### Detailed User Stories

1. **As a contributor**, I want to run `dotnet test --filter "TestCategory=RestUnit"` and see all REST client unit tests pass in under 5 seconds, so that I get rapid feedback during development.

2. **As a CI pipeline**, I want to execute REST unit tests on Linux runners without requiring Windows-specific dependencies, so that I can parallelize builds across platforms.

3. **As a maintainer**, I want query execution tests that validate WIQL query handling, field mapping, and result parsing, so that I know the adapter correctly transforms between Qwiq abstractions and SDK types.

4. **As a contributor**, I want to mock SDK responses using captured Azure DevOps API responses (converted to WireMock stubs), so that tests are fast, deterministic, and don't require authentication or network connectivity.

5. **As a new contributor**, I want clear documentation showing how to write unit tests for REST/SOAP adapters, so that I can follow established patterns when adding new functionality.

---

## Problem Statement

### Current Challenges

| Problem | Impact | Evidence |
|---------|--------|----------|
| **Integration-Only Tests** | All REST/SOAP tests require Azure DevOps connectivity | `TestCategory=REST`, `TestCategory=SOAP`, `TestCategory=IntegrationTests` all excluded from CI |
| **High Barrier to Entry** | New contributors need Azure DevOps credentials and sandbox access | Contributors cannot run full test suite locally |
| **Slow Feedback Loops** | Integration tests are slow (network latency, authentication) | Test runs take minutes instead of seconds |
| **CI Unreliability** | External dependency on Azure DevOps service availability | Builds fail due to transient network issues or service outages |
| **Limited Platform Coverage** | SOAP tests only run on Windows; REST tests could run cross-platform but are excluded from CI | No validation on Linux/macOS |

### Why This Matters

- **Quality Gate Failure**: Pull requests cannot be validated in CI without external dependencies
- **Contributor Friction**: High barrier to entry discourages external contributions
- **Regression Risk**: Lack of unit tests means adapter logic is only validated through manual integration testing
- **Platform Risk**: REST client (cross-platform) has no automated cross-platform validation

---

## Current State Analysis

### Existing Test Structure

| Test Project | Target Frameworks | Current Focus | CI Execution |
|--------------|-------------------|---------------|--------------|
| `Qwiq.Core.Tests` | net472;net8.0 | Core abstractions (interfaces, mocks) | ✅ Full coverage in CI |
| `Qwiq.Linq.Tests` | net472;net8.0 | LINQ provider, WIQL translation | ✅ Full coverage in CI |
| `Qwiq.Mapper.Tests` | net472;net8.0 | Object mapping | ✅ Full coverage in CI |
| `Qwiq.Identity.Tests` | net472;net8.0 | Identity management | ✅ Full coverage in CI |
| `Qwiq.IntegrationTests` | net472 | Full integration (REST + SOAP) | ❌ Excluded (requires Azure DevOps) |

### Existing Test Categories

```csharp
// Current categories (from copilot-instructions.md)
[TestCategory("localOnly")]        // Requires local TFS instance
[TestCategory("Benchmark")]         // Performance tests
[TestCategory("SOAP")]              // SOAP integration tests
[TestCategory("REST")]              // REST integration tests
[TestCategory("IntegrationTests")] // Full integration suite
```

**CI Test Filter** (from `.github/workflows/main.yml`):
```powershell
--filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
```

### Existing Test Patterns

```csharp
// ContextSpecification pattern (from Qwiq.Tests.Common)
[TestClass]
public class Given_some_context : ContextSpecification
{
    private MyClass _sut; // System Under Test

    public override void Given()
    {
        // Arrange - setup mocks and dependencies
        _sut = new MyClass();
    }

    public override void When()
    {
        // Act - perform the action being tested
        _sut.DoSomething();
    }

    [TestMethod]
    public void Then_expected_behavior()
    {
        // Assert using Shouldly
        _sut.Result.ShouldBe(expected);
    }
}
```

### Client Architecture

| Client | Project | Wrapper Pattern | SDK Dependency |
|--------|---------|-----------------|----------------|
| **REST** | `Qwiq.Core.Rest` | Thin adapter over VSS Client | `Microsoft.VisualStudio.Services.Client` |
| **SOAP** | `Qwiq.Core.Soap` | Thin adapter over TFS Client OM | `Microsoft.TeamFoundationServer.ExtendedClient` |

**Key Classes** (targets for unit tests):
- `WorkItemStore` - implements `IWorkItemStore`
- `WorkItem` - implements `IWorkItem`
- `Query` - implements `IQuery`
- `WorkItemStoreFactory` - creates store instances

---

## Functional Requirements

### Phase 1: REST Client Unit Tests (Priority)

#### FR1: Query Execution Tests
The system must validate REST client query execution logic without requiring Azure DevOps connectivity.

**Acceptance Criteria**:
- Mock `WorkItemTrackingHttpClient` responses for WIQL queries
- Validate query parameter handling (WIQL string, page size, time zone)
- Verify result parsing (work item IDs, field mappings)
- Test error handling (invalid WIQL, network errors, API errors)
- Tests run on all platforms (Windows, Linux, macOS)

#### FR2: Work Item Retrieval Tests
The system must validate REST client work item retrieval and field access patterns.

**Acceptance Criteria**:
- Mock `GetWorkItemsAsync` responses with captured Azure DevOps API responses (WireMock stubs)
- Validate field value extraction (System.Id, System.Title, custom fields)
- Test batch retrieval (multiple work items)
- Verify revision handling
- Test link retrieval and parsing

#### FR3: Authentication Flow Tests
The system must validate authentication option handling without requiring real credentials.

**Acceptance Criteria**:
- Mock authentication providers for each `AuthenticationType` (Windows, PAT, OAuth, Basic)
- Validate credential factory invocation
- Test connection URI handling
- Verify authentication error scenarios

#### FR4: Factory Pattern Tests
The system must validate `WorkItemStoreFactory` creation logic.

**Acceptance Criteria**:
- Test factory creates correct store instance for REST options
- Validate option validation (null checks, URI validation)
- Test error handling for invalid options

### Phase 2: SOAP Client Unit Tests (Secondary Priority)

#### FR5: SOAP Query Tests
The system must validate SOAP client query execution logic (Windows-only).

**Acceptance Criteria**:
- Mock `WorkItemStore` from TFS Client OM
- Validate query execution and result parsing
- Test SOAP-specific error handling
- Tests run on Windows only

#### FR6: SOAP Work Item Tests
The system must validate SOAP client work item retrieval patterns.

**Acceptance Criteria**:
- Mock TFS Client OM work item responses
- Validate field access patterns
- Test revision handling
- Verify link retrieval

### Phase 3: CI/CD Integration

#### FR7: Test Category Configuration
The system must provide granular test category controls for CI execution.

**Acceptance Criteria**:
- New test category `RestUnit` for REST unit tests
- New test category `SoapUnit` for SOAP unit tests
- CI filter updated to include `RestUnit` and `SoapUnit`
- Documentation updated with category usage

#### FR8: Cross-Platform Validation
The system must validate REST client on multiple platforms.

**Acceptance Criteria**:
- REST unit tests run on Linux, Windows, macOS in CI
- Build matrix configuration for multi-platform testing
- Platform-specific test exclusions (SOAP on Windows only)

---

## Technical Approach

### Mocking Strategy

**Use Moq to mock SDK types**:

```csharp
// Example: Mock WorkItemTrackingHttpClient for REST tests
[TestClass]
public class Given_REST_query_execution : ContextSpecification
{
    private Mock<WorkItemTrackingHttpClient> _mockClient;
    private IWorkItemStore _store;
    private WorkItemQueryResult _queryResult;

    public override void Given()
    {
        // Mock the HTTP client from VSS SDK
        _mockClient = new Mock<WorkItemTrackingHttpClient>(
            MockBehavior.Strict,
            new Uri("https://dev.azure.com/test"),
            Mock.Of<VssCredentials>(),
            new VssHttpRequestSettings());

        // Setup query response with captured WireMock stub data
        var mockResponse = new WorkItemQueryResult
        {
            WorkItems = new[]
            {
                new WorkItemReference { Id = 1, Url = "https://dev.azure.com/test/_apis/wit/workItems/1" },
                new WorkItemReference { Id = 2, Url = "https://dev.azure.com/test/_apis/wit/workItems/2" }
            }
        };

        _mockClient
            .Setup(x => x.QueryByWiqlAsync(
                It.IsAny<Wiql>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        // Create store with mocked client (via dependency injection or factory)
        _store = new WorkItemStore(_mockClient.Object);
    }

    public override void When()
    {
        _queryResult = _store.Query("SELECT [System.Id] FROM WorkItems WHERE [System.State] = 'Active'");
    }

    [TestMethod]
    public void Then_should_return_correct_work_item_count()
    {
        _queryResult.WorkItems.Count().ShouldBe(2);
    }

    [TestMethod]
    public void Then_should_return_correct_work_item_ids()
    {
        var ids = _queryResult.WorkItems.Select(wi => wi.Id).ToArray();
        ids.ShouldBe(new[] { 1, 2 });
    }
}
```

### Test Data Management

**Test Data Principles** (Updated per ADR-008):
1. **Captured Real Traffic**: Use real Azure DevOps API responses captured via Fiddler and converted to WireMock stubs
2. **Sandbox Environment Only**: Capture traffic from test/sandbox Azure DevOps instances, not production
3. **Deterministic**: Same captured responses every test execution
4. **Maintainable**: Stubs can be updated by recapturing traffic when APIs change
5. **Minimal Sensitive Data**: Stubs contain only test work item data, no credentials or tokens

**Note**: Original plan specified synthetic data only, but manual stub creation failed due to Azure DevOps SDK serialization requirements. Real captured traffic ensures authentic API response formats. See ADR-008 for full rationale.

**Example Captured Work Item Data** (from WireMock stubs):
```csharp
public static class TestWorkItems
{
    public static WorkItem CreateBasicBug()
    {
        return new WorkItem
        {
            Id = 1,
            Fields = new Dictionary<string, object>
            {
                ["System.Id"] = 1,
                ["System.WorkItemType"] = "Bug",
                ["System.Title"] = "Test Bug",
                ["System.State"] = "Active",
                ["System.AssignedTo"] = new IdentityRef { DisplayName = "Test User" }
            }
        };
    }
}
```

### Dependency Injection for Testability

**Current Architecture** (thin adapter):
```csharp
// Qwiq.Core.Rest/WorkItemStore.cs (simplified)
public class WorkItemStore : IWorkItemStore
{
    private readonly WorkItemTrackingHttpClient _client;

    internal WorkItemStore(WorkItemTrackingHttpClient client)
    {
        _client = client;
    }

    public IQuery Query(string wiql)
    {
        var result = _client.QueryByWiqlAsync(new Wiql { Query = wiql }).Result;
        return new Query(result, _client);
    }
}
```

**Testing Approach**:
- Use `internal` constructor with mocked `WorkItemTrackingHttpClient`
- `InternalsVisibleTo` already configured for test projects
- No refactoring needed - adapter is already testable

---

## Implementation Phases

### Phase 1: REST Client Unit Tests (Week 1-2)

**Scope**: Core REST client logic with cross-platform CI validation

**Tasks**:

| ID | Task | Deliverable | Acceptance |
|----|------|-------------|------------|
| R1.1 | Create test class structure in `Qwiq.Core.Tests` | `Given_REST_WorkItemStore_*` classes | File structure exists |
| R1.2 | Implement query execution tests with Moq | Mock `WorkItemTrackingHttpClient.QueryByWiqlAsync` | Tests pass locally |
| R1.3 | Implement work item retrieval tests | Mock `GetWorkItemsAsync` responses | Tests validate field mapping |
| R1.4 | Implement field access pattern tests | Test `IWorkItem.GetField<T>()` variants | Edge cases covered |
| R1.5 | Implement authentication flow tests | Mock credential providers | All `AuthenticationType` values tested |
| R1.6 | Implement factory pattern tests | Test `WorkItemStoreFactory.Create()` | Validation logic tested |
| R1.7 | Add `RestUnit` test category | Apply `[TestCategory("RestUnit")]` attribute | Category defined |
| R1.8 | Update CI filter to include `RestUnit` | Modify `.github/workflows/main.yml` | Tests run in CI |
| R1.9 | Add cross-platform matrix to CI | Test on Windows, Linux, macOS | All platforms pass |
| R1.10 | Document REST testing patterns | Add section to `TESTING.md` | Contributors have examples |

**Example Test Case**: Query Execution with Pagination

```csharp
[TestClass]
[TestCategory("RestUnit")]
public class Given_REST_query_with_pagination : ContextSpecification
{
    private Mock<WorkItemTrackingHttpClient> _mockClient;
    private IWorkItemStore _store;
    private IEnumerable<IWorkItem> _results;

    public override void Given()
    {
        _mockClient = new Mock<WorkItemTrackingHttpClient>(/*...*/);

        // First page
        _mockClient
            .Setup(x => x.QueryByWiqlAsync(
                It.Is<Wiql>(w => w.Query.Contains("SELECT")),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WorkItemQueryResult
            {
                WorkItems = Enumerable.Range(1, 100)
                    .Select(id => new WorkItemReference { Id = id })
                    .ToArray()
            });

        // GetWorkItemsAsync called with batched IDs
        _mockClient
            .Setup(x => x.GetWorkItemsAsync(
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<DateTime?>(),
                It.IsAny<WorkItemExpand?>(),
                It.IsAny<WorkItemErrorPolicy?>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<int> ids, IEnumerable<string> fields, DateTime? asOf, WorkItemExpand? expand, WorkItemErrorPolicy? errorPolicy, object userState, CancellationToken ct) =>
                ids.Select(id => TestWorkItems.CreateBasicWorkItem(id)).ToList());

        _store = CreateRestStore(_mockClient.Object);
    }

    public override void When()
    {
        _results = _store.Query("SELECT [System.Id] FROM WorkItems");
    }

    [TestMethod]
    public void Then_should_retrieve_all_work_items()
    {
        _results.Count().ShouldBe(100);
    }

    [TestMethod]
    public void Then_should_batch_requests_correctly()
    {
        // Verify GetWorkItemsAsync called with max batch size (200)
        _mockClient.Verify(
            x => x.GetWorkItemsAsync(
                It.Is<IEnumerable<int>>(ids => ids.Count() == 100),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<DateTime?>(),
                It.IsAny<WorkItemExpand?>(),
                It.IsAny<WorkItemErrorPolicy?>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
```

### Phase 2: SOAP Client Unit Tests (Week 3-4)

**Scope**: Windows-only SOAP client validation

**Tasks**:

| ID | Task | Deliverable | Acceptance |
|----|------|-------------|------------|
| S2.1 | Create test class structure | `Given_SOAP_WorkItemStore_*` classes | File structure exists |
| S2.2 | Implement query execution tests | Mock TFS Client OM query methods | Tests pass on Windows |
| S2.3 | Implement work item retrieval tests | Mock `WorkItemCollection` responses | Field access validated |
| S2.4 | Add `SoapUnit` test category | Apply `[TestCategory("SoapUnit")]` attribute | Category defined |
| S2.5 | Update CI filter for SOAP tests | Windows-only execution in CI | Tests run on Windows |
| S2.6 | Document SOAP testing patterns | Add section to `TESTING.md` | Contributors have examples |

**Note**: SOAP priority is lower; defer until REST tests are stable.

### Phase 3: Documentation and Patterns (Ongoing)

**Tasks**:

| ID | Task | Deliverable | Acceptance |
|----|------|-------------|------------|
| D3.1 | Update `TESTING.md` with new categories | Document `RestUnit` and `SoapUnit` | Clear category definitions |
| D3.2 | Add adapter testing guide | Section on mocking SDK types | Contributors have patterns |
| D3.3 | Create example tests | 3-5 representative examples | Copy-paste ready |
| D3.4 | Update `copilot-instructions.md` | Add unit test guidelines | Agents understand patterns |

---

## Testing Strategy

### Test Focus Areas

**Priority 1 (Must Have)**:
1. **Query Execution** - WIQL query handling, parameter passing, result parsing
2. **Work Item Retrieval** - Field access, batch retrieval, revision handling
3. **Error Handling** - Invalid input, SDK exceptions, null handling

**Priority 2 (Should Have)**:
4. **Authentication Flow** - Credential provider setup, connection initialization
5. **Factory Patterns** - Store creation, option validation

**Priority 3 (Nice to Have)**:
6. **Link Handling** - Work item links, attachments
7. **Advanced Queries** - AsOf queries, identity fields

### Test Types

| Test Type | Purpose | Example |
|-----------|---------|---------|
| **State Tests** | Verify correct output for given input | Query returns expected work items |
| **Behavior Tests** | Verify correct actions taken | Factory validates options before creating store |
| **Edge Case Tests** | Verify boundary conditions | Empty query results, null fields |
| **Error Tests** | Verify error handling | Invalid WIQL throws meaningful exception |

### Test Quality Standards

**Required for All Tests**:
- ✅ Fast execution (< 5 seconds per test class)
- ✅ Deterministic (no flaky tests)
- ✅ Isolated (no shared state between tests)
- ✅ Readable (clear Given/When/Then structure)
- ✅ Maintainable (captured stubs can be updated by recapturing traffic)

**Forbidden Practices**:
- ❌ Recording/playback of real Azure DevOps responses
- ❌ Hard-coded credentials or tokens
- ❌ Network calls to external services
- ❌ Shared mutable state between tests
- ❌ Tests that rely on execution order

---

## CI/CD Integration

### Test Category Definitions

**New Categories**:

```csharp
// REST client unit tests (cross-platform)
[TestCategory("RestUnit")]

// SOAP client unit tests (Windows-only)
[TestCategory("SoapUnit")]
```

**Updated Category Summary**:

| Category | Purpose | Platforms | CI Execution |
|----------|---------|-----------|--------------|
| `RestUnit` | REST client unit tests | All | ✅ Always |
| `SoapUnit` | SOAP client unit tests | Windows | ✅ Windows-only |
| `REST` | REST integration tests | All | ❌ Manual only |
| `SOAP` | SOAP integration tests | Windows | ❌ Manual only |
| `IntegrationTests` | Full integration suite | Windows | ❌ Manual only |
| `Benchmark` | Performance tests | All | ❌ Manual only |
| `localOnly` | Local TFS tests | Windows | ❌ Manual only |

### CI Pipeline Changes

**Current Filter** (`.github/workflows/main.yml`):
```powershell
--filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
```

**Updated Filter** (includes new categories):
```powershell
--filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
# RestUnit and SoapUnit are included by default (not excluded)
```

**Platform Matrix Configuration**:

```yaml
# .github/workflows/main.yml
strategy:
  matrix:
    os: [windows-latest, ubuntu-latest, macos-latest]

jobs:
  test:
    runs-on: ${{ matrix.os }}
    steps:
      - name: Run Unit Tests
        shell: pwsh
        run: |
          # Run all unit tests (includes RestUnit on all platforms)
          dotnet test Qwiq.sln --configuration Release --no-build `
            --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests" `
            --logger trx

      - name: Run SOAP Unit Tests (Windows Only)
        if: matrix.os == 'windows-latest'
        shell: pwsh
        run: |
          dotnet test Qwiq.sln --configuration Release --no-build `
            --filter "TestCategory=SoapUnit" `
            --logger trx
```

### Build Performance Targets

| Metric | Current | Target (After Phase 1) |
|--------|---------|------------------------|
| **Total CI Time** | ~5 minutes | ~5 minutes (unchanged) |
| **Unit Test Time** | ~10 seconds | ~15 seconds (+5s for REST unit tests) |
| **REST Unit Tests** | 0 | 30-50 tests |
| **Platform Coverage** | Windows only | Windows, Linux, macOS |

---

## Dependencies and Prerequisites

### Technical Dependencies

| Dependency | Version | Purpose | Availability |
|------------|---------|---------|--------------|
| **Moq** | 4.x | Mocking framework | ✅ Already in use |
| **Shouldly** | 4.x | Assertion library | ✅ Already in use |
| **MSTest** | 3.x | Test framework | ✅ Already in use |
| **.NET SDK** | 8.0 | Build tooling | ✅ Pinned in `global.json` |

### Project Prerequisites

| Prerequisite | Status | Notes |
|--------------|--------|-------|
| **SDK-Style Projects** | ✅ Complete | Migrated in Wave 0 |
| **Central Package Management** | ✅ Complete | `Directory.Packages.props` |
| **InternalsVisibleTo** | ✅ Configured | Test projects have access |
| **Nullable Annotations** | ✅ Complete (REST) | REST client fully annotated |
| **GitHub Actions CI** | ✅ Configured | `.github/workflows/main.yml` |

### Knowledge Prerequisites

**Contributors Need**:
- ✅ Basic understanding of mocking with Moq
- ✅ Familiarity with ContextSpecification pattern
- ✅ Understanding of Azure DevOps work item concepts (query, field, revision)

**Not Required**:
- ❌ Azure DevOps credentials or access
- ❌ TFS on-premises setup
- ❌ Deep knowledge of VSS SDK internals

---

## Risks and Mitigations

### Risk 1: Mock Drift from Real SDK Behavior

**Risk**: Mocked SDK responses diverge from actual SDK behavior, causing unit tests to pass but integration tests to fail.

**Likelihood**: Medium | **Impact**: High

**Mitigation**:
1. Retain integration tests as source of truth for SDK behavior
2. Review SDK release notes for breaking changes
3. Cross-validate unit test assumptions against integration tests
4. Document known SDK quirks in test comments

**Detection**: Integration tests will catch behavioral mismatches

### Risk 2: Over-Mocking (Testing the Mock)

**Risk**: Tests become too coupled to Moq setup, testing mock configuration rather than adapter logic.

**Likelihood**: Medium | **Impact**: Medium

**Mitigation**:
1. Focus tests on **output validation** (state testing) not call verification
2. Use `Mock.Verify()` sparingly, only for critical side effects
3. Code review guideline: "Is this testing the mock or the adapter?"
4. Document anti-patterns in `TESTING.md`

**Example Anti-Pattern**:
```csharp
// BAD: Testing the mock
[TestMethod]
public void Should_call_GetWorkItemsAsync()
{
    _mockClient.Verify(x => x.GetWorkItemsAsync(It.IsAny<IEnumerable<int>>()), Times.Once);
}

// GOOD: Testing the behavior
[TestMethod]
public void Should_return_work_items_with_correct_fields()
{
    var workItems = _store.Query("SELECT [System.Id] FROM WorkItems");
    workItems.First().Id.ShouldBe(1);
    workItems.First().Title.ShouldBe("Expected Title");
}
```

### Risk 3: Incomplete SDK Coverage

**Risk**: Not all SDK APIs are mockable (sealed classes, static methods), limiting test coverage.

**Likelihood**: Low | **Impact**: Medium

**Mitigation**:
1. Identify unmockable types during Phase 1 planning
2. Consider adapter refactoring if critical paths are unmockable
3. Document gaps in test coverage explicitly
4. Fall back to integration tests for unmockable scenarios

**Known Constraints**:
- `WorkItemTrackingHttpClient` is mockable (virtual methods)
- TFS Client OM has some sealed types (assess during Phase 2)

### Risk 4: Windows-Only SOAP Testing

**Risk**: SOAP unit tests only run on Windows, limiting contributor access.

**Likelihood**: High | **Impact**: Low

**Mitigation**:
1. Prioritize REST tests (cross-platform)
2. SOAP is maintenance-only mode (fewer changes)
3. Document Windows requirement clearly in `TESTING.md`
4. Provide cloud-based Windows testing options (GitHub Codespaces)

**Acceptance**: Acceptable trade-off given SOAP's maintenance-only status

### Risk 5: Contributor Friction (New Patterns)

**Risk**: Contributors unfamiliar with mocking patterns struggle to write tests.

**Likelihood**: Medium | **Impact**: Medium

**Mitigation**:
1. Provide 3-5 comprehensive test examples in `TESTING.md`
2. Document "copy-paste ready" test templates
3. Code review feedback on test quality
4. Incremental adoption (existing tests remain valid)

**Success Indicator**: Contributors submit PRs with unit tests without guidance

---

## Success Criteria

### Definition of Done: Phase 1 (REST)

**Must Have**:
- ✅ 30-50 REST unit tests covering query execution, work item retrieval, field access, authentication
- ✅ All tests pass locally on Windows, Linux, macOS
- ✅ All tests pass in CI on Windows, Linux, macOS
- ✅ Tests run in < 5 seconds per test class
- ✅ `RestUnit` test category defined and documented
- ✅ CI filter updated to include `RestUnit` tests
- ✅ Cross-platform CI matrix configured

**Should Have**:
- ✅ `TESTING.md` updated with REST testing patterns and examples
- ✅ Code coverage for critical REST paths (query, retrieve, field access)
- ✅ Zero failures on first CI run

**Nice to Have**:
- ✅ Mocking helper utilities for common SDK responses
- ✅ WireMock stubs with captured Azure DevOps API responses

### Quality Gates

**Required for PR Approval**:
1. All new REST/SOAP code has accompanying unit tests
2. Unit tests follow ContextSpecification pattern
3. No hard-coded credentials or real Azure DevOps data
4. Tests are deterministic (no flaky tests)
5. CI passes on all platforms (REST) or Windows (SOAP)

### Metrics

| Metric | Baseline | Phase 1 Target | Measurement |
|--------|----------|----------------|-------------|
| **REST Unit Test Count** | 0 | 30-50 | Test count in `Qwiq.Core.Tests` |
| **SOAP Unit Test Count** | 0 | 20-30 (Phase 2) | Test count in `Qwiq.Core.Tests` |
| **CI Execution Time** | ~10s | ~15s | CI logs |
| **Platform Coverage** | Windows | Windows, Linux, macOS | CI matrix runs |
| **Contributor Friction** | High (requires Azure DevOps) | Low (local-only) | Contributor feedback |

---

## Estimated Effort

### Phase 1: REST Client Unit Tests

| Task | Estimated Effort | Assignee Type |
|------|------------------|---------------|
| **R1.1-R1.6**: Implement REST tests | 8-12 hours | Intermediate developer |
| **R1.7-R1.9**: CI integration | 2-4 hours | Maintainer |
| **R1.10**: Documentation | 2-3 hours | Maintainer |
| **Total Phase 1** | **12-19 hours** | ~2-3 days |

### Phase 2: SOAP Client Unit Tests

| Task | Estimated Effort | Assignee Type |
|------|------------------|---------------|
| **S2.1-S2.3**: Implement SOAP tests | 6-10 hours | Intermediate developer (Windows) |
| **S2.4-S2.5**: CI integration | 1-2 hours | Maintainer |
| **S2.6**: Documentation | 1-2 hours | Maintainer |
| **Total Phase 2** | **8-14 hours** | ~1-2 days |

### Phase 3: Documentation

| Task | Estimated Effort | Assignee Type |
|------|------------------|---------------|
| **D3.1-D3.4**: Documentation updates | 2-4 hours | Maintainer |

### Total Estimated Effort

**Phase 1 (REST)**: 12-19 hours (~2-3 days)
**Phase 2 (SOAP)**: 8-14 hours (~1-2 days)
**Phase 3 (Docs)**: 2-4 hours (~0.5 days)

**Total**: **22-37 hours** (~4-6 days elapsed time for a single developer)

### Assumptions

- Developer has experience with Moq and MSTest
- Developer has local Qwiq development environment setup
- Code reviews are timely (< 1 day turnaround)
- No major SDK compatibility issues discovered

---

## Open Questions

**None.** All ambiguities resolved during clarifying questions phase.

### Assumptions Made

1. **Existing Test Infrastructure**: `Qwiq.Core.Tests` project is the correct location for REST/SOAP unit tests
2. **Moq Compatibility**: `WorkItemTrackingHttpClient` and TFS Client OM types are mockable via Moq
3. **SDK Stability**: Microsoft SDKs will not introduce breaking changes during implementation
4. **Contributor Buy-In**: Contributors will adopt unit testing patterns when documented
5. **CI Runner Availability**: GitHub Actions provides sufficient Windows, Linux, macOS runner capacity

---

## Appendix: Example Test Classes

### Example 1: REST Query Execution

```csharp
[TestClass]
[TestCategory("RestUnit")]
public class Given_REST_query_with_valid_WIQL : ContextSpecification
{
    private Mock<WorkItemTrackingHttpClient> _mockClient;
    private IWorkItemStore _store;
    private IEnumerable<IWorkItem> _results;

    public override void Given()
    {
        _mockClient = CreateMockRestClient();

        _mockClient
            .Setup(x => x.QueryByWiqlAsync(
                It.Is<Wiql>(w => w.Query == "SELECT [System.Id] FROM WorkItems WHERE [System.State] = 'Active'"),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WorkItemQueryResult
            {
                WorkItems = new[]
                {
                    new WorkItemReference { Id = 1 },
                    new WorkItemReference { Id = 2 },
                    new WorkItemReference { Id = 3 }
                }
            });

        _mockClient
            .Setup(x => x.GetWorkItemsAsync(
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<DateTime?>(),
                It.IsAny<WorkItemExpand?>(),
                It.IsAny<WorkItemErrorPolicy?>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<int> ids, IEnumerable<string> fields, DateTime? asOf, WorkItemExpand? expand, WorkItemErrorPolicy? errorPolicy, object userState, CancellationToken ct) =>
                ids.Select(id => TestWorkItems.CreateActiveWorkItem(id)).ToList());

        _store = new WorkItemStore(_mockClient.Object);
    }

    public override void When()
    {
        _results = _store.Query("SELECT [System.Id] FROM WorkItems WHERE [System.State] = 'Active'");
    }

    [TestMethod]
    public void Then_should_return_three_work_items()
    {
        _results.Count().ShouldBe(3);
    }

    [TestMethod]
    public void Then_all_work_items_should_be_active()
    {
        _results.All(wi => wi.State == "Active").ShouldBeTrue();
    }
}
```

### Example 2: REST Field Access

```csharp
[TestClass]
[TestCategory("RestUnit")]
public class Given_REST_work_item_with_custom_fields : ContextSpecification
{
    private Mock<WorkItemTrackingHttpClient> _mockClient;
    private IWorkItem _workItem;

    public override void Given()
    {
        _mockClient = CreateMockRestClient();

        var mockWorkItem = new WorkItem
        {
            Id = 1,
            Fields = new Dictionary<string, object>
            {
                ["System.Id"] = 1,
                ["System.Title"] = "Custom Field Test",
                ["System.WorkItemType"] = "Bug",
                ["Custom.Priority"] = "High",
                ["Custom.Severity"] = 3
            }
        };

        _mockClient
            .Setup(x => x.GetWorkItemAsync(
                It.Is<int>(id => id == 1),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<DateTime?>(),
                It.IsAny<WorkItemExpand?>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockWorkItem);

        var store = new WorkItemStore(_mockClient.Object);
        _workItem = store.GetWorkItem(1);
    }

    [TestMethod]
    public void Then_should_access_custom_string_field()
    {
        _workItem.GetField<string>("Custom.Priority").ShouldBe("High");
    }

    [TestMethod]
    public void Then_should_access_custom_int_field()
    {
        _workItem.GetField<int>("Custom.Severity").ShouldBe(3);
    }

    [TestMethod]
    public void Then_should_return_null_for_missing_field()
    {
        _workItem.GetField<string>("Custom.NonExistent").ShouldBeNull();
    }
}
```

### Example 3: REST Error Handling

```csharp
[TestClass]
[TestCategory("RestUnit")]
public class Given_REST_query_with_invalid_WIQL : ContextSpecification
{
    private Mock<WorkItemTrackingHttpClient> _mockClient;
    private IWorkItemStore _store;
    private Exception _exception;

    public override void Given()
    {
        _mockClient = CreateMockRestClient();

        _mockClient
            .Setup(x => x.QueryByWiqlAsync(
                It.IsAny<Wiql>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new VssServiceException("Invalid WIQL syntax"));

        _store = new WorkItemStore(_mockClient.Object);
    }

    public override void When()
    {
        _exception = Record.Exception(() =>
            _store.Query("INVALID WIQL SYNTAX"));
    }

    [TestMethod]
    public void Then_should_throw_meaningful_exception()
    {
        _exception.ShouldNotBeNull();
        _exception.Message.ShouldContain("Invalid WIQL syntax");
    }
}
```

---

## References

- **Repository**: [Qwiq on GitHub](https://github.com/rjmurillo/Qwiq)
- **Current Test Structure**: See `copilot-instructions.md` → Test Configuration section
- **Mocking Framework**: [Moq Documentation](https://github.com/moq/moq4)
- **Assertion Library**: [Shouldly Documentation](https://github.com/shouldly/shouldly)
- **Azure DevOps SDK**: [Microsoft.VisualStudio.Services.Client](https://www.nuget.org/packages/Microsoft.VisualStudio.Services.Client/)
- **TFS Client OM**: [Microsoft.TeamFoundationServer.ExtendedClient](https://www.nuget.org/packages/Microsoft.TeamFoundationServer.ExtendedClient/)

---

**Document Version**: 1.0
**Approved By**: [Pending]
**Next Review Date**: After Phase 1 Completion
