---
applyTo: "**/*.ps1"
---

# PowerShell Script Instructions

> **MANDATORY**: You MUST follow these instructions when editing any PowerShell script in this repository.

## Quick Reference

- Scripts are in `scripts/` directory
- Use PowerShell 7+ compatible syntax
- Follow existing patterns in similar scripts
- Test scripts locally before committing

## Context Loading

When working on PowerShell files, you MUST:

1. Read this entire instruction file before making changes
2. Check existing scripts for established patterns
3. Test scripts locally
4. Complete the Validation Checklist before submitting

## Script Organization

```
scripts/
└── init/
    ├── Initialize-DownloadLatest.ps1
    ├── Initialize-Environment.ps1
    ├── Initialize-InstallFromNuget.ps1
    ├── Initialize-NuGet.ps1
    ├── Restore-ToolPackages.ps1
    └── Update-Environment.ps1
```

## PowerShell Standards

### Parameter Blocks

```powershell
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$Path,

    [Parameter(Mandatory = $false)]
    [switch]$Force
)
```

### Error Handling

```powershell
try {
    # Operation
}
catch {
    Write-Error "Failed to complete operation: $_"
    throw
}
```

### Logging

```powershell
Write-Verbose "Starting operation..."
Write-Information "Important status update"
Write-Warning "Potential issue detected"
Write-Error "Operation failed"
```

## Common Operations

### Building the Solution

```powershell
# Recommended: Single-threaded to avoid file locking
dotnet build Qwiq.sln /m:1 /nodeReuse:false -c Release

# Standard (may have issues on Windows)
dotnet build Qwiq.sln -c Release
```

### Running Tests

```powershell
$filter = "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
dotnet test Qwiq.sln --no-build -c Release --filter $filter
```

### Restoring Tools

```powershell
dotnet tool restore
```

## Validation Checklist

Before submitting changes, verify:

- [ ] Script runs without errors
- [ ] Parameters are properly validated
- [ ] Error handling is in place
- [ ] Verbose/Debug output is appropriate
- [ ] Script is idempotent where applicable

## Common Mistakes to AVOID

```powershell
# ❌ WRONG: Using aliases (not portable)
ls | % { $_.Name }

# ✅ CORRECT: Full cmdlet names
Get-ChildItem | ForEach-Object { $_.Name }

# ❌ WRONG: No error handling
Remove-Item $path

# ✅ CORRECT: With error handling
Remove-Item $path -ErrorAction Stop

# ❌ WRONG: Hardcoded paths
$path = "C:\src\Qwiq"

# ✅ CORRECT: Relative or parameterized
$path = Join-Path $PSScriptRoot ".."
```

## Decision Trees

### When Creating New Scripts

1. Does a similar script exist? → Extend or refactor existing
2. Is this a one-time operation? → Consider inline in workflow
3. Will this be reused? → Create proper module or script

### When to Stop and Ask

- Creating new initialization scripts
- Modifying environment setup
- Changing build/deploy scripts

## Related Instruction Files

- [yaml.instructions.md](yaml.instructions.md) - For workflows that call scripts
- [generic.instructions.md](generic.instructions.md) - For multi-file changes
