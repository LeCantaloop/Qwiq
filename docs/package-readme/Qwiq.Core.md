# Qwiq.Core

Quick Work Item Query - Core interfaces and abstractions for Azure DevOps and Team Foundation Server work item access.

## Overview

Qwiq.Core provides the foundational interfaces and types for querying and manipulating work items in Azure DevOps Services and Team Foundation Server. It offers a clean, interface-based abstraction over the native TFS Client OM, making your code easier to test and maintain.

## Key Features

- **Interface-based design**: All types have interfaces for easy mocking and testing
- **Factory patterns**: Create work item stores without tight coupling to implementations
- **Multiple client support**: Choose between REST (modern) or SOAP (legacy) clients
- **Multi-targeting**: Supports .NET Framework 4.7.2, .NET Standard 2.0, and .NET 8.0

## Quick Start

### Installation

```bash
dotnet add package Qwiq.Core
```

### Basic Usage

```csharp
using Qwiq;
using Qwiq.Credentials;

// Create authentication options
var uri = new Uri("https://dev.azure.com/yourorg");
var options = new AuthenticationOptions(
    uri,
    AuthenticationTypes.Windows
);

// Create work item store (requires REST or SOAP client package)
IWorkItemStore store = WorkItemStoreFactory.Default.Create(options);

// Query work items
var query = "SELECT [System.Id], [System.Title] FROM WorkItems WHERE [System.State] = 'Active'";
var workItems = store.Query(query);

foreach (var workItem in workItems)
{
    Console.WriteLine($"{workItem.Id}: {workItem.Title}");
}
```

## Client Implementations

Qwiq.Core is an abstraction layer. You need to install one of the client implementation packages:

- **Qwiq.Client.Rest**: Modern HTTP/JSON client for Azure DevOps Services and Server
- **Qwiq.Client.Soap**: Legacy SOAP/XML client for Team Foundation Server (Windows only)

Both clients implement the same `IWorkItemStore` interface, allowing you to switch between them without changing your application code.

## Testing Support

For unit testing, use the `Qwiq.Mocks` package which provides in-memory implementations:

```csharp
using Qwiq.Mocks;

// Create mock store for testing
var store = new MockWorkItemStore();
store.Add(new MockWorkItem("Bug") { Title = "Test Bug" });

// Test your code
var results = myService.QueryBugs(store);
Assert.Single(results);
```

## Architecture

```
Your Application
       ↓
  Qwiq.Core (interfaces)
       ↓
  Qwiq.Client.Rest or Qwiq.Client.Soap
       ↓
  Azure DevOps / TFS
```

## Related Packages

- **Qwiq.Linq**: LINQ query provider for type-safe queries
- **Qwiq.Mapper**: Map work items to strongly-typed POCOs
- **Qwiq.Identity**: Identity resolution and management
- **Qwiq.Mocks**: Mock implementations for testing

## Documentation

- [GitHub Repository](https://github.com/rjmurillo/Qwiq)
- [API Documentation](https://github.com/rjmurillo/Qwiq/wiki)
- [Sample Code](https://github.com/rjmurillo/Qwiq/tree/master/samples)

## License

MIT License - see [LICENSE](https://github.com/rjmurillo/Qwiq/blob/master/LICENSE) for details.
