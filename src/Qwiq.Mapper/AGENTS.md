# Qwiq.Mapper Component Guide

## Component Overview

**Qwiq.Mapper** provides object mapping functionality to convert `IWorkItem` instances into strongly-typed POCOs (Plain Old CLR Objects) using attribute-based field definitions.

## Purpose

- Map work items to strongly-typed domain objects
- Use attributes to declare field mappings
- Support identity field resolution in bulk
- Enable custom mapping strategies
- Reduce boilerplate for field access

## Key Characteristics

- **Target Frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
- **Platform**: Cross-platform
- **Dependencies**: Qwiq.Core, Qwiq.Linq
- **Pattern**: Strategy pattern for mapping approaches

## Architecture

### Mapping Pipeline

```text
IWorkItem
    ↓
WorkItemMapper (orchestrator)
    ↓
IWorkItemMapperStrategy (strategy selection)
    ↓
AttributeMapperStrategy (attribute-based mapping)
    ↓
BulkIdentityAwareAttributeMapperStrategy (with identity resolution)
    ↓
Strongly-Typed POCO
```

### Key Types

| Type                                       | Purpose                              | Location                     |
| ------------------------------------------ | ------------------------------------ | ---------------------------- |
| `WorkItemMapper`                           | Main mapping orchestrator            | `WorkItemMapper.cs`          |
| `IWorkItemMapperStrategy`                  | Strategy interface                   | `IWorkItemMapperStrategy.cs` |
| `AttributeMapperStrategy`                  | Attribute-based mapping              | Strategy implementation      |
| `BulkIdentityAwareAttributeMapperStrategy` | Identity resolution                  | Strategy implementation      |
| `WorkItemLinksMapperStrategy`              | Link collection mapping              | Strategy implementation      |
| `FieldDefinitionAttribute`                 | Map property to field                | `Attributes/`                |
| `WorkItemTypeAttribute`                    | Declare work item type               | `Attributes/`                |
| `IdentityFieldAttribute`                   | Mark identity field for bulk resolve | `Attributes/`                |

## Key Entry Points

### Defining Mapped Types

```csharp
[WorkItemType("Bug")]
public class Bug : IIdentifiable<int?>
{
    [FieldDefinition("System.Id")]
    public int? Id { get; set; }

    [FieldDefinition("System.Title")]
    public string? Title { get; set; }

    [FieldDefinition("System.AssignedTo")]
    [IdentityField]  // Enables bulk identity resolution
    public string? AssignedTo { get; set; }

    [FieldDefinition("System.State")]
    public string? State { get; set; }
}
```

### Using the Mapper

```csharp
var mapper = new WorkItemMapper(identityService);
var bugs = mapper.Create<Bug>(workItems);
```

### With LINQ Queries

```csharp
var query = new Query<Bug>(store, mapper);
var activeBugs = query
    .Where(bug => bug.State == "Active")
    .ToList();
```

## Mapping Attributes

### [WorkItemType]

Declares the TFS work item type this class represents:

```csharp
[WorkItemType("Bug")]           // Maps to "Bug" work item type
[WorkItemType("Task")]          // Maps to "Task" work item type
[WorkItemType("User Story")]    // Maps to "User Story" work item type
```

### [FieldDefinition]

Maps a property to a TFS field reference name:

```csharp
[FieldDefinition("System.Id")]
public int? Id { get; set; }

[FieldDefinition("System.Title")]
public string? Title { get; set; }

[FieldDefinition("MyCompany.CustomField")]
public string? CustomField { get; set; }
```

**Use CoreFieldRefNames constants** for well-known fields:

```csharp
// ✅ CORRECT: Using constant
[FieldDefinition(CoreFieldRefNames.Title)]
public string? Title { get; set; }

// ❌ WRONG: Magic string
[FieldDefinition("System.Title")]
public string? Title { get; set; }
```

### [IdentityField]

Marks fields for bulk identity resolution:

```csharp
[FieldDefinition("System.AssignedTo")]
[IdentityField]  // Identity will be resolved in bulk
public string? AssignedTo { get; set; }

[FieldDefinition("System.CreatedBy")]
[IdentityField]
public string? CreatedBy { get; set; }
```

**Benefits**:

- Single batch call to identity service for all identity fields
- Improves performance when mapping many work items
- Resolves display names, email addresses, etc.

## Mapping Strategies

### AttributeMapperStrategy

Default strategy using attributes:

```csharp
var strategy = new AttributeMapperStrategy();
var mapper = new WorkItemMapper(strategy);
var items = mapper.Create<MyType>(workItems);
```

### BulkIdentityAwareAttributeMapperStrategy

Includes identity resolution:

```csharp
var strategy = new BulkIdentityAwareAttributeMapperStrategy(identityService);
var mapper = new WorkItemMapper(strategy);
var items = mapper.Create<MyType>(workItems);
```

### Custom Strategies

Implement `IWorkItemMapperStrategy` for custom mapping:

```csharp
public class CustomMappingStrategy : IWorkItemMapperStrategy
{
    public IEnumerable<T> Create<T>(IEnumerable<IWorkItem> workItems)
    {
        foreach (var wi in workItems)
        {
            yield return CreateInstance<T>(wi);
        }
    }

    private T CreateInstance<T>(IWorkItem workItem)
    {
        // Custom mapping logic
    }
}
```

## Testing Guidelines

### Testing Mapped Types

```csharp
[TestMethod]
public void Should_map_work_item_to_bug()
{
    var workItem = new MockWorkItem("Bug")
    {
        Id = 123,
        Title = "Test Bug",
        ["System.State"] = "Active"
    };

    var mapper = new WorkItemMapper();
    var bug = mapper.Create<Bug>(new[] { workItem }).Single();

    bug.Id.ShouldBe(123);
    bug.Title.ShouldBe("Test Bug");
    bug.State.ShouldBe("Active");
}
```

### Testing Identity Resolution

```csharp
[TestMethod]
public void Should_resolve_identity_fields_in_bulk()
{
    var identityService = new MockIdentityManagementService();
    identityService.AddIdentity("user@domain.com", "User Name");

    var workItem = new MockWorkItem("Bug")
    {
        ["System.AssignedTo"] = "user@domain.com"
    };

    var strategy = new BulkIdentityAwareAttributeMapperStrategy(identityService);
    var mapper = new WorkItemMapper(strategy);
    var bug = mapper.Create<Bug>(new[] { workItem }).Single();

    bug.AssignedTo.ShouldBe("User Name");
}
```

## Common Patterns

### IIdentifiable Interface

Mapped types should implement `IIdentifiable<T>` for tracking:

```csharp
public class MyWorkItem : IIdentifiable<int?>
{
    [FieldDefinition("System.Id")]
    public int? Id { get; set; }
}
```

### Nullable Properties

Use nullable types for optional fields:

```csharp
public class Bug
{
    [FieldDefinition("System.Id")]
    public int? Id { get; set; }  // int? - ID might be null for new items

    [FieldDefinition("System.Title")]
    public string? Title { get; set; }  // string? - Title might be missing
}
```

### Collection Properties

Map link collections:

```csharp
public class Bug
{
    public IEnumerable<ILink>? Links { get; set; }
    public IEnumerable<IRelatedLink>? RelatedLinks { get; set; }
}
```

## Integration with LINQ

The `MapperTeamFoundationServerWorkItemQueryProvider` combines LINQ with mapping:

```csharp
var query = new Query<Bug>(
    store,
    new MapperTeamFoundationServerWorkItemQueryProvider(mapper, fieldMapper)
);

var results = query.Where(b => b.State == "Active").ToList();
// Results are already mapped to Bug type
```

## Common Mistakes to Avoid

❌ **Don't use magic strings for standard fields**

```csharp
// ❌ WRONG: Magic string
[FieldDefinition("System.Title")]

// ✅ CORRECT: Constant
[FieldDefinition(CoreFieldRefNames.Title)]
```

❌ **Don't forget [WorkItemType] attribute**

```csharp
// ❌ WRONG: Missing WorkItemType
public class Bug { }

// ✅ CORRECT: Declares type
[WorkItemType("Bug")]
public class Bug { }
```

❌ **Don't map every field manually when using attributes**

```csharp
// ❌ WRONG: Manual mapping when attributes exist
var bug = new Bug
{
    Id = (int?)workItem["System.Id"],
    Title = (string?)workItem["System.Title"]
};

// ✅ CORRECT: Use mapper
var bug = mapper.Create<Bug>(new[] { workItem }).Single();
```

❌ **Don't resolve identities one at a time**

```csharp
// ❌ WRONG: Individual identity calls
foreach (var workItem in workItems)
{
    var identity = identityService.ReadIdentity(workItem.AssignedTo);
}

// ✅ CORRECT: Bulk resolution with BulkIdentityAwareAttributeMapperStrategy
var strategy = new BulkIdentityAwareAttributeMapperStrategy(identityService);
```

✅ **Do use CoreFieldRefNames constants** - Avoid magic strings

✅ **Do implement IIdentifiable&lt;T&gt;** - For trackable entities

✅ **Do use nullable types** - For optional fields

✅ **Do use BulkIdentityAwareAttributeMapperStrategy** - When mapping identity fields

✅ **Do test mapping with MockWorkItem** - Verify attribute configuration

## Error Handling

### AttributeMapException

Thrown when mapping fails:

```csharp
try
{
    var items = mapper.Create<MyType>(workItems);
}
catch (AttributeMapException ex)
{
    // Field name mismatch, type conversion failure, etc.
    Console.WriteLine($"Mapping failed: {ex.Message}");
}
```

### Common Causes

- Field reference name doesn't exist in work item
- Type conversion fails (e.g., string to int)
- Required field is missing
- Invalid [FieldDefinition] reference name

## Performance Considerations

- **Use BulkIdentityAwareAttributeMapperStrategy** - Batches identity resolution
- **Map only needed fields** - Don't declare unused properties
- **Cache mapper instances** - Reuse across queries
- **Use IEnumerable&lt;T&gt;** - Supports deferred execution

## Related Components

- **Qwiq.Core** - Base work item interfaces
- **Qwiq.Linq** - Query provider for mapped types
- **Qwiq.Mapper.Identity** - Identity-specific mapping extensions
- **Qwiq.Mocks** - Mock work items for testing

## Advanced Topics

### Custom Type Converters

Field values are converted using `ITypeParser`:

```csharp
public interface ITypeParser
{
    bool TryParse(Type type, object? value, out object? result);
}
```

Implement for custom type conversions.

### Validation

Use `IAnnotatedPropertyValidator` for validation:

```csharp
public interface IAnnotatedPropertyValidator
{
    void Validate(object instance);
}
```

Validates that required fields are populated after mapping.

### Work Item Links

Use `WorkItemLinksMapperStrategy` to map link collections:

```csharp
var strategy = new WorkItemLinksMapperStrategy();
// Maps Links, RelatedLinks, Hyperlinks properties
```
