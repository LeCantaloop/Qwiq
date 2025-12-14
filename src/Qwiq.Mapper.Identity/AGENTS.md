# Qwiq.Mapper.Identity Component Guide

## Component Overview

**Qwiq.Mapper.Identity** integrates identity resolution with the work item mapping system, enabling bulk identity resolution during object mapping.

## Purpose

- Resolve identity fields during work item mapping
- Batch identity resolution for performance
- Convert identity field values to display names, emails, etc.
- Reduce round-trips to identity service

## Key Characteristics

- **Target Frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
- **Platform**: Cross-platform
- **Dependencies**: Qwiq.Core, Qwiq.Mapper, Qwiq.Identity

## Key Pattern: BulkIdentityAwareAttributeMapperStrategy

This strategy resolves all identity fields in a single batch call:

```csharp
var strategy = new BulkIdentityAwareAttributeMapperStrategy(identityService);
var mapper = new WorkItemMapper(strategy);

// All [IdentityField] properties resolved in one batch
var bugs = mapper.Create<Bug>(workItems);
```

## Marking Identity Fields

Use `[IdentityField]` attribute:

```csharp
[WorkItemType("Bug")]
public class Bug
{
    [FieldDefinition("System.AssignedTo")]
    [IdentityField]  // Will be resolved in bulk
    public string? AssignedTo { get; set; }

    [FieldDefinition("System.CreatedBy")]
    [IdentityField]  // Will be resolved in bulk
    public string? CreatedBy { get; set; }
}
```

## How It Works

1. Mapper collects all work items to map
2. Strategy identifies all `[IdentityField]` properties
3. Extracts unique identity values from all work items
4. Makes **one batch call** to `IIdentityManagementService.ReadIdentities()`
5. Replaces identity values with resolved display names

## Performance Benefits

```csharp
// Without BulkIdentityAwareAttributeMapperStrategy:
// - Maps 100 work items
// - Each has 2 identity fields
// - Makes 200 identity service calls (slow!)

// With BulkIdentityAwareAttributeMapperStrategy:
// - Maps 100 work items
// - Collects all unique identities
// - Makes 1 batch identity service call (fast!)
```

## Testing

```csharp
[TestMethod]
public void Should_resolve_identity_fields_in_bulk()
{
    var mockIdentityService = new MockIdentityManagementService();
    mockIdentityService.AddIdentity("user@domain.com", "User Name");

    var workItem = new MockWorkItem("Bug")
    {
        ["System.AssignedTo"] = "user@domain.com"
    };

    var strategy = new BulkIdentityAwareAttributeMapperStrategy(mockIdentityService);
    var mapper = new WorkItemMapper(strategy);
    var bug = mapper.Create<Bug>(new[] { workItem }).Single();

    bug.AssignedTo.ShouldBe("User Name");
}
```

## Related Components

- **Qwiq.Mapper** - Base mapping functionality
- **Qwiq.Identity** - Identity service interfaces
- **Qwiq.Core** - Work item interfaces

## Common Mistakes to Avoid

❌ **Don't use regular AttributeMapperStrategy for identity fields** - No resolution

```csharp
// ❌ WRONG: Won't resolve identities
var strategy = new AttributeMapperStrategy();

// ✅ CORRECT: Resolves identities in bulk
var strategy = new BulkIdentityAwareAttributeMapperStrategy(identityService);
```

❌ **Don't forget [IdentityField] attribute** - Won't be resolved

```csharp
// ❌ WRONG: Missing attribute
[FieldDefinition("System.AssignedTo")]
public string? AssignedTo { get; set; }

// ✅ CORRECT: Has attribute
[FieldDefinition("System.AssignedTo")]
[IdentityField]
public string? AssignedTo { get; set; }
```

✅ **Do use for any identity field** - AssignedTo, CreatedBy, ChangedBy, etc.

✅ **Do test bulk resolution** - Verify performance and correctness

✅ **Do provide identity service** - Required constructor parameter
