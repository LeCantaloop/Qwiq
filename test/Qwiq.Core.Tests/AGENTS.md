# Qwiq.Core.Tests Component Guide

## Component Overview

**Qwiq.Core.Tests** contains unit tests for the Qwiq.Core component, testing core interfaces, implementations, and utilities.

## Purpose

- Test core work item interfaces and implementations
- Verify field collection behavior
- Test revision handling
- Validate type parsers and utilities

## Key Characteristics

- **Target Frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
- **Test Framework**: MSTest
- **Pattern**: ContextSpecification (from Qwiq.Tests.Common)
- **Mocks**: Uses Qwiq.Mocks

## Test Structure

Tests use ContextSpecification pattern:

```csharp
[TestClass]
public class Given_work_item : ContextSpecification
{
    private MockWorkItem? _workItem;

    public override void Given()
    {
        _workItem = new MockWorkItem("Bug");
    }

    [TestMethod]
    public void Then_has_expected_type()
    {
        _workItem!.Type.ShouldBe("Bug");
    }
}
```

## Test Categories

### Core Interface Tests

- `IWorkItem` behavior
- `IRevision` behavior
- `IFieldDefinition` and field collections
- `IProject` and project collections

### Utility Tests

- `TypeParser` type conversions
- Field name constants (CoreFieldRefNames)
- Comparer implementations

### Edge Cases

- Null handling
- Empty collections
- Field access errors

## Common Test Patterns

### Field Access Testing

```csharp
[TestMethod]
public void Should_get_field_value()
{
    _workItem!["System.Title"] = "Test";
    _workItem["System.Title"].ShouldBe("Test");
}
```

### Revision Testing

```csharp
[TestMethod]
public void Should_access_revision_fields()
{
    var revision = _workItem!.Revisions.First();
    revision["System.State"].ShouldBe("New");
}
```

## Related Components

- **Qwiq.Core** - Component under test
- **Qwiq.Mocks** - Mock implementations
- **Qwiq.Tests.Common** - Test infrastructure
