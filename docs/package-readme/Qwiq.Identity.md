# Qwiq.Identity

Identity resolution and management services for Azure DevOps and Team Foundation Server.

## Overview

Qwiq.Identity provides services for resolving and managing identities in Azure DevOps and TFS, including conversion between different identity representations (display names, user principal names, domain accounts) and team membership queries.

## Key Features

- **Identity resolution**: Convert between display names, UPNs, and identity descriptors
- **Bulk operations**: Efficient batch identity resolution
- **Team membership**: Query team and group membership
- **Multiple formats**: Support for various identity string formats

## Installation

```bash
dotnet add package Qwiq.Core
dotnet add package Qwiq.Client.Rest  # or Qwiq.Client.Soap
dotnet add package Qwiq.Identity
```

## Quick Start

### Get Identity Service

```csharp
using Qwiq.Identity;

var store = /* create your work item store */;
var identityService = store.GetService<IIdentityManagementService>();
```

### Read Identities

```csharp
// By display name
var identity = identityService.ReadIdentity(
    IdentitySearchFactor.DisplayName,
    "John Doe"
);

Console.WriteLine($"UPN: {identity.UniqueName}");
Console.WriteLine($"Email: {identity.Attribute("Mail")}");

// By user principal name
var identity2 = identityService.ReadIdentity(
    IdentitySearchFactor.AccountName,
    "john.doe@contoso.com"
);
```

### Bulk Identity Resolution

```csharp
var displayNames = new[] { "John Doe", "Jane Smith", "Bob Johnson" };

var identities = identityService.ReadIdentities(
    IdentitySearchFactor.DisplayName,
    displayNames
);

foreach (var identity in identities)
{
    Console.WriteLine($"{identity.DisplayName} - {identity.UniqueName}");
}
```

## Identity Search Factors

The `IdentitySearchFactor` enum specifies how to search for identities:

```csharp
public enum IdentitySearchFactor
{
    // Display name (e.g., "John Doe")
    DisplayName,

    // User principal name (e.g., "john.doe@contoso.com")
    AccountName,

    // Domain account (e.g., "CONTOSO\\johndoe")
    Alias,

    // Team Foundation identity GUID
    Identifier,

    // Email address
    MailAddress
}
```

## Working with Teams

### Get Team Members

```csharp
var teamService = store.GetService<ICommonStructureService>();

// Get project
var project = teamService.GetProject("MyProject");

// Get team
var team = teamService.GetTeam(project.Uri, "MyTeam");

// Get team members
var members = identityService.ReadIdentities(
    IdentitySearchFactor.Identifier,
    team.Members.Select(m => m.TeamFoundationId.ToString())
);
```

## Identity Descriptors

Identity descriptors provide a stable reference to identities:

```csharp
var descriptor = identity.Descriptor;

Console.WriteLine($"Identifier: {descriptor.Identifier}");
Console.WriteLine($"Identity Type: {descriptor.IdentityType}");

// Use descriptor to get identity
var sameIdentity = identityService.ReadIdentity(
    IdentitySearchFactor.Identifier,
    descriptor.Identifier
);
```

## Use with Mapper

Combine with Qwiq.Mapper for automatic identity field resolution:

```csharp
using Qwiq.Mapper;
using Qwiq.Mapper.Identity;

var identityService = store.GetService<IIdentityManagementService>();
var strategy = new BulkIdentityAwareAttributeMapperStrategy(identityService);
var mapper = new WorkItemMapper(strategy);

[WorkItemType("Bug")]
public class Bug
{
    [FieldDefinition("System.Id")]
    public int? Id { get; set; }

    [FieldDefinition("System.AssignedTo")]
    [IdentityField]  // Will be resolved to full identity
    public string AssignedTo { get; set; }

    [FieldDefinition("System.CreatedBy")]
    [IdentityField]
    public string CreatedBy { get; set; }
}

// All identity fields resolved in one batch operation
var bugs = workItems.Select(wi => mapper.Map<Bug>(wi));
```

## Caching

Identity lookups can be expensive. Consider caching results:

```csharp
public class CachedIdentityService
{
    private readonly IIdentityManagementService _inner;
    private readonly ConcurrentDictionary<string, IIdentity> _cache = new();

    public CachedIdentityService(IIdentityManagementService inner)
    {
        _inner = inner;
    }

    public IIdentity ReadIdentity(IdentitySearchFactor factor, string value)
    {
        var key = $"{factor}:{value}";
        return _cache.GetOrAdd(key, _ => _inner.ReadIdentity(factor, value));
    }
}
```

## Error Handling

Handle identity resolution failures:

```csharp
try
{
    var identity = identityService.ReadIdentity(
        IdentitySearchFactor.DisplayName,
        "Unknown User"
    );
}
catch (IdentityNotFoundException ex)
{
    Console.WriteLine($"Identity not found: {ex.SearchValue}");
}
```

## Best Practices

1. **Use bulk operations**: Always prefer `ReadIdentities` over multiple `ReadIdentity` calls
2. **Cache results**: Identity data changes infrequently
3. **Handle nulls**: Not all identities may be resolvable
4. **Choose right factor**: Use `Identifier` for stable lookups, `DisplayName` for user-facing
5. **Check permissions**: Ensure service account has identity read permissions

## Common Patterns

### Display Name to Email

```csharp
string GetEmailFromDisplayName(string displayName)
{
    var identity = identityService.ReadIdentity(
        IdentitySearchFactor.DisplayName,
        displayName
    );
    return identity?.Attribute("Mail") ?? "unknown@contoso.com";
}
```

### Validate Team Membership

```csharp
bool IsTeamMember(string userDisplayName, string teamName)
{
    var team = /* get team */;
    var members = identityService.ReadIdentities(
        IdentitySearchFactor.Identifier,
        team.Members.Select(m => m.TeamFoundationId.ToString())
    );

    return members.Any(m => m.DisplayName == userDisplayName);
}
```

## Related Packages

- **Qwiq.Core**: Core interfaces (required)
- **Qwiq.Client.Rest**: REST client implementation
- **Qwiq.Identity.Soap**: SOAP-specific identity services
- **Qwiq.Mapper.Identity**: Identity-aware mapping

## Documentation

- [GitHub Repository](https://github.com/rjmurillo/Qwiq)
- [Identity Management Examples](https://github.com/rjmurillo/Qwiq/wiki/Identity-Examples)

## License

MIT License - see [LICENSE](https://github.com/rjmurillo/Qwiq/blob/master/LICENSE) for details.
