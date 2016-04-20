[cmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string] $repoRoot
)

Set-StrictMode -Version 2
$ErrorActionPreference = "Stop"

# Prefer the absolute path of the environment's nuget.exe if it exists
$nuget = (Get-Command -ErrorAction SilentlyContinue -Name nuget).Path
if (!$nuget) {
    # Get NuGet path
    $nuget = Get-NugetPath
    if (!$nuget) {
        throw "NuGet.exe could not be found."
    }
}

"Using NuGet from '$nuget'" | Write-Verbose

Get-ChildItem -Filter *.sln -Path $repoRoot -Recurse -ErrorAction SilentlyContinue | ForEach-Object {
    $solutionDir = $_.Directory.FullName

    Get-ChildItem -Filter packages.config -Path $solutionDir -Recurse -ErrorAction SilentlyContinue | Select-Object -ExpandProperty FullName | ForEach-Object {
        $nugetArgs = "restore $($_) -SolutionDir `"$($solutionDir)`" -DisableParallelProcessing"
        $nugetCommand = "& `"$nuget`" --% $nugetArgs"
        Exec -Command { Invoke-Expression $nugetCommand }
    }
    $packages = Get-NuGetPackageVersions -RepositoryRoot $solutionDir
    if ($packages -ne $null) {
        $conflicts = Get-NuGetPackageVersionConflicts -Packages $packages
        if ($conflicts -ne $null) {
            Write-NuGetPackageVersionConflicts $conflicts
        }
    }
}