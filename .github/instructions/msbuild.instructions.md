---
applyTo: "**/*.{props,targets}"
---

# MSBuild Property/Target File Instructions

> **MANDATORY**: You MUST follow these instructions when editing any .props or .targets file in this repository.

## Quick Reference

- `Directory.Build.props` - Repository-wide MSBuild properties
- `Directory.Build.targets` - Repository-wide build targets
- `Directory.Packages.props` - Central Package Management versions
- Changes affect ALL projects in the repository

## Critical Warning

> ⚠️ **These files affect every project in the repository.**
>
> Do NOT modify `Directory.Build.props`, `Directory.Build.targets`, or `Directory.Packages.props`
> as part of feature/bugfix PRs unless explicitly tasked with build system changes.

## Context Loading

When working on MSBuild files, you MUST:

1. Read this entire instruction file before making changes
2. Understand the impact on all projects
3. Build with single-threaded mode to avoid file locking
4. Complete the Validation Checklist before submitting

## File Purposes

### Directory.Build.props

Contains shared properties applied to all projects:

- Target framework defaults
- Nullable reference types configuration
- Warning/error settings
- Package metadata (Authors, License, etc.)
- Assembly info generation settings

### Directory.Build.targets

Contains shared build targets:

- Post-build actions
- Custom build steps

### Directory.Packages.props

Central Package Management - ALL package versions defined here:

```xml
<ItemGroup>
  <PackageVersion Include="PackageName" Version="1.2.3" />
</ItemGroup>
```

## Analyzer Configuration

Analyzer diagnostic severities are configured in `.editorconfig`, NOT in MSBuild files.

See [editorconfig.instructions.md](editorconfig.instructions.md) for analyzer rules.

## Build Commands

Always use single-threaded build to avoid Windows file locking:

```powershell
# Single-threaded build (recommended)
dotnet build Qwiq.sln /m:1 /nodeReuse:false -c Release

# Standard build (may fail with file locking on Windows)
dotnet build Qwiq.sln -c Release
```

## Package Test Validation

**CRITICAL**: When modifying Directory.Build.props, Directory.Build.targets, or Directory.Packages.props, package metadata changes and baselines MUST be validated.

### Required Steps

**ALWAYS run package tests before committing:**

```powershell
# Build packages
dotnet build Qwiq.sln -c Release

# Run package tests
dotnet test test/Qwiq.Package.Tests/Qwiq.Package.Tests.csproj --no-build -c Release
```

### If Package Tests Fail

Review the diff between `.received` and `.verified` files:

```powershell
# View differences
ls test/Qwiq.Package.Tests/*.received.*

# If changes are expected (e.g., metadata updates), rebaseline:
dotnet verify accept -w test/Qwiq.Package.Tests

# Or manually (in non-interactive environments):
cd test/Qwiq.Package.Tests
for file in *.received.*; do cp "$file" "${file/received/verified}"; done
```

### Common Triggers for Rebaselining

- Adding/removing `PackageReference` in Directory.Packages.props
- Changing build configuration (`DebugType`, `IncludeSymbols`, `SymbolPackageFormat`, etc.)
- Updating DotNet.ReproducibleBuilds or SourceLink packages
- Building from a feature branch (branch name appears in repository metadata)
- Changing package metadata properties (Authors, Description, etc.)

**Example**: Switching from `DebugType=portable` to `DebugType=embedded` changes package contents and manifests.

## Validation Checklist

Before submitting changes, verify:

- [ ] `dotnet build Qwiq.sln /m:1 /nodeReuse:false -c Release` succeeds
- [ ] 0 errors, minimal warnings
- [ ] All test projects still build
- [ ] `dotnet test` with filters passes
- [ ] **Package tests pass** (required for Directory.Build.props/targets changes)
- [ ] Package baselines updated if metadata changed
- [ ] Changes documented in PR description
- [ ] Impact on all projects understood

## Validation Evidence Requirements

Include in your PR description:

```markdown
## MSBuild Validation Log

Build output showing successful compilation:

- [x] All projects build
- [x] No new warnings
- [x] Tests pass

## CI Evidence

Link to CI run: [#123](link)
```

## Common Mistakes to AVOID

```xml
<!-- ❌ WRONG: Adding NoWarn in props (use .editorconfig instead) -->
<NoWarn>$(NoWarn);CA1234</NoWarn>

<!-- ❌ WRONG: Package version in csproj -->
<PackageReference Include="Package" Version="1.0.0" />

<!-- ✅ CORRECT: Version in Directory.Packages.props -->
<PackageVersion Include="Package" Version="1.0.0" />
```

## Decision Trees

### When Adding a New Package Version

1. Check if package already exists in `Directory.Packages.props`
2. Add `<PackageVersion Include="..." Version="..." />` to props file
3. Reference without version in consuming .csproj
4. Build all affected projects

### When Modifying Build Properties

1. Understand current behavior
2. Test change on single project first if possible
3. Build entire solution with `/m:1`
4. Run full test suite
5. Document impact in PR

### When to Stop and Ask

- Changing `<TreatWarningsAsErrors>`
- Modifying target framework defaults
- Adding conditional compilation symbols
- Changing package metadata
- Any change you don't fully understand

## Related Instruction Files

- [project.instructions.md](project.instructions.md) - For .csproj/.sln files
- [editorconfig.instructions.md](editorconfig.instructions.md) - For analyzer severities
- [generic.instructions.md](generic.instructions.md) - For multi-file changes
