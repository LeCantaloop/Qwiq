# Qwiq.Linq.Identity Component Guide

## Component Overview

**Qwiq.Linq.Identity** provides LINQ query extensions for identity-specific operations in work item queries.

## Purpose

- Identity-aware LINQ query extensions
- Support for `InGroup()` and `NotInGroup()` WIQL operators
- Enable identity-based filtering in type-safe LINQ queries

## Key Characteristics

- **Target Frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
- **Platform**: Cross-platform
- **Dependencies**: Qwiq.Core, Qwiq.Linq, Qwiq.Identity

## Key Extension Methods

### InGroup

Filter by group membership in WIQL:

```csharp
var query = store.Query<WorkItem>()
    .Where(wi => wi.AssignedTo.InGroup("[Project]\\Contributors"));
```

Generates WIQL:

```sql
WHERE [System.AssignedTo] IN GROUP '[Project]\Contributors'
```

### NotInGroup

Exclude group members:

```csharp
var query = store.Query<WorkItem>()
    .Where(wi => wi.AssignedTo.NotInGroup("[Project]\\Readers"));
```

Generates WIQL:

```sql
WHERE [System.AssignedTo] NOT IN GROUP '[Project]\Readers'
```

## Common Patterns

### Query by Team Membership

```csharp
var teamWork = store.Query<WorkItem>()
    .Where(wi => wi.AssignedTo.InGroup($"[{projectName}]\\{teamName}"));
```

### Exclude System Accounts

```csharp
var userWork = store.Query<WorkItem>()
    .Where(wi => wi.CreatedBy.NotInGroup("[Server]\\Service Accounts"));
```

## Testing

Test WIQL generation:

```csharp
[TestMethod]
public void Should_generate_in_group_clause()
{
    var query = new Query<WorkItem>(mockStore)
        .Where(wi => wi.AssignedTo.InGroup("[Project]\\Team"));

    var wiql = WiqlTranslator.Translate(query.Expression);
    wiql.ShouldContain("IN GROUP");
}
```

## Related Components

- **Qwiq.Linq** - Base LINQ provider (extends with identity operators)
- **Qwiq.Identity** - Identity services
- **Qwiq.Core** - Core work item interfaces

## Common Mistakes to Avoid

❌ **Don't use string methods with InGroup** - Not supported

```csharp
// ❌ WRONG: String operations not supported in WIQL
.Where(wi => wi.AssignedTo.ToUpper().InGroup("..."))

// ✅ CORRECT: Direct field usage
.Where(wi => wi.AssignedTo.InGroup("..."))
```

✅ **Do use correct group path format** - `[Project]\GroupName`

✅ **Do test WIQL generation** - Verify correct syntax
