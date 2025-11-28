# Copilot Coding Agent Instructions for QWIQ

## Repository Overview

QWIQ (**Q**uick **W**ork **I**tem **Q**uery) is a .NET Framework library providing a simplified API for querying Azure DevOps / Team Foundation Server work items. It wraps the TFS Client OM with cleaner interfaces, factory patterns, and mock support.

**Key Characteristics:**
- .NET Framework 4.6 library (legacy project format)
- **Windows-only build requirement** (requires Visual Studio / MSBuild)
- Uses packages.config for NuGet dependencies
- Uses Nerdbank.GitVersioning for version management

## Build & Test Commands

### Prerequisites
- **Windows machine with Visual Studio 2017+** required
- .NET Framework 4.6 SDK/targeting pack
- MSBuild and NuGet CLI

### Build Commands (Windows only)
```powershell
# Restore NuGet packages (basic)
nuget restore Qwiq.sln

# Build solution with MSBuild
msbuild Qwiq.sln /p:Configuration=Release /p:Platform="Any CPU" /v:minimal /m
```

**CI-aligned restore command:** If you encounter restore issues, use the exact CI command:
```powershell
nuget restore Qwiq.sln -NonInteractive -PackagesDirectory packages -ConfigFile nuget.config
```
This ensures packages go into the local `packages/` directory and uses the repo's NuGet configuration.

### Test Commands
The workflow uses VSTest to run tests:
```powershell
# Run tests (exclude integration/local-only tests)
vstest.console.exe <TestAssembly.dll> /TestCaseFilter:"TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST"
```

**Test Categories to Exclude:**
- `localOnly` - Requires local TFS instance
- `Benchmark` - Performance tests
- `SOAP` / `REST` - Integration tests requiring server
- `IntegrationTests` - Full integration tests (excluded by assembly name pattern)

## Project Layout

### Source Projects (`src/`)
| Project | Description |
|---------|-------------|
| `Qwiq.Core` | Core interfaces and abstractions |
| `Qwiq.Core.Rest` | REST API client implementation |
| `Qwiq.Core.Soap` | SOAP client implementation |
| `Qwiq.Linq` | LINQ query provider |
| `Qwiq.Mapper` | Object mapping layer |
| `Qwiq.Identity` / `.Soap` | Identity management |

### Test Projects (`test/`)
- Unit tests: `Qwiq.Core.Tests`, `Qwiq.Linq.Tests`, `Qwiq.Mapper.Tests`, `Qwiq.Identity.Tests`
- Integration tests: `Qwiq.Integration.Tests`
- Mocks: `Qwiq.Mocks`
- Benchmarks: `Qwiq.Benchmark`, `*.Benchmark.Tests`

### Key Entry Points
For most feature work, start with these locations before searching broadly:
- `WorkItemStoreFactory` in `Qwiq.Core` - Store creation and connection
- `WiqlTranslator` in `Qwiq.Linq` - LINQ-to-WIQL query translation
- `Qwiq.Mocks` and `ContextSpecification` in `Qwiq.Tests.Common` - Testing patterns

### Key Configuration Files
- `build/targets/common.props` - Shared MSBuild properties
- `nuget.config` - NuGet package sources
- `version.json` - Nerdbank.GitVersioning configuration
- `.editorconfig` - Code style (4-space indent, CRLF line endings)
- `appveyor.yml` - Legacy CI configuration (AppVeyor)

## Critical Build Notes

### 1. Windows-Only Build
- This project uses legacy .csproj format and targets .NET Framework 4.6
- Cannot build on Linux/macOS - requires Windows with .NET Framework SDK
- GitHub Actions workflow uses `windows-latest` runner

### 2. NuGet Restore
- Uses packages.config (not PackageReference)
- Always run `nuget restore` before building
- Packages are restored to solution-level `packages/` directory

### 3. Warning Configuration
Common warnings suppressed in `build/targets/common.props`:
- NoWarn 1591 (missing XML docs)
- TreatWarningsAsErrors is enabled

## Common Issues & Workarounds

### Security Considerations
1. **Current state:** Project targets .NET Framework 4.6. When upgrading, target 4.7.2+ (known security issues in older versions)
2. Be cautious when updating packages - check for breaking API changes

### Code Style
1. Uses JetBrains.Annotations for null annotations (existing pattern)
2. Follow existing patterns for exception handling and null checks
3. Use `Contract.Requires` for parameter validation

### Known Patterns
1. Factory pattern used extensively (e.g., `WorkItemStoreFactory`)
2. Interfaces for all public types to support mocking
3. Linked `AssemblyInfo.Common.cs` for shared assembly attributes

## CI/CD Pipeline

The main workflow (`.github/workflows/main.yml`) runs on:
- Push to `develop` or `master`
- Pull requests to `develop` or `master`

**Expected Workflow Steps:**
1. Checkout with `fetch-depth: 0` (for versioning)
2. Setup MSBuild, NuGet, VSTest
3. Restore NuGet packages
4. Build solution with MSBuild
5. Run tests with VSTest and category filters
6. Upload test results and binaries

### When Editing GitHub Actions Workflows
When adding or updating .NET workflows in this repo, follow these guidelines:
- Use `windows-latest` runner (not `windows-2019` which is retired)
- If using `actions/setup-dotnet`, prefer `global-json-file: ./global.json` over `dotnet-version`
- Add `dotnet tool restore` after setting up .NET when relying on tools like Nerdbank.GitVersioning
- Include deterministic build flags: `/p:Deterministic=true /p:UseSharedCompilation=false /nodeReuse:false`
- Upload binlogs as artifacts for debugging: `/bl:./artifacts/logs/build.binlog`
- Prefer `.runsettings` files or environment variables for complex test filters instead of long inline strings

## When Making Changes

**Do:**
1. **Always restore before building:** `nuget restore Qwiq.sln`
2. **Build with MSBuild:** `msbuild Qwiq.sln /p:Configuration=Release`
3. **Test on Windows only** - this is a .NET Framework project
4. **Follow existing code patterns** - check similar files for conventions
5. **Update packages.config** when adding new NuGet packages
6. **Run tests with appropriate filters** to exclude integration tests

**Do not:**
- Modify `Directory.Build.props`, `Directory.Build.targets`, or `nuget.config` as part of feature/bugfix PRs - these centralize repo-wide behavior
- Upgrade critical NuGet dependencies (`Microsoft.TeamFoundationServer.*`, `Microsoft.VisualStudio.Services.*`, `Newtonsoft.Json`) unless explicitly tasked with dependency updates
- Attempt large-scale migrations (SDK-style conversion, target framework changes, removing SOAP support) as incidental changes - these require dedicated PRs

## Trust These Instructions

These instructions are accurate for the current state of the repository. If something doesn't work as documented, first verify the instructions before exploring alternatives. Report any discrepancies found during your work.
