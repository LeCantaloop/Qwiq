# Qwiq.Linq Component Guide

## Component Overview

**Qwiq.Linq** provides LINQ query support for work items by translating LINQ expressions to WIQL (Work Item Query Language). This enables developers to write type-safe queries using C# LINQ syntax.

## Purpose

- Translate C# LINQ expressions to WIQL query strings
- Provide WIQL-specific extension methods (AsOf, WasEver, InGroup, etc.)
- Map .NET property names to TFS field reference names
- Enable compile-time query validation

## Key Characteristics

- **Target Frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
- **Platform**: Cross-platform
- **Dependencies**: Qwiq.Core
- **Pattern**: Query Provider pattern (IQueryable&lt;T&gt;)

## Architecture

### LINQ Translation Pipeline

```text
C# LINQ Expression
    ↓
Query<T> (implements IQueryable<T>)
    ↓
WiqlQueryProvider (IQueryProvider)
    ↓
QueryRewriter (ExpressionVisitor)
    ↓
WiqlTranslator (generates WIQL string)
    ↓
IWorkItemStore.Query(wiql)
    ↓
Work Item Results
```

### Key Types

| Type                | Purpose                                   | Location             |
| ------------------- | ----------------------------------------- | -------------------- |
| `Query<T>`          | Entry point, implements IOrderedQueryable | `Query.cs`           |
| `WiqlQueryProvider` | Orchestrates translation                  | Provider class       |
| `QueryRewriter`     | Transforms expression tree                | `Visitors/`          |
| `WiqlTranslator`    | Generates WIQL from expression            | `WiqlTranslator.cs`  |
| `IFieldMapper`      | Maps properties to field names            | `IFieldMapper.cs`    |
| `TranslatedQuery`   | Result containing WIQL + context          | `TranslatedQuery.cs` |

## Key Entry Points

### Creating Queries

```csharp
var query = new Query<WorkItem>(store);
var results = query
    .Where(wi => wi.State == "Active")
    .OrderBy(wi => wi.ChangedDate)
    .ToList();
```

### Field Mapping

Use `IFieldMapper` to map property names to field references:

```csharp
public class SimpleFieldMapper : IFieldMapper
{
    public string GetFieldName(string propertyName)
    {
        // Map C# property to TFS field
        return propertyName switch
        {
            "Id" => CoreFieldRefNames.Id,
            "Title" => CoreFieldRefNames.Title,
            _ => propertyName
        };
    }
}
```

## WIQL Extension Methods

### AsOf - Historical Queries

```csharp
// Get work items as they existed at a specific time
var historicalQuery = query.AsOf(DateTime.UtcNow.AddDays(-7));
```

### WasEver - Historical State Checks

```csharp
// Find items that were ever in "Active" state
var items = query.Where(wi => wi.State.WasEver("Active"));
```

### InGroup / NotInGroup - Group Membership

```csharp
// Check if assigned to group members
var items = query.Where(wi => wi.AssignedTo.InGroup("[Project]\\Contributors"));
var items = query.Where(wi => wi.AssignedTo.NotInGroup("[Project]\\Readers"));
```

## Supported LINQ Operations

### Fully Supported

✅ **Where** - All standard comparison operators
✅ **OrderBy / OrderByDescending / ThenBy** - Multiple sort keys
✅ **Take** - Limit result count
✅ **Contains** - With arrays and IEnumerable&lt;T&gt;

### Partially Supported

⚠️ **Select** - Always returns full work items (no field projection)
⚠️ **String methods** - StartsWith, EndsWith, Contains (case-sensitive only)

### Not Supported

❌ **String case methods** - ToUpper, ToLower, ToUpperInvariant
❌ **Aggregations** - Count, Sum, Max, Min, Average
❌ **Joins** - Join, GroupJoin
❌ **Grouping** - GroupBy
❌ **Some collections in Contains** - Collection&lt;T&gt;, HashSet&lt;T&gt; (use arrays or IEnumerable&lt;T&gt;)

## Testing Guidelines

### Test Expression Translation

```csharp
[TestMethod]
public void Should_translate_where_clause()
{
    var query = new Query<WorkItem>(mockStore)
        .Where(wi => wi.State == "Active");

    var wiql = WiqlTranslator.Translate(query.Expression);
    wiql.ShouldContain("WHERE [System.State] = 'Active'");
}
```

### Test Field Mapping

```csharp
[TestMethod]
public void Should_map_property_to_field()
{
    var mapper = new SimpleFieldMapper();
    var fieldName = mapper.GetFieldName("Title");
    fieldName.ShouldBe(CoreFieldRefNames.Title);
}
```

## Common Patterns

### Custom Field Mapping

```csharp
public class CustomFieldMapper : IFieldMapper
{
    private readonly Dictionary<string, string> _mappings = new()
    {
        ["Id"] = CoreFieldRefNames.Id,
        ["Title"] = CoreFieldRefNames.Title,
        ["CustomField"] = "MyCompany.CustomField"
    };

    public string GetFieldName(string propertyName)
    {
        return _mappings.TryGetValue(propertyName, out var fieldName)
            ? fieldName
            : propertyName;
    }
}
```

### Cached Field Mapping

Use `CachingFieldMapper` to improve performance:

```csharp
var cachedMapper = new CachingFieldMapper(baseMapper);
```

### Query Rewriting

The `QueryRewriter` visitor transforms LINQ expressions:

- Converts method calls to WIQL equivalents
- Resolves constants and parameters
- Handles nested expressions

## Directory Structure

- **Root**: Main query provider and translator
- **Visitors/**: Expression tree visitors (QueryRewriter, etc.)
- **Fragments/**: WIQL query fragments
- **WiqlExpressions/**: WIQL-specific expression nodes

## Expression Tree Transformation

### Example: Where Clause

```csharp
// C# LINQ
wi => wi.State == "Active"

// Expression Tree
BinaryExpression(Equal,
    MemberExpression(wi, "State"),
    ConstantExpression("Active"))

// WIQL
WHERE [System.State] = 'Active'
```

### Example: Contains

```csharp
// C# LINQ
wi => new[] { "Bug", "Task" }.Contains(wi.Type)

// WIQL
WHERE [System.WorkItemType] IN ('Bug', 'Task')
```

## Integration with Qwiq.Mapper

The `Qwiq.Mapper` component extends this with attribute-based field mapping:

```csharp
[FieldDefinition("System.Title")]
public string Title { get; set; }
```

When using with mapper, use `MapperTeamFoundationServerWorkItemQueryProvider`.

## Common Mistakes to Avoid

❌ **Don't use unsupported operations** - Causes `NotSupportedException`

```csharp
// ❌ WRONG: ToUpper not supported
query.Where(wi => wi.State.ToUpper() == "ACTIVE")

// ✅ CORRECT: Direct comparison
query.Where(wi => wi.State == "Active")
```

❌ **Don't expect field projection in Select**

```csharp
// ❌ WRONG: Expects only Title field
var titles = query.Select(wi => wi.Title).ToList();
// Always returns full WorkItem objects

// ✅ CORRECT: Select after materialization
var titles = query.ToList().Select(wi => wi.Title);
```

❌ **Don't use Collection&lt;T&gt; or HashSet&lt;T&gt; in Contains**

```csharp
// ❌ WRONG: HashSet not supported
var states = new HashSet<string> { "Active", "Closed" };
query.Where(wi => states.Contains(wi.State))

// ✅ CORRECT: Use array or IEnumerable<T>
var states = new[] { "Active", "Closed" };
query.Where(wi => states.Contains(wi.State))
```

✅ **Do use constants from CoreFieldRefNames** - For field mapper implementations

✅ **Do cache field mappings** - Use CachingFieldMapper for performance

✅ **Do test complex queries** - Verify WIQL generation

✅ **Do handle NotSupportedException** - For operations that can't translate

## Error Handling

### NotSupportedException

Thrown when LINQ operation cannot be translated to WIQL:

```csharp
try
{
    var results = query.Where(wi => wi.Title.ToUpper().Contains("BUG"));
}
catch (NotSupportedException ex)
{
    // Handle unsupported operation
}
```

### ArgumentException

Thrown for invalid WIQL queries:

```csharp
// Empty or malformed WIQL
store.Query(""); // Throws ArgumentException
```

## Performance Considerations

- **Field mapping is called frequently** - Use caching
- **Expression tree traversal is recursive** - Deep expressions may impact performance
- **WIQL is executed server-side** - Use filters in Where clause, not post-materialization

## Related Components

- **Qwiq.Core** - Base interfaces and store
- **Qwiq.Mapper** - Attribute-based field mapping (extends this)
- **Qwiq.Linq.Identity** - Identity-specific query extensions

## Debugging Tips

### View Generated WIQL

```csharp
var query = store.Query<WorkItem>()
    .Where(wi => wi.State == "Active");

var translated = WiqlTranslator.Translate(query.Expression);
Console.WriteLine(translated.Wiql);
```

### Expression Tree Inspection

Use `Expression.ToString()` to see the expression tree structure during debugging.

### Common Translation Issues

- **String comparison case** - WIQL is case-insensitive by default
- **Date formatting** - WIQL requires ISO 8601 format
- **Field names** - Must match TFS reference names exactly
