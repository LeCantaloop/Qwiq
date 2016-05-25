[cmdletBinding()]
param(
    [Parameter(Mandatory=$true)] [string] $repoRoot
)

$vsTestPath = """${ENV:PROGRAMFILES(x86)}\Microsoft Visual Studio 12.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"""

$asm = Get-ChildItem -Path $repoRoot -Filter *.csproj -Recurse | ForEach-Object {
    [xml]$doc = Get-Content $_.FullName
    $assembly = ($doc.project.PropertyGroup.assemblyname)

    $path = $_.Directory.FullName
    $assemblyName = ("$assembly").trim() + ".dll"

    $paths = @()

    foreach($node in $doc.Project.PropertyGroup.OutputPath) {
        if ($node) {
            $assemblyPath = "$path\$node$assemblyName"
            if (Test-Path $assemblyPath) {
                $paths += $assemblyPath
            }
        }
    }

    return $paths
}

$testArgs = @()
$testArgs += "/TestCaseFilter:`"TestCategory!=localOnly`""
$testArgs += "/UseVsixExtensions:false"
$testArgs += "/Logger:trx"

$args = ($asm+$testArgs) -Join " "

$testCommand = "& $vsTestPath --% $args"

Exec -Command { Invoke-Expression $testCommand }