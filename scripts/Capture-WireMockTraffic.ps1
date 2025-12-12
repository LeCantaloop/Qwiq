<#
.SYNOPSIS
    Captures HTTP traffic from integration tests for WireMock stub generation.

.DESCRIPTION
    This script runs the REST integration tests while capturing HTTP traffic.
    The captured traffic can be used to create WireMock stubs for offline testing.

.PARAMETER ProxyPort
    The port number for the proxy server. Default is 8888.

.PARAMETER OutputPath
    The path where captured traffic will be saved. Default is ./WireMockRecordings.

.PARAMETER UseFiddler
    If specified, assumes Fiddler is running and sets the proxy accordingly.

.EXAMPLE
    # Run with default settings (requires WireMock or Fiddler running on port 8888)
    .\Capture-WireMockTraffic.ps1

.EXAMPLE
    # Run with Fiddler on default port 8866
    .\Capture-WireMockTraffic.ps1 -UseFiddler

.NOTES
    Prerequisites:
    1. Either Fiddler or WireMock must be running as a proxy
    2. HTTPS decryption must be enabled in the proxy
    3. The proxy's root certificate must be trusted

    For Fiddler:
    - Enable "Decrypt HTTPS traffic" in Tools > Options > HTTPS
    - Export and trust the Fiddler root certificate

    For WireMock:
    - Start WireMock with proxy mode enabled
    - Configure SSL certificate handling
#>

[CmdletBinding()]
param(
    [int]$ProxyPort = 8888,
    [string]$OutputPath = "./WireMockRecordings",
    [switch]$UseFiddler
)

$ErrorActionPreference = "Stop"

# Fiddler default port
if ($UseFiddler) {
    $ProxyPort = 8866
}

Write-Host "=== WireMock Traffic Capture ===" -ForegroundColor Cyan
Write-Host ""

# Check if proxy is running
$proxyUrl = "http://localhost:$ProxyPort"
Write-Host "Checking proxy at $proxyUrl..." -ForegroundColor Yellow

try {
    $null = Invoke-WebRequest -Uri $proxyUrl -TimeoutSec 2 -ErrorAction SilentlyContinue
}
catch {
    Write-Warning "Proxy may not be running on port $ProxyPort"
    Write-Host ""
    Write-Host "Please ensure one of the following is running:" -ForegroundColor Yellow
    Write-Host "  1. Fiddler with HTTPS decryption enabled (default port 8866)"
    Write-Host "  2. Charles Proxy with SSL proxying enabled"
    Write-Host "  3. WireMock in proxy mode"
    Write-Host ""

    $continue = Read-Host "Continue anyway? (y/n)"
    if ($continue -ne 'y') {
        exit 1
    }
}

# Create output directory
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$outputDir = Join-Path $OutputPath $timestamp
New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
Write-Host "Output directory: $outputDir" -ForegroundColor Green

# Set proxy environment variables
Write-Host ""
Write-Host "Setting proxy environment variables..." -ForegroundColor Yellow
$env:HTTP_PROXY = $proxyUrl
$env:HTTPS_PROXY = $proxyUrl
$env:NO_PROXY = "localhost,127.0.0.1"

# Also set for .NET
[System.Net.WebRequest]::DefaultWebProxy = New-Object System.Net.WebProxy($proxyUrl, $false)

Write-Host "  HTTP_PROXY = $env:HTTP_PROXY"
Write-Host "  HTTPS_PROXY = $env:HTTPS_PROXY"

# Run integration tests
Write-Host ""
Write-Host "Running REST integration tests..." -ForegroundColor Yellow
Write-Host ""

$testProject = Join-Path $PSScriptRoot "..\test\Qwiq.Integration.Tests\Qwiq.IntegrationTests.csproj"

try {
    # Run a subset of tests to capture the key traffic
    dotnet test $testProject `
        --filter "FullyQualifiedName~Given_WorkItemStore_When_Querying" `
        --logger "console;verbosity=normal" `
        --no-build
}
catch {
    Write-Warning "Some tests may have failed, but traffic should still be captured"
}

# Clear proxy settings
Write-Host ""
Write-Host "Clearing proxy settings..." -ForegroundColor Yellow
$env:HTTP_PROXY = $null
$env:HTTPS_PROXY = $null
[System.Net.WebRequest]::DefaultWebProxy = $null

Write-Host ""
Write-Host "=== Capture Complete ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Export captured traffic from your proxy tool"
Write-Host "  2. Save relevant requests/responses to: test\Qwiq.Integration.Tests\WireMock\Stubs\"
Write-Host "  3. Key endpoints to capture:"
Write-Host "     - GET /_apis/connectionData"
Write-Host "     - GET /_apis/resourceAreas"
Write-Host "     - POST /{project}/_apis/wit/wiql"
Write-Host "     - POST /_apis/wit/workitemsbatch"
Write-Host ""

if ($UseFiddler) {
    Write-Host "In Fiddler:" -ForegroundColor Green
    Write-Host "  1. Select the sessions for qwiq-sandbox.visualstudio.com"
    Write-Host "  2. Right-click > Export Sessions > All Sessions"
    Write-Host "  3. Choose 'HTTPArchive v1.2' format"
    Write-Host "  4. Save to: $outputDir\fiddler-capture.har"
}
