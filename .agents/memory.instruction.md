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

# Solutions Repository

## CI/CD Best Practices (Learned from PR #22)
- **Legacy Package Removal**: GitVersionTask 4.0.0 and Microsoft.Net.Compilers 2.10.0 are incompatible with MSBuild 17.x
  - GitVersionTask causes MSBuild internal errors on windows-latest
  - Microsoft.Net.Compilers is redundant (MSBuild 17.x has built-in Roslyn)
  - If these packages exist, they MUST be removed from ALL locations: packages.config, PackageReference, and .csproj Import statements
  - Check both src/ and test/ projects for PackageReference entries (test projects often missed)

- **Directory.Build.targets**: Required workaround file that:
  - Neutralizes EnsureNuGetPackageBuildImports target (prevents "missing import" errors after package removal)
  - Disables legacy FxCop in Release builds (exits with code 1 on warnings, failing CI)
  - Should be committed to repository, not generated dynamically

- **Init Scripts**: init.ps1 may fail due to deprecated credential providers
  - Old: Microsoft.VisualStudio.Services.NuGet.CredentialProvider (deprecated, no longer available)
  - New: Azure Artifacts Credential Provider (if needed)
  - Best approach for public feeds: Skip init.ps1 entirely, use GitHub Actions native NuGet setup

- **GitVersion in CI**: Use gittools/actions (GitHub Actions integration) instead of GitVersionTask NuGet package
  - Setup: `gittools/actions/gitversion/setup@v3.0.0`
  - Execute: `gittools/actions/gitversion/execute@v3.0.0`
  - Works with windows-latest and MSBuild 17.x

- **Test Execution**: Use vswhere to find vstest.console.exe dynamically
  - Fallback to hardcoded path if vswhere fails
  - Find path like: `Join-Path $vstestPath "Common7\\IDE\\CommonExtensions\\Microsoft\\TestWindow\\vstest.console.exe"`
  - Use TestCaseFilter for category exclusions

- **Ubuntu Runner**: .NET Framework 4.6 doesn't build on Linux
  - Include Ubuntu in matrix to satisfy requirements
  - Add skip message explaining why build is skipped
  - Don't upload test results from Ubuntu (no tests run)

## GitHub Actions for .NET Framework 4.6
- Use microsoft/setup-msbuild@v2 (not actions/setup-dotnet)
- Use windows-latest (not windows-2019) - MSBuild 17.x is compatible if legacy packages removed
- For VSTest on Windows: Use darenm/Setup-VSTest action OR find dynamically with vswhere
- Ubuntu/Mono builds: Skip entirely with notice message (not compatible with .NET Framework 4.6)
- NuGet package vulnerabilities: Known issue with System.IdentityModel.Tokens.Jwt causing NU1902 warnings
  - Use /p:WarningsNotAsErrors=NU1902 in MSBuild command to downgrade to warnings
- Test execution: Use vstest.console.exe with TestCaseFilter to exclude categories
- Create composite action for reusable build steps (setup-restore-build)
- Use NuGet caching with actions/cache@v4 for faster builds

## Common Mistakes to Avoid (From PR #22)
1. **Incomplete Package Removal**: Removing packages from packages.config but missing PackageReference entries in test projects
2. **Dynamic Directory.Build.targets**: Creating it in workflow instead of committing it to repo
3. **Wrong Windows Runner**: Using windows-2019 when windows-latest works with proper package cleanup
4. **Ubuntu Test Results**: Uploading test results from Ubuntu when no tests actually run
5. **Init.ps1 Dependencies**: Relying on init.ps1 without checking for deprecated credential provider issues

## GitVersioning Solution (PR #29)
Instead of removing GitVersionTask from all projects (massive change), use Nerdbank.GitVersioning:
- Create `version.json` at repo root with version configuration
- Create `Directory.Build.props` to add Nerdbank.GitVersioning package reference to all projects
- Create `Directory.Build.targets` to neutralize GitVersionTask imports (override targets with no-ops)
- This allows build to succeed without touching individual project files
- Nerdbank.GitVersioning is compatible with MSBuild 17.x and actively maintained
