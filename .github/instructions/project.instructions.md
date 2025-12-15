---
applyTo: "**/*.{csproj,sln}"
---

# Project File Instructions

> **MANDATORY**: You MUST follow these instructions when editing any project file (.csproj, .sln) in this repository.

## Quick Reference

- This repository uses **SDK-style projects** with multi-targeting
- **Central Package Management** via `Directory.Packages.props`
- Do NOT add `Version` attributes to PackageReference elements
- Target frameworks vary by project type (see table below)

## Context Loading

When working on project files, you MUST:

1. Read this entire instruction file before making changes
2. Cross-reference with [msbuild.instructions.md](msbuild.instructions.md) for .props/.targets
3. Check `Directory.Packages.props` for package versions
4. Complete the Validation Checklist before submitting

## Target Framework Reference

| Project Type   | Target Frameworks              | Notes                                  |
| -------------- | ------------------------------ | -------------------------------------- |
| Core libraries | `net472;netstandard2.0;net8.0` | Full multi-targeting                   |
| SOAP projects  | `net472` only                  | Windows-only, TFS Client OM dependency |
| REST projects  | `net472;netstandard2.0;net8.0` | Cross-platform capable                 |
| Test projects  | `net472;net8.0`                | Skip netstandard for tests             |

## Central Package Management

All package versions are defined in `Directory.Packages.props` at the repository root.

### Adding a New Package

1. Add version to `Directory.Packages.props`:

   ```xml
   <PackageVersion Include="NewPackage" Version="1.2.3" />
   ```

2. Reference in .csproj WITHOUT version:

   ```xml
   <PackageReference Include="NewPackage" />
   ```

### Common Mistakes to AVOID

```xml
<!-- ❌ WRONG: Version in csproj (breaks Central Package Management) -->
<PackageReference Include="Shouldly" Version="4.2.0" />

<!-- ✅ CORRECT: Version-less reference -->
<PackageReference Include="Shouldly" />
```

## InternalsVisibleTo Configuration

After migration to SDK-style projects, `InternalsVisibleTo` is defined in .csproj files:

```xml
<ItemGroup>
  <InternalsVisibleTo Include="TestProjectAssemblyName" />
</ItemGroup>
```

### Current Configuration

| Source Project                | Visible To                                    |
| ----------------------------- | --------------------------------------------- |
| `Qwiq.Core.csproj`            | `Qwiq.Core.UnitTests`, `Qwiq.Mocks`           |
| `Qwiq.Client.Rest.csproj`     | `Qwiq.IntegrationTests`                       |
| `Qwiq.Client.Soap.csproj`     | `Qwiq.Identity.Soap`, `Qwiq.IntegrationTests` |
| `Qwiq.Identity.Soap.csproj`   | `Qwiq.IntegrationTests`                       |
| `Qwiq.Mapper.Identity.csproj` | `Qwiq.Identity.UnitTests`                     |

If you encounter `'Type' is inaccessible due to its protection level` errors, add an `InternalsVisibleTo` entry.

## SDK-Style Project Structure

### Required Properties (inherited from Directory.Build.props)

These are set repository-wide and should NOT be overridden:

- `<Nullable>enable</Nullable>`
- `<ImplicitUsings>enable</ImplicitUsings>`
- `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`

### Project-Specific Properties

Each project should define:

```xml
<PropertyGroup>
  <TargetFrameworks>net472;net8.0</TargetFrameworks>
  <Description>Project-specific description for NuGet</Description>
  <PackageId>Qwiq.ProjectName</PackageId>
</PropertyGroup>
```

## PedanticMode: Warnings as Errors

All projects inherit `TreatWarningsAsErrors` via the `PedanticMode` property, controlled in `build/targets/codeanalysis/CodeAnalysis.targets`.

### Default Behavior

- **CI builds**: `PedanticMode=true` (warnings treated as errors)
- **Local builds**: `PedanticMode` defaults to `$(ContinuousIntegrationBuild)` value

### Build Commands

```powershell
# Strict build (warnings as errors) - CI default
dotnet build Qwiq.sln -c Release /p:PedanticMode=true

# Flexible build (warnings allowed) - for diagnosing analyzers
dotnet build Qwiq.sln -c Release /p:PedanticMode=false
```

### When to Use `/p:PedanticMode=false`

- Diagnosing noisy analyzer rules
- Investigating new analyzer violations
- Prototyping changes with temporary warnings

**Important**: All CI builds enforce `PedanticMode=true`. Fix warnings before committing.

## Validation Checklist

Before submitting changes, verify:

- [ ] `dotnet restore Qwiq.sln` succeeds
- [ ] `dotnet build Qwiq.sln -c Release` succeeds with 0 errors (PedanticMode=true)
- [ ] No new warnings introduced
- [ ] Tests pass with filters applied
- [ ] **Package tests pass** (if modifying packable projects or build configuration)
- [ ] Package baselines updated if manifests/contents changed
- [ ] Package versions are in `Directory.Packages.props` (not individual csproj)
- [ ] InternalsVisibleTo entries are correct

## Decision Trees

### When Adding a New Project

1. Determine target frameworks based on project type
2. Create SDK-style .csproj with minimal properties
3. Add to Qwiq.sln
4. Add InternalsVisibleTo if tests need internal access
5. Verify build succeeds

### When Updating Dependencies

1. Check if package exists in `Directory.Packages.props`
2. If new: add to `Directory.Packages.props` first
3. Update version in `Directory.Packages.props` only
4. Run security scan if updating major versions
5. Test all affected projects

### When to Stop and Ask

- Adding new target frameworks
- Changing multi-targeting strategy
- Updating TFS/Azure DevOps SDK packages
- Modifying Central Package Management structure

## Related Instruction Files

- [msbuild.instructions.md](msbuild.instructions.md) - For Directory.Build.props/targets
- [csharp.instructions.md](csharp.instructions.md) - For C# code changes
- [generic.instructions.md](generic.instructions.md) - For multi-file changes
