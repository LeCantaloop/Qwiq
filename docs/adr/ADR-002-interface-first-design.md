# ADR-002: Interface-First Design

- **Status**: Accepted
- **Date**: 2025-12-06
- **Decision Makers**: Qwiq Development Team
- **Supersedes**: None
- **Superseded by**: None

## Context

Qwiq is a library that wraps the Azure DevOps / TFS work item tracking APIs. The underlying TFS Client OM uses concrete classes that are difficult to mock and test. Additionally, Qwiq supports multiple backend implementations (REST, SOAP) and needs to provide a consistent API surface.

### Problem Statement

How can we design an API that:

1. Is easy to mock and test without Azure DevOps connectivity
2. Abstracts away differences between REST and SOAP implementations
3. Maintains type safety and compile-time checking
4. Follows SOLID principles, particularly dependency inversion
5. Allows future extensibility without breaking changes

### Forces

- **Testability**: Unit tests should not require Azure DevOps infrastructure
- **Abstraction**: Hide implementation details from consumers
- **Type Safety**: Leverage C# strong typing
- **Backward Compatibility**: Changes should not break existing consumers
- **Performance**: Abstraction overhead should be minimal

## Decision

We will design Qwiq with an **interface-first approach** where all public types are defined as interfaces, and consumers interact exclusively with these interfaces rather than concrete implementations.

### Core Interfaces

```csharp
// Core work item tracking
public interface IWorkItemStore { }
public interface IWorkItem { }
public interface IRevision { }
public interface IFieldDefinition { }
public interface IAttachment { }
public interface ILink { }

// Identity management
public interface IIdentityManagementService { }
public interface ITeamFoundationIdentity { }

// Collections
public interface IFieldDefinitionCollection : IReadOnlyList<IFieldDefinition> { }
public interface IRevisionCollection : IReadOnlyList<IRevision> { }
```

### Implementation Pattern

```csharp
// Interface (public)
public interface IWorkItem
{
    int Id { get; }
    string Title { get; }
    IFieldDefinitionCollection Fields { get; }
    IRevisionCollection Revisions { get; }
}

// Implementation (internal)
internal sealed class RestWorkItem : IWorkItem
{
    // REST-specific implementation
}

internal sealed class SoapWorkItem : IWorkItem
{
    // SOAP-specific implementation
}

// Mock (test assembly)
public sealed class MockWorkItem : IWorkItem
{
    // In-memory test implementation
}
```

### Usage Pattern

```csharp
// Consumer code always uses interfaces
IWorkItemStore store = WorkItemStoreFactory.Default.Create(options);
IEnumerable<IWorkItem> items = store.Query("SELECT [System.Id] FROM WorkItems");

foreach (IWorkItem item in items)
{
    Console.WriteLine($"{item.Id}: {item.Title}");
}

// Testing with mocks
var mockStore = new MockWorkItemStore();
mockStore.Add(new MockWorkItem { Id = 1, Title = "Test" });
```

## Consequences

### Positive

1. **Testability**: Easy to create mocks, no Azure DevOps dependency
2. **Flexibility**: Can swap REST/SOAP/Mock implementations transparently
3. **Abstraction**: Consumers don't depend on Azure DevOps SDK types
4. **SOLID Compliance**:
   - Interface Segregation Principle (focused interfaces)
   - Dependency Inversion Principle (depend on abstractions)
5. **Future-Proof**: New implementations don't break existing code
6. **Clean API**: Hides TFS Client OM complexity

### Negative

1. **Indirection**: One extra layer between consumer and implementation
2. **Maintenance**: Interface and implementation must stay in sync
3. **IDE Navigation**: "Go to Definition" shows interface, not implementation
4. **Verbosity**: More types to maintain (interface + implementation)

### Trade-offs

- **Performance**: Minimal overhead (interface dispatch is cheap in .NET)
- **Discoverability**: Interfaces make API surface clearer, but hide implementation details
- **Coupling**: Loose coupling (good), but requires disciplined interface design

### Risks

- **Breaking Changes**: Interface modifications are breaking for consumers
  - _Mitigation_: Use semantic versioning, extensive testing, API compatibility analyzers
- **Interface Bloat**: Too many small interfaces can be confusing
  - _Mitigation_: Design cohesive interfaces, use interface segregation judiciously

## Alternatives Considered

### Alternative 1: Abstract Classes

```csharp
// ❌ Rejected
public abstract class WorkItemBase
{
    public abstract int Id { get; }
    public virtual string GetFieldValue(string fieldName) { }
}
```

**Rejected because:**

- Single inheritance limitation in C#
- Harder to compose behaviors
- More coupling than interfaces

### Alternative 2: Concrete Classes with Virtual Methods

```csharp
// ❌ Rejected
public class WorkItem
{
    public virtual int Id { get; set; }
    public virtual string Title { get; set; }
}
```

**Rejected because:**

- Exposes implementation details
- Harder to test without subclassing
- Breaks encapsulation

### Alternative 3: Expose TFS Client OM Directly

```csharp
// ❌ Rejected
public Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem GetWorkItem(int id);
```

**Rejected because:**

- Tight coupling to Microsoft SDK
- Impossible to unit test without TFS
- Windows-only (SOAP client limitation)
- Breaking changes when Microsoft updates SDK

## Implementation Guidelines

### DO

- ✅ Define all public types as interfaces
- ✅ Mark implementations as `internal sealed`
- ✅ Use covariant/contravariant generics where appropriate (`out T`, `in T`)
- ✅ Provide mock implementations in `Qwiq.Mocks` assembly
- ✅ Document interface contracts with XML comments

### DON'T

- ❌ Expose concrete types in public API
- ❌ Use abstract classes instead of interfaces (unless inheritance is truly needed)
- ❌ Let Microsoft SDK types leak into public API
- ❌ Create "marker interfaces" with no members (use attributes instead)

## Related Decisions

- [ADR-001: Factory Pattern for WorkItemStore](ADR-001-factory-pattern-workitemstore.md) - Explains how interfaces are instantiated
- [ADR-003: REST vs SOAP Strategy](ADR-003-rest-vs-soap-strategy.md) - Multiple implementations necessitate interface abstraction

## References

- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Dependency Inversion Principle](https://en.wikipedia.org/wiki/Dependency_inversion_principle)
- [Interface-based Programming](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/)
- [Moq - Mocking Interfaces](https://github.com/moq/moq4)

## Revision History

| Date       | Author        | Changes                                                  |
| ---------- | ------------- | -------------------------------------------------------- |
| 2025-12-06 | Copilot Agent | Initial ADR documenting interface-first design principle |
