# Copilot Coding Agent Instructions for QWIQ

## Repository Overview

QWIQ (**Q**uick **W**ork **I**tem **Q**uery) is a .NET library providing a simplified API for querying Azure DevOps / Team Foundation Server work items. It wraps the TFS Client OM with cleaner interfaces, factory patterns, and mock support.

**Key Characteristics:**

- Modern SDK-style projects with multi-targeting
- Target frameworks: `net472`, `netstandard2.0`, `net8.0` (varies by project)
- **Windows-only build requirement** for full framework coverage
- Central Package Management via `Directory.Packages.props`
- Nerdbank.GitVersioning for version management (via dotnet tool manifest)

## Build & Test Commands

### Prerequisites

- **Windows machine** required for `net472` targets (SOAP client)
- .NET 8.0 SDK (pinned in `global.json`)
- Visual Studio 2022+ or VS Code with C# extension

### Build Commands

```powershell
# Restore tools (nbgv for versioning)
dotnet tool restore

# Restore packages and build
dotnet restore Qwiq.sln
dotnet build Qwiq.sln --configuration Release

# Or single command (restore is implicit)
dotnet build Qwiq.sln -c Release
```

### Test Commands

```powershell
# Run tests with category exclusions
dotnet test Qwiq.sln --configuration Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
```

**Test Categories to Exclude:**

- `localOnly` - Requires local TFS instance
- `Benchmark` - Performance tests
- `SOAP` / `REST` - Integration tests requiring server
- `IntegrationTests` - Full integration tests

## Project Layout

### Source Projects (`src/`)

| Project              | Target Frameworks            | Description                         |
| -------------------- | ---------------------------- | ----------------------------------- |
| `Qwiq.Core`          | net472;netstandard2.0;net8.0 | Core interfaces and abstractions    |
| `Qwiq.Core.Rest`     | net472;netstandard2.0;net8.0 | REST API client implementation      |
| `Qwiq.Core.Soap`     | net472                       | SOAP client (Windows only)          |
| `Qwiq.Linq`          | net472;net8.0                | LINQ query provider                 |
| `Qwiq.Mapper`        | net472;net8.0                | Object mapping layer                |
| `Qwiq.Identity`      | net472;net8.0                | Identity management                 |
| `Qwiq.Identity.Soap` | net472                       | Identity SOAP client (Windows only) |

### Test Projects (`test/`)

| Project                 | Target Frameworks | Description            |
| ----------------------- | ----------------- | ---------------------- |
| `Qwiq.Core.Tests`       | net472;net8.0     | Core unit tests        |
| `Qwiq.Linq.Tests`       | net472;net8.0     | LINQ provider tests    |
| `Qwiq.Mapper.Tests`     | net472;net8.0     | Mapper tests           |
| `Qwiq.Identity.Tests`   | net472;net8.0     | Identity tests         |
| `Qwiq.IntegrationTests` | net472            | Full integration tests |
| `Qwiq.Mocks`            | net472;net8.0     | Mock implementations   |

### Key Entry Points

For most feature work, start with these locations before searching broadly:

- `WorkItemStoreFactory` in `Qwiq.Core` - Store creation and connection
- `WiqlTranslator` in `Qwiq.Linq` - LINQ-to-WIQL query translation
- `Qwiq.Mocks` and `ContextSpecification` in `Qwiq.Tests.Common` - Testing patterns

### Key Configuration Files

| File                        | Purpose                                        |
| --------------------------- | ---------------------------------------------- |
| `global.json`               | Pins .NET SDK version (8.0.100)                |
| `Directory.Build.props`     | Shared MSBuild properties, package metadata    |
| `Directory.Build.targets`   | Shared build targets                           |
| `Directory.Packages.props`  | Central Package Management                     |
| `.config/dotnet-tools.json` | Dotnet tool manifest (nbgv)                    |
| `version.json`              | Nerdbank.GitVersioning configuration           |
| `.editorconfig`             | Code style (4-space indent, CRLF line endings) |
| `nuget.config`              | NuGet package sources                          |

## Critical Build Notes

### 1. Windows-Only Build for SOAP

- SOAP projects (`Qwiq.Core.Soap`, `Qwiq.Identity.Soap`) require Windows
- They depend on `Microsoft.TeamFoundationServer.ExtendedClient` which only supports `net472`
- GitHub Actions workflow uses `windows-latest` runner

### 2. Multi-Targeting Strategy

- Core libraries: `net472;netstandard2.0;net8.0`
- SOAP projects: `net472` only (Windows dependency)
- REST projects: `net472;netstandard2.0;net8.0`
- Test projects: `net472;net8.0`

### 3. Central Package Management

- All package versions are defined in `Directory.Packages.props`
- Individual csproj files use `<PackageReference Include="..." />` without versions
- To add a new package: add version to `Directory.Packages.props`, then reference in csproj

### 4. SDK-Style Packaging

- NuGet packages are built using SDK pack (no .nuspec files)
- Package metadata is in `Directory.Build.props` (Authors, Copyright, License, etc.)
- Project-specific metadata in individual csproj files (Description, PackageId)

## Code Style & Patterns

### ⚠️ IMPORTANT: Nullable Reference Types Enabled

- C# nullable reference types are enabled (`<Nullable>enable</Nullable>`)
- Use `?` suffix for nullable reference types (e.g., `string?`, `IWorkItem?`)
- JetBrains.Annotations (`[NotNull]`, `[CanBeNull]`, etc.) have been removed
- Use runtime null checks with `ArgumentNullException` for parameter validation

### Null Validation Pattern

```csharp
// CORRECT: Runtime null check with clear exception
public void Method(SomeType parameter)
{
    if (parameter == null) throw new ArgumentNullException(nameof(parameter));
    // or for constructor base calls:
    // : base(parameter?.Property ?? throw new ArgumentNullException(nameof(parameter)))
}

// CORRECT: Nullable annotations
public string? GetValue() => _value; // Can return null
public void SetValue(string value) { } // Cannot be null
```

### Exception Handling

- Use `ArgumentNullException` for null parameters
- Use `ArgumentException` for invalid (but non-null) parameters
- Use `Contract.Requires` for design-by-contract assertions (optional)

### Known Patterns

1. Factory pattern used extensively (e.g., `WorkItemStoreFactory`)
2. Interfaces for all public types to support mocking
3. Internal types marked with `internal` visibility
4. Lazy initialization for expensive operations

### Nullable Reference Types Status

Nullable reference types are enabled repository-wide. Status by project:

- **Qwiq.Core**: ✅ Fully annotated (0 nullable warnings)
- **Qwiq.Core.Rest**: ⚠️ Partially annotated (~42 warnings remaining)
- **Qwiq.Core.Soap**: ⚠️ Needs annotation
- **Qwiq.Linq**: ⚠️ Needs annotation (~128 warnings)
- **Qwiq.Identity**: ⚠️ Needs annotation (~28 warnings)
- **Qwiq.Mapper**: ⚠️ Needs annotation

Common nullable patterns in this codebase:

- Use `T?` for properties that can legitimately return null
- Use `null!` for lazy-initialized fields that are guaranteed to be set before use
- Update both interfaces AND implementations when changing nullability
- Use `[MaybeNullWhen(false)]` attribute for Try\* out parameters

## CI/CD Pipeline

The main workflow (`.github/workflows/main.yml`) runs on:

- Push to `develop` or `master`
- Pull requests to `develop` or `master`

**Expected Workflow Steps:**

1. Checkout with `fetch-depth: 0` (for versioning)
2. Setup .NET SDK using `global.json`
3. Restore dotnet tools (`dotnet tool restore`)
4. Restore packages (`dotnet restore`)
5. Build solution (`dotnet build`)
6. Run tests with category filters
7. Upload test results and binaries

### When Editing GitHub Actions Workflows

When adding or updating .NET workflows in this repo, follow these guidelines:

- Use `windows-latest` runner (not `windows-2019` which is retired)
- If using `actions/setup-dotnet`, prefer `global-json-file: ./global.json` over `dotnet-version`
- Add `dotnet tool restore` after setting up .NET when relying on tools like Nerdbank.GitVersioning
- Include deterministic build flags: `/p:Deterministic=true /p:UseSharedCompilation=false /nodeReuse:false`
- Upload binlogs as artifacts for debugging: `/bl:./artifacts/logs/build.binlog`
- Prefer `.runsettings` files or environment variables for complex test filters instead of long inline strings

## ⚠️ CRITICAL: Commit Practices

**This is very important.** All changes must be committed incrementally, with small, atomic commits.

### Conventional Commits Required

Use the [Conventional Commits](https://www.conventionalcommits.org/) format:

```
<type>(<scope>): <short description>

<optional body with more details>
```

**Types:**

- `fix` - Bug fixes (e.g., `fix(core): add null guard to QueryDefinition constructor`)
- `feat` - New features (e.g., `feat(linq): add support for Contains operator`)
- `refactor` - Code restructuring without behavior change
- `docs` - Documentation only
- `test` - Adding or fixing tests
- `chore` - Maintenance tasks (dependencies, CI, tooling)
- `style` - Code formatting (no logic changes)
- `build` - Build system changes
- `ci` - CI/CD configuration changes

**Scopes** (optional but encouraged):

- `core` - Qwiq.Core changes
- `rest` - REST client changes
- `soap` - SOAP client changes
- `linq` - LINQ provider changes
- `mapper` - Mapper changes
- `identity` - Identity management changes
- `ci` - CI/CD pipeline

### Work Incrementally

1. **Small commits** - Each commit should represent ONE logical change
2. **Atomic commits** - Each commit must build successfully on its own
3. **Verify before committing** - Build the affected project(s) before each commit
4. **Don't batch unrelated changes** - Separate bug fixes from refactoring from features

### Commit Workflow

```powershell
# 1. Make a focused change
# 2. Build to verify it works
dotnet build src/Qwiq.Core/Qwiq.Core.csproj -c Debug

# 3. Stage only related files
git add src/Qwiq.Core/SomeFile.cs

# 4. Commit with conventional message
git commit -m "fix(core): add null guard to prevent NullReferenceException"

# 5. Repeat for next logical change
```

### Bad vs Good Examples

❌ **Bad:** One giant commit with 40 files mixing bug fixes, refactoring, and new features

✅ **Good:** Three separate commits:

1. `fix(core): add null guards to constructor parameters`
2. `refactor: migrate AssemblyInfo to SDK-generated attributes`
3. `docs: update copilot-instructions with commit guidelines`

### Why This Matters

- **Code review** - Smaller commits are easier to review
- **Git bisect** - Atomic commits help find when bugs were introduced
- **Reverts** - Can revert a specific change without losing unrelated work
- **History** - Clean history tells the story of the codebase

## When Making Changes

1. **Build with dotnet CLI:** `dotnet build Qwiq.sln -c Release`
2. **Test with filters:** `dotnet test --filter "TestCategory!=localOnly&..."`
3. **Follow existing code patterns** - check similar files for conventions
4. **Use Central Package Management** - add versions to `Directory.Packages.props`
5. **No JetBrains annotations** - use runtime null checks instead
6. **Update task list** - if working from `.agents/PR31-TASK-LIST.md`
7. **Commit incrementally** - small, atomic commits with conventional messages

## Compatibility Shims

The `src/Qwiq.Core/Compatibility/` directory contains shims for API compatibility across different .NET versions and package versions.

### IdentityTypeMapper

- Location: `src/Qwiq.Core/Compatibility/IdentityTypeMapper.cs`
- Purpose: Compatibility shim for class removed in Microsoft.VisualStudio.Services.Client v19+
- Can be removed when: Qwiq drops support for identity type mapping OR Microsoft restores this class

### NullableAttributes

- Location: `src/Qwiq.Core/Compatibility/NullableAttributes.cs`
- Purpose: Provides nullable attribute definitions (`MaybeNullWhenAttribute`, `AllowNullAttribute`, `NotNullAttribute`, etc.) for `net472` and `netstandard2.0` targets
- Conditionally compiled: `#if NETFRAMEWORK || NETSTANDARD2_0`
- **Important:** Do NOT use the `Polyfill` NuGet package - it conflicts with polyfills in Microsoft.VisualStudio.Services.Client, causing 300+ ambiguous method errors
- Can be removed when: The project drops support for net472/netstandard2.0

## InternalsVisibleTo Configuration

After migration to SDK-style projects, `InternalsVisibleTo` attributes are defined in individual `.csproj` files (not in AssemblyInfo.cs which was removed).

**Pattern:**

```xml
<ItemGroup>
  <InternalsVisibleTo Include="TestProjectAssemblyName" />
</ItemGroup>
```

**Current configuration:**

- `Qwiq.Core.csproj` → `Qwiq.Core.UnitTests`, `Qwiq.Mocks`
- `Qwiq.Client.Rest.csproj` → `Qwiq.IntegrationTests`
- `Qwiq.Client.Soap.csproj` → `Qwiq.Identity.Soap`, `Qwiq.IntegrationTests`
- `Qwiq.Identity.Soap.csproj` → `Qwiq.IntegrationTests`
- `Qwiq.Mapper.Identity.csproj` → `Qwiq.Identity.UnitTests`

If you encounter `'Type' is inaccessible due to its protection level` errors in tests, add an `InternalsVisibleTo` entry to the source project.

## ⚠️ Build Troubleshooting

### Windows File Locking Issues

Parallel builds on Windows can fail with file access errors. Use single-threaded build:

```powershell
dotnet build /m:1 /nodeReuse:false -v:minimal
```

### Nullable Warning Suppressions

The following nullable warnings are suppressed repository-wide in `Directory.Build.props` to allow gradual migration:

- `CS8600-CS8604` - Null assignment/conversion warnings
- `CS8605` - Unboxing possibly null value
- `CS8618-CS8620` - Non-nullable field/property initialization
- `CS8625` - Cannot convert null literal
- `CS8629` - Nullable value type may be null
- `CS8764-CS8769` - Nullability of reference type

### Package Conflicts to Avoid

| Package    | Problem                                                         | Solution                                       |
| ---------- | --------------------------------------------------------------- | ---------------------------------------------- |
| `Polyfill` | Conflicts with VSS Client polyfills (317 ambiguous errors)      | Use custom `NullableAttributes.cs`             |
| `Should`   | Legacy assertion library, conflicts with modern test frameworks | Use `Shouldly` with `ShouldExtensions.cs` shim |

## Do's and Don'ts

**Do:**

1. **Always restore before building:** `dotnet restore Qwiq.sln`
2. **Build with dotnet CLI:** `dotnet build Qwiq.sln -c Release`
3. **Test on Windows only** - this is a .NET Framework project
4. **Follow existing code patterns** - check similar files for conventions
5. **Use Central Package Management** - add versions to `Directory.Packages.props`, not individual csproj files
6. **Run tests with appropriate filters** to exclude integration tests

**Do not:**

- Modify `Directory.Build.props`, `Directory.Build.targets`, or `nuget.config` as part of feature/bugfix PRs - these centralize repo-wide behavior
- Upgrade critical NuGet dependencies (`Microsoft.TeamFoundationServer.*`, `Microsoft.VisualStudio.Services.*`, `Newtonsoft.Json`) unless explicitly tasked with dependency updates
- Attempt large-scale migrations (SDK-style conversion, target framework changes, removing SOAP support) as incidental changes - these require dedicated PRs
- Use the `Polyfill` NuGet package - it conflicts with VSS Client polyfills
- Add `Version` attributes to PackageReference when using Central Package Management (add version to `Directory.Packages.props` instead)

## Test Configuration

### Test Categories

Tests are categorized to allow selective execution:

| Category           | Description                 | When to Run                      |
| ------------------ | --------------------------- | -------------------------------- |
| (default)          | Unit tests                  | Always (CI)                      |
| `localOnly`        | Requires local TFS instance | Manual, local dev                |
| `Benchmark`        | Performance benchmarks      | Manual                           |
| `SOAP`             | SOAP integration tests      | Manual, with TFS credentials     |
| `REST`             | REST integration tests      | Manual, with Azure DevOps access |
| `IntegrationTests` | Full integration suite      | Manual, with server access       |

### Package Tests

The `Qwiq.Package.Tests` project validates NuGet package contents using Verify. These tests:

- Require `dotnet pack` to run first (packages must exist)
- Compare package manifests and contents against verified baselines
- Will fail if run without first creating packages

### Integration Tests

Integration tests in `Qwiq.IntegrationTests` require:

- TFS/Azure DevOps server credentials
- Access to `https://microsoft.visualstudio.com/defaultcollection` (or configure `IntegrationSettings.cs`)
- Windows environment (SOAP tests use net472)

## Trust These Instructions

These instructions reflect the modernized state of the repository (PR #31). The repository has been migrated from:

- ❌ Legacy .csproj format → ✅ SDK-style projects
- ❌ packages.config → ✅ Central Package Management
- ❌ .nuspec files → ✅ SDK-style packaging
- ❌ JetBrains.Annotations → ✅ Runtime null checks
- ❌ .NET Framework 4.6 → ✅ Multi-targeting (net472/netstandard2.0/net8.0)
