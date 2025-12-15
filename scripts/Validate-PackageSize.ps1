#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Validates NuGet package sizes to detect unexpected bloat.
.DESCRIPTION
    Checks that .nupkg files don't exceed the 2MB threshold.
    Prevents regressions from accidental inclusion of large files.
.PARAMETER PackagePath
    Path to directory containing .nupkg files. Defaults to artifacts/package/release.
.PARAMETER MaxSizeMB
    Maximum allowed package size in MB. Defaults to 2.
.EXAMPLE
    .\Validate-PackageSize.ps1
    Validates packages in default location with 2MB threshold.
.EXAMPLE
    .\Validate-PackageSize.ps1 -PackagePath "artifacts/package/debug" -MaxSizeMB 3
    Validates debug packages with custom 3MB threshold.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$PackagePath = "artifacts/package/release",
    
    [Parameter(Mandatory = $false)]
    [int]$MaxSizeMB = 2
)

$ErrorActionPreference = "Stop"

Write-Host "📦 Validating package sizes in: $PackagePath" -ForegroundColor Cyan
Write-Host "   Maximum allowed size: ${MaxSizeMB}MB" -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path $PackagePath)) {
    Write-Error "Package path not found: $PackagePath"
    exit 1
}

$maxBytes = $MaxSizeMB * 1024 * 1024  # Convert MB to bytes explicitly for clarity
$nupkgFiles = Get-ChildItem -Path $PackagePath -Filter "*.nupkg" -Recurse

if ($nupkgFiles.Count -eq 0) {
    Write-Warning "No .nupkg files found in $PackagePath"
    exit 0
}

Write-Host "Found $($nupkgFiles.Count) package(s) to validate:" -ForegroundColor Cyan
Write-Host ""

$failures = @()

foreach ($file in $nupkgFiles) {
    $sizeMB = [math]::Round($file.Length / 1MB, 2)
    
    if ($file.Length -gt $maxBytes) {
        $failures += "❌ $($file.Name): ${sizeMB}MB (exceeds ${MaxSizeMB}MB threshold)"
        Write-Host "❌ $($file.Name): ${sizeMB}MB" -ForegroundColor Red
    } else {
        Write-Host "✅ $($file.Name): ${sizeMB}MB" -ForegroundColor Green
    }
}

Write-Host ""

if ($failures.Count -gt 0) {
    Write-Host "Package size validation FAILED:" -ForegroundColor Red
    Write-Host ""
    $failures | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
    Write-Host ""
    Write-Host "Investigate potential bloat:" -ForegroundColor Yellow
    Write-Host "  - Embedded resources" -ForegroundColor Yellow
    Write-Host "  - Unintended files in package" -ForegroundColor Yellow
    Write-Host "  - Large dependencies" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

Write-Host "✅ All packages within size threshold" -ForegroundColor Green
exit 0
