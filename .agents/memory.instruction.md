---
applyTo: '**'
---

# Coding Preferences
- Use windows-latest for the Windows runner (not windows-2019)
- Keep init.ps1 execution minimal or skip if it causes issues with deprecated packages
- GitVersion should use gittools/actions instead of GitVersionTask NuGet package

# Project Architecture
- .NET Framework 4.6 project (not modern .NET)
- Uses MSBuild for building (MSBuild 17.x on windows-latest)
- Solution file: Qwiq.sln
- Project structure: src/ and test/ directories
- Mix of packages.config and PackageReference-based projects
- Has Directory.Build.targets file that neutralizes NuGet package import validation
- Has build/targets/common.props that sets project-wide build settings

# Solutions Repository

## CI/CD Best Practices (Learned from PR #22 and PR #29)

### NuGet Package Restore Issues
- **CRITICAL**: For legacy packages.config projects, use `-PackagesDirectory packages` with nuget restore
- Without this flag, NuGet may not restore packages to the expected `packages/` folder
- Projects reference packages via relative paths like `..\..\packages\PackageName.Version\`
- If packages aren't in that folder, MSBuild will fail with missing reference errors

### Legacy FxCop (FxCopCmd.exe) Issues
- **Problem**: Legacy FxCop runs as post-build step and generates warnings that fail CI (TreatWarningsAsErrors)
- **Root Cause**: `RunCodeAnalysis=true` is set in `build/targets/common.props`
- **Solution**: Set `RunCodeAnalysis=false` in common.props and Directory.Build.targets
- **Modern Alternative**: Use Microsoft.CodeAnalysis.NetAnalyzers (Roslyn-based analyzers)

### Legacy Package Removal
- GitVersionTask 4.0.0 and Microsoft.Net.Compilers 2.10.0 are incompatible with MSBuild 17.x
  - GitVersionTask causes MSBuild internal errors on windows-latest
  - Microsoft.Net.Compilers is redundant (MSBuild 17.x has built-in Roslyn)
  - If these packages exist, they MUST be removed from ALL locations: packages.config, PackageReference, and .csproj Import statements
  - Check both src/ and test/ projects for PackageReference entries (test projects often missed)

- **Directory.Build.targets**: Required workaround file that:
  - Neutralizes EnsureNuGetPackageBuildImports target (prevents "missing import" errors after package removal)
  - Disables legacy FxCop in Release builds (exits with code 1 on warnings, failing CI)
  - Provides no-op targets for GitVersionTask (GetVersion, UpdateAssemblyInfo, GenerateGitVersionInformation)
  - Should be committed to repository, not generated dynamically

### Init Scripts
- init.ps1 may fail due to deprecated credential providers
  - Old: Microsoft.VisualStudio.Services.NuGet.CredentialProvider (deprecated, no longer available)
  - New: Azure Artifacts Credential Provider (if needed)
  - Best approach for public feeds: Skip init.ps1 entirely, use GitHub Actions native NuGet setup

### GitVersion in CI
- Use Nerdbank.GitVersioning instead of GitVersionTask NuGet package
- Add via Directory.Build.props with PackageReference
- Works with windows-latest and MSBuild 17.x
- version.json provides configuration

### Test Execution
- Use vswhere to find vstest.console.exe dynamically
- Or use darenm/Setup-VSTest action
- Use TestCaseFilter for category exclusions

### Ubuntu Runner
- .NET Framework 4.6 doesn't build on Linux
- Include Ubuntu in matrix to satisfy requirements
- Add skip message explaining why build is skipped
- Don't upload test results from Ubuntu (no tests run)

## GitHub Actions for .NET Framework 4.6
- Use microsoft/setup-msbuild@v2 (not actions/setup-dotnet)
- Use windows-latest (not windows-2019) - MSBuild 17.x is compatible if legacy packages removed
- **IMPORTANT**: Use `nuget restore Qwiq.sln -NonInteractive -PackagesDirectory packages` for packages.config projects
- For VSTest on Windows: Use darenm/Setup-VSTest action OR find dynamically with vswhere
- Ubuntu/Mono builds: Skip entirely with notice message (not compatible with .NET Framework 4.6)
- NuGet package vulnerabilities: Known issue with System.IdentityModel.Tokens.Jwt causing NU1902 warnings
- Test execution: Use vstest.console.exe with TestCaseFilter to exclude categories
- Use NuGet caching with actions/cache@v4 for faster builds

## Common Mistakes to Avoid
1. **Incomplete Package Removal**: Removing packages from packages.config but missing PackageReference entries in test projects
2. **Dynamic Directory.Build.targets**: Creating it in workflow instead of committing it to repo
3. **Wrong Windows Runner**: Using windows-2019 when windows-latest works with proper package cleanup
4. **Ubuntu Test Results**: Uploading test results from Ubuntu when no tests actually run
5. **Init.ps1 Dependencies**: Relying on init.ps1 without checking for deprecated credential provider issues
6. **Missing -PackagesDirectory**: Not specifying packages directory for legacy projects, causing package restore to use global cache
7. **FxCop Enabled in Common.props**: build/targets/common.props sets RunCodeAnalysis=true which must be disabled

## Legacy Package Removal (PR #22 and #29)
Both GitVersionTask 4.0.0 and Microsoft.Net.Compilers 2.10.0 MUST be completely removed:
- Neutralization via Directory.Build.targets doesn't work
- Packages get restored by NuGet and MSBuild loads incompatible assemblies before targets execute
- GitVersionTask: "MSB0001: Internal MSBuild Error: Task Instance should be null"
- Microsoft.Net.Compilers: "DisableSdkPath parameter is not supported by the Csc task"

**Required removal approach:**
1. Remove from all packages.config files
2. Remove PackageReference entries from all .csproj files
3. Remove Error condition checks
4. Remove Import statements
5. Replace functionality:
   - GitVersionTask → Nerdbank.GitVersioning (via Directory.Build.props)
   - Microsoft.Net.Compilers → Not needed (MSBuild 17.x has built-in Roslyn)

**FxCop Suppression:**
- Disable legacy FxCop globally via build/targets/common.props (set RunCodeAnalysis=false)
- Also disable in Directory.Build.targets for redundancy
- FxCop is deprecated in favor of Roslyn analyzers (Microsoft.CodeAnalysis.NetAnalyzers)
