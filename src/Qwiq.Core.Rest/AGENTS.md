# Qwiq.Core.Rest Component Guide

## Component Overview

**Qwiq.Core.Rest** is the REST API client implementation for Azure DevOps Services and Server. It provides modern HTTP-based access to work items using the Azure DevOps REST APIs.

## Purpose

- Modern REST API client for Azure DevOps
- Cross-platform support (Windows, Linux, macOS)
- Support for Azure DevOps Services (cloud) and Server (on-premises)
- HTTP/JSON based communication

## Key Characteristics

- **Target Frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
- **Platform**: Cross-platform
- **Dependencies**: Qwiq.Core, Microsoft.VisualStudio.Services.Client
- **API Type**: HTTP/JSON (REST)
- **Authentication**: PAT, OAuth, Azure AD

## Architecture

### REST Client Implementation

Implements core interfaces from Qwiq.Core using Azure DevOps REST APIs:

- `IWorkItemStore` → REST-based work item store
- `IWorkItem` → Work item from REST API responses
- `IRevision` → Revision history from REST endpoints
- `IQuery` → REST-based query execution

### Key Entry Point

```csharp
var options = new AuthenticationOptions(
    new Uri("https://dev.azure.com/myorg"),
    AuthenticationTypes.PersonalAccessToken,
    credentialsFactory
);

var store = Qwiq.Client.Rest.WorkItemStoreFactory.Default.Create(options);
```

## Authentication Types

### Personal Access Token (Recommended)

```csharp
var credentialsFactory = new Func<ICredentials>(() =>
    new NetworkCredential("PAT", personalAccessToken));

var options = new AuthenticationOptions(
    collectionUri,
    AuthenticationTypes.PersonalAccessToken,
    credentialsFactory
);
```

### OAuth

```csharp
var options = new AuthenticationOptions(
    collectionUri,
    AuthenticationTypes.OAuth,
    () => new NetworkCredential("OAuth", accessToken)
);
```

### Azure AD (Windows)

```csharp
var options = new AuthenticationOptions(
    collectionUri,
    AuthenticationTypes.Windows,
    () => CredentialCache.DefaultNetworkCredentials
);
```

## REST API Specifics

### Field Differences from SOAP

REST API returns additional system fields not in SOAP:

- `System.AreaLevel1` through `System.AreaLevel7`
- `System.IterationLevel1` through `System.IterationLevel7`

These are classification hierarchy fields automatically populated by Azure DevOps.

### API Version

The client uses Azure DevOps REST API version 7.0+. Key endpoints:

- `/_apis/wit/workitems` - Get work items by IDs
- `/_apis/wit/wiql` - Execute WIQL queries
- `/_apis/wit/queries` - Saved query management
- `/_apis/projects` - Project enumeration

## Testing Guidelines

### Integration Tests

REST integration tests are in `Qwiq.Integration.Tests` with category `[TestCategory("REST")]`.

**Note**: These tests present an interactive login dialog and require:

- Network connectivity to Azure DevOps
- Valid authentication credentials
- Not suitable for headless CI/CD

### Mock Testing

Use `Qwiq.Mocks` for unit testing:

```csharp
var store = new MockWorkItemStore();
store.Add(new MockWorkItem("Bug") { Title = "Test" });
```

### WireMock Testing

Use `Qwiq.WireMock.Tests` to record/replay HTTP traffic:

```csharp
// Record mode: Capture real REST traffic
// Playback mode: Use recorded responses
```

## Common Patterns

### Connection Pooling

REST client reuses HTTP connections internally. Don't create multiple stores unnecessarily:

```csharp
// ✅ CORRECT: Reuse store instance
var store = WorkItemStoreFactory.Default.Create(options);
var items1 = store.Query("SELECT [System.Id] FROM WorkItems");
var items2 = store.Query("SELECT [System.Id] FROM WorkItems WHERE [State] = 'Active'");

// ❌ WRONG: Creates new connections
foreach (var query in queries)
{
    var store = WorkItemStoreFactory.Default.Create(options);
    var items = store.Query(query);
}
```

### Error Handling

```csharp
try
{
    var items = store.Query(wiql);
}
catch (Microsoft.VisualStudio.Services.Common.VssServiceException ex)
{
    // REST API errors (401, 404, etc.)
    Console.WriteLine($"API Error: {ex.Message}");
}
catch (HttpRequestException ex)
{
    // Network/connectivity errors
    Console.WriteLine($"Network Error: {ex.Message}");
}
```

## Common Mistakes to Avoid

❌ **Don't hardcode organization URLs** - Use configuration

```csharp
// ❌ WRONG: Hardcoded URL
var uri = new Uri("https://dev.azure.com/mycompany");

// ✅ CORRECT: From configuration
var uri = new Uri(configuration["AzureDevOps:OrganizationUrl"]);
```

❌ **Don't log PATs or tokens** - Security risk

```csharp
// ❌ WRONG: Logs sensitive data
Console.WriteLine($"Using PAT: {pat}");

// ✅ CORRECT: Log without credentials
Console.WriteLine("Authenticating to Azure DevOps");
```

❌ **Don't use project URL as collection URI**

```csharp
// ❌ WRONG: Project-level URL
new Uri("https://dev.azure.com/myorg/MyProject")

// ✅ CORRECT: Organization-level URL
new Uri("https://dev.azure.com/myorg")
```

✅ **Do dispose store when done** - Releases HTTP connections

✅ **Do use PAT authentication** - Most reliable for automation

✅ **Do handle VssServiceException** - API-specific errors

✅ **Do use WireMock for testing** - Avoid hitting real APIs in tests

## Related Components

- **Qwiq.Core** - Core interfaces implemented by this client
- **Qwiq.Core.Soap** - Alternative SOAP-based client
- **Qwiq.WireMock.Tests** - HTTP traffic recording/playback
- **Qwiq.Integration.Tests** - Integration test suite

## API Limitations

### Rate Limiting

Azure DevOps Services applies rate limits:

- 200 requests per user per second
- Larger queries may take multiple pages

Handle rate limiting gracefully:

```csharp
catch (VssServiceException ex) when (ex.HttpStatusCode == 429)
{
    // Too Many Requests - implement exponential backoff
    await Task.Delay(TimeSpan.FromSeconds(60));
    // Retry
}
```

### Page Size

Work item queries are paginated:

- Default page size: 200 items
- Configurable via `WorkItemStoreConfiguration`

```csharp
var config = new WorkItemStoreConfiguration
{
    PageSize = 200  // Between 50-200
};
```

### Field Expansion

By default, queries return basic fields. Use `WorkItemExpand` for more:

```csharp
store.GetWorkItems(ids, WorkItemExpand.All);  // All fields
store.GetWorkItems(ids, WorkItemExpand.Links); // Include links
```

## Performance Tips

- **Batch work item retrieval** - Get multiple IDs in one call
- **Use field filters** - Request only needed fields
- **Cache work items** - Avoid redundant queries
- **Reuse store instances** - HTTP connection pooling

## Debugging

### Enable HTTP Logging

Set environment variable or configure logging:

```powershell
$env:AZURE_DEVOPS_EXT_PAT_DEBUG = "1"
```

### View REST Requests

Use Fiddler or browser DevTools to inspect:

- Request URLs and headers
- Response payloads
- Authentication tokens (be careful with PATs!)

### Common Issues

| Issue                       | Cause                          | Solution                                     |
| --------------------------- | ------------------------------ | -------------------------------------------- |
| 401 Unauthorized            | Invalid/expired PAT            | Regenerate PAT with required scopes          |
| 404 Not Found               | Wrong organization/project URL | Verify URL is organization-level             |
| 403 Forbidden               | Insufficient PAT permissions   | Grant "Work Items (Read)" scope              |
| VssServiceException (empty) | Network/proxy issues           | Check network connectivity and proxy         |
| Null reference in field     | Field doesn't exist            | Check field name spelling and work item type |
