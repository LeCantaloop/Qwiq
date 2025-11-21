---
applyTo: '**'
---

# Coding Preferences
[To be discovered]

# Project Architecture
- .NET Framework 4.6 project (not modern .NET)
- Uses MSBuild for building
- Solution file: Qwiq.sln
- Uses AppVeyor for CI (to be converted to GitHub Actions)
- Project structure: src/ and test/ directories

# Solutions Repository
- GitHub Actions for .NET Framework 4.6: Use microsoft/setup-msbuild@v2 (not actions/setup-dotnet)
- For VSTest on Windows: Use darenm/Setup-VSTest action
- Ubuntu/Mono builds: Pre-installed on ubuntu-latest runners, use continue-on-error for best-effort
- NuGet package vulnerabilities: Known issue with System.IdentityModel.Tokens.Jwt causing NU1902 warnings
- Test execution: Use vstest.console.exe with TestCaseFilter to exclude categories
