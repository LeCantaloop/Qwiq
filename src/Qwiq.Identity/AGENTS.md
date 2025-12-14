# Qwiq.Identity Component Guide

## Component Overview

**Qwiq.Identity** provides identity management services for resolving user and group information in Azure DevOps / TFS.

## Purpose

- Resolve user identities (display names, emails, unique names)
- Query group memberships
- Batch identity resolution for performance
- Abstract identity services from both REST and SOAP clients

## Key Characteristics

- **Target Frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
- **Platform**: Cross-platform
- **Dependencies**: Qwiq.Core
- **Pattern**: Service interface for identity operations

## Key Interfaces

### IIdentityManagementService

Primary interface for identity operations:

```csharp
public interface IIdentityManagementService
{
    ITeamFoundationIdentity ReadIdentity(string identity);
    IEnumerable<ITeamFoundationIdentity> ReadIdentities(IEnumerable<string> identities);
    // Additional methods for groups, search, etc.
}
```

### ITeamFoundationIdentity

Represents a user or group:

```csharp
public interface ITeamFoundationIdentity
{
    string DisplayName { get; }
    string UniqueName { get; }
    IdentityDescriptor Descriptor { get; }
    bool IsContainer { get; }  // True for groups
}
```

## Common Patterns

### Bulk Identity Resolution

Always resolve identities in bulk for performance:

```csharp
// ✅ CORRECT: Bulk resolution
var identities = identityService.ReadIdentities(userNames);

// ❌ WRONG: Individual calls
foreach (var userName in userNames)
{
    var identity = identityService.ReadIdentity(userName);
}
```

### Display Name Resolution

```csharp
var identity = identityService.ReadIdentity("user@domain.com");
var displayName = identity?.DisplayName ?? "Unknown User";
```

## Testing

Use `MockIdentityManagementService` from `Qwiq.Mocks`:

```csharp
var mockService = new MockIdentityManagementService();
mockService.AddIdentity("user@domain.com", "User Name");

var identity = mockService.ReadIdentity("user@domain.com");
identity.DisplayName.ShouldBe("User Name");
```

## Related Components

- **Qwiq.Core** - Base interfaces
- **Qwiq.Identity.Soap** - SOAP client implementation
- **Qwiq.Mapper.Identity** - Mapper integration for identity fields
- **Qwiq.Linq.Identity** - LINQ extensions for identity queries

## Common Mistakes to Avoid

❌ **Don't resolve identities individually in loops** - Use bulk methods

❌ **Don't assume identity exists** - Check for null returns

✅ **Do use bulk resolution** - Much faster for multiple identities

✅ **Do cache identity lookups** - Identities rarely change
