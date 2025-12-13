# ADR-005: Central Package Management

- **Status**: Accepted
- **Date**: 2025-12-06
- **Decision Makers**: Qwiq Development Team
- **Supersedes**: None
- **Superseded by**: None

## Context

Qwiq is a multi-project solution with:

- 9 source projects (Qwiq.Core, Qwiq.Client.Rest, Qwiq.Client.Soap, Qwiq.Linq, Qwiq.Mapper, Qwiq.Identity, etc.)
- 7 test projects (unit tests, integration tests, mocks)
- Multiple target frameworks (net472, netstandard2.0, net8.0)
- Shared dependencies across projects (e.g., Newtonsoft.Json, Shouldly, MSTest)

### Problem Statement

How can we manage NuGet package versions across the solution to:

1. Ensure all projects use consistent package versions
2. Simplify dependency upgrades
3. Avoid version conflicts and assembly binding redirects
4. Make it obvious which packages are used where
5. Reduce merge conflicts in project files

### Forces

- **Consistency**: Different projects must not use different versions of the same package
- **Maintainability**: Updating packages should be easy
- **Discoverability**: Developers should easily see all package versions
- **Build Performance**: Restore time matters
- **Upgrade Safety**: Need to test cross-project impacts of upgrades

## Decision

We will use **.NET Central Package Management (CPM)** via `Directory.Packages.props` to define all package versions in one location.

### Implementation

#### Directory Structure

```text
Qwiq/
├── Directory.Build.props          # Shared MSBuild properties
├── Directory.Build.targets        # Shared MSBuild targets
├── Directory.Packages.props       # ⭐ Central package versions
├── src/
│   └── Qwiq.Core/
│       └── Qwiq.Core.csproj       # No version attributes
└── test/
    └── Qwiq.Core.Tests/
        └── Qwiq.Core.Tests.csproj # No version attributes
```

#### Directory.Packages.props

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <CentralPackageTransitivePinningEnabled>true</CentralPackageTransitivePinningEnabled>
  </PropertyGroup>

  <ItemGroup Label="Production Dependencies">
    <!-- Azure DevOps SDK -->
    <PackageVersion Include="Microsoft.TeamFoundationServer.ExtendedClient" Version="19.240.1" />
    <PackageVersion Include="Microsoft.VisualStudio.Services.Client" Version="19.240.1" />

    <!-- Core Dependencies -->
    <PackageVersion Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageVersion Include="System.Net.Http" Version="4.3.4" />

    <!-- Polyfills -->
    <PackageVersion Include="Microsoft.Bcl.AsyncInterfaces" Version="8.0.0" />
  </ItemGroup>

  <ItemGroup Label="Test Dependencies">
    <PackageVersion Include="MSTest.TestAdapter" Version="3.1.1" />
    <PackageVersion Include="MSTest.TestFramework" Version="3.1.1" />
    <PackageVersion Include="Shouldly" Version="4.2.1" />
    <PackageVersion Include="Moq" Version="4.16.0" />
    <PackageVersion Include="coverlet.collector" Version="6.0.0" />
  </ItemGroup>

  <ItemGroup Label="Build Tools">
    <PackageVersion Include="Nerdbank.GitVersioning" Version="3.6.143" />
    <PackageVersion Include="Microsoft.CodeAnalysis.PublicApiAnalyzers" Version="3.3.4" />
    <PackageVersion Include="Microsoft.SourceLink.GitHub" Version="8.0.0" />
  </ItemGroup>
</Project>
```

#### Project Files (Version-less References)

```xml
<!-- src/Qwiq.Core/Qwiq.Core.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net472;netstandard2.0;net8.0</TargetFrameworks>
  </PropertyGroup>

  <ItemGroup>
    <!-- ✅ No Version attribute - managed centrally -->
    <PackageReference Include="Newtonsoft.Json" />
    <PackageReference Include="Microsoft.SourceLink.GitHub" PrivateAssets="All" />
  </ItemGroup>
</Project>
```

### Usage Pattern

```powershell
# Add new package version to Directory.Packages.props
# <PackageVersion Include="Polly" Version="8.0.0" />

# Reference package in project (no version)
# <PackageReference Include="Polly" />

# Update package version (single location)
# Directory.Packages.props: Version="8.0.0" → Version="8.1.0"
```

### Upgrade Workflow

```powershell
# 1. Update version in Directory.Packages.props
# <PackageVersion Include="Shouldly" Version="4.2.1" />

# 2. Build entire solution to test impact
dotnet build Qwiq.sln -c Release

# 3. Run all tests to verify compatibility
dotnet test Qwiq.sln -c Release --no-build

# 4. Commit single file change
git add Directory.Packages.props
git commit -m "deps: upgrade Shouldly to 4.2.1"
```

## Consequences

### Positive

1. **Single Source of Truth**: All package versions in one file
2. **Easy Upgrades**: Update version once, affects all projects
3. **Consistency**: Impossible to have version conflicts
4. **Discoverability**: One file shows all dependencies
5. **Merge Friendly**: Fewer project file conflicts
6. **Audit Trail**: Git history clearly shows version changes
7. **Security**: Easier to audit and update vulnerable packages
8. **Transitive Pinning**: Control indirect dependency versions

### Negative

1. **Initial Learning Curve**: Developers must learn CPM pattern
2. **Global Impact**: Version change affects all projects (could break some)
3. **Per-Project Overrides**: More complex when needed
4. **Tooling Support**: Some older tools don't understand CPM

### Trade-offs

- **Flexibility vs Consistency**: Lose per-project version flexibility, gain consistency
- **Blast Radius**: Changes impact all projects, but that's also a feature (find issues early)
- **Discoverability**: All versions in one place (good) but not in project files (could be confusing)

### Risks

- **Unintended Upgrades**: Updating one project's need upgrades all
  - _Mitigation_: Thorough testing, run full test suite before committing
- **Breaking Changes**: Package upgrade breaks multiple projects at once
  - _Mitigation_: Test before merging, use semantic versioning awareness
- **Downgrade Difficulty**: Can't easily have different versions for different projects
  - _Mitigation_: Use TFM-specific package versions if absolutely necessary

## Alternatives Considered

### Alternative 1: Traditional Per-Project Versioning

```xml
<!-- ❌ Rejected -->
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  </ItemGroup>
</Project>
```

**Rejected because:**

- Version drift across projects
- Difficult to upgrade consistently
- Merge conflicts in project files
- No single source of truth

### Alternative 2: Paket Package Manager

```text
source https://api.nuget.org/v3/index.json
nuget Newtonsoft.Json 13.0.3
```

**Rejected because:**

- Additional tool to learn and maintain
- Non-standard in .NET ecosystem
- CPM provides same benefits natively

### Alternative 3: NuGet.Config with Version Ranges

```xml
<!-- ❌ Rejected -->
<packageRestore>
  <allowedVersions>
    <package id="Newtonsoft.Json" allowedVersions="[13.0,14.0)" />
  </allowedVersions>
</packageRestore>
```

**Rejected because:**

- Non-deterministic builds (versions can change)
- Doesn't solve consistency problem
- Harder to audit

### Alternative 4: Git Submodules for Dependencies

**Rejected because:**

- Massive complexity
- Breaks NuGet ecosystem
- Build time explosion

## Implementation Guidelines

### DO

- ✅ Define all package versions in `Directory.Packages.props`
- ✅ Use `<PackageReference Include="..." />` without Version in projects
- ✅ Group packages with `<ItemGroup Label="...">` for organization
- ✅ Enable transitive pinning for supply chain security
- ✅ Test full solution after version upgrades
- ✅ Use conventional commits for dependency updates: `deps: upgrade Package to X.Y.Z`

### DON'T

- ❌ Add `Version` attributes in project files (will cause build error)
- ❌ Override versions in projects (defeats purpose, use sparingly)
- ❌ Update versions without testing
- ❌ Mix CPM and traditional versioning

### Exception: Per-TFM Versions

If absolutely necessary, use conditions:

```xml
<ItemGroup>
  <PackageVersion Include="System.Text.Json" Version="8.0.0" Condition="'$(TargetFramework)' != 'net472'" />
  <PackageVersion Include="Newtonsoft.Json" Version="13.0.3" Condition="'$(TargetFramework)' == 'net472'" />
</ItemGroup>
```

## Migration Path (Historical)

Qwiq migrated from traditional to CPM during Wave 0 modernization:

1. **Phase 1**: Created `Directory.Packages.props` with current versions
2. **Phase 2**: Enabled `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`
3. **Phase 3**: Removed `Version` attributes from all project files
4. **Phase 4**: Verified build and tests pass

## Related Decisions

- [ADR-004: Multi-Targeting Approach](ADR-004-multi-targeting-approach.md) - TFM-specific package versions
- W2.14: Dependency Review Action - Security scanning relies on CPM

## References

- [Central Package Management (CPM) Documentation](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management)
- [NuGet Package Versioning](https://learn.microsoft.com/en-us/nuget/concepts/package-versioning)
- [Semantic Versioning](https://semver.org/)
- [Directory.Build.props Documentation](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory)

## Revision History

| Date       | Author        | Changes                                                     |
| ---------- | ------------- | ----------------------------------------------------------- |
| 2025-12-06 | Copilot Agent | Initial ADR documenting Central Package Management strategy |
