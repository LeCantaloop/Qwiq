# Session Log: WireMock Test Fix

**Date**: 2025-12-12
**Branch**: `chore/modernize-4`
**Focus**: Investigate and fix WireMock test failures

## Summary

Investigated why WireMock tests were failing and implemented a proper fix by moving tests to a dedicated .NET 8+ project where WireMock works correctly.

## Root Cause Analysis

### Problem

WireMock.Net has a **known OWIN hosting deadlock issue** on .NET Framework 4.7.2 when running in MSTest runners. The server starts and binds to the port, but the internal OWIN middleware never processes HTTP requests.

### Evidence

1. TCP listener tests passed - networking is functional
2. HttpListener tests passed - HTTP serving works natively
3. WireMock server reported `IsStarted: True` with correct port
4. All HTTP requests to WireMock timed out (never responded)
5. `HandleRequestsSynchronously = true` setting did not fix the issue

### Documented Issues

- WireMock.Net GitHub Issue #393, #470, #1089 - all describe the same deadlock behavior
- The issue occurs specifically with OWIN hosting on .NET Framework in test runners

### Constraint

The `Qwiq.Integration.Tests` project targets only `net472` due to its dependency on `Microsoft.TeamFoundationServer.ExtendedClient`, which only works on .NET Framework.

## Solution Implemented

Created a new dedicated test project `Qwiq.WireMock.Tests` targeting `net8.0;net9.0;net10.0` where WireMock uses Kestrel hosting instead of OWIN.

### Files Created

- `test/Qwiq.WireMock.Tests/Qwiq.WireMock.Tests.csproj`
- `test/Qwiq.WireMock.Tests/AzureDevOpsWireMockExtensions.cs` (using System.Text.Json)
- `test/Qwiq.WireMock.Tests/WireMockRestContextSpecification.cs`
- `test/Qwiq.WireMock.Tests/WireMockRestStoreContext.cs`
- `test/Qwiq.WireMock.Tests/WireMockQueryTests.cs`
- `test/Qwiq.WireMock.Tests/WireMock/Stubs/azure-devops-stubs.json`
- `test/Qwiq.WireMock.Tests/WireMock/Stubs/azure-devops-stubs-extracted.json`

### Files Modified

- `src/Qwiq.Core/Qwiq.Core.csproj` - Added `InternalsVisibleTo` for `Qwiq.WireMock.Tests`
- `src/Qwiq.Core.Rest/Qwiq.Client.Rest.csproj` - Added `InternalsVisibleTo` for `Qwiq.WireMock.Tests`
- `Qwiq.sln` - Added new test project

### Files Removed

- `test/Qwiq.Integration.Tests/WireMock/` (entire folder)
- WireMock.Net package reference from `Qwiq.IntegrationTests.csproj`

## Verification

```text
Test Run Successful.
Total tests: 9
     Passed: 9
Total time: 1.5301 Seconds
```

All 9 WireMock tests pass on .NET 8.0.

## Key Insights

1. **WireMock.Net OWIN Issue**: This is a well-documented issue that affects .NET Framework 4.7.2+ in test runners. The server starts but requests deadlock.

2. **Kestrel Works**: On .NET Core/.NET 5+, WireMock uses Kestrel hosting which does not have the deadlock issue.

3. **Captured Stubs Are Complete**: The `azure-devops-stubs-extracted.json` file contains full Azure DevOps API responses including connection data with all service definitions. No custom handshake setup is needed.

4. **Previous Agent's Conclusion Was Correct**: The previous agent correctly identified the OWIN deadlock issue but their solution (adding `[Ignore]` attributes) was incomplete. The proper fix was to move to a TFM where WireMock works.

## Commits

1. `31ea4218` - fix(tests): move WireMock tests to dedicated .NET 8+ project

## Decisions Made

| Decision                                           | Rationale                                                                                       |
| -------------------------------------------------- | ----------------------------------------------------------------------------------------------- |
| Create separate project instead of multi-targeting | Integration.Tests requires net472 for TFS Extended Client; WireMock requires net8.0+ to work    |
| Use System.Text.Json                               | Modern .NET projects should use built-in JSON; Newtonsoft.Json still available via dependencies |
| Keep captured stubs from real traffic              | The extracted stubs contain complete API responses including service definitions                |
| Remove WireMock from Integration.Tests             | Cleaning up non-functional code; tests can only pass in the new project                         |

## Files Changed Summary

- Created: 7 new files in `test/Qwiq.WireMock.Tests/`
- Modified: 4 files (2 csproj for InternalsVisibleTo, solution file, IntegrationTests.csproj)
- Deleted: 8 files from `test/Qwiq.Integration.Tests/WireMock/`
