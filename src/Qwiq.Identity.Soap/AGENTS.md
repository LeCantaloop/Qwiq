# Qwiq.Identity.Soap Component Guide

## Component Overview

**Qwiq.Identity.Soap** provides SOAP-based identity management for legacy TFS on-premises installations.

## Purpose

- SOAP client for TFS identity services
- Support legacy TFS 2015, 2017, 2018 installations
- Windows-only identity resolution

## Key Characteristics

- **Target Frameworks**: `net472` only
- **Platform**: **Windows only**
- **Dependencies**: Qwiq.Core, Qwiq.Core.Soap, Qwiq.Identity
- **API Type**: SOAP/XML

## ⚠️ Critical: Windows-Only Requirement

This component requires:

- Windows operating system
- .NET Framework 4.7.2+
- TFS Client OM (Microsoft.TeamFoundationServer.ExtendedClient)

## Implementation

Implements `IIdentityManagementService` using TFS Client OM's identity services.

All implementation types are `internal` - only the factory is public.

## Common Patterns

### Creating SOAP Identity Service

```csharp
var identityService = SoapIdentityManagementServiceFactory.Create(tfsConnection);
var identities = identityService.ReadIdentities(userNames);
```

## Testing

Use `MockIdentityManagementService` instead of real SOAP service for unit tests.

Integration tests with real SOAP service require Windows and TFS connectivity.

## Related Components

- **Qwiq.Identity** - Base identity interfaces
- **Qwiq.Core.Soap** - SOAP client (shares TFS connection)
- **Qwiq.Integration.Tests** - Integration tests (Windows only)

## Common Mistakes to Avoid

❌ **Don't try to build on Linux/macOS** - Windows only

❌ **Don't expose TFS Client OM types** - Keep internal

✅ **Do use for TFS on-premises** - When SOAP is only option

✅ **Do migrate to REST when possible** - Better performance
