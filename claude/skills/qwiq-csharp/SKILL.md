---
name: qwiq-csharp
description: C# coding patterns for QWIQ including nullable reference types, null validation, exception handling, and established architectural patterns. Use when editing C# files, fixing nullable warnings, or reviewing code quality.
---

# QWIQ C# Coding Patterns

## Quick Reference

| Task                         | Pattern                                                                       |
| ---------------------------- | ----------------------------------------------------------------------------- |
| Null parameter check         | `if (param == null) throw new ArgumentNullException(nameof(param));`          |
| Null check in ctor base call | `base(service?.Property ?? throw new ArgumentNullException(nameof(service)))` |
| Fix CS8618 (field not init)  | Initialize in declaration or ALL constructors                                 |
| Nullable return              | `public string? GetValue()`                                                   |
| Null-conditional access      | `obj?.Property?.Method()`                                                     |

**Never use:** `null!`, `= null!`, JetBrains.Annotations, duplicate validation

## Purpose

This skill provides guidance for writing and maintaining C# code in the QWIQ repository. It covers nullable reference types (enabled repository-wide), null validation patterns, exception handling, and architectural conventions that ensure consistency across the codebase.

## When To Use

- Editing any `.cs` file in the repository
- Fixing CS8xxx nullable reference type warnings
- Adding null validation to methods or constructors
- Implementing exception handling
- Following established design patterns (Factory, Strategy, etc.)
- Code review for C# changes

## Instructions

### 1. Nullable Reference Types

Nullable reference types are enabled repository-wide (`<Nullable>enable</Nullable>`).

**Required Patterns:**

```csharp
// Nullable return type - use when null is a valid return
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
```

**Forbidden Patterns:**

```csharp
// ❌ NEVER use null! to suppress CS8618 - this hides the problem
private readonly Dictionary<string, object?> _fields = null!;

// ❌ NEVER use JetBrains.Annotations (removed from codebase)
[NotNull] public string Value { get; }

// ❌ NEVER duplicate validation
Contract.Requires(param != null);
if (param == null) throw new ArgumentNullException(nameof(param)); // Pick ONE
```

### 2. Field Initialization

When fixing CS8618 warnings, always use proper initialization:

```csharp
// ✅ Initialize in declaration
private readonly MyType _field = new MyType();

// ✅ Initialize in ALL constructors
private readonly MyType _field;
protected MyClass() { _field = new MyType(); }
protected MyClass(MyType field) { _field = field; }

// ✅ Make nullable if null is valid
private readonly MyType? _field;
protected MyClass() { _field = null; } // Explicitly null
```

### 3. Exception Handling

```csharp
// Always log before handling or rethrowing
catch (Exception ex)
{
    System.Diagnostics.Trace.TraceError($"Operation failed: {ex.Message}");
    throw; // or return appropriate value with documentation
}

// Use appropriate exception types
if (parameter == null) throw new ArgumentNullException(nameof(parameter));
if (string.IsNullOrEmpty(value)) throw new ArgumentException("Value cannot be empty", nameof(value));
```

**Never swallow exceptions silently** - empty `catch { }` blocks hide bugs.

### 4. Known Class Patterns

**Revision Dual-Constructor Pattern:**

```csharp
// Constructor 1: Revision accessed via WorkItem.Revisions collection
public Revision(IWorkItem workItem, int index) { }

// Constructor 2: Standalone revision (field snapshot, no WorkItem reference)
public Revision(IFieldDefinitionCollection fieldDefinitions, int index) { }
// When WorkItem is null (constructor 2), Revision.Id returns null
```

**Null-Conditional for Link Types:**

```csharp
// LinkTypeEnd.ImmutableName may be null - always use ?.
string.Equals(rl.LinkTypeEnd?.ImmutableName, linkTypeEndName, StringComparison.OrdinalIgnoreCase)
```

### 5. Architectural Patterns

Follow these established patterns:

| Pattern             | Usage                | Example                                                  |
| ------------------- | -------------------- | -------------------------------------------------------- |
| Factory             | Object creation      | `WorkItemStoreFactory.Default.Create(options)`           |
| Strategy            | Varying behavior     | `AttributeMapperStrategy`, `WorkItemLinksMapperStrategy` |
| Interface-first     | All public types     | `IWorkItem`, `IWorkItemStore`, `IFieldDefinition`        |
| Lazy initialization | Expensive operations | Field definition collections                             |

### 6. Quality Attributes

- **Testability**: Build seams with DI, interfaces, or pure functions
- **Low coupling**: One responsibility per class, communicate via interfaces
- **Reuse constants**: Use `CoreFieldRefNames`, `TestData.cs`, not literals
- **Hide variations**: REST vs SOAP behind strategies/providers

## Anti-Patterns (What NOT to Do)

| Anti-Pattern                     | Why It's Bad                                  | Do This Instead                      |
| -------------------------------- | --------------------------------------------- | ------------------------------------ |
| `= null!` suppression            | Hides null safety issues, defeats NRT purpose | Initialize properly or make nullable |
| `Contract.Requires` + null check | Duplicate validation, confusing               | Choose ONE validation approach       |
| JetBrains.Annotations            | Removed from codebase, conflicts with NRT     | Use built-in nullable annotations    |
| Magic strings `"System.Id"`      | Typo-prone, no compile-time check             | Use `CoreFieldRefNames.Id`           |
| Empty catch `{ }`                | Swallows errors silently                      | Log and rethrow or handle explicitly |
| `public` fields                  | No encapsulation, breaking changes            | Use properties with backing fields   |
| God classes                      | Too many responsibilities                     | Split into focused classes           |
| Concrete dependencies            | Hard to test, tightly coupled                 | Depend on interfaces                 |

## Examples

### Example 1: Fixing Nullable Warning

**User goal:** Fix CS8618 warning on a field

**Process:**

1. Identify if field should be nullable or always initialized
2. If always initialized: add initialization in declaration or all constructors
3. If nullable: add `?` suffix and update all usages
4. Write test to verify behavior before and after change

**Expected output:** Field properly initialized without `null!` suppression

### Example 2: Adding Null Validation

**User goal:** Add parameter validation to a public method

**Process:**

1. Check if parameter is used before any null check
2. Add `if (param == null) throw new ArgumentNullException(nameof(param));`
3. For constructor base calls, use `param?.Property ?? throw new ArgumentNullException(nameof(param))`
4. Do NOT add both `Contract.Requires` and runtime check

**Expected output:** Method throws `ArgumentNullException` for null input

## Troubleshooting

### CS8618: Non-nullable field not initialized

**Symptom:** `Non-nullable field '_field' must contain a non-null value when exiting constructor`

**Decision tree:**

1. Should the field ever be null? → Yes: Add `?` to make it `MyType? _field`
2. Can it be initialized at declaration? → Yes: `private readonly MyType _field = new();`
3. Must it come from constructor? → Initialize in ALL constructors

**Never:** Use `= null!` to suppress the warning

### CS8602: Dereference of possibly null reference

**Symptom:** Warning when accessing `.Property` on potentially null object

**Fixes:**

```csharp
// Option 1: Null check first
if (obj != null) { var x = obj.Property; }

// Option 2: Null-conditional
var x = obj?.Property;

// Option 3: Null-coalescing with default
var x = obj?.Property ?? defaultValue;
```

### CS8604: Possible null argument

**Symptom:** Passing value to non-nullable parameter when compiler thinks it could be null

**Fixes:**

```csharp
// Option 1: Add runtime null check before call
if (value == null) throw new ArgumentNullException(nameof(value));
DoSomething(value);  // Compiler now knows it's not null

// Option 2: Use null-forgiving only if you KNOW it's not null
DoSomething(value!);  // Only when logically certain
```

### Interface nullability mismatch

**Symptom:** CS8619/CS8767 when implementation differs from interface

**Fix:** Align nullability between interface and implementation. Update ALL implementations when changing interface.

## Related Resources

- See [REFERENCE.md](./REFERENCE.md) for nullable migration status by project
- See [../qwiq-testing/SKILL.md](../qwiq-testing/SKILL.md) for TDD requirements when refactoring
- See [../nullable-migration/SKILL.md](../nullable-migration/SKILL.md) for migration tracking
