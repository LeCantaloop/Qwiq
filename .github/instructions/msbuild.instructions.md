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

## Validation Checklist

Before submitting changes, verify:

- [ ] `dotnet build Qwiq.sln /m:1 /nodeReuse:false -c Release` succeeds
- [ ] 0 errors, minimal warnings
- [ ] All test projects still build
- [ ] `dotnet test` with filters passes
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
