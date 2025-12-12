<#
.SYNOPSIS
    Converts a Fiddler HAR file to WireMock stub mappings.

.DESCRIPTION
    This script parses a HAR (HTTP Archive) file exported from Fiddler and
    extracts the relevant Azure DevOps API calls, converting them to WireMock
    stub mappings that can be used for offline testing.

.PARAMETER HarFile
    Path to the HAR file to convert.

.PARAMETER OutputFile
    Path where the WireMock stubs JSON will be saved.

.PARAMETER FilterHost
    Only include requests to this host. Default is "qwiq-sandbox.visualstudio.com".

.EXAMPLE
    .\Convert-HarToWireMock.ps1 -HarFile ".agents\qwiq.har" -OutputFile "test\Qwiq.Integration.Tests\WireMock\Stubs\azure-devops-stubs.json"
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$HarFile,

    [Parameter(Mandatory = $false)]
    [string]$OutputFile = ".\wiremock-stubs.json",

    [Parameter(Mandatory = $false)]
    [string]$FilterHost = "qwiq-sandbox.visualstudio.com"
)

$ErrorActionPreference = "Stop"

Write-Host "=== HAR to WireMock Converter ===" -ForegroundColor Cyan
Write-Host ""

# Read and parse HAR file
Write-Host "Reading HAR file: $HarFile" -ForegroundColor Yellow
$harContent = Get-Content $HarFile -Raw
$har = $harContent | ConvertFrom-Json

Write-Host "Found $($har.log.entries.Count) entries in HAR file" -ForegroundColor Green

# Filter entries
$relevantEntries = $har.log.entries | Where-Object {
    $_.request.url -match $FilterHost -and
    $_.request.method -ne "CONNECT" -and  # Skip HTTPS tunnel setup
    $_.response.status -eq 200  # Only successful responses
}

Write-Host "Filtered to $($relevantEntries.Count) relevant entries" -ForegroundColor Green

# Key endpoints we need for WireMock
$keyEndpoints = @(
    "/_apis/connectionData",
    "/_apis/resourceAreas",
    "/_apis/wit/wiql",
    "/_apis/wit/workItems",
    "/_apis/wit/workitemsbatch",
    "/_apis/wit/workItemTypes",
    "/_apis/wit/fields",
    "/_apis/projects"
)

$mappings = @{}  # Use hashtable for deduplication

foreach ($entry in $relevantEntries) {
    $url = $entry.request.url
    $method = $entry.request.method

    # Parse URL to get path
    $uri = [System.Uri]$url
    $path = $uri.AbsolutePath

    # Check if this is a key endpoint
    $isKeyEndpoint = $false
    foreach ($endpoint in $keyEndpoints) {
        if ($path -like "*$endpoint*") {
            $isKeyEndpoint = $true
            break
        }
    }

    if (-not $isKeyEndpoint) {
        continue
    }

    # Create unique key for deduplication
    $mappingKey = "$method|$path"
    if ($mappings.ContainsKey($mappingKey)) {
        Write-Host "  Skipping duplicate: $method $path" -ForegroundColor DarkGray
        continue
    }

    Write-Host "  Processing: $method $path" -ForegroundColor Gray

    # Get response body
    $responseBody = $null
    if ($entry.response.content.text) {
        if ($entry.response.content.encoding -eq "base64") {
            $responseBody = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($entry.response.content.text))
        } else {
            $responseBody = $entry.response.content.text
        }
    }

    # Skip if no response body
    if (-not $responseBody) {
        Write-Host "    Skipping - no response body" -ForegroundColor DarkGray
        continue
    }

    # Try to parse as JSON to validate
    try {
        # Use -AsHashtable to handle empty string property names
        $jsonBody = $responseBody | ConvertFrom-Json -AsHashtable -ErrorAction Stop
        # Re-serialize to ensure proper formatting (use -Depth 100 for deeply nested objects)
        $responseBody = $jsonBody | ConvertTo-Json -Depth 100 -Compress
    } catch {
        Write-Host "    Skipping - not valid JSON: $($_.Exception.Message)" -ForegroundColor DarkGray
        continue
    }    # Build request matcher
    $requestMatcher = @{
        method = $method
        urlPath = $path
    }

    # Add query parameters if present
    if ($uri.Query) {
        $requestMatcher.urlPathPattern = "$path.*"
        $requestMatcher.Remove("urlPath")
    }

    # Create mapping name
    $mappingName = "$method $path"
    if ($path -match "connectionData") { $mappingName = "Connection Data - VssConnection Handshake" }
    elseif ($path -match "resourceAreas") { $mappingName = "Resource Areas - Location Service" }
    elseif ($path -match "wiql") { $mappingName = "WIQL Query" }
    elseif ($path -match "workitemsbatch") { $mappingName = "Work Items Batch" }
    elseif ($path -match "workItems") { $mappingName = "Work Items" }
    elseif ($path -match "workItemTypes") { $mappingName = "Work Item Types" }
    elseif ($path -match "fields") { $mappingName = "Field Definitions" }
    elseif ($path -match "projects") { $mappingName = "Project Info" }

    # Build response headers
    $responseHeaders = @{
        "Content-Type" = "application/json; charset=utf-8"
    }

    # Create WireMock mapping
    $mapping = @{
        name = $mappingName
        request = $requestMatcher
        response = @{
            status = [int]$entry.response.status
            headers = $responseHeaders
            body = $responseBody
        }
    }

    $mappings[$mappingKey] = $mapping
    Write-Host "    Added mapping: $mappingName" -ForegroundColor Green
}

# Create final WireMock stubs structure
$wireMockStubs = @{
    mappings = @($mappings.Values)
}

# Save to file
Write-Host ""
Write-Host "Saving $($mappings.Values.Count) mappings to: $OutputFile" -ForegroundColor Yellow

$wireMockStubs | ConvertTo-Json -Depth 20 | Set-Content $OutputFile -Encoding UTF8

Write-Host ""
Write-Host "=== Conversion Complete ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Review the generated stubs in: $OutputFile"
Write-Host "  2. Copy to: test\Qwiq.Integration.Tests\WireMock\Stubs\"
Write-Host "  3. Remove [Ignore] from WireMock tests"
Write-Host "  4. Run WireMock tests to verify"
