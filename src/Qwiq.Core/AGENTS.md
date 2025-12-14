# Qwiq.Core Component Guide

## Component Overview

**Qwiq.Core** is the foundational library providing core interfaces, abstractions, and base implementations for the QWIQ work item query system. All other components depend on this package.

## Purpose

- Define public interfaces for work items, queries, stores, and identity
- Provide base implementations and common utilities
- Establish field reference name constants and core types
- Support both REST and SOAP client implementations

## Key Characteristics

- **Target Frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
- **Platform**: Cross-platform (except SOAP-specific features)
- **Public API**: Strictly controlled via PublicApiAnalyzers
- **Packaging**: Ships as NuGet package with API compatibility validation

## Architecture

### Core Interfaces

| Interface                 | Purpose                                    | Key Methods/Properties          |
| ------------------------- | ------------------------------------------ | ------------------------------- |
| `IWorkItemStore`          | Work item storage and querying             | `Query()`, `GetWorkItem()`      |
| `IWorkItem`               | Single work item with fields and revisions | `Id`, `Fields`, `Revisions`     |
| `IRevision`               | Historical snapshot of work item state     | `Index`, `Fields`               |
| `IFieldDefinition`        | Metadata about a field type                | `ReferenceName`, `Name`, `Type` |
| `IQuery`                  | Query execution and iteration              | `RunQuery()`, `WorkItems`       |
| `ITeamFoundationIdentity` | User/group identity information            | `DisplayName`, `UniqueName`     |

### Directory Structure

- **Root**: Interface definitions and base implementations
- **Compatibility/**: Polyfills for older target frameworks
- **Credentials/**: Authentication and credential providers
- **Exceptions/**: Custom exception types

### Field Reference Names

Use constants from `CoreFieldRefNames` for well-known fields:

```csharp
// ✅ CORRECT: Use constants
var title = workItem[CoreFieldRefNames.Title];

// ❌ WRONG: Magic strings
var title = workItem["System.Title"];
```

## Key Entry Points

| Type                            | Purpose                        | When to Use                          |
| ------------------------------- | ------------------------------ | ------------------------------------ |
| `WorkItemStoreFactory`          | Create work item store         | Starting point for all queries       |
| `CoreFieldRefNames`             | Field reference name constants | Accessing standard work item fields  |
| `CoreLinkTypeEndReferenceNames` | Link type constants            | Working with work item relationships |
| `IWorkItemStore`                | Main query interface           | Implementing client adapters         |

## Design Patterns

### Factory Pattern

All work item stores are created via `IWorkItemStoreFactory`:

```csharp
var store = WorkItemStoreFactory.Default.Create(options);
```

### Interface-Based Design

All types expose interfaces to support:

- Dependency injection
- Unit testing with mocks (`Qwiq.Mocks`)
- Multiple implementations (REST, SOAP)

### Revision Pattern

Work items track history through revisions:

```csharp
var currentValue = workItem[fieldName];
var historicalValue = workItem.Revisions[3][fieldName];
```

## Testing Guidelines

### Use Qwiq.Mocks

```csharp
var store = new MockWorkItemStore();
store.Add(new MockWorkItem("Bug") { Title = "Test" });
```

### Test Interface Contracts

When changing interfaces, verify:

- Both REST and SOAP implementations still compile
- Mock implementations in `Qwiq.Mocks` are updated
- Public API files are updated (run `dotnet format`)

## Common Patterns to Follow

### Nullable Reference Types

```csharp
// ✅ CORRECT: Nullable return for optional values
public string? GetFieldValue(string fieldName) { }

// ✅ CORRECT: Non-null with validation
public void SetField(string fieldName, object value)
{
    if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));
}
```

### Field Access

```csharp
// ✅ CORRECT: Safe field access
object? value = workItem.Fields.Contains(fieldName)
    ? workItem[fieldName]
    : null;

// ❌ WRONG: Direct access without checking existence
var value = workItem[fieldName]; // May throw
```

### Collection Handling

```csharp
// ✅ CORRECT: IEnumerable enumeration
foreach (var revision in workItem.Revisions)
{
    ProcessRevision(revision);
}

// ❌ WRONG: Assuming indexable
var first = workItem.Revisions[0]; // Revisions is IEnumerable, not IList
```

## Public API Management

### Adding New Public Types/Members

1. Add to `PublicAPI.Unshipped.txt` (or `PublicAPI.Unshipped.net472.txt` for framework-specific)
2. Run `dotnet format` to auto-update if needed
3. Mark types `internal` if they should not be public

### Framework-Specific APIs

- `net472` has special polyfill types in `Compatibility/`
- Use `PublicAPI.Shipped.net472.txt` for net472-only public APIs
- Common polyfills: `NullableAttributes`, `IdentityTypeMapper`

## InternalsVisibleTo

This component exposes internals to:

- `Qwiq.Core.UnitTests` - Unit tests
- `Qwiq.Mocks` - Mock implementations
- `Qwiq.Client.Soap`, `Qwiq.Client.Rest` - Client implementations
- `Qwiq.Integration.Tests` - Integration tests
- `Qwiq.Mapper`, `Qwiq.Mapper.Identity` - Mapping layer
- `Qwiq.Identity.Soap` - Identity SOAP client

When adding internal types, consider which assemblies need access.

## Build Considerations

### Multi-Targeting

All target frameworks must build successfully:

```powershell
dotnet build src/Qwiq.Core/Qwiq.Core.csproj -c Release
```

### Package Validation

This project has `EnablePackageValidation` enabled:

- API changes are detected automatically
- Breaking changes fail the build
- See test/Qwiq.Package.Tests for validation tests

## Dependencies

### Key Packages

- **Microsoft.VisualStudio.Services.Client** - Azure DevOps client libraries
- **Microsoft.Identity.Client** - MSAL authentication
- **Castle.Core** - Proxy generation
- **Newtonsoft.Json** - JSON serialization

### Dependency Notes

- Do NOT upgrade Azure DevOps SDK packages without extensive testing
- Version conflicts are resolved in `Directory.Packages.props`

## Related Components

- **Qwiq.Core.Rest** - REST API client implementation
- **Qwiq.Core.Soap** - SOAP API client implementation (depends on Core)
- **Qwiq.Linq** - LINQ query provider (depends on Core)
- **Qwiq.Mapper** - Object mapping (depends on Core)
- **Qwiq.Mocks** - Mock implementations for testing (depends on Core)

## Common Mistakes to Avoid

❌ **Don't hardcode field names** - Use `CoreFieldRefNames` constants

❌ **Don't make internal types public** - Add to PublicAPI files if needed

❌ **Don't break interface contracts** - All implementations must remain compatible

❌ **Don't use JetBrains.Annotations** - Removed in favor of C# nullable types

❌ **Don't skip null checks** - Use runtime validation for public APIs

✅ **Do use factory pattern** - All stores created via `WorkItemStoreFactory`

✅ **Do expose interfaces** - Public types should implement interfaces

✅ **Do test with mocks** - Use `Qwiq.Mocks` for unit tests

✅ **Do maintain API compatibility** - Package validation enforces this
