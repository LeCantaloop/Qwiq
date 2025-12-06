# Qwiq.Mapper.Identity

Identity-aware mapping strategies for converting Azure DevOps / TFS work items to strongly-typed objects.

## Overview

This package extends [Qwiq.Mapper](https://www.nuget.org/packages/Qwiq.Mapper/) with identity-aware mapping strategies that automatically resolve identity fields during object mapping. It provides bulk identity resolution for optimal performance when mapping collections of work items.

## Key Features

- 🚀 **Bulk Identity Resolution** - Resolve all identity fields in a single batch operation
- 🎯 **Attribute-Based Configuration** - Mark identity fields with `[IdentityField]` attribute
- ⚡ **Performance Optimized** - Minimize API calls through batching
- 🔄 **Transparent Integration** - Works seamlessly with existing mapper patterns

## Installation

```powershell
dotnet add package Qwiq.Mapper.Identity
```

**Prerequisites:**

- [Qwiq.Mapper](https://www.nuget.org/packages/Qwiq.Mapper/)
- [Qwiq.Identity](https://www.nuget.org/packages/Qwiq.Identity/)

**Target Frameworks:** net472, net8.0

## Quick Start

```csharp
using Qwiq.Mapper;
using Qwiq.Mapper.Attributes;
using Qwiq.Identity;

// Define model with identity fields
[WorkItemType("Task")]
public class Task : IIdentifiable<int?>
{
    [FieldDefinition("System.Id")]
    public int? Id { get; set; }

    [FieldDefinition("System.AssignedTo")]
    [IdentityField]  // Marks for bulk resolution
    public string? AssignedTo { get; set; }

    [FieldDefinition("System.ChangedBy")]
    [IdentityField]
    public string? ChangedBy { get; set; }
}

// Use identity-aware mapper
var store = WorkItemStoreFactory.Default.Create(options);
var mapper = new WorkItemMapper(store);

// Query and map with bulk identity resolution
var workItems = store.Query("SELECT [System.Id], [System.AssignedTo], [System.ChangedBy] FROM WorkItems");
var tasks = workItems.Select(wi => mapper.Create<Task>(wi));

// All identity fields resolved in single batch operation
foreach (var task in tasks)
{
    Console.WriteLine($"Task {task.Id}: Assigned to {task.AssignedTo}");
}
```

## Identity Resolution Strategies

### BulkIdentityAwareAttributeMapperStrategy

The core strategy that detects and resolves identity fields:

```csharp
// Automatically used by WorkItemMapper when identity service is available
var mapper = new WorkItemMapper(store);

// The mapper detects [IdentityField] attributes and:
// 1. Collects all unique identity values
// 2. Resolves them in a single batch operation
// 3. Maps resolved identities to target objects
var result = mapper.Create<Task>(workItem);
```

### Manual Strategy Configuration

```csharp
using Qwiq.Mapper.Strategies;

// Create custom mapper with specific strategies
var strategies = new IWorkItemMapperStrategy[]
{
    new BulkIdentityAwareAttributeMapperStrategy(identityService),
    new AttributeMapperStrategy(),
    new WorkItemLinksMapperStrategy()
};

var mapper = new WorkItemMapper(strategies);
```

## Performance Comparison

```csharp
// ❌ WITHOUT bulk resolution: N API calls
foreach (var workItem in workItems)
{
    var assignedTo = workItem["System.AssignedTo"];
    var identity = identityService.ReadIdentity(IdentitySearchFactor.DisplayName, assignedTo);
    // Separate API call per identity field per work item
}

// ✅ WITH bulk resolution: 1 API call
var mapper = new WorkItemMapper(store);
var tasks = workItems.Select(wi => mapper.Create<Task>(wi));
// All identities resolved in single batch operation
```

For 100 work items with 2 identity fields each:

- **Without bulk resolution**: 200 API calls
- **With bulk resolution**: 1 API call

## Advanced Usage

### Multiple Identity Fields

```csharp
[WorkItemType("Bug")]
public class Bug
{
    [FieldDefinition("System.AssignedTo")]
    [IdentityField]
    public string? AssignedTo { get; set; }

    [FieldDefinition("System.CreatedBy")]
    [IdentityField]
    public string? CreatedBy { get; set; }

    [FieldDefinition("System.ChangedBy")]
    [IdentityField]
    public string? ChangedBy { get; set; }

    [FieldDefinition("Microsoft.VSTS.Common.ResolvedBy")]
    [IdentityField]
    public string? ResolvedBy { get; set; }
}

// All 4 identity fields resolved in single batch
var bugs = workItems.Select(wi => mapper.Create<Bug>(wi));
```

### Custom Identity Processing

```csharp
[WorkItemType("UserStory")]
public class UserStory
{
    [FieldDefinition("System.AssignedTo")]
    [IdentityField]
    public string? AssignedTo { get; set; }

    // Derived property from identity field
    public string? AssignedToEmail =>
        GetIdentityEmail(AssignedTo);
}
```

## Best Practices

1. **Mark All Identity Fields**: Apply `[IdentityField]` to all properties containing user identity data
2. **Batch Map Operations**: Map collections rather than individual items
3. **Reuse Mapper Instances**: Create one mapper and reuse for efficiency
4. **Combine with LINQ**: Use with Qwiq.Linq for type-safe queries

```csharp
// ✅ GOOD: Bulk mapping
var mapper = new WorkItemMapper(store);
var results = workItems.Select(wi => mapper.Create<Bug>(wi)).ToList();

// ❌ AVOID: Individual mapping
foreach (var workItem in workItems)
{
    var mapper = new WorkItemMapper(store); // Creates new instance each time
    var bug = mapper.Create<Bug>(workItem); // Separate resolution per item
}
```

## Related Packages

- **[Qwiq.Mapper](https://www.nuget.org/packages/Qwiq.Mapper/)** - Core object mapping functionality
- **[Qwiq.Identity](https://www.nuget.org/packages/Qwiq.Identity/)** - Identity resolution services
- **[Qwiq.Linq.Identity](https://www.nuget.org/packages/Qwiq.Linq.Identity/)** - LINQ with identity support

## Documentation

- [Repository](https://github.com/rjmurillo/Qwiq)
- [Mapper Documentation](https://github.com/rjmurillo/Qwiq/blob/master/docs/package-readme/Qwiq.Mapper.md)
- [Identity Documentation](https://github.com/rjmurillo/Qwiq/blob/master/docs/package-readme/Qwiq.Identity.md)

## License

[MIT License](https://github.com/rjmurillo/Qwiq/blob/master/LICENSE)
