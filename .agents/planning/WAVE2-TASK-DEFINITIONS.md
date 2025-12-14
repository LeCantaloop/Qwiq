# Wave 2 Task Definitions: Qwiq Modernization

> **Purpose**: Detailed task definitions for Wave 2 modernization tasks with updated priorities and requirements
>
> **Last Updated**: December 5, 2025
> **Status**: Planning - Ready for Implementation

---

## Table of Contents

1. [W2.11 Release Automation](#w211-release-automation-critical)
2. [W2.13 SBOM Generation](#w213-sbom-generation-high)
3. [W2.16 REST/SOAP Unit Test Coverage](#w216-restsoap-unit-test-coverage-high)
4. [Task Dependencies](#task-dependencies)
5. [Implementation Sequence](#implementation-sequence)

---

## W2.11 Release Automation (CRITICAL)

### Overview

**Task ID**: W2.11
**Title**: Create Release Workflow with Composite Actions
**Status**: 📋 Planned
**Effort**: M (2-3 days)
**Priority**: **CRITICAL** - Blocks manual release process
**Dependencies**: W1.2 (Source Link - Complete ✅)

### Goal

Automate NuGet package publishing on version tags using GitHub Actions, following moq.analyzers patterns with DRY composite actions and workflow reuse.

### Key Requirements

1. **Trigger on Tags**: Use `v*` pattern for version tags
2. **Windows Runner**: Use `windows-latest` (net472 requirement)
3. **Workflow Reuse**: Call `main.yml` via `workflow_call` to avoid duplication
4. **Composite Action**: Create reusable setup-restore-build action
5. **NuGet Publishing**: Use `--skip-duplicate` to prevent re-publish errors
6. **GitHub Release**: Attach packages and auto-generate release notes

### Files to Create/Modify

| File                                            | Action | Description                                 |
| ----------------------------------------------- | ------ | ------------------------------------------- |
| `.github/workflows/release.yml`                 | Create | Main release workflow triggered by tags     |
| `.github/actions/setup-dotnet-build/action.yml` | Create | Composite action for common setup steps     |
| `.github/workflows/main.yml`                    | Modify | Add `workflow_call` trigger for reusability |
| `README.md`                                     | Update | Document release process                    |

### Implementation Details

#### 1. Composite Action: `.github/actions/setup-dotnet-build/action.yml`

```yaml
name: "Setup .NET and Build"
description: "Reusable composite action for .NET setup, restore, and build"

inputs:
  configuration:
    description: "Build configuration (Debug/Release)"
    required: false
    default: "Release"

  enable-pack:
    description: "Enable NuGet pack during build"
    required: false
    default: "false"

runs:
  using: composite
  steps:
    - name: Setup .NET SDK
      uses: actions/setup-dotnet@v4
      with:
        global-json-file: ./global.json
      shell: pwsh

    - name: Restore .NET tools
      run: dotnet tool restore
      shell: pwsh

    - name: Restore NuGet packages
      run: dotnet restore Qwiq.sln
      shell: pwsh

    - name: Build solution
      shell: pwsh
      run: |
        $targets = if ('${{ inputs.enable-pack }}' -eq 'true') { '/t:Build,Pack' } else { '' }
        dotnet build Qwiq.sln -c ${{ inputs.configuration }} --no-restore $targets /p:ContinuousIntegrationBuild=true /m:1 /nodeReuse:false /bl:./artifacts/logs/build.binlog
```

#### 2. Main Workflow: `.github/workflows/main.yml` Update

```yaml
name: Main build

on:
  push:
    branches:
      - develop
      - master
  pull_request:
  workflow_dispatch:
  workflow_call: # ADD THIS - enables reuse from release.yml
    inputs:
      configuration:
        type: string
        default: "Release"
# Rest of existing workflow...
```

#### 3. Release Workflow: `.github/workflows/release.yml`

```yaml
name: Release

on:
  push:
    tags:
      - "v*"
  workflow_dispatch: # Allow manual trigger for testing

permissions:
  contents: write # Required for creating GitHub releases
  packages: write # Required for publishing packages

jobs:
  # Reuse main build workflow for consistency
  build:
    uses: ./.github/workflows/main.yml
    with:
      configuration: Release

  publish:
    needs: build
    runs-on: windows-latest

    steps:
      - name: Checkout
        uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Download packages
        uses: actions/download-artifact@v4
        with:
          name: packages-windows-latest
          path: ./packages

      - name: Setup .NET SDK
        uses: actions/setup-dotnet@v4
        with:
          global-json-file: ./global.json

      - name: Publish to NuGet.org
        shell: pwsh
        env:
          NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}
        run: |
          $packages = Get-ChildItem ./packages -Recurse -Include *.nupkg, *.snupkg
          foreach ($package in $packages) {
            Write-Host "Publishing: $($package.Name)"
            dotnet nuget push $package.FullName `
              --source https://api.nuget.org/v3/index.json `
              --api-key $env:NUGET_API_KEY `
              --skip-duplicate
          }

      - name: Create GitHub Release
        uses: softprops/action-gh-release@v2
        with:
          files: |
            ./packages/**/*.nupkg
            ./packages/**/*.snupkg
          generate_release_notes: true
          draft: false
          prerelease: ${{ contains(github.ref_name, '-') }}
```

### Validation & Testing

**Before Merging PR**:

1. **Local Build Test**:

   ```powershell
   dotnet build Qwiq.sln -c Release /t:Build,Pack
   # Verify packages created in src/*/bin/Release/
   ```

2. **Workflow Syntax Validation**:

   ```powershell
   # Install actionlint
   gh extension install actionlint
   actionlint .github/workflows/release.yml
   ```

3. **Test with Pre-release Tag**:

   ```powershell
   # Create test tag
   git tag v1.0.0-alpha.1
   git push origin v1.0.0-alpha.1
   # Verify workflow runs in GitHub Actions UI
   ```

4. **Verify NuGet Push**:
   - Check package appears on NuGet.org (may take 5-10 minutes)
   - Verify `--skip-duplicate` prevents errors on re-push

### Acceptance Criteria

- [ ] `release.yml` workflow created and validated
- [ ] Composite action `setup-dotnet-build` created
- [ ] `main.yml` supports `workflow_call` trigger
- [ ] NuGet API key stored as GitHub secret (`NUGET_API_KEY`)
- [ ] Test tag push triggers release workflow
- [ ] Packages published to NuGet.org successfully
- [ ] GitHub Release created with packages attached
- [ ] Release notes auto-generated from commits
- [ ] Pre-release versions marked correctly
- [ ] `--skip-duplicate` prevents re-publish errors
- [ ] Documentation updated in README.md

### Common Issues & Solutions

| Issue                         | Solution                                       |
| ----------------------------- | ---------------------------------------------- |
| `401 Unauthorized` from NuGet | Verify `NUGET_API_KEY` secret is set correctly |
| Packages not found            | Ensure `build` job uploads packages artifact   |
| Source Link validation fails  | Only run on pushed tags, not manual triggers   |
| `409 Conflict` on duplicate   | Expected behavior with `--skip-duplicate`      |

### References

- [moq.analyzers release.yml](https://github.com/rjmurillo/moq.analyzers/blob/main/.github/workflows/release.yml)
- [GitHub Actions: Reusing workflows](https://docs.github.com/en/actions/using-workflows/reusing-workflows)
- [Creating composite actions](https://docs.github.com/en/actions/creating-actions/creating-a-composite-action)

---

## W2.13 SBOM Generation (HIGH)

### Overview

**Task ID**: W2.13
**Title**: Generate Software Bill of Materials (SBOM)
**Status**: 📋 Planned
**Effort**: M (1 day)
**Priority**: **HIGH** - Security compliance requirement
**Dependencies**: W2.11 (Release Automation)

### Goal

Generate SPDX-compliant Software Bill of Materials (SBOM) in BOTH build and release pipelines using Microsoft SBOM Tool. Build pipeline creates validation SBOM, release pipeline creates authoritative SBOM attached to GitHub releases.

### Key Requirements

1. **Two SBOM Workflows**:
   - **Build Pipeline**: Validation SBOM for CI/CD verification
   - **Release Pipeline**: Authoritative SBOM attached to GitHub Release
2. **Microsoft SBOM Tool**: Use official Microsoft tooling for .NET projects
3. **SPDX 2.2 Format**: Industry-standard SBOM format
4. **Complete Dependency Graph**: Include all direct and transitive dependencies
5. **Package Metadata**: Include version, license, and vulnerability data

### Files to Create/Modify

| File                                       | Action | Description                                |
| ------------------------------------------ | ------ | ------------------------------------------ |
| `.github/workflows/main.yml`               | Modify | Add SBOM generation step to build          |
| `.github/workflows/release.yml`            | Modify | Add SBOM generation and upload to releases |
| `.github/actions/generate-sbom/action.yml` | Create | Composite action for SBOM generation       |
| `SECURITY.md`                              | Update | Document SBOM availability                 |

### Implementation Details

#### 1. Composite Action: `.github/actions/generate-sbom/action.yml`

```yaml
name: "Generate SBOM"
description: "Generate Software Bill of Materials using Microsoft SBOM Tool"

inputs:
  build-drop-path:
    description: "Path to build output directory"
    required: true

  output-path:
    description: "Path to output SBOM files"
    required: true

  package-name:
    description: "Name of the package"
    required: true
    default: "Qwiq"

  package-version:
    description: "Version of the package"
    required: false
    default: "0.0.0-dev"

runs:
  using: composite
  steps:
    - name: Install Microsoft SBOM Tool
      shell: pwsh
      run: dotnet tool install --global Microsoft.Sbom.DotNetTool

    - name: Generate SBOM
      shell: pwsh
      run: |
        sbom-tool generate `
          -BuildDropPath ${{ inputs.build-drop-path }} `
          -OutputPath ${{ inputs.output-path }} `
          -PackageName ${{ inputs.package-name }} `
          -PackageVersion ${{ inputs.package-version }} `
          -NamespaceUriBase https://github.com/rjmurillo/Qwiq `
          -Verbosity Information

    - name: Validate SBOM
      shell: pwsh
      run: |
        if (-not (Test-Path "${{ inputs.output-path }}/_manifest/spdx_2.2/*.spdx.json")) {
          Write-Error "SBOM generation failed - no SPDX file found"
          exit 1
        }
        Write-Host "✅ SBOM generated successfully"
```

#### 2. Build Pipeline: `.github/workflows/main.yml` Update

```yaml
name: Main build

# ... existing triggers and jobs ...

jobs:
  build:
    # ... existing build steps ...

    - name: Generate SBOM (Validation)
      uses: ./.github/actions/generate-sbom
      with:
        build-drop-path: ./src
        output-path: ./artifacts/sbom
        package-name: Qwiq
        package-version: ${{ github.sha }}

    - name: Upload SBOM artifact
      uses: actions/upload-artifact@v4
      if: success()
      with:
        name: sbom-validation-${{ matrix.os }}
        path: ./artifacts/sbom/_manifest/spdx_2.2/
        if-no-files-found: error
```

#### 3. Release Pipeline: `.github/workflows/release.yml` Update

```yaml
name: Release

# ... existing triggers and jobs ...

jobs:
  publish:
    needs: build
    runs-on: windows-latest

    steps:
      # ... existing checkout and download steps ...

      - name: Get version from tag
        id: get-version
        shell: pwsh
        run: |
          $version = "${{ github.ref_name }}" -replace '^v', ''
          echo "version=$version" >> $env:GITHUB_OUTPUT

      - name: Generate Authoritative SBOM
        uses: ./.github/actions/generate-sbom
        with:
          build-drop-path: ./packages
          output-path: ./artifacts/sbom
          package-name: Qwiq
          package-version: ${{ steps.get-version.outputs.version }}

      # ... existing NuGet publish steps ...

      - name: Create GitHub Release
        uses: softprops/action-gh-release@v2
        with:
          files: |
            ./packages/**/*.nupkg
            ./packages/**/*.snupkg
            ./artifacts/sbom/_manifest/spdx_2.2/*.spdx.json
          generate_release_notes: true
          draft: false
          prerelease: ${{ contains(github.ref_name, '-') }}
```

### SBOM Content Verification

**Expected SBOM Contents**:

```json
{
  "spdxVersion": "SPDX-2.2",
  "dataLicense": "CC0-1.0",
  "SPDXID": "SPDXRef-DOCUMENT",
  "name": "Qwiq",
  "documentNamespace": "https://github.com/rjmurillo/Qwiq/...",
  "packages": [
    {
      "name": "Qwiq.Core",
      "SPDXID": "SPDXRef-Package-...",
      "versionInfo": "x.y.z",
      "supplier": "Organization: rjmurillo",
      "licenseConcluded": "MIT",
      "externalRefs": [...]
    },
    // ... all NuGet dependencies ...
  ],
  "relationships": [...]
}
```

### Validation & Testing

**Before Merging PR**:

1. **Local SBOM Generation**:

   ```powershell
   # Install tool
   dotnet tool install --global Microsoft.Sbom.DotNetTool

   # Generate SBOM
   sbom-tool generate `
     -BuildDropPath ./src `
     -OutputPath ./test-sbom `
     -PackageName Qwiq `
     -PackageVersion 1.0.0-test

   # Verify output
   Get-Content ./test-sbom/_manifest/spdx_2.2/*.spdx.json | ConvertFrom-Json
   ```

2. **SBOM Validation**:

   ```powershell
   # Verify all expected packages present
   $sbom = Get-Content ./test-sbom/_manifest/spdx_2.2/*.spdx.json | ConvertFrom-Json
   $sbom.packages | Where-Object { $_.name -like "Qwiq*" }
   ```

3. **GitHub Actions Test**:
   - Push to feature branch and verify SBOM artifact uploaded
   - Check artifact contents match expected structure

### Acceptance Criteria

- [ ] Microsoft SBOM Tool composite action created
- [ ] Build pipeline generates validation SBOM
- [ ] SBOM artifact uploaded in CI builds
- [ ] Release pipeline generates authoritative SBOM
- [ ] SBOM attached to GitHub releases
- [ ] SPDX 2.2 format validated
- [ ] All Qwiq packages included in SBOM
- [ ] All NuGet dependencies listed with versions
- [ ] License information included for all components
- [ ] SBOM generation errors fail the build
- [ ] Documentation updated in SECURITY.md

### Common Issues & Solutions

| Issue                | Solution                                              |
| -------------------- | ----------------------------------------------------- |
| SBOM tool not found  | Ensure `dotnet tool install` runs before generation   |
| Empty SBOM           | Verify `BuildDropPath` points to built artifacts      |
| Missing dependencies | Ensure NuGet restore completed before SBOM generation |
| Large SBOM file      | Expected for projects with many dependencies          |

### SBOM Use Cases

1. **Vulnerability Scanning**: Feed SBOM to security scanning tools (Dependabot, Snyk)
2. **License Compliance**: Automated license audit from SBOM data
3. **Supply Chain Security**: Track provenance of all dependencies
4. **Regulatory Compliance**: Meet NTIA minimum elements for SBOM

### References

- [Microsoft SBOM Tool](https://github.com/microsoft/sbom-tool)
- [SPDX Specification 2.2](https://spdx.github.io/spdx-spec/)
- [NTIA SBOM Minimum Elements](https://www.ntia.gov/report/2021/minimum-elements-software-bill-materials-sbom)

---

## W2.16 REST/SOAP Unit Test Coverage (HIGH)

### Overview

**Task ID**: W2.16
**Title**: REST and SOAP Client Unit Test Coverage
**Status**: 📋 Planned
**Effort**: L (2-3 weeks)
**Priority**: **HIGH** - Enables CI validation without Azure DevOps connectivity
**Dependencies**: Explainer PRD (created), GitHub issue for tracking

### Goal

Implement comprehensive unit test coverage for REST and SOAP client adapters, enabling CI validation without requiring Azure DevOps connectivity. Tests will use mocking to isolate adapter logic from underlying SDKs.

### Phases

This task is split into two sequential phases:

| Phase       | Target      | Effort          | Priority | Platform       |
| ----------- | ----------- | --------------- | -------- | -------------- |
| **Phase 1** | REST Client | M (1-1.5 weeks) | High     | Cross-platform |
| **Phase 2** | SOAP Client | L (1.5-2 weeks) | Medium   | Windows-only   |

### Key Requirements

1. **No External Dependencies**: All tests run offline without Azure DevOps
2. **Mock SDK Responses**: Use Moq to mock `WorkItemTrackingHttpClient` (REST) and TFS OM (SOAP)
3. **Existing Patterns**: Use `Moq`, `Shouldly`, and `ContextSpecification` patterns
4. **New Test Categories**: `RestUnit` and `SoapUnit` for CI filtering
5. **CI Integration**: Update GitHub Actions to run unit tests on all platforms
6. **Documentation**: Clear testing patterns for contributors

### PRD Reference

**Complete requirements defined in**:
`c:\src\GitHub\rjmurillo\Qwiq\.agents\explainer-rest-soap-unit-tests.md`

**GitHub Issue**: [Create tracking issue in Phase 1]

**Key PRD Sections**:

- Section 7: Functional Requirements
- Section 8: Technical Approach
- Section 9: Implementation Phases
- Section 10: Testing Strategy

---

### Phase 1: REST Client Unit Tests

#### Overview

**Scope**: Cross-platform REST client validation
**Timeline**: Week 1-2 (10-15 days)
**Platform**: Windows, Linux, macOS

#### Files to Create/Modify

| File                                                        | Action | Description                      |
| ----------------------------------------------------------- | ------ | -------------------------------- |
| `test/Qwiq.Core.Rest.Tests/`                                | Create | New test project for REST client |
| `test/Qwiq.Core.Rest.Tests/Qwiq.Core.Rest.UnitTests.csproj` | Create | Test project file                |
| `test/Qwiq.Core.Rest.Tests/Given_REST_WorkItemStore_*.cs`   | Create | Test classes for REST adapter    |
| `.github/workflows/main.yml`                                | Modify | Add `RestUnit` tests to CI       |
| `TESTING.md`                                                | Update | Document REST testing patterns   |
| `coverage.runsettings`                                      | Modify | Include REST tests in coverage   |

#### Implementation Tasks

| ID    | Task                                   | Deliverable                                       | Acceptance               |
| ----- | -------------------------------------- | ------------------------------------------------- | ------------------------ |
| R1.1  | Create REST test project structure     | `Qwiq.Core.Rest.UnitTests.csproj`                 | Project builds           |
| R1.2  | Add test infrastructure dependencies   | Moq, Shouldly, MSTest packages                    | Dependencies resolved    |
| R1.3  | Implement query execution tests        | `Given_REST_WorkItemStore_query_execution.cs`     | WIQL queries mocked      |
| R1.4  | Implement work item retrieval tests    | `Given_REST_WorkItemStore_work_item_retrieval.cs` | GetWorkItems mocked      |
| R1.5  | Implement field access pattern tests   | `Given_REST_WorkItemStore_field_access.cs`        | Field mapping validated  |
| R1.6  | Implement authentication flow tests    | `Given_REST_WorkItemStore_authentication.cs`      | All auth types tested    |
| R1.7  | Implement factory pattern tests        | `Given_WorkItemStoreFactory_REST.cs`              | Factory validation logic |
| R1.8  | Add `RestUnit` test category           | Apply `[TestCategory("RestUnit")]`                | Category defined         |
| R1.9  | Update CI filter to include `RestUnit` | Modify `.github/workflows/main.yml`               | Tests run in CI          |
| R1.10 | Add cross-platform CI matrix           | Test on Windows, Linux, macOS                     | All platforms pass       |
| R1.11 | Document REST testing patterns         | Add section to `TESTING.md`                       | Contributor guide exists |

#### Example Test Implementation

**Test Class**: `Given_REST_WorkItemStore_query_execution.cs`

```csharp
namespace Qwiq.Client.Rest.UnitTests
{
    [TestClass]
    [TestCategory("RestUnit")]
    public class Given_REST_query_execution : ContextSpecification
    {
        private Mock<WorkItemTrackingHttpClient> _mockClient;
        private IWorkItemStore _store;
        private IEnumerable<IWorkItem> _results;

        public override void Given()
        {
            // Arrange: Mock SDK responses
            _mockClient = new Mock<WorkItemTrackingHttpClient>(
                new Uri("https://dev.azure.com/test"),
                Mock.Of<VssCredentials>());

            _mockClient
                .Setup(x => x.QueryByWiqlAsync(
                    It.IsAny<Wiql>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new WorkItemQueryResult
                {
                    WorkItems = new[]
                    {
                        new WorkItemReference { Id = 1 },
                        new WorkItemReference { Id = 2 }
                    }
                });

            _store = new WorkItemStore(_mockClient.Object);
        }

        public override void When()
        {
            // Act: Execute query
            _results = _store.Query("SELECT [System.Id] FROM WorkItems");
        }

        [TestMethod]
        public void Then_should_return_work_items()
        {
            _results.ShouldNotBeEmpty();
            _results.Count().ShouldBe(2);
        }

        [TestMethod]
        public void Then_should_call_SDK_query_method()
        {
            _mockClient.Verify(
                x => x.QueryByWiqlAsync(
                    It.IsAny<Wiql>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
```

#### Critical Test Scenarios

1. **Query Execution**:

   - Basic WIQL query
   - Query with field selection
   - Query with AsOf date
   - Query with pagination (>200 results)
   - Empty result set

2. **Work Item Retrieval**:

   - GetWorkItem by ID
   - GetWorkItems batch (multiple IDs)
   - Batch size limits (200 max per request)
   - Field filtering
   - Work item not found (404)

3. **Field Access Patterns**:

   - `IWorkItem.GetField<T>(fieldName)`
   - `IWorkItem[fieldName]` indexer
   - Field type conversions
   - Missing field handling

4. **Authentication Flows**:

   - PersonalAccessToken authentication
   - OAuth token authentication
   - Windows authentication (if applicable)
   - Credential validation errors

5. **Factory Pattern**:
   - `WorkItemStoreFactory.Create(options)`
   - Invalid connection URI
   - Missing credentials
   - Authentication type mismatch

#### CI Integration: `.github/workflows/main.yml` Update

```yaml
env:
  # Update test filter to include RestUnit
  TEST_FILTER: "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=IntegrationTests"
  # RestUnit tests are NOT excluded - they run in CI

jobs:
  build:
    strategy:
      matrix:
        os: [windows-latest, ubuntu-latest, macos-latest]  # ADD macOS
      fail-fast: false

    # ... existing build steps ...

    - name: Run tests with coverage
      shell: pwsh
      run: dotnet test Qwiq.sln -c Release --no-build --filter "${{ env.TEST_FILTER }}" --logger trx --collect:"XPlat Code Coverage"
```

#### Phase 1 Acceptance Criteria

- [ ] REST test project created and building
- [ ] All critical test scenarios implemented
- [ ] `RestUnit` test category applied to all tests
- [ ] Tests run successfully on Windows, Linux, macOS
- [ ] CI filter updated to include `RestUnit` tests
- [ ] Cross-platform CI matrix includes macOS
- [ ] Test coverage >80% for REST adapter code
- [ ] Zero external dependencies (no Azure DevOps connectivity)
- [ ] Documentation added to `TESTING.md`
- [ ] All tests pass in CI

---

### Phase 2: SOAP Client Unit Tests

#### Overview

**Scope**: Windows-only SOAP client validation
**Timeline**: Week 3-4 (10-15 days)
**Platform**: Windows only (net472 requirement)

**Note**: Phase 2 is DEFERRED until Phase 1 is stable and complete.

#### Files to Create/Modify

| File                                                        | Action | Description                         |
| ----------------------------------------------------------- | ------ | ----------------------------------- |
| `test/Qwiq.Core.Soap.Tests/`                                | Create | New test project for SOAP client    |
| `test/Qwiq.Core.Soap.Tests/Qwiq.Core.Soap.UnitTests.csproj` | Create | Test project file (net472 only)     |
| `test/Qwiq.Core.Soap.Tests/Given_SOAP_WorkItemStore_*.cs`   | Create | Test classes for SOAP adapter       |
| `.github/workflows/main.yml`                                | Modify | Add `SoapUnit` tests (Windows only) |
| `TESTING.md`                                                | Update | Document SOAP testing patterns      |

#### Implementation Tasks

| ID   | Task                                | Deliverable                                       | Acceptance                |
| ---- | ----------------------------------- | ------------------------------------------------- | ------------------------- |
| S2.1 | Create SOAP test project structure  | `Qwiq.Core.Soap.UnitTests.csproj`                 | Project builds on Windows |
| S2.2 | Add SOAP test dependencies          | TFS Client OM mocks                               | Dependencies resolved     |
| S2.3 | Implement query execution tests     | `Given_SOAP_WorkItemStore_query_execution.cs`     | TFS OM queries mocked     |
| S2.4 | Implement work item retrieval tests | `Given_SOAP_WorkItemStore_work_item_retrieval.cs` | WorkItemCollection mocked |
| S2.5 | Implement field access tests        | `Given_SOAP_WorkItemStore_field_access.cs`        | Field access validated    |
| S2.6 | Add `SoapUnit` test category        | Apply `[TestCategory("SoapUnit")]`                | Category defined          |
| S2.7 | Update CI filter for SOAP tests     | Modify `.github/workflows/main.yml`               | Tests run on Windows only |
| S2.8 | Document SOAP testing patterns      | Add section to `TESTING.md`                       | Contributor guide exists  |

#### SOAP-Specific Challenges

1. **TFS Client OM Complexity**:

   - `WorkItemStore` is sealed class (requires wrapper interface)
   - `WorkItemCollection` is concrete class
   - Field access via `WorkItem.Fields[name]`

2. **Mocking Strategy**:

   - Create `IWorkItemStore` interface wrapping TFS OM
   - Mock interface instead of sealed classes
   - Use test doubles for `WorkItemCollection`

3. **Windows-Only CI**:
   - SOAP tests run ONLY on `windows-latest` runner
   - Skip SOAP tests on Linux/macOS
   - Filter: `TestCategory!=SoapUnit` on non-Windows

#### Phase 2 Acceptance Criteria

- [ ] SOAP test project created (net472 target)
- [ ] All critical test scenarios implemented
- [ ] `SoapUnit` test category applied
- [ ] Tests run successfully on Windows
- [ ] CI runs SOAP tests only on Windows
- [ ] Test coverage >70% for SOAP adapter code
- [ ] Zero external dependencies (no TFS connectivity)
- [ ] Documentation added to `TESTING.md`
- [ ] All tests pass on Windows

---

### Documentation Requirements

#### `TESTING.md` Update

Add new sections:

````markdown
## Unit Testing Without Azure DevOps

### REST Client Unit Tests

REST client tests use Moq to mock `WorkItemTrackingHttpClient` responses.
Tests run on all platforms (Windows, Linux, macOS) without Azure DevOps.

**Test Category**: `RestUnit`

**Running REST Tests**:

```powershell
dotnet test --filter "TestCategory=RestUnit"
```
```text
````

**Example Test**:
[Include example from R1.3 above]

### SOAP Client Unit Tests

SOAP client tests use Moq to mock TFS Client OM responses.
Tests run on Windows only (net472 requirement).

**Test Category**: `SoapUnit`

**Running SOAP Tests**:

```
```powershell
dotnet test --filter "TestCategory=SoapUnit"
```

### Writing Adapter Tests

When testing thin adapters:

1. Focus on adapter behavior, not SDK verification
2. Use synthetic test data, not recorded responses
3. Mock at SDK boundaries (`WorkItemTrackingHttpClient`, TFS OM)
4. Test error handling and edge cases
5. Validate output state, not mock call counts

````text

### Validation & Testing

**Before Merging Each Phase**:

1. **Local Test Execution**:
   ```powershell
   # Phase 1: REST tests
   dotnet test --filter "TestCategory=RestUnit"

   # Phase 2: SOAP tests (Windows only)
   dotnet test --filter "TestCategory=SoapUnit"
````

1. **Code Coverage Validation**:

   ```
   ```powershell
   dotnet test --collect:"XPlat Code Coverage" --filter "TestCategory=RestUnit"
   # Verify coverage >80% for REST adapter code
   ```

2. **CI Validation**:

   - Push feature branch and verify all CI platforms pass
   - Check test execution logs for cross-platform success

3. **Integration Test Compatibility**:
   - Ensure unit tests don't conflict with existing integration tests
   - Verify `TestCategory` filtering works correctly

### Overall Acceptance Criteria

- [ ] Phase 1 (REST) complete and merged
- [ ] Phase 2 (SOAP) complete and merged
- [ ] All tests pass in CI without Azure DevOps connectivity
- [ ] Test coverage increased by 15-20%
- [ ] Contributors can run all tests locally
- [ ] Documentation complete in `TESTING.md`
- [ ] GitHub issue tracking created and closed
- [ ] PRD requirements fully implemented

### References

- **PRD**: `explainer-rest-soap-unit-tests.md`
- **Existing Patterns**: `test/Qwiq.Core.Tests/` for ContextSpecification examples
- **Integration Tests**: `test/Qwiq.Integration.Tests/` for comparison

---

## Task Dependencies

### Dependency Graph

```text
W2.11 (Release Automation)
  └─> W2.13 (SBOM Generation)

W2.16 (REST/SOAP Unit Tests)
  ├─> Phase 1: REST Tests (independent)
  └─> Phase 2: SOAP Tests (depends on Phase 1 complete)
```

### Critical Path

1. **W2.11** (Release Automation) - **CRITICAL** - Blocks manual releases
2. **W2.13** (SBOM Generation) - **HIGH** - Security compliance
3. **W2.16 Phase 1** (REST Tests) - **HIGH** - CI validation

### Prerequisites

| Task     | Prerequisites      | Status      |
| -------- | ------------------ | ----------- |
| W2.11    | W1.2 (Source Link) | ✅ Complete |
| W2.13    | W2.11              | 📋 Planned  |
| W2.16-P1 | PRD created        | ✅ Complete |
| W2.16-P2 | Phase 1 complete   | ⏳ Waiting  |

---

## Implementation Sequence

### Recommended Order

1. **Week 1**: W2.11 (Release Automation)

   - High impact, unblocks manual release process
   - Prerequisite for W2.13

2. **Week 2**: W2.13 (SBOM Generation)

   - Builds on W2.11
   - Security compliance requirement

3. **Week 3-4**: W2.16 Phase 1 (REST Unit Tests)

   - High value for CI validation
   - Cross-platform testing

4. **Week 5-6**: W2.16 Phase 2 (SOAP Unit Tests)
   - Lower priority than Phase 1
   - Windows-only testing

### Parallel Work Opportunities

- W2.11 and W2.16 Phase 1 can be worked in parallel (no dependencies)
- W2.13 can start while W2.16 Phase 1 is in progress

---

## Version History

| Version | Date        | Author       | Changes                                          |
| ------- | ----------- | ------------ | ------------------------------------------------ |
| 1.0     | Dec 5, 2025 | AI Assistant | Initial task definitions for W2.11, W2.13, W2.16 |

---

## Next Steps

1. **Create GitHub Issues**: One issue per task (W2.11, W2.13, W2.16)
2. **Link to Explainer**: Reference `explainer-rest-soap-unit-tests.md` for W2.16
3. **Assign Priorities**: Confirm Critical/High priorities with maintainer
4. **Begin Implementation**: Start with W2.11 (Release Automation)
