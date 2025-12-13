# Session 37: CI Workflow Refactoring with Composite Actions

**Date**: 2025-12-13
**Branch**: `chore/modernize-4`
**PR**: #97

## Purpose

Refactor GitHub Actions workflows to use reusable composite actions, following the pattern from moq.analyzers. Separate CodeQL and mutation testing into dedicated workflows.

## Work Completed

### 1. Created Composite Actions

**`.github/actions/setup-dotnet/action.yml`**

- Minimal action for .NET SDK setup using global.json
- Used by all workflow jobs for consistent SDK configuration

**`.github/actions/restore-build/action.yml`**

- Configurable restore and build action with inputs:
  - `solution` - Solution file path (default: Qwiq.sln)
  - `configuration` - Build config (default: Release)
  - `enable-pack` - Enable package generation (default: false)
  - `generate-binlog` - Generate binary log (default: false)
  - `binlog-path` - Binary log output path
  - `additional-build-args` - Extra MSBuild arguments
- Uses `$ErrorActionPreference = 'Stop'` for proper error handling
- Uses environment variables instead of direct input interpolation (security best practice)
- Creates binlog directory if needed

### 2. Created Separate Workflows

**`.github/workflows/codeql.yml`**

- Dedicated CodeQL security analysis workflow
- Triggers: push (develop/master), pull_request, weekly schedule
- Multi-platform: windows-latest and ubuntu-latest
- Uses composite actions for setup and build

**`.github/workflows/mutation-testing.yml`**

- Dedicated Stryker.NET mutation testing workflow
- Triggers: workflow_dispatch (manual), weekly schedule (Monday 3 AM UTC)
- Windows-only (for net472 TFM support)
- Uses composite actions for setup and build
- Advisory mode (does not block builds)

### 3. Simplified main.yml

- Removed workflow_dispatch input for mutation testing
- Removed schedule trigger (moved to dedicated workflows)
- Removed security-events permission (CodeQL now separate)
- Removed CodeQL init/analyze steps
- Removed mutation-testing job entirely
- Now focuses only on: build, test, pack, SBOM generation

## Design Decisions

### Two Composite Actions Instead of One

The moq.analyzers pattern (single action) wouldn't work for Qwiq because CodeQL must initialize BETWEEN checkout and build:

```text
Checkout → Setup .NET → CodeQL Init → Restore+Build → CodeQL Analyze
                ↑              ↑
           Action 1       Action 2 (after CodeQL init)
```

If we put everything in one composite action, we couldn't insert CodeQL init between checkout and restore/build.

### Code Review Improvements Applied

Based on csharp-pod and csharp-expert review feedback:

1. Added `$ErrorActionPreference = 'Stop'` to all PowerShell scripts
2. Used environment variables for inputs (avoids injection issues)
3. Added directory creation for binlog path
4. Added `::group::` markers for cleaner build output

## Files Changed

**Created:**

- `.github/actions/setup-dotnet/action.yml`
- `.github/actions/restore-build/action.yml`
- `.github/workflows/codeql.yml`
- `.github/workflows/mutation-testing.yml`

**Modified:**

- `.github/workflows/main.yml` - Simplified, uses composite actions

## Commits

- `b22afa7c` - refactor(ci): extract reusable composite actions and separate workflows

## Verification

| Check                | Status                        |
| -------------------- | ----------------------------- |
| Local build          | ✅ 0 errors, 0 warnings       |
| Main build (Windows) | ✅ SUCCESS                    |
| Main build (Linux)   | ✅ SUCCESS                    |
| CodeQL Analysis      | ✅ Running (takes ~10-15 min) |
| DevSkim              | ✅ SUCCESS                    |
| PSScriptAnalyzer     | ✅ SUCCESS                    |
| Dependency Review    | ✅ SUCCESS                    |
| Secret Scanning      | ✅ SUCCESS                    |

## Next Steps

1. Monitor CodeQL workflow completion on PR #97
2. Consider adding NuGet caching to composite actions (future enhancement)
3. Test mutation-testing workflow manually or wait for weekly schedule

## Key Files for Reference

- `.github/actions/restore-build/action.yml` - Main composite action
- `.github/workflows/main.yml` - Simplified main workflow
- `.github/workflows/codeql.yml` - Separated CodeQL workflow
- `.github/workflows/mutation-testing.yml` - Separated mutation testing workflow
