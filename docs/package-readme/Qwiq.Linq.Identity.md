# Qwiq.Linq.Identity

Identity-aware LINQ query extensions for Azure DevOps / Team Foundation Server work items.

## Overview

This package extends [Qwiq.Linq](https://www.nuget.org/packages/Qwiq.Linq/) with identity-aware query capabilities, enabling efficient resolution of identity fields during LINQ queries. It combines the type-safe LINQ query provider with bulk identity resolution for optimal performance.

## Key Features

- 🔍 **Bulk Identity Resolution** - Resolve identity fields for entire query result sets
- ⚡ **Performance Optimized** - Single batch operation instead of per-item resolution
- 🎯 **Seamless Integration** - Works transparently with existing LINQ queries
- 🧩 **Attribute-Based** - Mark identity fields with `[IdentityField]` attribute

## Installation

```powershell
dotnet add package Qwiq.Linq.Identity
```

**Prerequisites:**

- [Qwiq.Linq](https://www.nuget.org/packages/Qwiq.Linq/)
- [Qwiq.Identity](https://www.nuget.org/packages/Qwiq.Identity/)
- [Qwiq.Mapper](https://www.nuget.org/packages/Qwiq.Mapper/) (for attribute mapping)

**Target Frameworks:** net472, net8.0

## Quick Start

```csharp
using Qwiq;
using Qwiq.Linq;
using Qwiq.Mapper;
using Qwiq.Mapper.Attributes;

// Define model with identity field
[WorkItemType("Bug")]
public class Bug : IIdentifiable<int?>
{
    [FieldDefinition("System.Id")]
    public int? Id { get; set; }

    [FieldDefinition("System.AssignedTo")]
    [IdentityField]  // Enables bulk identity resolution
    public string? AssignedTo { get; set; }

    [FieldDefinition("System.CreatedBy")]
    [IdentityField]
    public string? CreatedBy { get; set; }
}

// Query with identity resolution
var store = WorkItemStoreFactory.Default.Create(options);
var mapper = new WorkItemMapper(store);

var bugs = store.Query<Bug>()
    .Where(b => b.State == "Active")
    .ToList();

// Map with bulk identity resolution
var mappedBugs = bugs.Select(wi => mapper.Create<Bug>(wi));

// Identity fields are efficiently resolved in bulk
foreach (var bug in mappedBugs)
{
    Console.WriteLine($"Assigned to: {bug.AssignedTo}");
}
```

## How It Works

The identity-aware mapper strategy detects `[IdentityField]` attributes and performs bulk resolution:

```csharp
// Traditional approach: N+1 queries
foreach (var workItem in workItems)
{
    // Separate identity lookup per work item
    var identity = identityService.ReadIdentity(workItem.AssignedTo);
}

// Identity-aware LINQ: Single batch operation
var mapper = new WorkItemMapper(store);
var results = mapper.Create<Bug>(workItems); // Resolves all identities in one call
```

## Best Practices

1. **Mark Identity Fields**: Use `[IdentityField]` attribute on properties containing user names
2. **Batch Operations**: Query multiple work items and map together for optimal performance
3. **Combine with LINQ**: Use type-safe LINQ queries before mapping

```csharp
// ✅ GOOD: Bulk resolution
var results = store.Query<Bug>()
    .Where(b => b.Priority == 1)
    .ToList()
    .Select(wi => mapper.Create<Bug>(wi))
    .ToList();

// ❌ AVOID: Per-item resolution
foreach (var workItem in workItems)
{
    var bug = mapper.Create<Bug>(workItem); // Resolves identity per item
}
```

## Related Packages

- **[Qwiq.Linq](https://www.nuget.org/packages/Qwiq.Linq/)** - LINQ query provider for work items
- **[Qwiq.Identity](https://www.nuget.org/packages/Qwiq.Identity/)** - Identity resolution services
- **[Qwiq.Mapper](https://www.nuget.org/packages/Qwiq.Mapper/)** - Attribute-based object mapping
- **[Qwiq.Mapper.Identity](https://www.nuget.org/packages/Qwiq.Mapper.Identity/)** - Identity-aware mapper strategies

## Documentation

- [Repository](https://github.com/rjmurillo/Qwiq)
- [LINQ Provider Documentation](https://github.com/rjmurillo/Qwiq/blob/master/docs/package-readme/Qwiq.Linq.md)
- [Identity Services Documentation](https://github.com/rjmurillo/Qwiq/blob/master/docs/package-readme/Qwiq.Identity.md)

## License

[MIT License](https://github.com/rjmurillo/Qwiq/blob/master/LICENSE)
