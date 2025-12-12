<#
.SYNOPSIS
    Migrates API entries from PublicAPI.Unshipped.txt to PublicAPI.Shipped.txt files.

.DESCRIPTION
    This script moves all API entries from PublicAPI.Unshipped.txt files to their corresponding
    PublicAPI.Shipped.txt files. This should be run when releasing a new version to mark all
    current APIs as stable/shipped.

    The script handles:
    - Standard PublicAPI.Shipped.txt and PublicAPI.Unshipped.txt files
    - Framework-specific files (e.g., PublicAPI.Shipped.net472.txt, PublicAPI.Unshipped.net472.txt)
    - Preserves the #nullable enable header
    - Sorts API entries alphabetically
    - Removes duplicates

.PARAMETER Path
    The root path to search for PublicAPI files. Defaults to the src directory.

.PARAMETER WhatIf
    Shows what would be done without making changes.

.EXAMPLE
    .\Migrate-PublicApiToShipped.ps1
    Migrates all API entries in the src directory.

.EXAMPLE
    .\Migrate-PublicApiToShipped.ps1 -WhatIf
    Shows what would be migrated without making changes.
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter()]
    [string]$Path = (Join-Path $PSScriptRoot "..\..\src")
)

$ErrorActionPreference = 'Stop'

# Resolve the path
$Path = Resolve-Path $Path

Write-Host "🔍 Searching for PublicAPI files in: $Path" -ForegroundColor Cyan

# Find all Unshipped files (both standard and framework-specific)
$unshippedFiles = Get-ChildItem -Path $Path -Recurse -Filter "PublicAPI.Unshipped*.txt"

if ($unshippedFiles.Count -eq 0) {
    Write-Host "⚠️ No PublicAPI.Unshipped*.txt files found." -ForegroundColor Yellow
    exit 0
}

Write-Host "📦 Found $($unshippedFiles.Count) Unshipped file(s) to process" -ForegroundColor Green

$totalMigrated = 0

foreach ($unshippedFile in $unshippedFiles) {
    $directory = $unshippedFile.DirectoryName
    $fileName = $unshippedFile.Name
    
    # Determine the corresponding Shipped file name
    # PublicAPI.Unshipped.txt -> PublicAPI.Shipped.txt
    # PublicAPI.Unshipped.net472.txt -> PublicAPI.Shipped.net472.txt
    $shippedFileName = $fileName -replace 'Unshipped', 'Shipped'
    $shippedFile = Join-Path $directory $shippedFileName
    
    Write-Host "`n📄 Processing: $($unshippedFile.FullName)" -ForegroundColor White
    
    # Read unshipped content
    $unshippedContent = Get-Content $unshippedFile.FullName -Raw
    $unshippedLines = Get-Content $unshippedFile.FullName
    
    # Filter out empty lines and the #nullable enable header
    $apiEntries = $unshippedLines | Where-Object { 
        $_ -and 
        $_.Trim() -ne '' -and 
        $_.Trim() -ne '#nullable enable'
    }
    
    if ($apiEntries.Count -eq 0) {
        Write-Host "  ⏭️ No API entries to migrate (file is empty or only has header)" -ForegroundColor Gray
        continue
    }
    
    Write-Host "  📊 Found $($apiEntries.Count) API entries to migrate" -ForegroundColor Cyan
    
    # Read existing shipped content (if file exists)
    $existingShippedEntries = @()
    if (Test-Path $shippedFile) {
        $shippedLines = Get-Content $shippedFile
        $existingShippedEntries = $shippedLines | Where-Object { 
            $_ -and 
            $_.Trim() -ne '' -and 
            $_.Trim() -ne '#nullable enable'
        }
        Write-Host "  📋 Existing shipped entries: $($existingShippedEntries.Count)" -ForegroundColor Gray
    }
    
    # Combine and deduplicate
    $allEntries = @($existingShippedEntries) + @($apiEntries) | 
        Where-Object { $_ } | 
        Sort-Object -Unique
    
    Write-Host "  📝 Total unique entries after merge: $($allEntries.Count)" -ForegroundColor Green
    
    if ($PSCmdlet.ShouldProcess($shippedFile, "Write $($allEntries.Count) API entries")) {
        # Write to shipped file with #nullable enable header
        $shippedContent = "#nullable enable`n" + ($allEntries -join "`n") + "`n"
        Set-Content -Path $shippedFile -Value $shippedContent -NoNewline -Encoding UTF8
        
        # Clear the unshipped file (keep only the header)
        $unshippedNewContent = "#nullable enable`n"
        Set-Content -Path $unshippedFile.FullName -Value $unshippedNewContent -NoNewline -Encoding UTF8
        
        Write-Host "  ✅ Migrated $($apiEntries.Count) entries to $shippedFileName" -ForegroundColor Green
        $totalMigrated += $apiEntries.Count
    }
}

Write-Host "`n🎉 Migration complete! Total entries migrated: $totalMigrated" -ForegroundColor Green
