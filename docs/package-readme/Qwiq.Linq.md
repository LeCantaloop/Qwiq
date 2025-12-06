# Qwiq.Linq

LINQ query provider for type-safe Azure DevOps and TFS work item queries.

## Overview

Qwiq.Linq provides a LINQ-to-WIQL (Work Item Query Language) provider that allows you to write type-safe queries using C# LINQ syntax instead of string-based WIQL.

## Key Features

- **Type-safe queries**: Compile-time checking for query syntax
- **IntelliSense support**: Full editor support for query construction
- **WIQL extension methods**: Support for TFS-specific operators (AsOf, WasEver, InGroup)
- **Automatic translation**: LINQ expressions converted to optimized WIQL

## Installation

```bash
dotnet add package Qwiq.Core
dotnet add package Qwiq.Client.Rest  # or Qwiq.Client.Soap
dotnet add package Qwiq.Linq
```

## Quick Start

### Basic Query

```csharp
using Qwiq;
using Qwiq.Linq;

var store = /* create your work item store */;
var context = new WorkItemStoreContext(store);

// LINQ query
var activeBugs = from wi in context.WorkItems
                 where wi.Type == "Bug"
                    && wi.State == "Active"
                 select wi;

foreach (var bug in activeBugs)
{
    Console.WriteLine($"{bug.Id}: {bug.Title}");
}
```

### Fluent Syntax

```csharp
var bugs = context.WorkItems
    .Where(wi => wi.Type == "Bug")
    .Where(wi => wi.State == "Active")
    .Where(wi => wi.AssignedTo == "John Doe");
```

## WIQL-Specific Extensions

### AsOf - Historical Queries

Query work items as they existed at a specific point in time:

```csharp
var lastWeek = DateTime.UtcNow.AddDays(-7);
var historicalBugs = context.WorkItems
    .Where(wi => wi.Type == "Bug")
    .AsOf(lastWeek);
```

### WasEver - Historical State

Find work items that ever had a specific value:

```csharp
var everActive = context.WorkItems
    .Where(wi => wi.State.WasEver("Active"));
```

### InGroup / NotInGroup - Group Membership

Query based on Azure DevOps group membership:

```csharp
var teamBugs = context.WorkItems
    .Where(wi => wi.Type == "Bug")
    .Where(wi => wi.AssignedTo.InGroup("[Project]\\Contributors"));

var externalBugs = context.WorkItems
    .Where(wi => wi.Type == "Bug")
    .Where(wi => wi.AssignedTo.NotInGroup("[Project]\\Team Members"));
```

## Supported LINQ Operations

### Supported

- `Where` - Filter conditions
- `Select` - Field projection (always returns full work item)
- `OrderBy` / `OrderByDescending` - Sorting
- `Take` - Limit results
- Standard comparison operators: `==`, `!=`, `<`, `>`, `<=`, `>=`
- String operations: `Contains`, `StartsWith`, `EndsWith`
- Collection operations: `Contains` on arrays and `IEnumerable<T>`

### Not Supported

These operations will throw `NotSupportedException`:

- `Count()`, `Sum()`, `Average()`, `Min()`, `Max()` - Use WIQL aggregation instead
- `GroupBy` - Not supported by WIQL
- `Join` - Use work item links instead
- String case methods: `ToUpper()`, `ToLower()`
- Complex projections in `Select`

## Field Mapping

Use the `IFieldMapper` interface to map .NET property names to TFS field names:

```csharp
public class BugFieldMapper : IFieldMapper
{
    public string Map(string propertyName) => propertyName switch
    {
        nameof(Bug.Id) => "System.Id",
        nameof(Bug.Title) => "System.Title",
        nameof(Bug.Severity) => "Microsoft.VSTS.Common.Severity",
        _ => propertyName
    };
}

var query = new Query<Bug>(store, new BugFieldMapper());
```

## Advanced Examples

### Complex Filtering

```csharp
var criticalBugs = context.WorkItems
    .Where(wi => wi.Type == "Bug")
    .Where(wi => wi["Microsoft.VSTS.Common.Severity"] == "1 - Critical")
    .Where(wi => wi.State != "Closed")
    .Where(wi => wi.ChangedDate > DateTime.UtcNow.AddDays(-30));
```

### Combining with Direct WIQL

You can still use direct WIQL when needed:

```csharp
var wiql = @"
    SELECT [System.Id], [System.Title]
    FROM WorkItems
    WHERE [System.TeamProject] = @project
      AND [System.WorkItemType] = 'Bug'
";

var workItems = store.Query(wiql, new { project = "MyProject" });
```

## Performance Tips

1. **Filter early**: Apply type and state filters first
2. **Limit fields**: WIQL always returns all fields, but filter in LINQ afterward
3. **Use pagination**: Apply `Take()` for large result sets
4. **Cache results**: Work items are immutable snapshots

## Related Packages

- **Qwiq.Core**: Core interfaces (required)
- **Qwiq.Client.Rest**: REST client implementation
- **Qwiq.Mapper**: Map work items to POCOs
- **Qwiq.Linq.Identity**: Identity-aware LINQ queries

## Documentation

- [GitHub Repository](https://github.com/rjmurillo/Qwiq)
- [WIQL Reference](https://learn.microsoft.com/en-us/azure/devops/boards/queries/wiql-syntax)
- [Query Examples](https://github.com/rjmurillo/Qwiq/wiki/Query-Examples)

## License

MIT License - see [LICENSE](https://github.com/rjmurillo/Qwiq/blob/master/LICENSE) for details.
