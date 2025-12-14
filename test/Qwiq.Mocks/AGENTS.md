# Qwiq.Mocks Component Guide

## Component Overview

**Qwiq.Mocks** provides in-memory mock implementations of Qwiq interfaces for unit testing. This enables fast, isolated tests without requiring a real Azure DevOps / TFS connection.

## Purpose

- In-memory implementations of `IWorkItemStore`, `IWorkItem`, `IRevision`, etc.
- Fast unit testing without external dependencies
- Predictable test data and behavior
- Support for all Qwiq core interfaces

## Key Characteristics

- **Target Frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
- **Platform**: Cross-platform
- **Dependencies**: Qwiq.Core
- **Pattern**: Mock objects implementing real interfaces

## Key Mock Types

| Mock Type                       | Implements                   | Purpose                      |
| ------------------------------- | ---------------------------- | ---------------------------- |
| `MockWorkItemStore`             | `IWorkItemStore`             | In-memory work item storage  |
| `MockWorkItem`                  | `IWorkItem`                  | Work item with field storage |
| `MockRevision`                  | `IRevision`                  | Revision history             |
| `MockIdentityManagementService` | `IIdentityManagementService` | Identity resolution          |
| `MockFieldDefinitionCollection` | `IFieldDefinitionCollection` | Field metadata               |
| `MockQuery`                     | `IQuery`                     | Query execution              |

## Common Usage Patterns

### Basic Work Item Testing

```csharp
[TestMethod]
public void Should_query_work_items()
{
    // Arrange
    var store = new MockWorkItemStore();
    store.Add(new MockWorkItem("Bug")
    {
        Id = 123,
        Title = "Test Bug",
        ["System.State"] = "Active"
    });

    // Act
    var items = store.Query("SELECT [System.Id] FROM WorkItems");

    // Assert
    items.ShouldHaveSingleItem();
    items.First().Id.ShouldBe(123);
}
```

### Field Access

```csharp
var workItem = new MockWorkItem("Bug")
{
    ["System.Title"] = "Test Title",
    ["System.AssignedTo"] = "user@domain.com",
    ["Custom.Field"] = 42
};

workItem.Title.ShouldBe("Test Title");
workItem["System.AssignedTo"].ShouldBe("user@domain.com");
workItem["Custom.Field"].ShouldBe(42);
```

### Revisions

```csharp
var workItem = new MockWorkItem("Bug");
workItem.AddRevision(new Dictionary<string, object>
{
    ["System.Title"] = "Original Title",
    ["System.State"] = "New"
});
workItem.AddRevision(new Dictionary<string, object>
{
    ["System.Title"] = "Updated Title",
    ["System.State"] = "Active"
});

workItem.Revisions.Count().ShouldBe(2);
workItem.Revisions.First()["System.Title"].ShouldBe("Original Title");
```

### Identity Service

```csharp
var identityService = new MockIdentityManagementService();
identityService.AddIdentity("user@domain.com", "Display Name");

var identity = identityService.ReadIdentity("user@domain.com");
identity.DisplayName.ShouldBe("Display Name");
identity.UniqueName.ShouldBe("user@domain.com");
```

## Mock Store Patterns

### Adding Multiple Items

```csharp
var store = new MockWorkItemStore();
store.Add(new MockWorkItem("Bug") { Id = 1, Title = "Bug 1" });
store.Add(new MockWorkItem("Bug") { Id = 2, Title = "Bug 2" });
store.Add(new MockWorkItem("Task") { Id = 3, Title = "Task 1" });
```

### Querying by Type

```csharp
var bugs = store.Query("SELECT [System.Id] FROM WorkItems WHERE [System.WorkItemType] = 'Bug'");
bugs.Count().ShouldBe(2);
```

### Get by ID

```csharp
var workItem = store.GetWorkItem(123);
workItem.ShouldNotBeNull();
workItem.Id.ShouldBe(123);
```

## Testing with ContextSpecification

Use the `ContextSpecification` base class from `Qwiq.Tests.Common`:

```csharp
[TestClass]
public class Given_mock_work_item_store : ContextSpecification
{
    private MockWorkItemStore? _store;
    private IEnumerable<IWorkItem>? _results;

    public override void Given()
    {
        _store = new MockWorkItemStore();
        _store.Add(new MockWorkItem("Bug") { Id = 1, Title = "Test" });
    }

    public override void When()
    {
        _results = _store!.Query("SELECT [System.Id] FROM WorkItems");
    }

    [TestMethod]
    public void Then_should_return_work_items()
    {
        _results.ShouldHaveSingleItem();
    }
}
```

## Mock Behavior

### Field Storage

- Fields stored in-memory dictionary
- Case-insensitive field name comparison
- Supports any field name (no schema validation)

### WIQL Queries

- MockWorkItemStore supports basic WIQL filtering
- `WHERE [System.WorkItemType] = 'Type'`
- `WHERE [System.Id] = value`
- Complex queries may not be fully supported (use filtering manually)

### Revisions

- Add revisions with `AddRevision()`
- Revisions are indexed chronologically
- Access via `workItem.Revisions[index]` or LINQ

## Common Testing Patterns

### Arrange-Act-Assert

```csharp
// Arrange
var store = new MockWorkItemStore();
store.Add(new MockWorkItem("Bug") { Id = 1 });

// Act
var result = myService.QueryBugs(store);

// Assert
result.ShouldNotBeEmpty();
```

### Isolated Tests

Each test should create its own mock store:

```csharp
// ✅ CORRECT: Isolated store per test
[TestMethod]
public void Test1()
{
    var store = new MockWorkItemStore();
    // Test with this store
}

[TestMethod]
public void Test2()
{
    var store = new MockWorkItemStore();
    // Test with different store - no shared state
}
```

## Common Mistakes to Avoid

❌ **Don't share mock stores between tests** - Tests should be isolated

```csharp
// ❌ WRONG: Class-level shared store
private static MockWorkItemStore _sharedStore = new();

// ✅ CORRECT: Test-level isolated store
[TestMethod]
public void Test()
{
    var store = new MockWorkItemStore();
}
```

❌ **Don't assume complex WIQL support** - Use basic queries or filter manually

```csharp
// ❌ WRONG: Complex WIQL may not work
var items = store.Query("SELECT * FROM WorkItems WHERE [Title] CONTAINS 'test' ORDER BY [ChangedDate]");

// ✅ CORRECT: Simple query + LINQ filtering
var items = store.Query("SELECT [System.Id] FROM WorkItems")
    .Where(wi => wi.Title?.Contains("test") == true)
    .OrderBy(wi => wi.ChangedDate);
```

❌ **Don't test mock implementations** - Test your code using mocks

```csharp
// ❌ WRONG: Testing the mock itself
[TestMethod]
public void MockWorkItem_should_store_fields() { }

// ✅ CORRECT: Testing your code with mocks
[TestMethod]
public void MyService_should_query_active_bugs()
{
    var store = new MockWorkItemStore();
    var result = myService.QueryActiveBugs(store);
    // Assert on your service behavior
}
```

✅ **Do use mocks for unit tests** - Fast and isolated

✅ **Do create new mocks per test** - Avoid shared state

✅ **Do use simple WIQL or post-filter** - More predictable

✅ **Do test your code, not the mocks** - Mocks are tools

## Related Components

- **Qwiq.Core** - Interfaces that mocks implement
- **Qwiq.Tests.Common** - Shared test infrastructure (ContextSpecification)
- **Qwiq.Core.Tests** - Example usage of mocks
- **Qwiq.Linq.Tests** - LINQ provider tests using mocks

## When NOT to Use Mocks

Use integration tests (`Qwiq.Integration.Tests`) for:

- Testing actual REST/SOAP client implementations
- Verifying real Azure DevOps / TFS behavior
- Testing complex queries against real data
- Validating authentication flows

Use mocks for:

- Fast unit tests
- Testing business logic that uses `IWorkItemStore`
- Isolated component tests
- Testing edge cases and error conditions
