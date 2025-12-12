# GitHub Actions Workflow Testing Guide

This document provides information about the GitHub Actions workflow and local testing.

## Current Status

✅ **Workflow Created**: `.github/workflows/main.yml` has been created and pushed
✅ **Code Review**: Completed - all feedback addressed
✅ **Security Scan**: Passed CodeQL analysis with 0 alerts
⚠️ **Nullable Annotations**: Partially complete - see Nullable Status below

## Nullable Reference Types Status

Nullable reference types (`<Nullable>enable</Nullable>`) are enabled repository-wide. Current annotation status:

| Project        | Status      | Notes                                    |
| -------------- | ----------- | ---------------------------------------- |
| Qwiq.Core      | ✅ Complete | 0 nullable warnings                      |
| Qwiq.Core.Rest | ✅ Complete | 0 nullable warnings                      |
| Qwiq.Core.Soap | ⚠️ Pending  | Not yet annotated                        |
| Qwiq.Linq      | ⚠️ Partial  | ~94 warnings - TranslatedQuery annotated |
| Qwiq.Identity  | ✅ Complete | 0 nullable warnings                      |
| Qwiq.Mapper    | ⚠️ Pending  | Not yet annotated                        |
| Test Projects  | ⚠️ Pending  | Mocks and tests need annotation          |

### Common Nullable Patterns

When annotating code:

1. **Nullable value can be null**: Use `T?` suffix (e.g., `string?`, `IWorkItem?`)
2. **Lazy initialized fields**: Use `null!` for fields set later (e.g., `private string _field = null!;`)
3. **Method returns nullable**: Change return type to `T?`
4. **Parameter validation**: Add `if (param == null) throw new ArgumentNullException(nameof(param));`
5. **Null-forgiving operator**: Use `!` when you know value is non-null but compiler doesn't (e.g., `collection!.Count`)
6. **OfType filter**: Use `.OfType<T>()` to filter out nulls from collections

To check nullable warnings in a project:

```powershell
dotnet build src/Qwiq.Core/Qwiq.Core.csproj -c Release 2>&1 | Select-String "error CS8"
```

## Build & Test Commands (SDK-Style Projects)

### Prerequisites

- **Windows machine** required for `net472` targets (SOAP client)
- .NET 8.0 SDK (pinned in `global.json`)
- Visual Studio 2022+ or VS Code with C# extension

### Local Build Commands

```powershell
# Restore tools (nbgv for versioning)
dotnet tool restore

# Restore packages and build
dotnet restore Qwiq.sln
dotnet build Qwiq.sln --configuration Release

# Or single command (restore is implicit)
dotnet build Qwiq.sln -c Release
```

### Local Test Commands

```powershell
# Run tests with category exclusions
dotnet test Qwiq.sln --configuration Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
```

**Test Categories to Exclude:**

- `localOnly` - Requires local TFS instance
- `Benchmark` - Performance tests
- `SOAP` / `REST` - Integration tests requiring server
- `IntegrationTests` - Full integration tests

---

## Package Baseline Testing

### Overview

The Qwiq repository uses [Verify.Nupkg](https://github.com/MattKotsenas/Verify.Nupkg) for snapshot testing of NuGet package structure and manifests. This ensures package contents remain consistent across builds and prevents accidental changes to package structure.

### How It Works

1. **Package Discovery**: Tests scan `src/**/bin/Release/` for `.nupkg` files
2. **Deduplication**: Multiple versions from incremental builds are deduplicated by timestamp (latest wins)
3. **Snapshot Generation**: Verify.Nupkg extracts ZIP contents and generates:
   - ASCII tree view of package structure
   - Scrubbed `.nuspec` manifest (version numbers normalized)
4. **Baseline Comparison**: Generated snapshots compared against `.verified` files

### Running Package Tests

```powershell
# Run all package tests
dotnet test test/Qwiq.Package.Tests/Qwiq.Package.Tests.csproj --configuration Release

# Run package tests after building packages
dotnet build Qwiq.sln -c Release  # Generates packages via GeneratePackageOnBuild
dotnet test test/Qwiq.Package.Tests/Qwiq.Package.Tests.csproj --configuration Release --no-build
```

**Expected Output**: 10 tests (9 library packages + 1 test per package)

### Symbol Package Testing (Deferred)

Symbol packages (`.snupkg`) are **not currently baselined** because Verify.Nupkg doesn't support the `.snupkg` extension yet.

**Current Behavior:**

- Tests skip `.snupkg` files with logged message
- CI validates symbol packages using `dotnet sourcelink test` (limited scope)
- Upstream feature request: [MattKotsenas/Verify.Nupkg#38](https://github.com/MattKotsenas/Verify.Nupkg/issues/38)

**When Upstream Adds Support:**

1. Update `GetPackages()` to include `*.snupkg` in discovery
2. Remove skip logging
3. Regenerate 9 `.snupkg.verified` baseline files
4. Test count will increase to 18 (9 .nupkg + 9 .snupkg)

### Updating Baselines

When package structure intentionally changes:

```powershell
# Regenerate baselines (requires DiffEngine for interactive review)
dotnet test test/Qwiq.Package.Tests/Qwiq.Package.Tests.csproj -c Release

# Accept changes in diff tool (e.g., VS Code, Beyond Compare)
# Commit updated .verified files
```

### Package Deduplication

**Problem**: Incremental builds create multiple package versions in `bin/Release/`, causing test prefix collisions.

**Solution**: Tests deduplicate by package discriminator (ID without version/extension), keeping only the latest by timestamp:

```csharp
.GroupBy(GetPackageDiscriminator, StringComparer.OrdinalIgnoreCase)
.Select(group => group.OrderByDescending(fileInfo => fileInfo.LastWriteTimeUtc).First())
```

**Example**: If `Qwiq.Core.2.0.1.nupkg` and `Qwiq.Core.2.0.2.nupkg` both exist, only `2.0.2` is tested.

### References

- Feature request: `docs/issues/verify-nupkg-snupkg-support.md`
- Test implementation: `test/Qwiq.Package.Tests/PackageTests.cs`
- MIGRATION_NOTES.md: Package testing modernization section

## Workflow Overview

The workflow runs on Windows:

### Windows Runner (Primary)

- **Purpose**: Full build and test execution
- **Tools**: .NET SDK, dotnet CLI
- **Steps**:
  1. Checkout code with full history (for versioning)
  2. Setup .NET SDK using `global.json`
  3. Restore dotnet tools (`dotnet tool restore`)
  4. Restore NuGet packages
  5. Build solution in Release configuration
  6. Run all unit tests (excluding IntegrationTests and specified categories)
  7. Upload test results and binaries

## How to Test Locally

1. Clone the repository
2. Ensure you have .NET 8.0 SDK installed
3. Run `dotnet tool restore` to restore build tools
4. Run `dotnet build Qwiq.sln -c Release` to build
5. Run `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"` to run tests

## Expected Outcomes

### Success Criteria

- ✅ Build completes successfully
- ✅ NuGet packages restore without errors
- ✅ Solution builds in Release configuration
- ✅ All unit tests pass (excluding specified categories)

### Known Potential Issues

#### 1. Windows-Only for SOAP Projects

SOAP projects (`Qwiq.Core.Soap`, `Qwiq.Identity.Soap`) require Windows and .NET Framework 4.7.2.

#### 2. Test Discovery Issues

**Symptom**: No test assemblies found

**Solution**: The tests are discovered using dotnet test. Verify that:

- Tests are building correctly
- Test project target frameworks include the framework you're testing on
- Test filter is not excluding all tests

## Monitoring CI Runs

### During Execution

1. Watch the Actions tab for real-time progress
2. Expand each step to see detailed logs
3. Pay attention to warnings even if build succeeds

### After Completion

1. Check test results artifact for detailed test output
2. Review binaries artifact to ensure all expected DLLs are present
3. Look for any warnings or errors in the logs

## Iterating on Failures

If the workflow fails, follow these steps:

1. **Capture Error Information**

   - Copy full error messages
   - Note which step failed
   - Check exit codes and stack traces

2. **Determine Root Cause**

   - Is it a known issue (see above)?
   - Is it a configuration problem?
   - Is it a missing dependency?

3. **Apply Fix**

   - I can modify the workflow based on error information
   - May need to add workarounds or alternative approaches
   - Document any changes in commits

4. **Re-test**
   - Push updated workflow
   - Trigger new run
   - Verify fix resolved the issue

## Comparison with AppVeyor

### What's Different

- **Platform**: GitHub Actions instead of AppVeyor
- **Build Tool**: dotnet CLI instead of MSBuild directly
- **Test Execution**: dotnet test instead of VSTest

### What's the Same

- **Build Configuration**: Release, Any CPU
- **Test Filters**: Same categories excluded (localOnly, Benchmark, SOAP, REST, IntegrationTests)
- **Multi-targeting**: net472, netstandard2.0, net8.0

---

## Code Coverage

### Coverage Gates

Code coverage is collected in CI and available as artifacts. The following gates apply to new code:

| Metric                     | Minimum  | Target    | Notes                    |
| -------------------------- | -------- | --------- | ------------------------ |
| Line Coverage (new code)   | 70%      | 80%       | Enforced for new PRs     |
| Branch Coverage (new code) | 60%      | 70%       | Logical path coverage    |
| Overall Line Coverage      | Baseline | Improving | Tracked but not blocking |

**Coverage Philosophy:**

- New code should meet target coverage (80% line, 70% branch)
- Existing code coverage tracked for visibility
- Coverage reports available as CI artifacts
- Focus on meaningful tests over hitting percentages

### Running Coverage Locally

To collect and view code coverage locally:

```powershell
# Run tests with coverage collection using repository settings
dotnet test Qwiq.sln --settings coverage.runsettings

# Or with explicit coverage collector
dotnet test Qwiq.sln --collect:"Code Coverage" --settings coverage.runsettings

# Generate HTML report using reportgenerator
dotnet tool restore  # Ensures reportgenerator is available
reportgenerator -reports:artifacts/TestResults/**/*.cobertura.xml -targetdir:./artifacts/coverage -reporttypes:Html

# Open the report
start ./artifacts/coverage/index.html  # Windows
open ./artifacts/coverage/index.html   # macOS
xdg-open ./artifacts/coverage/index.html  # Linux
```

### Coverage Configuration

Coverage settings are defined in `coverage.runsettings` at the repository root. Key settings:

| Setting               | Value     | Purpose                                           |
| --------------------- | --------- | ------------------------------------------------- |
| `Format`              | cobertura | CI-friendly XML output for GitHub Actions         |
| `IncludeTestAssembly` | False     | Excludes test assemblies from coverage metrics    |
| `SkipAutoProps`       | true      | Skips trivial auto-properties for cleaner metrics |
| `TargetPlatform`      | x64       | Matches CI environment                            |

**Included Assemblies** (explicit list):

- `Qwiq.Core.dll`, `Qwiq.Client.Rest.dll`, `Qwiq.Client.Soap.dll`
- `Qwiq.Linq.dll`, `Qwiq.Linq.Identity.dll`
- `Qwiq.Mapper.dll`, `Qwiq.Mapper.Identity.dll`
- `Qwiq.Identity.dll`, `Qwiq.Identity.Soap.dll`

**Excluded by Pattern**:

- Test projects (`*Tests*`, `*Mocks*`, `*Benchmark*`)
- Generated code (`*.g.cs`, `*.generated.cs`)
- Compatibility shims (`Compatibility\*`)
- Third-party assemblies (by public key token)

### CI Coverage Workflow

1. **Collection**: Tests run with `--collect:"XPlat Code Coverage"`
2. **Report Generation**: `reportgenerator` creates HTML and summary reports
3. **Artifact Upload**: Coverage reports uploaded as `coverage-report` artifact
4. **Review**: Download artifact from Actions tab to view detailed coverage

### Viewing CI Coverage Reports

1. Navigate to the GitHub Actions run
2. Scroll to "Artifacts" section at bottom
3. Download `coverage-report.zip`
4. Extract and open `index.html` in browser

### Coverage Best Practices

- Write tests for happy paths AND edge cases
- Cover error handling and null checks
- Test public APIs thoroughly
- Use mocks from `Qwiq.Mocks` for unit tests
- Integration tests supplement but don't replace unit test coverage
