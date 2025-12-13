# Qwiq.Identity.Soap

⚠️ Windows Only | Legacy SOAP Client

SOAP-specific identity management services for Team Foundation Server (TFS) and legacy Azure DevOps Server instances.

## Overview

This package extends [Qwiq.Identity](https://www.nuget.org/packages/Qwiq.Identity/) with SOAP client implementations for identity resolution and team membership queries. Use this package when working with on-premises TFS servers that require SOAP API access.

**Note:** This package only supports Windows and the `net472` target framework due to dependencies on the legacy TFS Client Object Model.

## When to Use

- ✅ Legacy TFS 2015-2018 on-premises servers
- ✅ Azure DevOps Server instances requiring SOAP authentication
- ✅ Windows-only environments
- ❌ Azure DevOps Services (use [Qwiq.Identity](https://www.nuget.org/packages/Qwiq.Identity/) with REST client instead)

## Installation

```powershell
dotnet add package Qwiq.Identity.Soap
```

**Target Frameworks:** net472

## Quick Start

```csharp
using Qwiq;
using Qwiq.Identity;

// Create SOAP work item store with identity services
var options = new AuthenticationOptions(
    new Uri("https://tfs.company.com/tfs/DefaultCollection"),
    AuthenticationTypes.Windows,
    credentialsFactory
);

var store = Qwiq.Client.Soap.WorkItemStoreFactory.Default.Create(options);
var identityService = store.GetService<IIdentityManagementService>();

// Resolve identity
var identity = identityService.ReadIdentity(IdentitySearchFactor.Alias, "username");
Console.WriteLine($"Display Name: {identity.DisplayName}");
```

## Features

- SOAP-based identity resolution
- Team membership queries via SOAP
- Windows authentication support
- Compatible with legacy TFS servers

## Migration to REST

For Azure DevOps Services or modern Azure DevOps Server, migrate to the REST client:

```csharp
// OLD: SOAP client
var store = Qwiq.Client.Soap.WorkItemStoreFactory.Default.Create(options);

// NEW: REST client (cross-platform, modern)
var store = Qwiq.Client.Rest.WorkItemStoreFactory.Default.Create(options);

// Identity service API remains the same
var identityService = store.GetService<IIdentityManagementService>();
```

## Related Packages

- **[Qwiq.Identity](https://www.nuget.org/packages/Qwiq.Identity/)** - Core identity services interfaces
- **[Qwiq.Client.Soap](https://www.nuget.org/packages/Qwiq.Client.Soap/)** - SOAP work item client
- **[Qwiq.Client.Rest](https://www.nuget.org/packages/Qwiq.Client.Rest/)** - Modern REST client (recommended)

## Documentation

- [Repository](https://github.com/rjmurillo/Qwiq)
- [Identity Services Documentation](https://github.com/rjmurillo/Qwiq/blob/master/docs/package-readme/Qwiq.Identity.md)

## License

[MIT License](https://github.com/rjmurillo/Qwiq/blob/master/LICENSE)
