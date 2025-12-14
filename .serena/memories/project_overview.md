# Qwiq Project Overview

## Purpose

**QWIQ** (**Q**uick **W**ork **I**tem **Q**uery) is a .NET library providing a simplified API for querying Azure DevOps / Team Foundation Server work items. It wraps the TFS Client OM with cleaner interfaces, factory patterns, and mock support.

## Tech Stack

- **Language**: C# (.NET 8.0 default, with net472 for Windows-specific SOAP components)
- **SDK**: .NET 10.0.101 (via global.json, rollForward: latestPatch)
- **Language Version**: C# preview (LangVersion)
- **Nullable**: Enabled
- **Implicit Usings**: Enabled

## Key Dependencies

- **Azure DevOps Client Libraries**: Microsoft.VisualStudio.Services.Client 16.153.0
- **Testing**: MSTest, Shouldly, Moq, WireMock.Net, Verify.Xunit
- **Benchmarking**: BenchmarkDotNet
- **Versioning**: Nerdbank.GitVersioning

## Two Client Implementations

| Client   | Project          | Use Case               | Platform              |
| -------- | ---------------- | ---------------------- | --------------------- |
| **REST** | `Qwiq.Core.Rest` | Modern Azure DevOps    | Cross-platform        |
| **SOAP** | `Qwiq.Core.Soap` | Legacy TFS on-premises | Windows-only (net472) |

Both implement `IWorkItemStore` via factory pattern: `WorkItemStoreFactory.Default.Create(options)`

## Project Structure

### Source Projects (src/)

| Directory              | Purpose                           |
| ---------------------- | --------------------------------- |
| `Qwiq.Core`            | Core interfaces and abstractions  |
| `Qwiq.Core.Rest`       | REST API client (cross-platform)  |
| `Qwiq.Core.Soap`       | SOAP client (Windows/net472 only) |
| `Qwiq.Identity`        | Identity management               |
| `Qwiq.Identity.Soap`   | SOAP-specific identity            |
| `Qwiq.Linq`            | LINQ-to-WIQL query provider       |
| `Qwiq.Linq.Identity`   | LINQ identity extensions          |
| `Qwiq.Mapper`          | Object mapping layer              |
| `Qwiq.Mapper.Identity` | Mapper identity extensions        |

### Test Projects (test/)

| Directory                | Purpose                        |
| ------------------------ | ------------------------------ |
| `Qwiq.Mocks`             | In-memory mock implementations |
| `Qwiq.Core.Tests`        | Core unit tests                |
| `Qwiq.Linq.Tests`        | LINQ provider tests            |
| `Qwiq.Mapper.Tests`      | Mapper tests                   |
| `Qwiq.Identity.Tests`    | Identity tests                 |
| `Qwiq.Integration.Tests` | Integration tests (server)     |
| `Qwiq.Benchmark`         | Performance benchmarks         |
| `Qwiq.WireMock.Tests`    | HTTP mock tests                |
| `Qwiq.Package.Tests`     | NuGet package verification     |

## Configuration Files

| File                       | Purpose                                        |
| -------------------------- | ---------------------------------------------- |
| `Directory.Build.props`    | Shared MSBuild properties, package metadata    |
| `Directory.Packages.props` | Central Package Management (all versions here) |
| `global.json`              | Pins .NET SDK version                          |
| `.editorconfig`            | Code style AND analyzer severity configuration |
| `coverage.runsettings`     | Test coverage settings                         |

## Architecture Highlights

### LINQ Provider

Translates C# LINQ to WIQL queries:

- `Query<T>` → `WiqlQueryProvider` → `QueryRewriter` → `WiqlTranslator` → WIQL string

### Mapper System

Converts `IWorkItem` to POCOs using attributes like `[WorkItemType("Bug")]` and `[FieldDefinition("System.Id")]`

### Mock System

`Qwiq.Mocks` provides in-memory implementations for unit testing
