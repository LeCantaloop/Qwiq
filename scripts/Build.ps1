[cmdletBinding()]
param(
    [Parameter(Mandatory=$true)] [string] $repoRoot,
    [Parameter(Mandatory=$false)] [boolean] $parallelBuild = $true
)

Set-StrictMode -Version 2
$ErrorActionPreference = "Stop"

# Prefer the absolute path of the environment's msbuild.exe if it exists
$msbuild = (Get-Command -ErrorAction SilentlyContinue -Name msbuild).Path
if (!$msbuild) {
    # Try to locate MSBuild and put it on the path
    Ensure-MSBuildPath

    $msbuild = (Get-Command -ErrorAction SilentlyContinue -Name msbuild).Path
    if (!$msbuild) {
        throw "MSBuild.exe could not be found."
    }
}

$buildCommandProperties = @{
        "Configuration" = "Release";
        "RestorePackages" = "False";
}

Get-ChildItem -Filter *.sln -Path $repoRoot -Recurse -ErrorAction SilentlyContinue | ForEach-Object {
    $solution = $_.FullName
    $solutionDir = $_.Directory.FullName

    "Performing operations on $solution" | Write-Verbose

    $buildArgs = (New-BuildCommandArguments -Properties $buildCommandProperties -Target Rebuild -ParallelBuild $parallelBuild)
    "Generated MSBuild arguments: $buildArgs" | Write-Verbose

    $buildCommand = "& `"$msbuild`" --% $buildArgs `"$solution`""
    Exec -Command { Invoke-Expression $buildCommand }
}