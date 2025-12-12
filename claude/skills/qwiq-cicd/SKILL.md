---
name: qwiq-cicd
description: CI/CD workflow guidance for QWIQ including GitHub Actions configuration, workflow best practices, and deployment patterns. Use when editing workflow files, debugging CI failures, or setting up new pipelines.
---

# QWIQ CI/CD Workflows

## Quick Reference

| Requirement    | Value                                                                                       |
| -------------- | ------------------------------------------------------------------------------------------- |
| Runner         | `windows-latest` (required for net472/SOAP)                                                 |
| Checkout depth | `fetch-depth: 0` (for GitVersioning)                                                        |
| SDK setup      | Use `global-json-file: ./global.json`                                                       |
| Pre-build      | `dotnet tool restore` (for nbgv)                                                            |
| Build flags    | `/p:Deterministic=true /p:UseSharedCompilation=false /nodeReuse:false`                      |
| Test filter    | `--filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=IntegrationTests"` |

**Key behavior:** `ContinuousIntegrationBuild=true` auto-enables `PedanticMode=true` (warnings as errors)

## Purpose

This skill provides guidance for GitHub Actions workflows in the QWIQ repository. It covers workflow configuration, runner requirements, build flags, and troubleshooting CI failures.

## When To Use

- Editing `.github/workflows/*.yml` files
- Debugging CI build or test failures
- Setting up new workflow jobs
- Understanding CI-specific build behavior

## Instructions

### 1. Standard Workflow Structure

```yaml
name: Build

on:
  push:
    branches: [develop, master]
  pull_request:
    branches: [develop, master]

jobs:
  build:
    runs-on: windows-latest # Required for net472/SOAP

    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0 # Required for GitVersioning

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          global-json-file: ./global.json # Use pinned SDK

      - name: Restore tools
        run: dotnet tool restore # For nbgv

      - name: Restore packages
        run: dotnet restore Qwiq.sln

      - name: Build
        run: dotnet build Qwiq.sln -c Release /p:Deterministic=true

      - name: Test
        run: dotnet test Qwiq.sln --no-build -c Release --filter "TestCategory!=localOnly&TestCategory!=Benchmark"
```

### 2. Key Requirements

| Requirement           | Reason                                    |
| --------------------- | ----------------------------------------- |
| `windows-latest`      | SOAP projects require Windows for net472  |
| `fetch-depth: 0`      | Nerdbank.GitVersioning needs full history |
| `dotnet tool restore` | Restores nbgv from tool manifest          |
| `global-json-file`    | Uses pinned SDK version                   |

### 3. Deterministic Build Flags

```yaml
run: dotnet build /p:Deterministic=true /p:UseSharedCompilation=false /nodeReuse:false
```

### 4. Test Filters

```yaml
# Exclude integration tests in CI
run: dotnet test --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
```

For complex filters, use `.runsettings` files instead of inline strings.

### 5. Artifact Upload

```yaml
- name: Upload binlog
  uses: actions/upload-artifact@v4
  with:
    name: build-logs
    path: ./artifacts/logs/build.binlog
  if: always()

- name: Upload packages
  uses: actions/upload-artifact@v4
  with:
    name: packages
    path: ./artifacts/package/Release/*.nupkg
```

### 6. CI-Specific Behavior

When `ContinuousIntegrationBuild=true` (set automatically):

- `PedanticMode=true` (warnings as errors)
- Deterministic output enabled
- Source link enabled

### 7. Common CI Failures

**Build succeeds locally but fails in CI:**

```powershell
# Reproduce CI environment locally
dotnet build -c Release /p:ContinuousIntegrationBuild=true /m:1
```

**File locking errors:**

```yaml
run: dotnet build /m:1 /nodeReuse:false
```

**Test timeout:**

```yaml
run: dotnet test --timeout 300000 # 5 minutes
```

## Examples

### Example 1: Adding a New Workflow Job

**User goal:** Add code coverage reporting

**Process:**

1. Add new job after `build` job
2. Use `needs: build` for dependency
3. Use same runner (`windows-latest`)
4. Add coverage tool and upload step

**Expected output:** New job that runs after build and uploads coverage

### Example 2: Debugging CI Failure

**User goal:** Fix "warnings treated as errors" failure

**Process:**

1. Check which warning code is failing
2. Either fix the warning in code, OR
3. Adjust severity in `.editorconfig` (not workflow)
4. Never use `--warnaserror-` in CI

**Expected output:** Warning fixed at source, not suppressed in CI

## Troubleshooting

### Workflow fails with "version not found"

**Cause:** `fetch-depth: 0` missing, GitVersioning can't compute version

**Fix:**

```yaml
- uses: actions/checkout@v4
  with:
    fetch-depth: 0 # Required for nbgv
```

### Build fails with CS warnings as errors

**Cause:** `PedanticMode=true` treats warnings as errors in CI

**Fixes:**

1. **Preferred:** Fix the warning in source code
2. **If necessary:** Adjust severity in `.editorconfig`
3. **Never:** Use `--warnaserror-` to suppress in workflow

### "Tool 'nbgv' not found"

**Cause:** `dotnet tool restore` not run before build

**Fix:** Add tool restore step:

```yaml
- name: Restore tools
  run: dotnet tool restore
```

### Tests timeout in CI

**Cause:** Default timeout too short for CI runners

**Fix:**

```yaml
- name: Test
  run: dotnet test --timeout 300000 # 5 minutes
```

### Artifacts not uploaded on failure

**Cause:** Missing `if: always()` condition

**Fix:**

```yaml
- name: Upload logs
  uses: actions/upload-artifact@v4
  with:
    name: build-logs
    path: ./artifacts/logs/
  if: always() # Upload even on failure
```

### "windows-2019 is deprecated"

**Fix:** Update to `windows-latest`:

```yaml
jobs:
  build:
    runs-on: windows-latest # Not windows-2019
```

## Related Resources

- See [REFERENCE.md](./REFERENCE.md) for workflow file locations and secrets
- See [../qwiq-build/SKILL.md](../qwiq-build/SKILL.md) for build configuration
- See [../qwiq-testing/SKILL.md](../qwiq-testing/SKILL.md) for test configuration
