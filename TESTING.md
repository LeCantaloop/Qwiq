# GitHub Actions Workflow Testing Guide

This document provides information about the new GitHub Actions workflow that replaces AppVeyor CI.

## Current Status

✅ **Workflow Created**: `.github/workflows/main.yml` has been created and pushed
✅ **Code Review**: Completed - all feedback addressed
✅ **Security Scan**: Passed CodeQL analysis with 0 alerts
⏳ **First Run**: Awaiting user approval (fork/first-party workflow requirement)

## Workflow Overview

The workflow runs on two platforms:

### Windows Runner (Primary)
- **Purpose**: Full build and test execution
- **Tools**: MSBuild, NuGet, VSTest
- **Steps**:
  1. Checkout code with full history
  2. Setup MSBuild, NuGet, and VSTest
  3. Initialize environment (runs init.ps1)
  4. Restore NuGet packages
  5. Build solution in Release configuration
  6. Run all unit tests (excluding IntegrationTests and specified categories)
  7. Upload test results and binaries

### Ubuntu Runner (Secondary)
- **Purpose**: Best-effort build verification
- **Tools**: Mono, MSBuild
- **Steps**:
  1. Checkout code
  2. Verify Mono/MSBuild availability
  3. Restore NuGet packages (continue-on-error)
  4. Build solution (continue-on-error)
  5. Skip tests (Mono doesn't support VSTest)
  6. Upload binaries if available

## How to Test

### 1. Approve Workflow Run
1. Go to the Pull Request: https://github.com/rjmurillo/Qwiq/pulls
2. Find PR #29: "[WIP] Convert AppVeyor CI to GitHub Actions"
3. Navigate to the "Actions" tab or "Checks" section
4. Approve the workflow run (if you have repository permissions)

### 2. Manual Trigger
Alternatively, you can manually trigger the workflow:
1. Go to: https://github.com/rjmurillo/Qwiq/actions/workflows/main.yml
2. Click "Run workflow"
3. Select the `copilot/convert-appveyor-to-github-action` branch
4. Click "Run workflow"

## Expected Outcomes

### Success Criteria
- ✅ Windows build completes successfully
- ✅ NuGet packages restore without errors
- ✅ Solution builds in Release configuration
- ✅ All unit tests pass (excluding specified categories)
- ✅ Test results uploaded as artifacts
- ✅ Binaries uploaded as artifacts
- ⚠️ Ubuntu build may fail (acceptable - best effort only)

### Known Potential Issues

#### 1. NU1902 NuGet Vulnerability Warnings
**Symptom**: Build fails with NU1902 warnings about System.IdentityModel.Tokens.Jwt

**Solution**: If this occurs, we can add a NuGet.Config to suppress vulnerability warnings:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="repositoryPath" value="packages" />
  </config>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

Or add MSBuild parameters to treat warnings as warnings (not errors).

#### 2. VSTest Path Issues
**Symptom**: Cannot find vstest.console.exe

**Solution**: The workflow uses darenm/Setup-VSTest@v1.3 which should add VSTest to PATH. If this fails, we can hardcode the path or use an alternative action.

#### 3. Test Discovery Issues
**Symptom**: No test assemblies found

**Solution**: The workflow searches for `*Tests.dll` in `bin\Release` directories. Verify that:
- Tests are building correctly
- Assembly naming convention matches pattern
- Release configuration is being used

#### 4. ~~Init.ps1 Failures~~ (FIXED)
**Status**: ✅ Fixed in commit d3b67fc

**Previous Issue**: The init.ps1 script tried to download deprecated NuGet credential provider causing build failures.

**Solution Applied**: Removed init.ps1 step from workflow. Not needed since:
- GitHub Actions already provides NuGet via `nuget/setup-nuget@v2`
- Public feeds don't require VSS credential provider
- Modern NuGet has built-in credential providers

## Monitoring the Run

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
- **Runners**: Windows + Ubuntu instead of just Windows
- **Test Execution**: Direct VSTest instead of AppVeyor's built-in test runner
- **Artifacts**: Explicit artifact uploads instead of automatic

### What's the Same
- **Build Configuration**: Release, Any CPU
- **Test Filters**: Same categories excluded (localOnly, Benchmark, SOAP, REST, IntegrationTests)
- **NuGet Restore**: Same approach
- **MSBuild**: Same build tool and parameters

## Next Steps After Successful Run

1. Remove `[WIP]` from PR title
2. Update PR description with test results
3. Consider adding:
   - Build status badge to README.md
   - Scheduled runs (nightly builds)
   - Additional quality checks
4. Merge PR to enable workflow on develop branch
5. Consider deprecating AppVeyor configuration

## Questions or Issues?

If you encounter any issues not covered in this guide:
1. Check the Actions logs for detailed error information
2. Document the issue in the PR comments
3. I can help debug and fix the problem
4. Consider the pragmatic approach: document remaining issues and iterate later

## Summary

The workflow is ready for testing. The main blockers are:
1. **Approval required** - You need to approve the first run
2. **Unknown runtime issues** - We won't know until first run completes

The approach has been to create a solid, well-researched foundation that should work on the first try, but be prepared to iterate if needed. The workflow follows .NET Framework best practices and mirrors the AppVeyor configuration as closely as possible.
