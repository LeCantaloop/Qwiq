# Qwiq.Mapper

Object mapping framework for converting Azure DevOps work items to strongly-typed POCOs.

## Overview

Qwiq.Mapper provides an attribute-based mapping system to convert `IWorkItem` instances into your own domain models, enabling strongly-typed access to work item data and better integration with your application's business logic.

## Key Features

- **Attribute-based mapping**: Simple declarative mapping using attributes
- **Type-safe access**: Work with POCOs instead of field dictionaries
- **Bulk identity resolution**: Efficient resolution of identity fields
- **Link mapping**: Automatic mapping of work item relationships
- **Extensible strategies**: Custom mapping logic via strategy pattern

## Installation

```bash
dotnet add package Qwiq.Core
dotnet add package Qwiq.Client.Rest  # or Qwiq.Client.Soap
dotnet add package Qwiq.Mapper
```

## Quick Start

### Define Your Model

```csharp
using Qwiq.Mapper.Attributes;

[WorkItemType("Bug")]
public class Bug : IIdentifiable<int?>
{
    [FieldDefinition("System.Id")]
    public int? Id { get; set; }

    [FieldDefinition("System.Title")]
    public string Title { get; set; }

    [FieldDefinition("System.State")]
    public string State { get; set; }

    [FieldDefinition("Microsoft.VSTS.Common.Severity")]
    public string Severity { get; set; }

    [FieldDefinition("System.AssignedTo")]
    [IdentityField]
    public string AssignedTo { get; set; }

    [FieldDefinition("System.Description")]
    public string Description { get; set; }
}
```

### Map Work Items

```csharp
using Qwiq.Mapper;

var store = /* create your work item store */;
var mapper = new WorkItemMapper();

// Query and map
var workItems = store.Query("SELECT * FROM WorkItems WHERE [System.WorkItemType] = 'Bug'");
var bugs = workItems.Select(wi => mapper.Map<Bug>(wi));

foreach (var bug in bugs)
{
    Console.WriteLine($"{bug.Id}: {bug.Title} - Severity: {bug.Severity}");
}
```

## Mapping Attributes

### WorkItemType

Specifies the TFS work item type this class represents:

```csharp
[WorkItemType("Bug")]
public class Bug { }

[WorkItemType("User Story")]
public class UserStory { }
```

### FieldDefinition

Maps a property to a TFS field:

```csharp
[FieldDefinition("System.Title")]
public string Title { get; set; }

[FieldDefinition("Microsoft.VSTS.Common.Priority")]
public int Priority { get; set; }
```

### IdentityField

Marks a field for bulk identity resolution (improves performance):

```csharp
[FieldDefinition("System.AssignedTo")]
[IdentityField]
public string AssignedTo { get; set; }

[FieldDefinition("System.CreatedBy")]
[IdentityField]
public string CreatedBy { get; set; }
```

## Advanced Mapping

### Collection Properties

Map work item links to collections:

```csharp
[WorkItemType("User Story")]
public class UserStory : IIdentifiable<int?>
{
    [FieldDefinition("System.Id")]
    public int? Id { get; set; }

    [FieldDefinition("System.Title")]
    public string Title { get; set; }

    // Map child tasks
    public IEnumerable<Task> Tasks { get; set; }
}
```

### Custom Mapping Strategy

Implement `IWorkItemMapperStrategy` for complex scenarios:

```csharp
public class CustomBugMapper : IWorkItemMapperStrategy
{
    public object Map(Type targetType, IWorkItem workItem, IWorkItemMapper mapper)
    {
        var bug = new Bug
        {
            Id = workItem.Id,
            Title = workItem.Title,
            // Custom logic here
            IsHighPriority = DetermineHighPriority(workItem)
        };
        return bug;
    }

    private bool DetermineHighPriority(IWorkItem wi)
    {
        var severity = wi["Microsoft.VSTS.Common.Severity"]?.ToString();
        var priority = wi["Microsoft.VSTS.Common.Priority"]?.ToString();
        return severity == "1 - Critical" || priority == "1";
    }
}
```

### Bulk Identity Resolution

Use `BulkIdentityAwareAttributeMapperStrategy` for efficient identity field mapping:

```csharp
var identityService = /* get identity service */;
var strategy = new BulkIdentityAwareAttributeMapperStrategy(identityService);
var mapper = new WorkItemMapper(strategy);

// All identity fields will be resolved in bulk
var bugs = workItems.Select(wi => mapper.Map<Bug>(wi));
```

## Integration with LINQ

Combine with Qwiq.Linq for powerful queries:

```csharp
using Qwiq.Linq;

var context = new WorkItemStoreContext(store);
var mapper = new WorkItemMapper();

var criticalBugs = context.WorkItems
    .Where(wi => wi.Type == "Bug")
    .Where(wi => wi["Microsoft.VSTS.Common.Severity"] == "1 - Critical")
    .ToList()
    .Select(wi => mapper.Map<Bug>(wi));
```

## Type Conversion

The mapper handles common type conversions automatically:

```csharp
[FieldDefinition("System.CreatedDate")]
public DateTime CreatedDate { get; set; }

[FieldDefinition("Microsoft.VSTS.Common.Priority")]
public int Priority { get; set; }

[FieldDefinition("Microsoft.VSTS.Scheduling.StoryPoints")]
public double? StoryPoints { get; set; }
```

## Error Handling

Handle mapping errors gracefully:

```csharp
try
{
    var bug = mapper.Map<Bug>(workItem);
}
catch (AttributeMapException ex)
{
    Console.WriteLine($"Mapping failed for field: {ex.FieldName}");
    Console.WriteLine($"Reason: {ex.Message}");
}
```

## Best Practices

1. **Use IdentityField**: Mark identity fields for bulk resolution
2. **Nullable types**: Use nullable types for optional fields
3. **Immutable models**: Consider making mapped objects immutable
4. **Validation**: Add validation logic in property setters
5. **Caching**: Cache mapper instances for better performance

## Related Packages

- **Qwiq.Core**: Core interfaces (required)
- **Qwiq.Linq**: LINQ query provider
- **Qwiq.Identity**: Identity management services
- **Qwiq.Mapper.Identity**: Identity-aware mapping strategies

## Documentation

- [GitHub Repository](https://github.com/rjmurillo/Qwiq)
- [Mapping Examples](https://github.com/rjmurillo/Qwiq/wiki/Mapping-Examples)
- [Custom Strategies](https://github.com/rjmurillo/Qwiq/wiki/Custom-Mapping-Strategies)

## License

MIT License - see [LICENSE](https://github.com/rjmurillo/Qwiq/blob/master/LICENSE) for details.
