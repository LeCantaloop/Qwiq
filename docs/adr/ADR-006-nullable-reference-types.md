# ADR-006: Nullable Reference Types Migration

- **Status**: Accepted
- **Date**: 2025-12-06
- **Decision Makers**: Qwiq Development Team
- **Supersedes**: None
- **Superseded by**: None

## Context

C# 8.0 introduced Nullable Reference Types (NRT), a compiler feature that helps prevent null reference exceptions by making nullability explicit in type signatures. This is a significant improvement for code safety and developer experience.

Qwiq was originally written before C# 8.0, using:

- JetBrains.Annotations (`[NotNull]`, `[CanBeNull]`, etc.) for nullability hints
- Runtime null checks with `ArgumentNullException`
- No compile-time null safety

### Problem Statement

How can we migrate Qwiq to use Nullable Reference Types while:

1. Minimizing breaking changes for consumers
2. Maintaining runtime null safety
3. Avoiding false positives from the compiler
4. Supporting older target frameworks (net472) that don't have NRT attributes
5. Completing the migration incrementally without blocking other work

### Forces

- **Safety**: NRT helps catch null reference bugs at compile time
- **Developer Experience**: Modern IDEs provide better nullability intellisense
- **Breaking Changes**: Changing nullability can break consumers
- **Migration Effort**: 9 source projects + 7 test projects
- **Polyfills**: net472 and netstandard2.0 need polyfill attributes
- **Legacy Code**: Large existing codebase to migrate

## Decision

We will **enable Nullable Reference Types repository-wide** and migrate incrementally using a phased approach by project, starting with core libraries and progressing to higher-level abstractions.

### Implementation

#### Enable NRT in Directory.Build.props

```xml
<Project>
  <PropertyGroup>
    <!-- Enable nullable reference types for all projects -->
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

#### Polyfills for Older Target Frameworks

```csharp
// src/Qwiq.Core/Compatibility/NullableAttributes.cs
#if NETFRAMEWORK || NETSTANDARD2_0
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue)
            => ReturnValue = returnValue;
        public bool ReturnValue { get; }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class MaybeNullWhenAttribute : Attribute
    {
        public MaybeNullWhenAttribute(bool returnValue)
            => ReturnValue = returnValue;
        public bool ReturnValue { get; }
    }
}
#endif
```

#### Remove JetBrains.Annotations

```csharp
// ❌ BEFORE (JetBrains.Annotations)
public void Method([NotNull] SomeType parameter)
{
    if (parameter == null) throw new ArgumentNullException(nameof(parameter));
}

// ✅ AFTER (Nullable Reference Types)
public void Method(SomeType parameter) // Non-nullable by default
{
    if (parameter == null) throw new ArgumentNullException(nameof(parameter));
}

// ✅ Nullable parameter
public void Method(SomeType? parameter)
{
    if (parameter == null) return; // or throw
}
```

#### Null Validation Pattern

```csharp
// Constructor with non-null parameter
public WorkItemQueryBuilder(IWorkItemStore store)
{
    _store = store ?? throw new ArgumentNullException(nameof(store));
}

// Method with nullable parameter
public IWorkItem? GetWorkItem(int? id)
{
    if (!id.HasValue) return null;
    return _store.Query($"SELECT * FROM WorkItems WHERE [System.Id] = {id.Value}")
        .FirstOrDefault();
}

// Try* pattern with out parameter
public bool TryGetWorkItem(int id, [NotNullWhen(true)] out IWorkItem? item)
{
    item = _store.Query($"SELECT * FROM WorkItems WHERE [System.Id] = {id}")
        .FirstOrDefault();
    return item != null;
}
```

### Migration Phases

| Phase       | Projects                                | Status      | Completion |
| ----------- | --------------------------------------- | ----------- | ---------- |
| **Phase 1** | Qwiq.Core                               | ✅ Complete | PR #52     |
| **Phase 2** | Qwiq.Client.Rest                        | ✅ Complete | PR #52     |
| **Phase 3** | Qwiq.Mocks                              | ✅ Complete | PR #52     |
| **Phase 4** | Qwiq.Linq                               | ✅ Complete | PR #52     |
| **Phase 5** | Qwiq.Mapper                             | ✅ Complete | PR #52     |
| **Phase 6** | Qwiq.Identity, Qwiq.Identity.Soap, etc. | ✅ Complete | PR #52     |

## Consequences

### Positive

1. **Compile-Time Safety**: Null reference bugs caught before runtime
2. **Better Intellisense**: IDEs show nullability information
3. **Self-Documenting**: Type signatures indicate nullable intent
4. **Removed Dependency**: No longer need JetBrains.Annotations
5. **Modern C#**: Aligns with C# language direction
6. **Reduced Runtime Exceptions**: Fewer `NullReferenceException` in production

### Negative

1. **Breaking Changes**: Nullability changes can break consumers
2. **Warning Noise**: Migration generates many CS8xxx warnings initially
3. **Polyfill Maintenance**: Need to maintain polyfill attributes for older TFMs
4. **False Positives**: Compiler sometimes flags valid code as potentially null
5. **Learning Curve**: Team must learn NRT best practices

### Trade-offs

- **Safety vs Flexibility**: Stricter null checking vs. more flexible code
- **Compile-Time vs Runtime**: Catch errors earlier (good) but more compiler warnings (bad during migration)
- **Annotation Burden**: More attributes needed (`!`, `?`, `[NotNullWhen]`, etc.)

### Risks

- **Consumer Breaks**: Changing method signatures can break consumers
  - _Mitigation_: Semantic versioning, extensive testing, API compatibility analyzers (W2.2)
- **Incomplete Migration**: Half-migrated codebase is confusing
  - _Mitigation_: Phased approach, track progress in modernize-TODO.md
- **Suppression Abuse**: Developers might use `!` operator to silence warnings
  - _Mitigation_: Code review, document proper patterns, avoid `null!` for field initialization

## Migration Guidelines

### DO

- ✅ Use non-nullable reference types by default
- ✅ Use `?` suffix for nullable reference types (e.g., `string?`)
- ✅ Use runtime null checks with `ArgumentNullException`
- ✅ Use `[NotNullWhen]` and `[MaybeNullWhen]` for Try\* methods
- ✅ Document nullability in XML comments
- ✅ Fix CS8618 by proper field initialization (not `null!` suppression)

### DON'T

- ❌ Use `null!` to suppress CS8618 warnings (hides the problem)
- ❌ Mix JetBrains.Annotations with NRT (removed during migration)
- ❌ Use `#nullable disable` to hide warnings (defeats purpose)
- ❌ Ignore CS8600-CS8769 warnings without understanding them
- ❌ Change nullability of public APIs without careful consideration

### Field Initialization Anti-Pattern

```csharp
// ❌ BAD: Suppresses CS8618, no guarantee of initialization
private readonly Dictionary<string, object?> _fields = null!;
protected internal WorkItemCore() { } // Forgot to initialize!

// ✅ GOOD: Initialize in declaration
private readonly Dictionary<string, object?> _fields = new();

// ✅ GOOD: Initialize in ALL constructors
private readonly Dictionary<string, object?> _fields;
protected internal WorkItemCore()
{
    _fields = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
}
```

### Test-Driven Development for Refactoring

When fixing CS8xxx warnings, use TDD:

1. **Write tests first** - Document current behavior
2. **Make changes** - Fix nullability annotations
3. **Verify tests still pass** - Ensure no behavior changed

## Suppression Strategy

CS8xxx warnings are suppressed in `.editorconfig` during gradual migration:

```editorconfig
# Temporary suppressions during migration (remove when complete)
dotnet_diagnostic.CS8600.severity = none  # Converting null literal or possible null value
dotnet_diagnostic.CS8601.severity = none  # Possible null reference assignment
dotnet_diagnostic.CS8602.severity = none  # Dereference of a possibly null reference
dotnet_diagnostic.CS8603.severity = none  # Possible null reference return
dotnet_diagnostic.CS8604.severity = none  # Possible null reference argument
# ... etc (10 rules total)
```

**Goal**: Remove all CS8xxx suppressions once migration is complete.

**Current Status**: 0 CS8xxx warnings in all source projects (verified Dec 5, 2025).

## Alternatives Considered

### Alternative 1: Keep JetBrains.Annotations

```csharp
// ❌ Rejected
[NotNull] public string Title { get; set; }
```

**Rejected because:**

- Third-party dependency
- Not compile-time enforced
- Doesn't integrate with C# language features
- Outdated approach

### Alternative 2: Delay NRT Until Breaking Version

**Rejected because:**

- Delays safety improvements
- Makes codebase harder to maintain
- C# ecosystem has already moved to NRT

### Alternative 3: Opt-In NRT Per Project

```xml
<!-- ❌ Rejected - Inconsistent -->
<Nullable>enable</Nullable> <!-- Only in some projects -->
```

**Rejected because:**

- Inconsistent developer experience
- Harder to maintain
- Partial migration is confusing

### Alternative 4: Use `Polyfill` NuGet Package

```xml
<!-- ❌ Rejected -->
<PackageReference Include="Polyfill" Version="1.0.0" />
```

**Rejected because:**

- Conflicts with Microsoft.VisualStudio.Services.Client polyfills (317 ambiguous method errors)
- Custom polyfill file is simpler and conflict-free

## Related Decisions

- [ADR-002: Interface-First Design](ADR-002-interface-first-design.md) - Interfaces must have correct nullability
- W1.9-W1.14: Nullable cleanup tasks
- W2.2: API Compatibility Baselines (detect nullability breaking changes)

## References

- [Nullable Reference Types Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references)
- [Nullable Attributes](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis)
- [CS8xxx Warning Reference](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/nullable-warnings)
- [Migration Guide](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-migration-strategies)

## Revision History

| Date       | Author        | Changes                                                       |
| ---------- | ------------- | ------------------------------------------------------------- |
| 2025-12-06 | Copilot Agent | Initial ADR documenting NRT migration strategy and completion |
