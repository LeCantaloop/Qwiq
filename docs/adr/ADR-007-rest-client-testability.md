# ADR-007: REST Client Testability via Dependency Injection

**Status**: Accepted  
**Date**: 2025-12-06  
**Deciders**: @rjmurillo  
**Context Tags**: Testing, Architecture, REST Client

---

## Context and Problem Statement

The Qwiq REST client (`Qwiq.Client.Rest`) is currently difficult to unit test without live Azure DevOps connectivity. The client is tightly coupled to the Azure DevOps SDK's `VssConnection` class, which makes direct HTTP calls to Azure DevOps services. This coupling creates several challenges:

1. **No URL Injection**: The `WorkItemStoreFactory` creates `VssConnection` instances directly with the configured endpoint URL, preventing test frameworks like WireMock.Net from intercepting HTTP traffic
2. **Static Singleton Pattern**: `TfsConnectionFactory.Default` uses a singleton pattern that doesn't expose hooks for test configuration
3. **CI/CD Friction**: All REST client tests currently require live Azure DevOps connectivity (marked with `TestCategory=REST`), making them unsuitable for CI pipelines
4. **Contributor Barriers**: New contributors cannot run REST client tests without Azure DevOps credentials

This issue became apparent during **W2.16 Phase 1** (REST Unit Test Coverage) when attempting to create unit tests with WireMock.Net.

---

## Decision Drivers

### Functional Requirements

- Enable fast, isolated unit testing of REST client logic
- Allow contributors to run tests without Azure DevOps access
- Support HTTP mocking frameworks (WireMock.Net) for deterministic testing
- Maintain existing public API compatibility where possible

### Quality Attributes

- **Testability**: Primary driver - must enable comprehensive unit test coverage
- **Maintainability**: Solution should be clean and follow established patterns
- **Backward Compatibility**: Minimize breaking changes to public API

### Constraints

- Azure DevOps SDK (`Microsoft.VisualStudio.Services.Client`) is a third-party dependency we cannot modify
- `VssConnection` class is sealed and cannot be mocked with traditional mocking frameworks
- Existing integration tests must continue to work

---

## Considered Options

### Option 1: Mock VssConnection with Moq

**Approach**: Use Moq to create test doubles for `VssConnection` and `WorkItemTrackingHttpClient`.

**Pros**:

- No changes to production code required
- Standard mocking approach

**Cons**:

- `VssConnection` is sealed - cannot be mocked directly
- Would require wrapping every SDK class with interfaces
- Complex due to async methods and internal SDK dependencies
- High maintenance burden as SDK evolves

**Verdict**: ❌ **Rejected** - Too complex, fragile, and high maintenance cost

---

### Option 2: HTTP Proxy with WireMock.Net

**Approach**: Configure `VssConnection.Settings` to route all HTTP traffic through WireMock server acting as a proxy.

**Pros**:

- No changes to factory pattern required
- Tests actual HTTP layer

**Cons**:

- WireMock.Net proxy mode has limitations with HTTPS and authentication
- Requires modifying `VssConnectionAdapter` to expose settings
- Complex setup for each test
- Doesn't test the adapter logic in isolation

**Verdict**: ❌ **Rejected** - Overly complex, doesn't achieve true unit testing

---

### Option 3: Refactor for Dependency Injection (RECOMMENDED)

**Approach**: Introduce internal abstractions and allow constructor injection of dependencies for testing.

#### 3a: Factory Method Pattern with Optional Parameters

```csharp
public class WorkItemStoreFactory : Qwiq.WorkItemStoreFactory
{
    // Existing public API - no breaking changes
    public override IWorkItemStore Create(AuthenticationOptions options)
    {
        return Create(options, TfsConnectionFactory.Default);
    }

    // New internal overload for testing
    internal IWorkItemStore Create(
        AuthenticationOptions options,
        ITfsConnectionFactory connectionFactory)
    {
        var tfsProxy = (IInternalTeamProjectCollection)connectionFactory.Create(options);
        var wis = CreateRestWorkItemStore(tfsProxy);
        return ExceptionHandlingDynamicProxyFactory.Create(wis);
    }
}
```

**Test Usage**:

```csharp
[TestMethod]
[TestCategory("RestUnit")]
public void Should_Query_Work_Items()
{
    // Arrange
    var mockConnectionFactory = new Mock<ITfsConnectionFactory>();
    mockConnectionFactory
        .Setup(f => f.Create(It.IsAny<AuthenticationOptions>()))
        .Returns(CreateMockConnection(wireMockServer.Url));

    var factory = WorkItemStoreFactory.Default;
    var store = factory.Create(testOptions, mockConnectionFactory.Object);

    // Act
    var results = store.Query("SELECT [System.Id] FROM WorkItems");

    // Assert
    results.ShouldNotBeEmpty();
}
```

**Pros**:

- ✅ No breaking changes to public API
- ✅ Clean separation of concerns
- ✅ Enables true unit testing with mocks
- ✅ Follows existing factory pattern
- ✅ Test-specific code is internal, not exposed to consumers

**Cons**:

- Requires refactoring existing factory classes
- Adds one internal overload method

#### 3b: Builder Pattern (Alternative)

```csharp
public class WorkItemStoreBuilder
{
    private Uri? _baseUri;
    private VssCredentials? _credentials;
    private ITfsConnectionFactory? _connectionFactory;

    public WorkItemStoreBuilder WithBaseUri(Uri uri) { _baseUri = uri; return this; }
    public WorkItemStoreBuilder WithCredentials(VssCredentials creds) { _credentials = creds; return this; }
    internal WorkItemStoreBuilder WithConnectionFactory(ITfsConnectionFactory factory)
    {
        _connectionFactory = factory;
        return this;
    }

    public IWorkItemStore Build()
    {
        var factory = _connectionFactory ?? TfsConnectionFactory.Default;
        // ... rest of build logic
    }
}
```

**Pros**:

- Fluent API for test configuration
- Explicit about what's being configured

**Cons**:

- More code to maintain
- Different pattern than existing factory approach

**Verdict**: ✅ **RECOMMENDED** - Option 3a (Factory Method with Internal Overload)

---

### Option 4: Integration Tests Only (Current State)

**Approach**: Accept that REST client tests require live Azure DevOps connectivity.

**Pros**:

- No code changes required
- Tests validate real integration

**Cons**:

- ❌ High barrier to entry for contributors
- ❌ Slow feedback loops (network latency)
- ❌ Cannot run in standard CI without credentials
- ❌ Flaky tests due to external dependencies

**Verdict**: ❌ **Rejected** - Does not meet W2.16 objectives

---

## Decision

**Adopt Option 3a: Factory Method Pattern with Internal Overload**

### Implementation Plan

#### Phase 1: Foundation (W2.16 continuation)

1. ✅ Add WireMock.Net, Moq, and Moq.Analyzers packages _(completed)_
2. ✅ Create test project structure _(completed)_
3. Refactor `WorkItemStoreFactory`:
   - Add internal `Create` overload accepting `ITfsConnectionFactory`
   - Existing public API calls internal method with `TfsConnectionFactory.Default`
4. Create mock implementations:
   - `MockTfsConnectionFactory` - returns mock `IInternalTeamProjectCollection`
   - `MockWorkItemTrackingHttpClient` wrapper (if needed)

#### Phase 2: Test Implementation

5. Implement REST unit tests:
   - Query execution (WIQL parsing, result mapping)
   - Work item retrieval by ID
   - Work item collections
   - Field definitions and types
   - Link type handling
6. Add `[TestCategory("RestUnit")]` to all REST unit tests
7. Update CI to run `TestCategory=RestUnit` on all platforms

#### Phase 3: SOAP Tests (Future)

8. Apply same pattern to `Qwiq.Client.Soap.WorkItemStoreFactory`
9. Create SOAP unit tests with Moq (Windows-only, `TestCategory=SoapUnit`)

---

## Consequences

### Positive

- ✅ **Testability**: Enables comprehensive unit test coverage without Azure DevOps dependency
- ✅ **Contributor Experience**: New contributors can run all unit tests locally
- ✅ **CI Performance**: Fast, reliable tests in every build
- ✅ **Backward Compatibility**: Public API unchanged
- ✅ **Maintainability**: Clean separation between production and test infrastructure

### Negative

- ⚠️ **Refactoring Effort**: Requires changes to factory classes (estimated 4-8 hours)
- ⚠️ **Test Infrastructure**: Need to create and maintain mock implementations
- ⚠️ **Internal API Surface**: Adds internal overloads (acceptable - tests are in same assembly via `InternalsVisibleTo`)

### Neutral

- Integration tests remain necessary to validate real Azure DevOps connectivity
- Mock complexity scales with the number of SDK types used (manageable with Moq)

---

## Validation

### Success Criteria

- [ ] REST client unit tests run without Azure DevOps connectivity
- [ ] All tests pass in CI (Linux, Windows, macOS)
- [ ] Test execution time < 5 seconds for REST unit test suite
- [ ] No breaking changes to public API
- [ ] Code coverage for REST client > 70%

### Testing Checklist

- [ ] Existing integration tests continue to pass
- [ ] New unit tests cover critical paths (query, retrieval, field mapping)
- [ ] Unit tests are fast (<100ms per test)
- [ ] Unit tests are deterministic (no flakiness)

---

## References

- **W2.16**: REST/SOAP Unit Test Coverage (modernize-TODO.md)
- **PRD**: [explainer-rest-soap-unit-tests.md](../.agents/explainer-rest-soap-unit-tests.md)
- **Related ADRs**:
  - ADR-001: Factory Pattern for Work Item Store Creation
  - ADR-003: REST and SOAP Strategy Pattern
- **External Resources**:
  - [WireMock.Net Documentation](https://github.com/WireMock-Net/WireMock.Net/wiki)
  - [Moq 4 Quick Start](https://github.com/devlooped/moq/wiki/Quickstart)
  - [Azure DevOps SDK](https://github.com/microsoft/azure-devops-dotnet-sdk)

---

## Notes

- This ADR was created during Session 20 (Phase 2C) when W2.16 Phase 1 infrastructure work revealed the testability limitation
- The decision to use internal overloads (Option 3a) balances testability with API stability
- Future enhancements could include a public builder API if external consumers request it
- Consider applying this pattern to other tightly-coupled SDK adapters in the codebase
