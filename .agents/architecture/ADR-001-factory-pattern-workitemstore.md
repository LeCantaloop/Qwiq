# ADR-001: Factory Pattern for WorkItemStore

- **Status**: Accepted
- **Date**: 2025-12-06
- **Decision Makers**: Qwiq Development Team
- **Supersedes**: None
- **Superseded by**: None

## Context

Qwiq supports multiple backends for accessing work items from Azure DevOps / Team Foundation Server:

- REST API (modern, cross-platform)
- SOAP API (legacy, Windows-only via TFS Client OM)

Users need a consistent way to create `IWorkItemStore` instances regardless of which backend they're using. The concrete implementation details (REST vs SOAP, authentication mechanisms, connection management) should be hidden from consumers.

### Problem Statement

How can we provide a flexible, testable API for creating work item store instances while:

1. Hiding implementation complexity from consumers
2. Supporting multiple backends (REST, SOAP)
3. Enabling dependency injection scenarios
4. Allowing mocking for unit tests
5. Maintaining backward compatibility

### Forces

- **Abstraction**: Consumers should depend on `IWorkItemStore` interface, not concrete types
- **Flexibility**: Support multiple authentication methods and connection options
- **Testability**: Enable easy mocking without requiring actual Azure DevOps connectivity
- **Discoverability**: Clear, intuitive API for creating store instances
- **Extensibility**: Allow future backend implementations without breaking changes

## Decision

We will use the **Abstract Factory Pattern** with static factory methods to create work item store instances.

### Implementation

Each client library provides a `WorkItemStoreFactory` class:

```csharp
// REST client
public sealed class WorkItemStoreFactory : IWorkItemStoreFactory
{
    public static readonly WorkItemStoreFactory Default = new();

    public IWorkItemStore Create(AuthenticationOptions options)
    {
        // Create REST-based implementation
    }
}

// SOAP client
public sealed class WorkItemStoreFactory : IWorkItemStoreFactory
{
    public static readonly WorkItemStoreFactory Default = new();

    public IWorkItemStore Create(AuthenticationOptions options)
    {
        // Create SOAP-based implementation
    }
}
```

### Usage Pattern

```csharp
// Consumer code
var options = new AuthenticationOptions(
    new Uri("https://dev.azure.com/org"),
    AuthenticationTypes.PersonalAccessToken,
    credentialsFactory
);

// REST client
IWorkItemStore store = Qwiq.Client.Rest.WorkItemStoreFactory.Default.Create(options);

// SOAP client
IWorkItemStore store = Qwiq.Client.Soap.WorkItemStoreFactory.Default.Create(options);

// Mocking for tests
IWorkItemStore store = new MockWorkItemStore();
```

### Benefits

1. **Single Responsibility**: Factory handles only object creation
2. **Dependency Inversion**: Consumers depend on `IWorkItemStore` abstraction
3. **Open/Closed**: New factories can be added without modifying existing code
4. **Testability**: Easy to inject mock implementations
5. **Discoverability**: `WorkItemStoreFactory.Default.Create()` is intuitive

## Consequences

### Positive

- Clear separation between construction and usage
- Testable code without Azure DevOps dependency
- Consistent API across REST and SOAP implementations
- Easy to extend with new backends (e.g., local file storage, in-memory)
- Supports dependency injection via `IWorkItemStoreFactory` interface

### Negative

- Slight indirection adds one extra layer to understand
- Factory classes must be kept in sync with `IWorkItemStore` interface changes
- Static factory pattern doesn't work well with some DI containers (mitigated by `IWorkItemStoreFactory` interface)

### Risks

- **Breaking Changes**: Changes to `AuthenticationOptions` could impact all consumers
  - _Mitigation_: Use optional parameters, obsolete old signatures before removal
- **Factory Proliferation**: Too many factory variants could confuse users
  - _Mitigation_: Keep single `Default` factory per client, document clearly

## Alternatives Considered

### Alternative 1: Direct Constructor

```csharp
// ❌ Rejected
var store = new RestWorkItemStore(options);
```

**Rejected because:**

- Exposes concrete types to consumers
- Harder to swap implementations
- Breaks dependency inversion principle

### Alternative 2: Service Locator

```csharp
// ❌ Rejected
var store = ServiceLocator.Resolve<IWorkItemStore>();
```

**Rejected because:**

- Considered anti-pattern in modern C# development
- Hides dependencies, makes testing harder
- Requires global service registry

### Alternative 3: Builder Pattern

```csharp
// ❌ Rejected
var store = new WorkItemStoreBuilder()
    .WithUrl(url)
    .WithAuthentication(authType)
    .Build();
```

**Rejected because:**

- Overkill for simple object creation
- More code for consumers to write
- Factory pattern is more discoverable

## Related Decisions

- [ADR-002: Interface-First Design](ADR-002-interface-first-design.md) - Defines why `IWorkItemStore` interface exists
- [ADR-003: REST vs SOAP Strategy](ADR-003-rest-vs-soap-strategy.md) - Explains why we need multiple factories

## References

- [Factory Method Pattern](https://refactoring.guru/design-patterns/factory-method)
- [Abstract Factory Pattern](https://refactoring.guru/design-patterns/abstract-factory)
- [Dependency Injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

## Revision History

| Date       | Author        | Changes                                          |
| ---------- | ------------- | ------------------------------------------------ |
| 2025-12-06 | Copilot Agent | Initial ADR documenting existing factory pattern |
