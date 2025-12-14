# Qwiq.Core.Soap Component Guide

## Component Overview

**Qwiq.Core.Soap** is the SOAP/XML API client implementation for legacy Team Foundation Server (TFS) on-premises installations. This component provides backward compatibility with older TFS versions.

## Purpose

- Legacy SOAP API client for TFS on-premises
- Support for TFS 2015, 2017, 2018, and Azure DevOps Server
- Windows-only authentication (NTLM, Kerberos)
- XML-based communication protocol

## Key Characteristics

- **Target Frameworks**: `net472` only
- **Platform**: **Windows only** (TFS Client OM requirement)
- **Dependencies**: Qwiq.Core, Microsoft.TeamFoundationServer.ExtendedClient
- **API Type**: SOAP/XML
- **Authentication**: Windows (NTLM, Kerberos), Basic

## ⚠️ Critical: Windows-Only Requirement

This component **CANNOT** be built or run on Linux/macOS because:

- `Microsoft.TeamFoundationServer.ExtendedClient` requires Windows
- TFS Client OM uses Windows-specific APIs
- NTLM/Kerberos authentication is Windows-only

**Build requirements**:

- Windows operating system
- .NET Framework 4.7.2+ SDK
- Visual Studio 2022 or later (recommended)

## Architecture

### SOAP Client Implementation

Implements core interfaces using TFS Client Object Model:

- `IWorkItemStore` → `Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore`
- `IWorkItem` → Wraps TFS `WorkItem` class
- `IRevision` → Wraps TFS `Revision` class
- `IQuery` → Wraps TFS `Query` class

### Key Entry Point

```csharp
var options = new AuthenticationOptions(
    new Uri("http://tfs.company.com:8080/tfs/DefaultCollection"),
    AuthenticationTypes.Windows,
    () => CredentialCache.DefaultNetworkCredentials
);

var store = Qwiq.Client.Soap.WorkItemStoreFactory.Default.Create(options);
```

## Authentication

### Windows Authentication (Default)

```csharp
var options = new AuthenticationOptions(
    collectionUri,
    AuthenticationTypes.Windows,
    () => CredentialCache.DefaultNetworkCredentials
);
```

### Specific Domain Credentials

```csharp
var options = new AuthenticationOptions(
    collectionUri,
    AuthenticationTypes.Windows,
    () => new NetworkCredential("username", "password", "DOMAIN")
);
```

### Basic Authentication

```csharp
var options = new AuthenticationOptions(
    collectionUri,
    AuthenticationTypes.Basic,
    () => new NetworkCredential("username", "password")
);
```

## SOAP vs REST Differences

### Fields Not Returned by SOAP

SOAP client does NOT return these classification fields:

- `System.AreaLevel1-7` (returned by REST)
- `System.IterationLevel1-7` (returned by REST)

### Authentication

| Method     | SOAP                            | REST           |
| ---------- | ------------------------------- | -------------- |
| Windows/AD | ✅ NTLM, Kerberos               | ✅ Azure AD    |
| PAT        | ❌ Not supported                | ✅ Recommended |
| OAuth      | ❌ Not supported                | ✅ Supported   |
| Basic      | ✅ HTTP Basic (not recommended) | ✅ Supported   |

### Performance

- **SOAP**: Single-threaded, synchronous operations
- **REST**: Supports async/await, better performance

## Testing Guidelines

### Integration Tests

SOAP integration tests are in `Qwiq.Integration.Tests` with category `[TestCategory("SOAP")]`.

**Note**: These tests present an interactive login dialog and require:

- Windows operating system
- Network connectivity to TFS server
- Domain credentials or Windows authentication
- Not suitable for headless CI/CD

### Unit Tests

All SOAP-specific classes are `internal`, so unit tests must be in `Qwiq.Integration.Tests` or use `InternalsVisibleTo`.

### Mock Testing

Use `Qwiq.Mocks` for testing code that uses `IWorkItemStore`:

```csharp
var store = new MockWorkItemStore();
// Tests don't need actual TFS connection
```

## Internal Visibility

Most SOAP implementation types are `internal` to avoid exposing TFS Client OM:

```csharp
// Internal - not exposed in public API
internal class SoapWorkItemStore : IWorkItemStore
{
    // Wraps Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore
}
```

Only factory and options are public.

## Common Patterns

### Connection Caching

TFS Client OM maintains connection pool internally:

```csharp
// ✅ CORRECT: Reuse store
var store = WorkItemStoreFactory.Default.Create(options);
var items1 = store.Query("SELECT [System.Id] FROM WorkItems");
var items2 = store.Query("SELECT [System.Id] FROM WorkItems WHERE [State] = 'Active'");
```

### Error Handling

```csharp
try
{
    var items = store.Query(wiql);
}
catch (Microsoft.TeamFoundation.WorkItemTracking.Client.ValidationException ex)
{
    // WIQL syntax errors
    Console.WriteLine($"Query Error: {ex.Message}");
}
catch (Microsoft.TeamFoundation.TeamFoundationServerException ex)
{
    // Server connection/permission errors
    Console.WriteLine($"TFS Error: {ex.Message}");
}
```

## Common Mistakes to Avoid

❌ **Don't try to build on Linux/macOS** - Windows only

```powershell
# ❌ WRONG: Will fail on Linux
dotnet build src/Qwiq.Core.Soap/Qwiq.Core.Soap.csproj

# ✅ CORRECT: Build on Windows
# (from Windows PowerShell)
dotnet build src/Qwiq.Core.Soap/Qwiq.Core.Soap.csproj
```

❌ **Don't expose TFS Client OM types** - Keep internal

```csharp
// ❌ WRONG: Exposes TFS type in public API
public WorkItem GetWorkItem(int id) { }

// ✅ CORRECT: Return interface
public IWorkItem GetWorkItem(int id) { }
```

❌ **Don't use for new projects** - Prefer REST client

```csharp
// ❌ WRONG: SOAP for new Azure DevOps Services project
var store = Qwiq.Client.Soap.WorkItemStoreFactory.Default.Create(options);

// ✅ CORRECT: REST for modern Azure DevOps
var store = Qwiq.Client.Rest.WorkItemStoreFactory.Default.Create(options);
```

❌ **Don't hardcode server URLs** - Use configuration

```csharp
// ❌ WRONG: Hardcoded
var uri = new Uri("http://tfs.company.com:8080/tfs/DefaultCollection");

// ✅ CORRECT: From config
var uri = new Uri(configuration["TFS:CollectionUrl"]);
```

✅ **Do use for TFS on-premises** - SOAP works when REST unavailable

✅ **Do keep types internal** - Don't expose TFS Client OM

✅ **Do build only on Windows** - Platform restriction

✅ **Do migrate to REST when possible** - Better performance and cross-platform

## Related Components

- **Qwiq.Core** - Core interfaces implemented by this client
- **Qwiq.Core.Rest** - Modern REST-based alternative
- **Qwiq.Identity.Soap** - SOAP-based identity client
- **Qwiq.Integration.Tests** - Integration tests (Windows only)

## TFS Version Compatibility

| TFS/AzDO Version      | SOAP Support | Recommended Client |
| --------------------- | ------------ | ------------------ |
| TFS 2015              | ✅ Full      | SOAP               |
| TFS 2017              | ✅ Full      | SOAP               |
| TFS 2018              | ✅ Full      | SOAP or REST       |
| Azure DevOps Server   | ✅ Full      | REST (preferred)   |
| Azure DevOps Services | ✅ Limited   | REST (only option) |

**Note**: Azure DevOps Services deprecated SOAP endpoints. Use REST client.

## Performance Considerations

- **Query size**: SOAP has no built-in paging; large queries can be slow
- **Network overhead**: XML serialization adds overhead vs JSON
- **Threading**: TFS Client OM is not thread-safe; avoid parallel queries
- **Connection reuse**: Store instances cache connections; reuse where possible

## Debugging

### Enable TFS Logging

Set environment variables for TFS Client OM diagnostics:

```powershell
$env:TF_ADDITIONAL_JAVA_ARGS = "-Djavax.net.debug=all"
```

### View SOAP Requests

Use Fiddler to inspect SOAP traffic:

- XML request/response payloads
- Authentication headers
- Error details

### Common Issues

| Issue                              | Cause                     | Solution                       |
| ---------------------------------- | ------------------------- | ------------------------------ |
| `TF30063` Authentication failed    | Invalid credentials       | Check username/password/domain |
| `TF26071` Invalid field name       | Field doesn't exist       | Verify field reference name    |
| `TF51005` Area/iteration not found | Invalid path              | Check classification paths     |
| `TF26027` Work item does not exist | Invalid ID or permissions | Verify ID and read permissions |
| Platform not supported             | Running on Linux/macOS    | Build/run on Windows           |

## Migration Path

### From SOAP to REST

```csharp
// Old SOAP code
var soapStore = Qwiq.Client.Soap.WorkItemStoreFactory.Default.Create(soapOptions);

// New REST code (drop-in replacement)
var restOptions = new AuthenticationOptions(
    new Uri("https://dev.azure.com/myorg"),
    AuthenticationTypes.PersonalAccessToken,
    credentialsFactory
);
var restStore = Qwiq.Client.Rest.WorkItemStoreFactory.Default.Create(restOptions);

// Both implement IWorkItemStore - rest of code unchanged
```

## Internal Implementation Notes

### Adapter Pattern

SOAP client adapts TFS Client OM to Qwiq interfaces:

```text
Qwiq.IWorkItemStore
    ↓
SoapWorkItemStore (internal adapter)
    ↓
Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore
```

### Why Internal?

TFS Client OM types:

- Are Windows-specific
- Have complex dependencies
- Don't follow Qwiq conventions
- Could break if exposed publicly

Keeping adapters internal allows changing TFS OM version without breaking Qwiq API.
