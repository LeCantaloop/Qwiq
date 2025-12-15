<#
.SYNOPSIS
    Validates that all packable projects produced NuGet packages.

.DESCRIPTION
    This script scans all .csproj files to identify projects marked as packable
    (IsPackable=true and GeneratePackageOnBuild=true), then validates that each
    produced a .nupkg file in the centralized package output directory.

    Qwiq uses embedded symbols (DebugType=embedded, IncludeSymbols=false) per ADR-012,
    so no separate .snupkg files are generated. Symbols are embedded in the .dll/.nupkg.

    The SDK places packages in artifacts/package/{Configuration} when ArtifactsPath is set.

    This is a CI build validation script - it should fail the build if any
    expected packages are missing.

.PARAMETER SolutionRoot
    The root directory containing the solution. Defaults to current directory.

.PARAMETER Configuration
    The build configuration to check. Defaults to 'Release'.

.PARAMETER SourcePaths
    Array of paths to search for .csproj files. Defaults to 'src' and 'test'.

.PARAMETER PackageOutputPath
    The directory where packages are output. Defaults to artifacts/package/{Configuration}.

.EXAMPLE
    .\Validate-PackageOutput.ps1
    Validates packages in the default artifacts/package/release directory.

.EXAMPLE
    .\Validate-PackageOutput.ps1 -Configuration Debug
    Validates packages built with Debug configuration.

.EXAMPLE
    .\Validate-PackageOutput.ps1 -PackageOutputPath "C:\custom\output"
    Validates packages in a custom output directory.

.NOTES
    This script should be run after 'dotnet build /t:Build,Pack' completes.
    It will exit with code 1 if any expected packages are missing.

    Qwiq uses embedded symbols per ADR-012. Symbols are embedded in .dll files
    within the .nupkg, eliminating the need for separate .snupkg symbol packages.
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$SolutionRoot = ".",

    [Parameter(Mandatory = $false)]
    [string]$Configuration = "Release",

    [Parameter(Mandatory = $false)]
    [string[]]$SourcePaths = @("src", "test"),

    [Parameter(Mandatory = $false)]
    [string]$PackageOutputPath = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Package Output Validation" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Solution root:  $SolutionRoot" -ForegroundColor White
Write-Host "  Configuration:  $Configuration" -ForegroundColor White
Write-Host "  Source paths:   $($SourcePaths -join ', ')" -ForegroundColor White

# Determine the package output path
# The SDK places packages in artifacts/package/{Configuration} when ArtifactsPath is set
if (-not $PackageOutputPath) {
    $PackageOutputPath = Join-Path $SolutionRoot "artifacts" "package" $Configuration.ToLower()
}

Write-Host "  Package output:   $PackageOutputPath" -ForegroundColor White

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
$foundPackages = @()

Write-Host "`n----------------------------------------" -ForegroundColor Cyan
Write-Host "Validating package output..." -ForegroundColor Cyan
Write-Host "----------------------------------------" -ForegroundColor Cyan

# Verify the package output directory exists
if (-not (Test-Path $PackageOutputPath)) {
    Write-Error "Package output directory not found: $PackageOutputPath"
    Write-Host "Ensure 'dotnet build /t:Build,Pack' completed successfully." -ForegroundColor Yellow
    exit 1
}

foreach ($proj in $packableProjects) {
    # Find .nupkg file (pattern: PackageName.*.nupkg, excluding .snupkg)
    $nupkg = Get-ChildItem -Path $PackageOutputPath -Filter "$($proj.Name).*.nupkg" -File |
        Where-Object { $_.Name -notmatch "\.snupkg$" } |
        Select-Object -First 1

    $status = ""
    $color = "Green"

    # Embedded symbols mode: only expect .nupkg (symbols are embedded)
    if ($nupkg) {
        $status = "OK (.nupkg with embedded symbols)"
        $foundPackages += [PSCustomObject]@{
            Name = $proj.Name
            Nupkg = $nupkg.Name
        }
    }
    else {
        $status = "MISSING (.nupkg not found)"
        $color = "Red"
        $missingNupkg += $proj.Name
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

# Exit with error if any packages are missing
if ($missingNupkg.Count -gt 0) {
    Write-Host "`nERROR: Package validation failed!" -ForegroundColor Red
    Write-Host "Build produced $($foundPackages.Count) of $($packableProjects.Count) expected packages." -ForegroundColor Red
    Write-Host "Ensure 'dotnet build /t:Build,Pack' completed successfully." -ForegroundColor Yellow
    exit 1
}

Write-Host "`nAll $($packableProjects.Count) packable projects produced their packages." -ForegroundColor Green
exit 0
