---
applyTo: "**/*.cs"
---

# C# File Instructions

> **MANDATORY**: You MUST follow these instructions when editing any C# file in this repository.

## Quick Reference

- Read [copilot-instructions.md](../copilot-instructions.md) for full repository guidelines
- Nullable reference types are ENABLED repository-wide
- Use runtime null checks, NOT JetBrains.Annotations
- Follow existing patterns in similar files

## Context Loading

When working on C# files, you MUST:

1. Read this entire instruction file before making changes
2. Cross-reference with [copilot-instructions.md](../copilot-instructions.md)
3. Check similar files for established patterns
4. Complete the Validation Checklist before submitting

## Nullable Reference Types

This repository uses C# nullable reference types (`<Nullable>enable</Nullable>`).

### Correct Patterns

```csharp
// Nullable return type
public string? GetValue() => _value;

// Non-null parameter with runtime validation
public void Method(SomeType parameter)
{
    if (parameter == null) throw new ArgumentNullException(nameof(parameter));
}

// Constructor with null check before base call
public MyClass(IService service)
    : base(service?.Property ?? throw new ArgumentNullException(nameof(service)))
{
}

// Nullable value type assertions in tests
value.HasValue.ShouldBeFalse(); // NOT: value.ShouldBeNull()
```

### Patterns to AVOID

```csharp
// ❌ JetBrains.Annotations (removed from codebase)
[NotNull] public string Value { get; }
[CanBeNull] public string? Name { get; }

// ❌ Using null! to suppress CS8618 (hides the problem!)
private readonly Dictionary<string, object?> _fields = null!;
protected internal MyClass() { } // _fields never initialized!

// ❌ Duplicate validation (pick ONE approach)
Contract.Requires(param != null);
if (param == null) throw new ArgumentNullException(nameof(param)); // Don't use both!

// ❌ Silent exception swallowing
catch (Exception) { return null; } // Log the error at minimum
```

### Field Initialization - CRITICAL

**NEVER use `= null!` to suppress CS8618 warnings.** This is suppressing the problem, not fixing it.

```csharp
// ❌ WRONG: Suppresses CS8618, no guarantee of initialization
private readonly MyType _field = null!;
protected MyClass() { } // Forgot to initialize!

// ✅ CORRECT: Initialize in declaration
private readonly MyType _field = new MyType();

// ✅ CORRECT: Initialize in ALL constructors
private readonly MyType _field;
protected MyClass() { _field = new MyType(); }
protected MyClass(MyType field) { _field = field; }

// ✅ CORRECT: Make nullable if null is valid
private readonly MyType? _field;
protected MyClass() { _field = null; } // Explicitly null
```

## Exception Handling

### Required Pattern

```csharp
catch (Exception ex)
{
    System.Diagnostics.Trace.TraceError($"Operation failed: {ex.Message}");
    throw; // or return appropriate value with clear documentation
}
```

### Rules

- **Never** swallow exceptions silently with empty catch blocks
- **Always** log exceptions before handling or rethrowing
- Use `ArgumentNullException` for null parameters
- Use `ArgumentException` for invalid (but non-null) parameters
- Choose ONE validation approach: `Contract.Requires` OR runtime null checks

## Known Class Patterns

### Revision Dual-Constructor Pattern

The `Revision` class has two constructors for different scenarios:

```csharp
// Constructor 1: Revision accessed via WorkItem.Revisions collection
public Revision(IWorkItem workItem, int index) { }

// Constructor 2: Standalone revision (field snapshot, no WorkItem reference)
public Revision(IFieldDefinitionCollection fieldDefinitions, int index) { }
```

When `WorkItem` is null (constructor 2), `Revision.Id` returns `null`.

### Null-Conditional Access for Link Types

```csharp
// LinkTypeEnd.ImmutableName may be null - use null-conditional
string.Equals(rl.LinkTypeEnd?.ImmutableName, linkTypeEndName, StringComparison.OrdinalIgnoreCase)
```

## Quality & Design Guidance

- **Make testing simple**: Inject dependencies, keep methods small, and expose seams so unit tests can plug in mocks from `Qwiq.Mocks` or Moq.
- **Keep responsibilities clear**: Each class should own one job. If a change touches several areas (Core, Mapper, Identity), double-check that only interfaces cross the boundaries.
- **Reuse identity and field values**: Pull names and constants from `CoreFieldRefNames`, `TestData`, and `IdentityConstants` to avoid drifting copies.
- **Leave configuration in config files**: Defaults belong in `Directory.Build.props`, `Directory.Packages.props`, or option classes—not hardcoded in methods.
- **Isolate REST vs SOAP differences**: Push variation into strategies, providers, or small helper classes instead of scattering `if` checks through call sites.
- **Program by intention**: Sketch the methods you wish existed, then implement them behind focused interfaces. This keeps methods cohesive and reveals missing seams.
- **Call out the pattern you follow**: If you add a Strategy, Adapter, Façade, or Factory, say so in code comments or reviews so future work stays aligned.
- **Create objects separately**: Build services through factories, builders, or constructor injection. Avoid new-ing up dependencies inside business logic.

## Test Patterns

### ContextSpecification Base Class

Unit tests follow the Given/When/Then pattern:

```csharp
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

### Test Data Guidelines

- Cover positive, negative, and edge cases
- Use `MockWorkItem`, `MockRevision`, etc. from `Qwiq.Mocks`
- `IEnumerable` collections need `.First()` or `.ToList()` for indexing
- SOAP-specific classes are `internal` and require TFS infrastructure

## ⚠️ MANDATORY: Test-Driven Development for Refactoring

When making refactoring changes (nullable fixes, initialization changes, interface updates), you **MUST** use TDD:

### TDD Workflow

#### Step 1: Write Tests BEFORE Code Changes

```csharp
// Document current behavior with tests
[TestClass]
public class Given_WorkItemCore_with_parameterless_constructor : ContextSpecification
{
    private TestableWorkItemCore? _result;

    public override void When()
    {
        _result = new TestableWorkItemCore();
    }

    [TestMethod]
    public void Should_allow_field_operations()
    {
        _result!.SetValue("test", "value");
        var value = _result!.GetValue("test");
        value.ShouldEqual("value");
    }
}
```

#### Step 2: Verify Tests Pass

```bash
dotnet test --filter "FullyQualifiedName~WorkItemCore"
# All tests should PASS before making changes
```

#### Step 3: Make Code Changes

```csharp
// Fix null! suppression with proper initialization
private readonly Dictionary<string, object?> _fields;

protected internal WorkItemCore()
{
    _fields = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
}
```

#### Step 4: Verify Tests Still Pass

```bash
dotnet test --filter "FullyQualifiedName~WorkItemCore"
# All tests should STILL PASS after changes
```

### When TDD is Required

- Fixing CS8618 warnings (field initialization)
- Fixing CS8625 warnings (null literal assignments)
- Removing `null!` suppressions
- Changing constructor signatures
- Any change that could affect runtime behavior

### Test Coverage for Initialization Fixes

When fixing field initialization, write tests for:

- All constructor overloads
- Lazy initialization behavior
- Property accessors
- Null handling
- Edge cases

## Validation Checklist

Before submitting changes, verify:

- [ ] Code compiles without errors
- [ ] No new warnings introduced
- [ ] Nullable annotations are correct
- [ ] Exception handling follows logging pattern
- [ ] Linting passes:
  - Run `dotnet format` to apply analyzer code fixes to C# files
  - Run `dotnet pprettier --write .` to auto-fix all formatting
- [ ] Tests pass: `dotnet test --filter "TestCategory!=localOnly&..."`
- [ ] Similar files checked for established patterns
- [ ] Architectural qualities reviewed (testability, cohesion, coupling) and no duplicated identity/configuration literals introduced

## Decision Trees

### When Adding Null Checks

1. Is the parameter used before validation? → Move check earlier
2. Does a base constructor need the value? → Use `?? throw` pattern
3. Is this a public API? → Document nullability in XML comments

### When to Stop and Ask

- Uncertain about nullable annotations
- Adding new exception types
- Changing public API signatures
- Modifying core interfaces (IWorkItem, IRevision, etc.)

## Related Instruction Files

- [project.instructions.md](project.instructions.md) - For .csproj files
- [msbuild.instructions.md](msbuild.instructions.md) - For Directory.Build.props/targets
- [generic.instructions.md](generic.instructions.md) - For multi-file changes
