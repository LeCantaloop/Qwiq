# Qwiq.Tests.Common Component Guide

## Component Overview

**Qwiq.Tests.Common** provides shared test infrastructure, utilities, and base classes used across all test projects in the Qwiq solution.

## Purpose

- Share test utilities and base classes
- Provide `ContextSpecification` pattern for BDD-style tests
- Common test data constants (`TestData`)
- Shouldly assertion extensions and compatibility shims

## Key Characteristics

- **Target Frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
- **Platform**: Cross-platform
- **Dependencies**: MSTest, Shouldly, Qwiq.Core
- **Pattern**: Shared test infrastructure

## Key Components

### ContextSpecification

Base class for behavior-driven (Given-When-Then) tests:

```csharp
[TestClass]
public class Given_some_context : ContextSpecification
{
    private MyClass? _sut;  // System Under Test
    private string? _result;

    public override void Given()
    {
        // Arrange - setup test context
        _sut = new MyClass();
    }

    public override void When()
    {
        // Act - perform the action being tested
        _result = _sut!.DoSomething();
    }

    [TestMethod]
    public void Then_expected_behavior()
    {
        // Assert - verify the outcome
        _result.ShouldBe("expected");
    }

    [TestMethod]
    public void Then_another_expected_behavior()
    {
        // Multiple assertions for same context
        _result.ShouldNotBeEmpty();
    }
}
```

### TestData

Centralized test constants for integration tests:

```csharp
public static class TestData
{
    // Work Item IDs
    public const int BasicWorkItemId = 1;
    public const int HierarchyParentId = 3;
    public const int HierarchyChildId = 2;
    public const int MapperBugId = 4;
    public const int WorkItemWithLinksId = 5;

    // Identity Information
    public const string TestUserUpn = "rjmurillo@msn.com";
    public const string TestUserAlias = "rjmurillo";
    public const string TestUserDisplayName = "Richard Murillo";

    // Project Information
    public const string ProjectName = "WIT";
    public static readonly Guid ProjectGuid = new("0a4c0240-1a67-45de-93db-fc1de9f54ffb");

    // Azure DevOps Organization
    public const string OrganizationUrl = "https://qwiq-sandbox.visualstudio.com/";
}
```

**Always use TestData constants** - Don't hardcode values in tests:

```csharp
// ✅ CORRECT: Using constants
var workItem = store.GetWorkItem(TestData.BasicWorkItemId);

// ❌ WRONG: Magic numbers
var workItem = store.GetWorkItem(1);
```

### ShouldExtensions

Compatibility shims for Shouldly assertions:

```csharp
// Shouldly compatibility for nullable value types
value.HasValue.ShouldBeFalse();  // NOT: value.ShouldBeNull()

// Custom assertions
collection.ShouldHaveSingleItem();
collection.ShouldBeEmpty();
```

## ContextSpecification Pattern

### Structure

1. **Given()** - Arrange test context (setup)
2. **When()** - Act on the system under test
3. **Then\_\*()** - Assert expected outcomes (multiple test methods)

### Benefits

- Clear separation of arrange-act-assert
- Reusable context across multiple assertions
- BDD-style naming for readability
- Reduces test code duplication

### Example: Testing LINQ Query Provider

```csharp
[TestClass]
public class Given_query_with_where_clause : ContextSpecification
{
    private MockWorkItemStore? _store;
    private Query<WorkItem>? _query;
    private IEnumerable<WorkItem>? _results;

    public override void Given()
    {
        _store = new MockWorkItemStore();
        _store.Add(new MockWorkItem("Bug") { Id = 1, State = "Active" });
        _store.Add(new MockWorkItem("Bug") { Id = 2, State = "Closed" });

        _query = new Query<WorkItem>(_store);
    }

    public override void When()
    {
        _results = _query!.Where(wi => wi.State == "Active").ToList();
    }

    [TestMethod]
    public void Then_returns_only_active_items()
    {
        _results.ShouldHaveSingleItem();
    }

    [TestMethod]
    public void Then_returned_item_has_correct_state()
    {
        _results!.First().State.ShouldBe("Active");
    }
}
```

## Test Organization

### Naming Convention

```csharp
[TestClass]
public class Given_[context]_When_[action] : ContextSpecification
public class Given_[context] : ContextSpecification
```

Examples:

- `Given_WorkItemStore_When_querying_by_id`
- `Given_empty_collection`
- `Given_mapped_work_item_with_identity_fields`

### Test Method Naming

```csharp
[TestMethod]
public void Then_[expected_behavior]()
{
    // Assertion
}
```

Examples:

- `Then_should_return_work_item()`
- `Then_throws_ArgumentNullException()`
- `Then_identity_field_is_resolved()`

## Common Testing Patterns

### Testing Exceptions

```csharp
public override void When()
{
    _exception = Should.Throw<ArgumentNullException>(() =>
    {
        _sut!.MethodThatThrows(null);
    });
}

[TestMethod]
public void Then_throws_expected_exception()
{
    _exception.ShouldNotBeNull();
    _exception.ParamName.ShouldBe("parameterName");
}
```

### Testing Null Returns

```csharp
[TestMethod]
public void Then_returns_null()
{
    _result.ShouldBeNull();
}
```

### Testing Collections

```csharp
[TestMethod]
public void Then_collection_contains_expected_items()
{
    _results.ShouldNotBeEmpty();
    _results.Count().ShouldBe(3);
    _results.First().Id.ShouldBe(TestData.BasicWorkItemId);
}
```

## Integration Test Support

### Test Data

All integration test data is in the **qwiq-sandbox** Azure DevOps organization:

- **Organization**: `https://qwiq-sandbox.visualstudio.com/`
- **Project**: `WIT` (GUID: `0a4c0240-1a67-45de-93db-fc1de9f54ffb`)
- **Test User**: Richard Murillo (`rjmurillo@msn.com`)

### Test Work Items

| ID  | Type       | Title                                      | Purpose                   |
| --- | ---------- | ------------------------------------------ | ------------------------- |
| 1   | Bug        | Integration Test                           | Basic work item tests     |
| 2   | Task       | Child Task for Integration Tests           | Child of ID 3 (hierarchy) |
| 3   | User Story | Parent Story for Integration Tests         | Parent for hierarchy      |
| 4   | Bug        | Bug for Mapper Integration Tests           | Mapper tests              |
| 5   | Bug        | Work Item with Links for Integration Tests | Work item with links      |

**Use TestData constants** for these IDs:

```csharp
var workItem = store.GetWorkItem(TestData.BasicWorkItemId);  // ID 1
var parent = store.GetWorkItem(TestData.HierarchyParentId);  // ID 3
```

## Common Mistakes to Avoid

❌ **Don't hardcode test data** - Use `TestData` constants

```csharp
// ❌ WRONG: Magic numbers and strings
var workItem = store.GetWorkItem(1);
var user = identityService.ReadIdentity("rjmurillo@msn.com");

// ✅ CORRECT: TestData constants
var workItem = store.GetWorkItem(TestData.BasicWorkItemId);
var user = identityService.ReadIdentity(TestData.TestUserUpn);
```

❌ **Don't share state in ContextSpecification** - Each test class is isolated

❌ **Don't use ShouldBeNull() for nullable value types** - Use `.HasValue.ShouldBeFalse()`

```csharp
// ❌ WRONG: Doesn't work with int?, DateTime?, etc.
nullableInt.ShouldBeNull();

// ✅ CORRECT: Check HasValue property
nullableInt.HasValue.ShouldBeFalse();
```

✅ **Do inherit from ContextSpecification** - For BDD-style tests

✅ **Do use TestData constants** - Centralized, documented values

✅ **Do separate Given-When-Then** - Clear test structure

✅ **Do use multiple Then\_ methods** - Test different aspects of same behavior

## Related Components

- **Qwiq.Core.Tests** - Core unit tests using these patterns
- **Qwiq.Linq.Tests** - LINQ provider tests
- **Qwiq.Mapper.Tests** - Mapper tests
- **Qwiq.Integration.Tests** - Integration tests using TestData
- **Qwiq.Mocks** - Mock implementations for unit tests

## Shouldly Assertions

Common assertions from Shouldly:

```csharp
result.ShouldBe(expected);
result.ShouldNotBe(unexpected);
result.ShouldBeNull();
result.ShouldNotBeNull();
collection.ShouldBeEmpty();
collection.ShouldNotBeEmpty();
collection.ShouldHaveSingleItem();
result.ShouldBeLessThan(10);
result.ShouldBeGreaterThan(0);
exception.ShouldBeOfType<ArgumentNullException>();
```

See [Shouldly documentation](https://docs.shouldly.io/) for complete API.
