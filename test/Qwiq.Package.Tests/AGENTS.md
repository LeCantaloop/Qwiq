# Qwiq.Package.Tests Component Guide

## Component Overview

**Qwiq.Package.Tests** validates NuGet package contents and metadata using Verify snapshot testing. These tests ensure packages contain expected files and manifests.

## Purpose

- Validate NuGet package structure
- Verify package manifest (.nuspec) contents
- Ensure correct files are packaged
- Detect unintended package changes
- Use snapshot testing for package validation

## Key Characteristics

- **Target Frameworks**: Test project TFMs
- **Test Framework**: MSTest + Verify
- **Pattern**: Snapshot testing
- **Focus**: NuGet package validation

## ⚠️ Critical: Requires `dotnet pack` First

These tests validate **existing** NuGet packages. They FAIL if packages don't exist.

**Required workflow**:

1. Build packages: `dotnet pack -c Release`
2. Run tests: `dotnet test test/Qwiq.Package.Tests`

## Test Structure

### Package Manifest Tests

Validate .nuspec contents (metadata, dependencies, etc.):

```csharp
[TestMethod]
public Task Should_have_correct_manifest_for_Qwiq_Core()
{
    var package = GetPackage("Qwiq.Core");
    var manifest = ReadManifest(package);

    return Verify(manifest)
        .UseFileName("Qwiq.Core#manifest");
}
```

### Package Contents Tests

Validate files included in package:

```csharp
[TestMethod]
public Task Should_include_expected_files_in_Qwiq_Core()
{
    var package = GetPackage("Qwiq.Core");
    var files = GetPackageFiles(package);

    return Verify(files)
        .UseFileName("Qwiq.Core#contents");
}
```

## Verified Baselines

Tests use `.verified.*` files as baselines:

- `Qwiq.Core#manifest.verified.nuspec` - Expected manifest
- `Qwiq.Core#contents.verified.txt` - Expected file list

When tests run, they generate `.received.*` files and compare to `.verified.*` files.

## Updating Baselines

When package contents change (e.g., adding README.md), baselines must be updated:

### Using Verify.Terminal (Recommended)

```powershell
# Install tool
dotnet tool install verify.tool

# Or restore from manifest
dotnet tool restore

# Review and accept changes interactively
dotnet verify review -w test/Qwiq.Package.Tests

# Or accept all changes
dotnet verify accept -w test/Qwiq.Package.Tests
```

### Manual PowerShell

```powershell
Get-ChildItem -Path "test\Qwiq.Package.Tests" -Filter "*.received.*" | ForEach-Object {
    $verifiedName = $_.Name -replace '\.received\.', '.verified.'
    Copy-Item -Path $_.FullName -Destination (Join-Path $_.DirectoryName $verifiedName) -Force
}
```

## Common Scenarios Requiring Baseline Updates

### Adding PackageReadmeFile

When adding README.md to packages:

1. Update .csproj with `<PackageReadmeFile>`
2. Build packages: `dotnet pack -c Release`
3. Run tests: `dotnet test test/Qwiq.Package.Tests` (will fail)
4. Review `.received.*` files - verify README.md is included
5. Update baselines: `dotnet verify accept -w test/Qwiq.Package.Tests`
6. Commit updated `.verified.*` files

### Changing Package Metadata

Metadata changes (icon, license, tags) require baseline updates:

1. Modify metadata in Directory.Build.props or .csproj
2. Pack and test (will fail)
3. Review differences
4. Accept baselines
5. Commit

## Common Mistakes to Avoid

❌ **Don't run without packing first** - Tests need existing packages

```powershell
# ❌ WRONG: Test without packages
dotnet test test/Qwiq.Package.Tests

# ✅ CORRECT: Pack then test
dotnet pack -c Release
dotnet test test/Qwiq.Package.Tests -c Release
```

❌ **Don't ignore baseline mismatches** - Review carefully

❌ **Don't manually edit .verified files** - Use verify.tool or copy from .received

✅ **Do review .received files** - Ensure changes are intentional

✅ **Do update baselines after package changes** - Keep tests in sync

✅ **Do commit .verified files** - Part of test suite

## Test Files Location

Verified baselines are in `test/Qwiq.Package.Tests/`:

```text
test/Qwiq.Package.Tests/
├── Qwiq.Core#manifest.verified.nuspec
├── Qwiq.Core#contents.verified.txt
├── Qwiq.Linq#manifest.verified.nuspec
├── Qwiq.Linq#contents.verified.txt
└── ... (one set per packaged project)
```

## Related Components

- All source projects that produce NuGet packages
- **Directory.Build.props** - Package metadata
- **Directory.Packages.props** - Package versions

## CI Integration

In CI, these tests validate that package contents match expectations:

- Fail if unexpected files are included
- Fail if metadata changes unexpectedly
- Ensure consistent package output across builds
