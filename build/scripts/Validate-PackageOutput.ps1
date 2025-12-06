<#
.SYNOPSIS
    Validates that all packable projects produced NuGet packages.

.DESCRIPTION
    This script scans all .csproj files to identify projects marked as packable
    (IsPackable=true and GeneratePackageOnBuild=true), then validates that each
    produced both a .nupkg and .snupkg file.

    This is a CI build validation script - it should fail the build if any
    expected packages are missing.

.PARAMETER SolutionRoot
    The root directory containing the solution. Defaults to current directory.

.PARAMETER Configuration
    The build configuration to check. Defaults to 'Release'.

.PARAMETER SourcePaths
    Array of paths to search for .csproj files. Defaults to 'src' and 'test'.

.EXAMPLE
    .\Validate-PackageOutput.ps1
    Validates packages in src/ and test/ directories.

.EXAMPLE
    .\Validate-PackageOutput.ps1 -Configuration Debug
    Validates packages built with Debug configuration.

.NOTES
    This script should be run after 'dotnet build /t:Build,Pack' completes.
    It will exit with code 1 if any expected packages are missing.
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$SolutionRoot = ".",

    [Parameter(Mandatory = $false)]
    [string]$Configuration = "Release",

    [Parameter(Mandatory = $false)]
    [string[]]$SourcePaths = @("src", "test")
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Package Output Validation" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Solution root:  $SolutionRoot" -ForegroundColor White
Write-Host "  Configuration:  $Configuration" -ForegroundColor White
Write-Host "  Source paths:   $($SourcePaths -join ', ')" -ForegroundColor White

# Find all packable projects by scanning csproj files
$packableProjects = @()

foreach ($sourcePath in $SourcePaths) {
    $fullPath = Join-Path $SolutionRoot $sourcePath
    if (-not (Test-Path $fullPath)) {
        Write-Warning "Source path not found: $fullPath"
        continue
    }

    $csprojFiles = Get-ChildItem -Path $fullPath -Recurse -Filter "*.csproj" -File

    foreach ($csproj in $csprojFiles) {
        [xml]$content = Get-Content $csproj.FullName

        # Check if project is packable
        $isPackable = $content.SelectSingleNode("//IsPackable")?.InnerText
        $generatePackageOnBuild = $content.SelectSingleNode("//GeneratePackageOnBuild")?.InnerText

        # A project is packable if both conditions are met (or if GeneratePackageOnBuild is true without IsPackable=false)
        $shouldPack = ($isPackable -eq "true" -and $generatePackageOnBuild -eq "true") -or
                      ($generatePackageOnBuild -eq "true" -and $isPackable -ne "false")

        if ($shouldPack) {
            # Determine package ID (PackageId or project name)
            $packageId = $content.SelectSingleNode("//PackageId")?.InnerText
            if (-not $packageId) {
                # Default to assembly name or project name
                $packageId = $content.SelectSingleNode("//AssemblyName")?.InnerText
                if (-not $packageId) {
                    $packageId = [System.IO.Path]::GetFileNameWithoutExtension($csproj.Name)
                }
            }

            $packableProjects += [PSCustomObject]@{
                Name = $packageId
                ProjectFile = $csproj.FullName
                ProjectDir = $csproj.DirectoryName
                BinPath = Join-Path $csproj.DirectoryName "bin" $Configuration
            }
        }
    }
}

Write-Host "`nFound $($packableProjects.Count) packable project(s):" -ForegroundColor Cyan
foreach ($proj in $packableProjects) {
    Write-Host "  - $($proj.Name)" -ForegroundColor White
}

if ($packableProjects.Count -eq 0) {
    Write-Error "No packable projects found! Check that projects have IsPackable=true and GeneratePackageOnBuild=true"
    exit 1
}

# Validate each packable project produced its packages
$missingNupkg = @()
$missingSnupkg = @()
$foundPackages = @()

Write-Host "`n----------------------------------------" -ForegroundColor Cyan
Write-Host "Validating package output..." -ForegroundColor Cyan
Write-Host "----------------------------------------" -ForegroundColor Cyan

foreach ($proj in $packableProjects) {
    $binPath = $proj.BinPath

    if (-not (Test-Path $binPath)) {
        Write-Host "  $($proj.Name): " -NoNewline -ForegroundColor White
        Write-Host "MISSING (bin folder not found)" -ForegroundColor Red
        $missingNupkg += $proj.Name
        $missingSnupkg += $proj.Name
        continue
    }

    # Find .nupkg file (pattern: PackageName.*.nupkg, excluding .snupkg)
    $nupkg = Get-ChildItem -Path $binPath -Filter "$($proj.Name).*.nupkg" -File |
        Where-Object { $_.Name -notmatch "\.snupkg$" } |
        Select-Object -First 1

    # Find .snupkg file
    $snupkg = Get-ChildItem -Path $binPath -Filter "$($proj.Name).*.snupkg" -File |
        Select-Object -First 1

    $status = ""
    $color = "Green"

    if ($nupkg -and $snupkg) {
        $status = "OK (.nupkg + .snupkg)"
        $foundPackages += [PSCustomObject]@{
            Name = $proj.Name
            Nupkg = $nupkg.Name
            Snupkg = $snupkg.Name
        }
    }
    elseif ($nupkg -and -not $snupkg) {
        $status = "PARTIAL (missing .snupkg)"
        $color = "Yellow"
        $missingSnupkg += $proj.Name
    }
    elseif (-not $nupkg -and $snupkg) {
        $status = "PARTIAL (missing .nupkg)"
        $color = "Yellow"
        $missingNupkg += $proj.Name
    }
    else {
        $status = "MISSING (no packages found)"
        $color = "Red"
        $missingNupkg += $proj.Name
        $missingSnupkg += $proj.Name
    }

    Write-Host "  $($proj.Name): " -NoNewline -ForegroundColor White
    Write-Host $status -ForegroundColor $color
}

# Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Package Validation Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Expected packages: $($packableProjects.Count)" -ForegroundColor White
Write-Host "  Found complete:    $($foundPackages.Count)" -ForegroundColor $(if ($foundPackages.Count -eq $packableProjects.Count) { "Green" } else { "Yellow" })

if ($missingNupkg.Count -gt 0) {
    Write-Host "  Missing .nupkg:    $($missingNupkg.Count)" -ForegroundColor Red
    foreach ($missing in $missingNupkg) {
        Write-Host "    - $missing" -ForegroundColor Red
    }
}

if ($missingSnupkg.Count -gt 0) {
    Write-Host "  Missing .snupkg:   $($missingSnupkg.Count)" -ForegroundColor Red
    foreach ($missing in $missingSnupkg) {
        Write-Host "    - $missing" -ForegroundColor Red
    }
}

# Exit with error if any packages are missing
if ($missingNupkg.Count -gt 0 -or $missingSnupkg.Count -gt 0) {
    Write-Host "`nERROR: Package validation failed!" -ForegroundColor Red
    Write-Host "Build produced $($foundPackages.Count) of $($packableProjects.Count) expected packages." -ForegroundColor Red
    Write-Host "Ensure 'dotnet build /t:Build,Pack' completed successfully." -ForegroundColor Yellow
    exit 1
}

Write-Host "`nAll $($packableProjects.Count) packable projects produced their packages." -ForegroundColor Green
exit 0
