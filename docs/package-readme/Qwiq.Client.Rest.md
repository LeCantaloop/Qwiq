# Qwiq.Client.Rest

Modern HTTP/JSON REST client for Azure DevOps Services and Server.

## Overview

Qwiq.Client.Rest is the recommended client implementation for modern Azure DevOps environments. It uses the Azure DevOps REST API for all operations, providing cross-platform compatibility and better performance than the legacy SOAP client.

## Key Features

- **Cross-platform**: Works on Windows, Linux, and macOS
- **Modern API**: Uses Azure DevOps REST API (HTTP/JSON)
- **Multi-targeting**: Supports .NET Framework 4.7.2, .NET Standard 2.0, and .NET 8.0
- **Full feature support**: Complete work item CRUD operations

## Installation

```bash
dotnet add package Qwiq.Core
dotnet add package Qwiq.Client.Rest
```

## Quick Start

### Windows Authentication

```csharp
using Qwiq;
using Qwiq.Credentials;
using Qwiq.Client.Rest;

var uri = new Uri("https://dev.azure.com/yourorg");
var options = new AuthenticationOptions(uri, AuthenticationTypes.Windows);

IWorkItemStore store = WorkItemStoreFactory.Default.Create(options);
```

### Personal Access Token (PAT)

```csharp
var uri = new Uri("https://dev.azure.com/yourorg");
var credentials = new Credential("YourPAT");
var options = new AuthenticationOptions(uri, AuthenticationTypes.PersonalAccessToken, credentials);

IWorkItemStore store = WorkItemStoreFactory.Default.Create(options);
```

### OAuth Token

```csharp
var uri = new Uri("https://dev.azure.com/yourorg");
var credentials = new Credential("YourOAuthToken");
var options = new AuthenticationOptions(uri, AuthenticationTypes.OAuth, credentials);

IWorkItemStore store = WorkItemStoreFactory.Default.Create(options);
```

## Usage Examples

### Query Work Items

```csharp
var query = @"
    SELECT [System.Id], [System.Title], [System.State]
    FROM WorkItems
    WHERE [System.WorkItemType] = 'Bug'
      AND [System.State] = 'Active'
";

var workItems = store.Query(query);
foreach (var item in workItems)
{
    Console.WriteLine($"Bug {item.Id}: {item.Title}");
}
```

### Get Work Item by ID

```csharp
var workItem = store.GetWorkItem(12345);
Console.WriteLine($"Title: {workItem.Title}");
Console.WriteLine($"State: {workItem.State}");
```

### Update Work Item

```csharp
var workItem = store.GetWorkItem(12345);
workItem.Fields["System.State"].Value = "Resolved";
workItem.Save();
```

## Authentication Best Practices

### Secure Credential Storage

Never hardcode credentials in your application. Use secure storage:

```csharp
// Use environment variables
var pat = Environment.GetEnvironmentVariable("AZURE_DEVOPS_PAT");

// Or Azure Key Vault
var secretClient = new SecretClient(vaultUri, new DefaultAzureCredential());
var secret = await secretClient.GetSecretAsync("azure-devops-pat");
var pat = secret.Value.Value;
```

### Minimum Required Scopes

When creating a PAT, use the minimum required scopes:

- **Work Items (Read)**: For read-only operations
- **Work Items (Write)**: For creating/updating work items

## REST vs SOAP

Use Qwiq.Client.Rest when:

- ✅ Targeting Azure DevOps Services
- ✅ Need cross-platform support
- ✅ Want better performance
- ✅ Building new applications

Use Qwiq.Client.Soap when:

- Legacy TFS on-premises with SOAP-only access
- Existing applications with SOAP dependencies

## Related Packages

- **Qwiq.Core**: Core interfaces (required)
- **Qwiq.Linq**: LINQ query provider
- **Qwiq.Mapper**: Object mapping
- **Qwiq.Identity**: Identity management

## Documentation

- [GitHub Repository](https://github.com/rjmurillo/Qwiq)
- [Azure DevOps REST API](https://learn.microsoft.com/en-us/rest/api/azure/devops/)

## License

MIT License - see [LICENSE](https://github.com/rjmurillo/Qwiq/blob/master/LICENSE) for details.
