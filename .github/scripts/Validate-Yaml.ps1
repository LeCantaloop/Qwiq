#!/usr/bin/env pwsh
<#
.SYNOPSIS
YAML Syntax Validation and Linting (For CI/CD Only - NOT for agents)

.DESCRIPTION
This script validates YAML syntax and checks for common issues.
Uses dotnet pprettier (formatting) and yamllint (linting).
Cross-platform compatible (Windows, Linux, macOS).

AGENTS: DO NOT USE THIS SCRIPT. The pre-commit hook validates YAML automatically.
Running this manually wastes tokens in an OODA loop.

.PARAMETER FilePath
Path to the YAML file to validate

.PARAMETER SkipLint
Skip yamllint checks, only validate with pprettier

.EXAMPLE
pwsh .github/scripts/Validate-Yaml.ps1 .github/workflows/main.yml

.EXAMPLE
pwsh .github/scripts/Validate-Yaml.ps1 .github/workflows/main.yml -SkipLint

.NOTES
Requires: dotnet pprettier (restored via: dotnet tool restore)
Optional: yamllint (pip install yamllint) for enhanced linting
This script is for CI/CD pipelines only.
#>

param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$FilePath,
    
    [Parameter(Mandatory = $false)]
    [switch]$SkipLint
)

# Check if file exists
if (-not (Test-Path $FilePath)) {
    Write-Error "File not found: $FilePath"
    exit 1
}

$hasErrors = $false

# Step 1: Validate with dotnet pprettier (formatting + syntax)
Write-Host "Checking YAML formatting and syntax..." -ForegroundColor Cyan
try {
    $pprettierCheck = dotnet pprettier --version 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "⚠ dotnet pprettier not found. Run 'dotnet tool restore' first." -ForegroundColor Yellow
        $hasErrors = $true
    }
    else {
        $output = dotnet pprettier --check $FilePath 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Formatting check passed" -ForegroundColor Green
        }
        else {
            Write-Host "✗ Formatting check failed" -ForegroundColor Red
            Write-Host $output -ForegroundColor Yellow
            $hasErrors = $true
        }
    }
}
catch {
    Write-Host "✗ Formatting check error: $_" -ForegroundColor Red
    $hasErrors = $true
}

# Step 2: Lint with yamllint (if available and not skipped)
if (-not $SkipLint) {
    Write-Host "`nChecking YAML linting rules..." -ForegroundColor Cyan
    
    $yamllintCmd = Get-Command yamllint -ErrorAction SilentlyContinue
    if ($null -eq $yamllintCmd) {
        Write-Host "ℹ yamllint not found (optional). Install with: pip install yamllint" -ForegroundColor Cyan
        Write-Host "  Skipping lint checks..." -ForegroundColor Cyan
    }
    else {
        try {
            $lintOutput = yamllint -f parsable $FilePath 2>&1
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✓ Lint check passed" -ForegroundColor Green
            }
            else {
                Write-Host "✗ Lint check failed" -ForegroundColor Red
                Write-Host $lintOutput -ForegroundColor Yellow
                $hasErrors = $true
            }
        }
        catch {
            Write-Host "✗ Lint check error: $_" -ForegroundColor Red
            $hasErrors = $true
        }
    }
}

# Final result
Write-Host ""
if ($hasErrors) {
    Write-Host "✗ YAML validation failed: $FilePath" -ForegroundColor Red
    exit 1
}
else {
    Write-Host "✓ YAML validation passed: $FilePath" -ForegroundColor Green
    exit 0
}
