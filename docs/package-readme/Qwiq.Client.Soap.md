# Qwiq.Client.Soap

Legacy SOAP/XML client for Team Foundation Server on-premises.

## Overview

Qwiq.Client.Soap provides compatibility with legacy Team Foundation Server installations that only support SOAP-based web services. This client is in maintenance mode and only receives bug fixes.

**⚠️ Windows Only**: This package requires .NET Framework 4.7.2 and only works on Windows.

## Installation

```bash
dotnet add package Qwiq.Core
dotnet add package Qwiq.Client.Soap
```

## Quick Start

```csharp
using Qwiq;
using Qwiq.Credentials;
using Qwiq.Client.Soap;

var uri = new Uri("http://tfs.contoso.com:8080/tfs/DefaultCollection");
var options = new AuthenticationOptions(uri, AuthenticationTypes.Windows);

IWorkItemStore store = WorkItemStoreFactory.Default.Create(options);

// Use the same IWorkItemStore interface as REST client
var workItems = store.Query("SELECT * FROM WorkItems WHERE [System.Id] = 1");
```

## When to Use SOAP Client

Use Qwiq.Client.Soap **only** when:

- Working with legacy Team Foundation Server 2015-2018
- Server doesn't support REST API
- Existing application requires SOAP endpoints

## Recommended Alternative

For new applications or when possible, use **Qwiq.Client.Rest** instead:

- Cross-platform support (Windows, Linux, macOS)
- Better performance
- Active development and feature additions
- Works with Azure DevOps Services and Server

## Migration Path

Switching from SOAP to REST is straightforward since both implement `IWorkItemStore`:

```csharp
// Before (SOAP)
using Qwiq.Client.Soap;
IWorkItemStore store = WorkItemStoreFactory.Default.Create(options);

// After (REST) - just change the using statement
using Qwiq.Client.Rest;
IWorkItemStore store = WorkItemStoreFactory.Default.Create(options);

// All other code remains the same!
```

## Platform Requirements

- **Target Framework**: .NET Framework 4.7.2
- **Operating System**: Windows only
- **Dependencies**: Microsoft.TeamFoundationServer.ExtendedClient

## Related Packages

- **Qwiq.Core**: Core interfaces (required)
- **Qwiq.Client.Rest**: Recommended modern alternative
- **Qwiq.Linq**: LINQ query provider
- **Qwiq.Mapper**: Object mapping

## Documentation

- [GitHub Repository](https://github.com/rjmurillo/Qwiq)
- [Migration Guide](https://github.com/rjmurillo/Qwiq/wiki/Migrating-from-SOAP-to-REST)

## License

MIT License - see [LICENSE](https://github.com/rjmurillo/Qwiq/blob/master/LICENSE) for details.
