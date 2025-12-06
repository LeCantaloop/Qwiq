# Qwiq.Mocks

Mock implementations for testing applications that use Qwiq.

## Overview

Qwiq.Mocks provides in-memory implementations of all core Qwiq interfaces, making it easy to write fast, reliable unit tests without needing a real Azure DevOps or TFS connection.

## Key Features

- **Complete mock implementations**: All core interfaces mocked
- **In-memory storage**: Fast test execution
- **Easy setup**: Minimal configuration needed
- **Isolated tests**: Each test gets its own mock store

## Installation

```bash
dotnet add package Qwiq.Mocks
```

## Quick Start

### Basic Mock Store

```csharp
using Qwiq.Mocks;
using Xunit;

public class WorkItemServiceTests
{
    [Fact]
    public void QueryBugs_ReturnsOnlyBugs()
    {
        // Arrange
        using var store = new MockWorkItemStore();
        store.Add(new MockWorkItem("Bug") { Title = "Test Bug" });
        store.Add(new MockWorkItem("Task") { Title = "Test Task" });

        var service = new WorkItemService(store);

        // Act
        var bugs = service.QueryBugs();

        // Assert
        Assert.Single(bugs);
        Assert.Equal("Test Bug", bugs.First().Title);
    }
}
```

### Create Mock Work Items

```csharp
// Simple work item
var workItem = new MockWorkItem("Bug")
{
    Id = 123,
    Title = "Critical Bug",
    State = "Active"
};

// Work item with custom fields
var workItem2 = new MockWorkItem("Bug");
workItem2.Fields["System.Id"].Value = 456;
workItem2.Fields["System.Title"].Value = "Another Bug";
workItem2.Fields["Microsoft.VSTS.Common.Severity"].Value = "1 - Critical";
```

### Mock Collections

```csharp
var store = new MockWorkItemStore();

// Add multiple work items
store.Add(new MockWorkItem("Bug") { Title = "Bug 1" });
store.Add(new MockWorkItem("Bug") { Title = "Bug 2" });
store.Add(new MockWorkItem("Task") { Title = "Task 1" });

// Query returns all items
var allItems = store.Query("SELECT * FROM WorkItems");
Assert.Equal(3, allItems.Count());
```

## Advanced Usage

### Mock Work Item with Links

```csharp
var parent = new MockWorkItem("User Story") { Id = 1, Title = "Story" };
var child = new MockWorkItem("Task") { Id = 2, Title = "Task" };

// Add parent-child relationship
var link = new MockWorkItemLink(
    parent.Id.Value,
    child.Id.Value,
    "System.LinkTypes.Hierarchy-Forward"
);

parent.Links.Add(link);

Assert.Single(parent.Links);
```

### Mock Revisions

```csharp
var workItem = new MockWorkItem("Bug") { Title = "Original Title" };
var store = new MockWorkItemStore();
store.Add(workItem);

// Create a revision
workItem.Title = "Updated Title";
workItem.Save();

Assert.Equal(2, workItem.Revisions.Count);
Assert.Equal("Original Title", workItem.Revisions[0].Fields["System.Title"].Value);
Assert.Equal("Updated Title", workItem.Revisions[1].Fields["System.Title"].Value);
```

### Mock Identity Service

```csharp
var identityService = new MockIdentityManagementService();

// Add test identities
identityService.AddIdentity(new MockIdentity
{
    DisplayName = "John Doe",
    UniqueName = "john.doe@contoso.com",
    TeamFoundationId = Guid.NewGuid()
});

// Resolve identity
var identity = identityService.ReadIdentity(
    IdentitySearchFactor.DisplayName,
    "John Doe"
);

Assert.Equal("john.doe@contoso.com", identity.UniqueName);
```

### Mock Field Definitions

```csharp
var definitions = new MockFieldDefinitionCollection();

definitions.Add(new MockFieldDefinition
{
    ReferenceName = "System.Title",
    Name = "Title",
    FieldType = FieldType.String
});

definitions.Add(new MockFieldDefinition
{
    ReferenceName = "Microsoft.VSTS.Common.Priority",
    Name = "Priority",
    FieldType = FieldType.Integer
});

var titleField = definitions["System.Title"];
Assert.Equal("Title", titleField.Name);
```

## Testing Patterns

### Arrange-Act-Assert with Mocks

```csharp
[Fact]
public void UpdateWorkItem_SetsState()
{
    // Arrange
    using var store = new MockWorkItemStore();
    var workItem = new MockWorkItem("Bug")
    {
        Id = 1,
        State = "New"
    };
    store.Add(workItem);

    var service = new WorkItemService(store);

    // Act
    service.ResolveWorkItem(1);

    // Assert
    var updated = store.GetWorkItem(1);
    Assert.Equal("Resolved", updated.State);
}
```

### Test Query Logic

```csharp
[Theory]
[InlineData("Active", 2)]
[InlineData("Resolved", 1)]
[InlineData("Closed", 0)]
public void QueryByState_ReturnsCorrectCount(string state, int expected)
{
    // Arrange
    using var store = new MockWorkItemStore();
    store.Add(new MockWorkItem("Bug") { State = "Active" });
    store.Add(new MockWorkItem("Bug") { State = "Active" });
    store.Add(new MockWorkItem("Bug") { State = "Resolved" });

    var service = new WorkItemService(store);

    // Act
    var results = service.QueryByState(state);

    // Assert
    Assert.Equal(expected, results.Count());
}
```

### Integration with Mapper

```csharp
[Fact]
public void MapWorkItem_ToStronglyTypedModel()
{
    // Arrange
    using var store = new MockWorkItemStore();
    var mockItem = new MockWorkItem("Bug")
    {
        Id = 123,
        Title = "Test Bug",
        ["Microsoft.VSTS.Common.Severity"] = "1 - Critical"
    };
    store.Add(mockItem);

    var mapper = new WorkItemMapper();

    // Act
    var workItem = store.GetWorkItem(123);
    var bug = mapper.Map<Bug>(workItem);

    // Assert
    Assert.Equal(123, bug.Id);
    Assert.Equal("Test Bug", bug.Title);
    Assert.Equal("1 - Critical", bug.Severity);
}
```

## Mock vs Real Implementation

| Feature   | MockWorkItemStore | Real Store             |
| --------- | ----------------- | ---------------------- |
| Speed     | ⚡ Instant        | 🐌 Network calls       |
| Isolation | ✅ Full           | ❌ Shared state        |
| Setup     | ✅ Minimal        | 🔧 Connection required |
| Coverage  | ✅ Unit tests     | ✅ Integration tests   |

## Best Practices

1. **One store per test**: Create fresh mocks for isolation
2. **Use `using` statements**: Properly dispose mock stores
3. **Arrange clearly**: Make test data setup obvious
4. **Test edge cases**: Null values, empty collections, etc.
5. **Integration tests separately**: Use mocks for unit tests, real store for integration

## Common Scenarios

### Test Exception Handling

```csharp
[Fact]
public void GetWorkItem_InvalidId_ThrowsException()
{
    using var store = new MockWorkItemStore();

    Assert.Throws<InvalidOperationException>(() =>
        store.GetWorkItem(99999)
    );
}
```

### Test Collection Operations

```csharp
[Fact]
public void BatchUpdate_UpdatesMultipleItems()
{
    using var store = new MockWorkItemStore();
    var items = Enumerable.Range(1, 5)
        .Select(i => new MockWorkItem("Bug") { Id = i, State = "New" })
        .ToList();

    items.ForEach(store.Add);

    var service = new WorkItemService(store);
    service.BatchResolve(new[] { 1, 2, 3 });

    Assert.Equal(3, store.Query("SELECT * FROM WorkItems WHERE [System.State] = 'Resolved'").Count());
    Assert.Equal(2, store.Query("SELECT * FROM WorkItems WHERE [System.State] = 'New'").Count());
}
```

## Related Packages

- **Qwiq.Core**: Core interfaces (required)
- **Qwiq.Mapper**: Object mapping for POCOs
- **xUnit / NUnit / MSTest**: Test frameworks

## Documentation

- [GitHub Repository](https://github.com/rjmurillo/Qwiq)
- [Testing Examples](https://github.com/rjmurillo/Qwiq/wiki/Testing-Examples)
- [Mock API Reference](https://github.com/rjmurillo/Qwiq/wiki/Mock-API)

## License

MIT License - see [LICENSE](https://github.com/rjmurillo/Qwiq/blob/master/LICENSE) for details.
