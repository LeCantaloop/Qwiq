# CLAUDE.md

This file provides guidance to Claude Code (<https://claude.ai/code>) when working with code in this repository.

## Repository Overview

QWIQ (**Q**uick **W**ork **I**tem **Q**uery) is a .NET library providing a simplified API for querying Azure DevOps / Team Foundation Server work items. It wraps the TFS Client OM with cleaner interfaces, factory patterns, and mock support.

## Build Commands

```powershell
# Restore tools (nbgv for versioning)
dotnet tool restore

# Build solution
dotnet build Qwiq.sln -c Release

# Strict build (warnings as errors) - used by CI
dotnet build Qwiq.sln -c Release /p:PedanticMode=true

# Flexible build (warnings allowed) - for diagnosing analyzer issues
dotnet build Qwiq.sln -c Release /p:PedanticMode=false

# Single-threaded build (avoids Windows file locking issues)
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
```

## Test Commands

```powershell
# Run unit tests (excludes integration tests requiring servers)
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

# Run a single test class
dotnet test Qwiq.sln -c Release --no-build --filter "FullyQualifiedName~ClassName"

# Run tests with code coverage
dotnet test Qwiq.sln -c Release --settings coverage.runsettings
```

## Linting Commands

```powershell
# Fix markdown issues
npx markdownlint-cli2 --fix "**/*.md"

# Fix C# formatting
dotnet format

# Fix general formatting (markdown, JSON)
dotnet pprettier --write .
```

## Architecture Overview

### Two Client Implementations

Both implement `IWorkItemStore` via factory pattern (`WorkItemStoreFactory.Default.Create(options)`):

| Client   | Project          | Use Case               | Platform              |
| -------- | ---------------- | ---------------------- | --------------------- |
| **REST** | `Qwiq.Core.Rest` | Modern Azure DevOps    | Cross-platform        |
| **SOAP** | `Qwiq.Core.Soap` | Legacy TFS on-premises | Windows-only (net472) |

### LINQ Provider

Translates C# LINQ to WIQL queries:

- `Query<T>` → `WiqlQueryProvider` → `QueryRewriter` → `WiqlTranslator` → WIQL string
- `IFieldMapper` maps .NET property names to TFS field reference names
- Special extension methods: `AsOf()`, `WasEver()`, `InGroup()`

### Mapper System

Converts `IWorkItem` to POCOs using attributes:

```csharp
[WorkItemType("Bug")]
public class Bug
{
    [FieldDefinition("System.Id")]
    public int? Id { get; set; }

    [FieldDefinition("System.Title")]
    public string Title { get; set; }
}
```

### Mock System

`Qwiq.Mocks` provides in-memory implementations for unit testing (`MockWorkItemStore`, `MockWorkItem`, etc.).

## Key Patterns

### Factory Pattern

All stores created via factories, never direct construction:

```csharp
IWorkItemStore store = WorkItemStoreFactory.Default.Create(options);
```

### Null Validation

Use runtime checks, not JetBrains annotations:

```csharp
public void Method(SomeType param)
{
    if (param == null) throw new ArgumentNullException(nameof(param));
}
```

### Test Pattern (ContextSpecification)

```csharp
[TestClass]
public class Given_context : ContextSpecification
{
    public override void Given() { /* Arrange */ }
    public override void When() { /* Act */ }

    [TestMethod]
    public void Then_behavior() { /* Assert with Shouldly */ }
}
```

## Project Structure

| Directory            | Contents                          |
| -------------------- | --------------------------------- |
| `src/Qwiq.Core`      | Core interfaces and abstractions  |
| `src/Qwiq.Core.Rest` | REST API client (cross-platform)  |
| `src/Qwiq.Core.Soap` | SOAP client (Windows/net472 only) |
| `src/Qwiq.Linq`      | LINQ-to-WIQL query provider       |
| `src/Qwiq.Mapper`    | Object mapping layer              |
| `src/Qwiq.Identity`  | Identity management               |
| `test/Qwiq.Mocks`    | Mock implementations for testing  |

## Configuration Files

| File                       | Purpose                                        |
| -------------------------- | ---------------------------------------------- |
| `Directory.Build.props`    | Shared MSBuild properties, package metadata    |
| `Directory.Packages.props` | Central Package Management (all versions here) |
| `global.json`              | Pins .NET SDK version                          |
| `.editorconfig`            | Code style AND analyzer severity configuration |

## Critical Notes

1. **Windows required** for SOAP projects (net472 + TFS Client OM dependency)
2. **Central Package Management** - add versions to `Directory.Packages.props`, not individual csproj files
3. **Nullable enabled** - use `?` suffix for nullable types, runtime null checks for validation
4. **Never commit artifacts/** - build outputs are gitignored
5. **Conventional commits** - use `fix(scope):`, `feat(scope):`, `refactor:`, etc.

## Detailed Documentation

See `.github/copilot-instructions.md` for comprehensive guidance including:

- Full architecture details and class relationships
- Exception handling patterns
- Test categories and integration test setup
- CI/CD pipeline details
- Troubleshooting patterns
