# Qwiq.Identity.Tests Component Guide

## Component Overview

**Qwiq.Identity.Tests** contains unit tests for identity management functionality in Qwiq.

## Purpose

- Test identity service interfaces
- Verify identity resolution behavior
- Test identity descriptor handling

## Key Characteristics

- **Target Frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
- **Test Framework**: MSTest
- **Pattern**: ContextSpecification
- **Focus**: Identity resolution

## Test Categories

### Identity Service Tests

```csharp
[TestMethod]
public void Should_read_identity()
{
    var identity = _identityService!.ReadIdentity("user@domain.com");
    identity.ShouldNotBeNull();
    identity.UniqueName.ShouldBe("user@domain.com");
}
```

### Bulk Resolution Tests

```csharp
[TestMethod]
public void Should_read_identities_in_bulk()
{
    var identities = _identityService!.ReadIdentities(new[]
    {
        "user1@domain.com",
        "user2@domain.com"
    });

    identities.Count().ShouldBe(2);
}
```

## Related Components

- **Qwiq.Identity** - Component under test
- **Qwiq.Mocks** - MockIdentityManagementService
- **Qwiq.Tests.Common** - Test infrastructure
