# Contributing to QWIQ

Thank you for your interest in contributing to QWIQ! This guide will help you get started with development, testing, and submitting changes.

## Table of Contents

- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Build Commands](#build-commands)
- [Testing Guide](#testing-guide)
- [Code Style](#code-style)
- [Architecture Overview](#architecture-overview)
- [Key Files Reference](#key-files-reference)
- [Troubleshooting](#troubleshooting)

## Getting Started

### Prerequisites

- **Windows machine** - Required for full framework coverage (`net472` SOAP client)
- **.NET 8.0 SDK** - Pinned version in `global.json`
- **Visual Studio 2022+** or **VS Code** with C# extension
- **Git** for version control

### Clone and Build

```powershell
# Clone the repository
git clone https://github.com/rjmurillo/Qwiq.git
cd Qwiq

# Restore dotnet tools (nbgv for versioning)
dotnet tool restore

# Build the solution
dotnet build Qwiq.sln -c Release
```

### Running Tests

```powershell
# Run unit tests only (excludes integration tests)
dotnet test Qwiq.sln -c Release --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
```

## Development Workflow

### Branching Strategy

- `master` - Production-ready code
- `develop` - Integration branch for features
- Feature branches - Created from `develop` for new work

### Commit Message Format

We use [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <short description>

<optional body with more details>
```

**Types:**

| Type       | Description                                 |
| ---------- | ------------------------------------------- |
| `fix`      | Bug fixes                                   |
| `feat`     | New features                                |
| `refactor` | Code restructuring without behavior change  |
| `docs`     | Documentation only                          |
| `test`     | Adding or fixing tests                      |
| `chore`    | Maintenance tasks (dependencies, CI, tools) |
| `style`    | Code formatting (no logic changes)          |
| `build`    | Build system changes                        |
| `ci`       | CI/CD configuration changes                 |

**Scopes** (optional but encouraged):

| Scope      | Description           |
| ---------- | --------------------- |
| `core`     | Qwiq.Core changes     |
| `rest`     | REST client changes   |
| `soap`     | SOAP client changes   |
| `linq`     | LINQ provider changes |
| `mapper`   | Mapper changes        |
| `identity` | Identity management   |
| `ci`       | CI/CD pipeline        |

**Examples:**

```
fix(core): add null guard to prevent NullReferenceException
feat(linq): add support for Contains operator
docs: update contributing guide with sandbox details
```

### Pull Request Process

1. Create a feature branch from `develop`
2. Make your changes with atomic commits
3. Ensure all tests pass locally
4. Push your branch and create a PR to `develop`
5. Address any review feedback
6. Once approved, your PR will be merged

### Work Incrementally

- **Small commits** - Each commit should represent ONE logical change
- **Atomic commits** - Each commit must build successfully on its own
- **Verify before committing** - Build the affected project(s) before each commit
- **Don't batch unrelated changes** - Separate bug fixes from refactoring from features

## Build Commands

### Full Solution Build

```powershell
# Standard release build
dotnet build Qwiq.sln -c Release

# Debug build
dotnet build Qwiq.sln -c Debug

# Strict build (warnings as errors) - matches CI behavior
dotnet build Qwiq.sln -c Release /p:PedanticMode=true

# Flexible build (warnings allowed) - for diagnosing analyzers
dotnet build Qwiq.sln -c Release /p:PedanticMode=false
```

**PedanticMode**: Controls whether warnings are treated as errors. Defaults to `true` on CI (via `ContinuousIntegrationBuild`). Use `/p:PedanticMode=false` locally when investigating noisy analyzer rules.

### Individual Project Builds

```powershell
# Build specific project
dotnet build src/Qwiq.Core/Qwiq.Core.csproj -c Debug

# Build with verbose output
dotnet build Qwiq.sln -c Release -v detailed
```

### Package Creation

```powershell
# Create NuGet packages
dotnet pack Qwiq.sln -c Release -o ./artifacts/packages
```

### Clean Build

```powershell
# Clean all build artifacts
dotnet clean Qwiq.sln

# Force clean (remove bin/obj manually)
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | Remove-Item -Recurse -Force
```

### Troubleshooting Build Issues

**Windows File Locking Issues:**

Parallel builds on Windows can fail with file access errors. Use single-threaded build:

```powershell
dotnet build /m:1 /nodeReuse:false -v:minimal
```

**Package Restore Issues:**

```powershell
# Clear NuGet cache and restore
dotnet nuget locals all --clear
dotnet restore Qwiq.sln
```

## Testing Guide

### Unit Tests

Unit tests run without external dependencies and should always pass:

```powershell
# Run all unit tests
dotnet test Qwiq.sln -c Release --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
```

### Integration Tests

Integration tests require access to the Azure DevOps sandbox environment.

#### Sandbox Environment

| Setting          | Value                                    |
| ---------------- | ---------------------------------------- |
| Organization URL | `https://qwiq-sandbox.visualstudio.com/` |
| Project Name     | `WIT`                                    |
| Project ID       | `0a4c0240-1a67-45de-93db-fc1de9f54ffb`   |
| Test User        | Richard Murillo (`rjmurillo@msn.com`)    |

#### Running Integration Tests

```powershell
# Run all integration tests (requires Windows + Azure DevOps access)
dotnet test test/Qwiq.Integration.Tests/Qwiq.IntegrationTests.csproj --logger "console;verbosity=detailed"

# Run only REST tests (skips SOAP which requires special auth)
dotnet test test/Qwiq.Integration.Tests/Qwiq.IntegrationTests.csproj --filter "TestCategory=REST|TestCategory=localOnly"

# Run excluding SOAP tests (for MSA accounts with MFA)
dotnet test test/Qwiq.Integration.Tests/Qwiq.IntegrationTests.csproj --filter "TestCategory!=SOAP"
```

#### Test Categories

| Category           | Description                 | When to Run                      |
| ------------------ | --------------------------- | -------------------------------- |
| (default)          | Unit tests                  | Always (CI)                      |
| `localOnly`        | Requires local TFS instance | Manual, local dev                |
| `Benchmark`        | Performance benchmarks      | Manual                           |
| `SOAP`             | SOAP integration tests      | Manual, with TFS credentials     |
| `REST`             | REST integration tests      | Manual, with Azure DevOps access |
| `IntegrationTests` | Full integration suite      | Manual, with server access       |

#### Test Work Items in Sandbox

| ID  | Type       | Purpose                    |
| --- | ---------- | -------------------------- |
| 1   | Bug        | Basic work item tests      |
| 2   | Task       | Child task (hierarchy)     |
| 3   | User Story | Parent for hierarchy tests |
| 4   | Bug        | Mapper tests               |
| 5   | Bug        | Work item with links       |
| 6   | Task       | Second child task          |

#### Environment Variables

| Variable               | Purpose                           |
| ---------------------- | --------------------------------- |
| `QWIQ_TEST_URL`        | Override sandbox organization URL |
| `QWIQ_PROJECT_GUID`    | Override project GUID             |
| `AZURE_DEVOPS_EXT_PAT` | PAT for authentication            |

#### Validating the Sandbox Environment

Before running integration tests, you can validate that the sandbox environment is correctly configured:

```powershell
# Set your PAT
$env:AZURE_DEVOPS_PAT = "your-pat-here"

# Run the validation script
.\scripts\Validate-SandboxEnvironment.ps1

# Or pass PAT directly
.\scripts\Validate-SandboxEnvironment.ps1 -PersonalAccessToken "your-pat-here"
```

The validation script checks:

- Connection to the Azure DevOps organization
- Existence of the WIT project
- Existence of required work items (IDs 1-7)
- Work Item 1 is assigned to the test user
- Work Item 5 has at least one attachment
- Required shared query folders exist

Exit codes:

- `0` - All validations passed
- `1` - One or more validations failed
- `2` - Script error (authentication, network issues)

### Test Patterns

Tests follow the `ContextSpecification` pattern:

```csharp
[TestClass]
public class Given_some_context : ContextSpecification
{
    private MyClass _sut;

    public override void Given()
    {
        _sut = new MyClass();
    }

    public override void When()
    {
        _sut.DoSomething();
    }

    [TestMethod]
    public void Then_expected_behavior()
    {
        _sut.Result.ShouldBe(expected);
    }
}
```

### Code Coverage

Code coverage helps ensure new code is properly tested. Coverage is collected in CI and available as artifacts.

#### Running Tests with Coverage

```powershell
# Run tests with coverage collection using the repository's coverage settings
dotnet test Qwiq.sln --settings coverage.runsettings

# Or with explicit coverage collection
dotnet test Qwiq.sln --collect:"Code Coverage" --settings coverage.runsettings
```

#### Coverage Configuration

Coverage settings are defined in `coverage.runsettings` at the repository root:

- **Format**: Cobertura XML (CI-friendly, integrates with GitHub Actions)
- **Included assemblies**: Only Qwiq.\* production assemblies
- **Excluded**: Test projects, mocks, benchmarks, third-party dependencies
- **Excluded attributes**: Generated code, debugger-hidden code, `[ExcludeFromCodeCoverage]`

#### Generating Coverage Reports

```powershell
# Install ReportGenerator (one-time)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report from coverage results
reportgenerator -reports:artifacts/TestResults/**/*.cobertura.xml -targetdir:./artifacts/coverage -reporttypes:Html

# Open the report
start ./artifacts/coverage/index.html
```

#### Coverage Guidelines

| Metric                     | Minimum | Target | Notes                    |
| -------------------------- | ------- | ------ | ------------------------ |
| Line Coverage (new code)   | 70%     | 80%    | Enforced for new PRs     |
| Branch Coverage (new code) | 60%     | 70%    | Logical path coverage    |
| Overall Line Coverage      | —       | —      | Tracked but not blocking |

**Best Practices:**

- Write tests for happy paths AND edge cases
- Cover error handling and null checks
- Test public APIs thoroughly
- Use mocks from `Qwiq.Mocks` for unit tests
- Mark intentionally untested code with `[ExcludeFromCodeCoverage]`

## Code Style

### Formatting and Linting

This repository uses automated formatting and linting tools:

**C# Formatting:**

```powershell
# Apply C# analyzer fixes
dotnet format

# Check formatting without applying changes
dotnet format --verify-no-changes
```

**Markdown Formatting:**

```powershell
# Format markdown files with Prettier (via PackedPrettier)
dotnet pprettier --write "**/*.md"

# Check markdown without applying changes
dotnet pprettier --check "**/*.md"
```

**Configuration Files:**

| File                      | Purpose                          |
| ------------------------- | -------------------------------- |
| `.editorconfig`           | Code style and analyzer severity |
| `.prettierrc`             | Prettier formatting rules        |
| `.prettierignore`         | Files to exclude from Prettier   |
| `.markdownlint-cli2.yaml` | Markdown linting rules           |

**Key Markdown Rules Enforced:**

- MD031: Blank lines around fenced code blocks
- MD040: Language identifiers on code blocks (e.g., ` ```csharp `)
- MD034: No bare URLs (use `<url>` or `[text](url)`)
- MD058: Blank lines around tables

### Nullable Reference Types

- C# nullable reference types are **enabled** (`<Nullable>enable</Nullable>`)
- Use `?` suffix for nullable reference types (e.g., `string?`, `IWorkItem?`)
- Use runtime null checks with `ArgumentNullException` for parameter validation

```csharp
public void Method(SomeType parameter)
{
    if (parameter == null) throw new ArgumentNullException(nameof(parameter));
}

public string? GetValue() => _value;
public void SetValue(string value) { }
```

### Exception Handling

- Use `ArgumentNullException` for null parameters
- Use `ArgumentException` for invalid (but non-null) parameters
- **Never swallow exceptions silently** - log errors or let them propagate
- When catching exceptions, log them:

```csharp
catch (Exception ex)
{
    System.Diagnostics.Trace.TraceError($"Operation failed: {ex.Message}");
    throw;
}
```

### Factory Patterns

The codebase extensively uses factory patterns:

```csharp
IWorkItemStore store = Qwiq.Client.Rest.WorkItemStoreFactory.Default.Create(options);
```

### General Guidelines

- Follow existing code patterns in similar files
- Use Central Package Management (versions in `Directory.Packages.props`)
- Avoid JetBrains annotations - use runtime null checks instead
- Keep interfaces for all public types to support mocking

## Architecture Overview

### Client Implementations

QWIQ provides two client implementations:

| Client   | Project            | Use Case                            | API Type  |
| -------- | ------------------ | ----------------------------------- | --------- |
| **REST** | `Qwiq.Client.Rest` | Modern Azure DevOps Services/Server | HTTP/JSON |
| **SOAP** | `Qwiq.Client.Soap` | Legacy TFS on-premises              | SOAP/XML  |

Both implement `IWorkItemStore` and use factory patterns for creation.

### LINQ Provider

The LINQ provider translates C# expressions to WIQL queries:

| Class               | Purpose                                         |
| ------------------- | ----------------------------------------------- |
| `Query<T>`          | Entry point implementing `IOrderedQueryable<T>` |
| `WiqlQueryProvider` | Orchestrates expression tree translation        |
| `QueryRewriter`     | Transforms LINQ to WIQL-compatible nodes        |
| `WiqlTranslator`    | Generates WIQL string from expression tree      |
| `IFieldMapper`      | Maps .NET property names to TFS field names     |

### Mapper System

Converts `IWorkItem` instances to strongly-typed POCOs:

```csharp
[WorkItemType("Bug")]
public class Bug : IIdentifiable<int?>
{
    [FieldDefinition("System.Id")]
    public int? Id { get; set; }

    [FieldDefinition("System.Title")]
    public string Title { get; set; }

    [FieldDefinition("System.AssignedTo")]
    [IdentityField]
    public string AssignedTo { get; set; }
}
```

### Mock System

`Qwiq.Mocks` provides in-memory implementations for unit testing:

| Mock Class                      | Implements                   | Purpose                      |
| ------------------------------- | ---------------------------- | ---------------------------- |
| `MockWorkItemStore`             | `IWorkItemStore`             | In-memory work item storage  |
| `MockWorkItem`                  | `IWorkItem`                  | Work item with field storage |
| `MockIdentityManagementService` | `IIdentityManagementService` | Identity resolution          |

## Key Files Reference

### Configuration Files

| File                        | Purpose                                        |
| --------------------------- | ---------------------------------------------- |
| `global.json`               | Pins .NET SDK version (8.0.100)                |
| `Directory.Build.props`     | Shared MSBuild properties, package metadata    |
| `Directory.Build.targets`   | Shared build targets                           |
| `Directory.Packages.props`  | Central Package Management                     |
| `.config/dotnet-tools.json` | Dotnet tool manifest (nbgv)                    |
| `version.json`              | Nerdbank.GitVersioning configuration           |
| `.editorconfig`             | Code style AND analyzer severity configuration |
| `nuget.config`              | NuGet package sources                          |

### Adding New Packages

1. Add the version to `Directory.Packages.props`:

   ```xml
   <PackageVersion Include="NewPackage" Version="1.0.0" />
   ```

2. Reference in your project file (without version):
   ```xml
   <PackageReference Include="NewPackage" />
   ```

### InternalsVisibleTo Setup

If tests need access to internal types, add to the source project's `.csproj`:

```xml
<ItemGroup>
  <InternalsVisibleTo Include="TestProjectAssemblyName" />
</ItemGroup>
```

### Project Entry Points

| Area             | Start Here                                    |
| ---------------- | --------------------------------------------- |
| Store creation   | `WorkItemStoreFactory` in `Qwiq.Core`         |
| LINQ query       | `WiqlTranslator` in `Qwiq.Linq`               |
| Testing patterns | `ContextSpecification` in `Qwiq.Tests.Common` |
| Mocks            | `MockWorkItemStore` in `Qwiq.Mocks`           |

## Troubleshooting

### Common Issues

**"Type is inaccessible due to its protection level"**

Add `InternalsVisibleTo` to the source project (see [InternalsVisibleTo Setup](#internalsvisibleto-setup)).

**Build fails with file locking errors**

Use single-threaded build:

```powershell
dotnet build /m:1 /nodeReuse:false -v:minimal
```

**SOAP tests fail with TF30063 authorization error**

SOAP tests require Windows integrated authentication. MSA accounts with MFA are not supported. Use:

```powershell
dotnet test --filter "TestCategory!=SOAP"
```

**Package restore fails**

Clear NuGet cache:

```powershell
dotnet nuget locals all --clear
dotnet restore Qwiq.sln
```

### Packages to Avoid

| Package    | Problem                                          | Alternative                        |
| ---------- | ------------------------------------------------ | ---------------------------------- |
| `Polyfill` | Conflicts with VSS Client polyfills (317 errors) | Use custom `NullableAttributes.cs` |
| `Should`   | Conflicts with modern test frameworks            | Use `Shouldly`                     |

### Getting Help

- Check existing issues on GitHub
- Review the `.github/copilot-instructions.md` for detailed technical reference
- Look at similar test files for patterns

---

## Dependency License Policy

QWIQ enforces a dependency license policy to protect library consumers from restrictive license requirements. This policy is automatically enforced via the [Dependency Review Action](.github/workflows/dependency-review.yml) on all pull requests.

### Allowed Licenses (Permissive)

The following licenses are **allowed** because they are permissive and compatible with QWIQ's MIT license:

| License          | Description                                                                                                  |
| ---------------- | ------------------------------------------------------------------------------------------------------------ |
| **MIT**          | Permissive: allows commercial use, modification, distribution with minimal restrictions. QWIQ's own license. |
| **Apache-2.0**   | Permissive with explicit patent grant. Compatible with MIT. Used by many Microsoft packages.                 |
| **BSD-3-Clause** | Permissive: similar to MIT with non-endorsement clause. Common in .NET ecosystem.                            |
| **0BSD**         | Public domain equivalent. No restrictions whatsoever.                                                        |

### Denied Licenses (Copyleft)

The following licenses are **denied** because they impose copyleft requirements that would restrict QWIQ's consumers:

| License      | Why Denied                                                                                                              |
| ------------ | ----------------------------------------------------------------------------------------------------------------------- |
| **GPL-2.0**  | Copyleft: requires derivative works to be GPL-licensed. Incompatible with MIT-licensed library distribution.            |
| **GPL-3.0**  | Stronger copyleft than GPL-2.0 with additional patent provisions. Would force QWIQ consumers to GPL-license their code. |
| **AGPL-3.0** | Network copyleft: even SaaS usage triggers license requirements. Extremely restrictive for library consumers.           |
| **LGPL-3.0** | "Lesser" GPL still requires source disclosure for modifications. Creates compliance burden for consumers.               |

### License Enforcement

Pull requests that introduce dependencies with denied licenses will **fail the dependency review check** and cannot be merged. If you believe a specific dependency is essential despite its license, please:

1. Open an issue explaining the use case
2. Explore alternative packages with permissive licenses
3. Request a license policy exception with business justification

### Vulnerability Policy

In addition to license restrictions, dependencies with **moderate or higher severity vulnerabilities** are blocked. This applies to both runtime and development dependencies.

To check for vulnerabilities before submitting a PR:

```powershell
# Restore dependencies
dotnet restore Qwiq.sln

# List dependencies (optional)
dotnet list package --vulnerable --include-transitive
```

---

## License

By contributing to QWIQ, you agree that your contributions will be licensed under the MIT License.
