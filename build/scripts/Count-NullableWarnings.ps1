#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Counts CS8xxx nullable reference type warnings across all source projects.

.DESCRIPTION
    This script builds each source project with CS8xxx warnings temporarily enabled
    and counts the warnings by type. Useful for baseline measurement and tracking
    progress during nullable reference type migration.

.PARAMETER ReportPath
    Optional path to save the report. Defaults to .agents/CS8xxx-baseline.md

.EXAMPLE
    ./build/scripts/Count-NullableWarnings.ps1
    Outputs warning counts to console and saves to .agents/CS8xxx-baseline.md

.EXAMPLE
    ./build/scripts/Count-NullableWarnings.ps1 -ReportPath ./warnings-report.md
    Saves report to custom location
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$ReportPath = ".agents/CS8xxx-baseline.md"
)

$ErrorActionPreference = "Stop"

# Change to repository root
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "CS8xxx Nullable Warning Counter" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Find all source projects
$projects = Get-ChildItem -Path "src" -Filter "*.csproj" -Recurse | Sort-Object FullName

$totalWarnings = 0
$results = @()

foreach ($project in $projects) {
    $projectName = $project.BaseName
    Write-Host "Analyzing $projectName..." -ForegroundColor Yellow
    
    # Build project with warnings enabled but not as errors
    # Suppress other warnings to focus on CS8xxx
    $buildOutput = dotnet build $project.FullName `
        -c Debug `
        --no-restore `
        /p:TreatWarningsAsErrors=false `
        /p:EnforceCodeStyleInBuild=false `
        /nologo `
        /v:quiet `
        2>&1 | Out-String
    
    # Count CS8xxx warnings
    $cs8Warnings = [regex]::Matches($buildOutput, "warning CS8\d{3}:")
    $warningCount = $cs8Warnings.Count
    
    # Group warnings by code
    $warningsByCode = $cs8Warnings | ForEach-Object { 
        if ($_ -match "CS8(\d{3})") {
            "CS8$($matches[1])"
        }
    } | Group-Object | Sort-Object Name
    
    $results += [PSCustomObject]@{
        Project = $projectName
        TotalWarnings = $warningCount
        WarningsByCode = $warningsByCode
        BuildOutput = $buildOutput
    }
    
    $totalWarnings += $warningCount
    
    if ($warningCount -eq 0) {
        Write-Host "  ✅ 0 warnings" -ForegroundColor Green
    } else {
        Write-Host "  ⚠️  $warningCount warnings" -ForegroundColor Red
        foreach ($group in $warningsByCode) {
            Write-Host "    - $($group.Name): $($group.Count)" -ForegroundColor Gray
        }
    }
    Write-Host ""
}

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "Summary: $totalWarnings total CS8xxx warnings" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Generate markdown report
$report = @"
# CS8xxx Nullable Reference Type Warning Baseline

**Generated**: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")  
**Repository**: Qwiq  
**Branch**: $(git branch --show-current)  
**Commit**: $(git rev-parse --short HEAD)

---

## Executive Summary

Total CS8xxx warnings across all source projects: **$totalWarnings**

## Warning Breakdown by Project

| Project | Total Warnings | Status |
|---------|----------------|--------|
"@

foreach ($result in $results) {
    $status = if ($result.TotalWarnings -eq 0) { "✅ Clean" } else { "⚠️ Needs attention" }
    $report += "`n| $($result.Project) | $($result.TotalWarnings) | $status |"
}

$report += @"

---

## Detailed Warnings by Project

"@

foreach ($result in $results) {
    $report += @"

### $($result.Project)

**Total Warnings**: $($result.TotalWarnings)

"@
    
    if ($result.TotalWarnings -eq 0) {
        $report += "✅ No CS8xxx warnings detected. Project is fully annotated.`n"
    } else {
        $report += "**Warnings by Code**:`n`n"
        $report += "| Code | Count | Description |`n"
        $report += "|------|-------|-------------|`n"
        
        foreach ($group in $result.WarningsByCode) {
            $description = switch ($group.Name) {
                "CS8600" { "Converting null literal or possible null value to non-nullable type" }
                "CS8601" { "Possible null reference assignment" }
                "CS8602" { "Dereference of a possibly null reference" }
                "CS8603" { "Possible null reference return" }
                "CS8604" { "Possible null argument for parameter" }
                "CS8605" { "Unboxing a possibly null value" }
                "CS8618" { "Non-nullable property must contain a non-null value when exiting constructor" }
                "CS8619" { "Nullability of reference types in value doesn't match target type" }
                "CS8620" { "Argument cannot be used for parameter due to nullability differences" }
                "CS8625" { "Cannot convert null literal to non-nullable reference type" }
                "CS8629" { "Nullable value type may be null" }
                "CS8764" { "Nullability of return type doesn't match overridden member" }
                "CS8765" { "Nullability of type of parameter doesn't match overridden member" }
                "CS8766" { "Nullability of reference types in return type doesn't match implicitly implemented member" }
                "CS8767" { "Nullability of reference types in type of parameter doesn't match implicitly implemented member" }
                "CS8769" { "Nullability of reference types in type of parameter doesn't match implemented member" }
                default { "Unknown warning" }
            }
            $report += "| $($group.Name) | $($group.Count) | $description |`n"
        }
    }
}

$report += @"

---

## Next Steps

Based on this baseline:

1. **Projects with 0 warnings**: Already complete, verify and document
2. **Projects with warnings**: Follow mitigation plan in CS8xxx-TODO.md

## Migration Priority

Per CS8xxx-mitigation.md PRD:

- **P0**: Qwiq.Core.Rest (partially complete)
- **P1**: Qwiq.Identity, Qwiq.Core.Soap
- **P2**: Qwiq.Linq, Qwiq.Mapper, Qwiq.Mapper.Identity

---

**End of Report**
"@

# Save report
$reportFullPath = Join-Path $repoRoot $ReportPath
$reportDir = Split-Path -Parent $reportFullPath
if (-not (Test-Path $reportDir)) {
    New-Item -Path $reportDir -ItemType Directory -Force | Out-Null
}

$report | Out-File -FilePath $reportFullPath -Encoding UTF8
Write-Host "Report saved to: $ReportPath" -ForegroundColor Green

# Return results object for scripting
return $results
