<#
.SYNOPSIS
    Measures test flakiness by running the test suite multiple iterations.

.DESCRIPTION
    Runs the Qwiq test suite multiple times (default: 50) to identify flaky tests.
    A test is considered flaky if it fails in some runs but not others.
    
    Results are saved to docs/metrics/test-flakiness-report.md

.PARAMETER Iterations
    Number of times to run the test suite (default: 50)

.PARAMETER Configuration
    Build configuration to test (default: Release)

.PARAMETER Filter
    Test filter to apply (default: excludes localOnly, Benchmark, SOAP, REST, IntegrationTests)

.EXAMPLE
    .\scripts\Measure-TestFlakiness.ps1
    
    Runs 50 iterations with default settings

.EXAMPLE
    .\scripts\Measure-TestFlakiness.ps1 -Iterations 100
    
    Runs 100 iterations for higher confidence

.NOTES
    Requires: .NET SDK, dotnet test
    Expected runtime: ~10-20 minutes for 50 iterations
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [int]$Iterations = 50,
    
    [Parameter(Mandatory = $false)]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory = $false)]
    [string]$Filter = "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

# Ensure we're in the repository root
$repoRoot = Split-Path -Parent $PSScriptRoot
Push-Location $repoRoot

try {
    Write-Host "=== Qwiq Test Flakiness Measurement ===" -ForegroundColor Cyan
    Write-Host "Iterations: $Iterations" -ForegroundColor Gray
    Write-Host "Configuration: $Configuration" -ForegroundColor Gray
    Write-Host "Filter: $Filter" -ForegroundColor Gray
    Write-Host ""
    
    # Build once before testing
    Write-Host "Building solution..." -ForegroundColor Yellow
    dotnet build Qwiq.sln -c $Configuration /m:1 /nodeReuse:false | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed with exit code $LASTEXITCODE"
    }
    Write-Host "Build successful" -ForegroundColor Green
    Write-Host ""
    
    # Data structures to track results
    $testResults = @{}  # testName -> @{ Passed = count, Failed = count, Skipped = count }
    $iterationResults = @()  # Array of iteration summaries
    
    $startTime = Get-Date
    
    # Run test suite multiple times
    for ($i = 1; $i -le $Iterations; $i++) {
        Write-Host "[$i/$Iterations] Running test iteration..." -ForegroundColor Cyan
        
        # Run tests and capture output
        $testOutput = dotnet test Qwiq.sln `
            -c $Configuration `
            --no-build `
            --filter $Filter `
            --logger "console;verbosity=normal" `
            2>&1
        
        $exitCode = $LASTEXITCODE
        
        # Parse the output for test results
        $passedCount = 0
        $failedCount = 0
        $skippedCount = 0
        
        # Extract test names and results from output
        $testOutput | ForEach-Object {
            $line = $_.ToString()
            
            # Match patterns like "Passed TestName [duration]"
            if ($line -match '^\s+(Passed|Failed|Skipped)\s+(.+?)\s+\[') {
                $result = $Matches[1]
                $testName = $Matches[2].Trim()
                
                # Initialize test tracking
                if (-not $testResults.ContainsKey($testName)) {
                    $testResults[$testName] = @{
                        Passed = 0
                        Failed = 0
                        Skipped = 0
                    }
                }
                
                # Increment appropriate counter
                $testResults[$testName][$result]++
                
                switch ($result) {
                    "Passed" { $passedCount++ }
                    "Failed" { $failedCount++ }
                    "Skipped" { $skippedCount++ }
                }
            }
            
            # Also capture summary lines
            if ($line -match 'Passed:\s+(\d+)') {
                $passedCount = [int]$Matches[1]
            }
            if ($line -match 'Failed:\s+(\d+)') {
                $failedCount = [int]$Matches[1]
            }
            if ($line -match 'Skipped:\s+(\d+)') {
                $skippedCount = [int]$Matches[1]
            }
        }
        
        $iterationResults += @{
            Iteration = $i
            Passed = $passedCount
            Failed = $failedCount
            Skipped = $skippedCount
            ExitCode = $exitCode
        }
        
        if ($failedCount -gt 0) {
            Write-Host "  Result: $passedCount passed, $failedCount FAILED, $skippedCount skipped" -ForegroundColor Red
        } else {
            Write-Host "  Result: $passedCount passed, $failedCount failed, $skippedCount skipped" -ForegroundColor Green
        }
    }
    
    $endTime = Get-Date
    $duration = $endTime - $startTime
    
    Write-Host ""
    Write-Host "=== Analysis Complete ===" -ForegroundColor Cyan
    Write-Host "Total time: $($duration.ToString('hh\:mm\:ss'))" -ForegroundColor Gray
    Write-Host ""
    
    # Analyze flaky tests
    $flakyTests = @()
    $totalTests = $testResults.Count
    
    foreach ($testName in $testResults.Keys) {
        $data = $testResults[$testName]
        $passedPct = ($data.Passed / $Iterations) * 100
        $failedPct = ($data.Failed / $Iterations) * 100
        
        # A test is flaky if it both passed and failed
        if ($data.Failed -gt 0 -and $data.Passed -gt 0) {
            $flakyTests += @{
                Name = $testName
                Passed = $data.Passed
                Failed = $data.Failed
                Skipped = $data.Skipped
                PassRate = $passedPct
                FailRate = $failedPct
            }
        }
    }
    
    # Calculate flake rate
    $flakeRate = if ($totalTests -gt 0) { ($flakyTests.Count / $totalTests) * 100 } else { 0 }
    
    # Generate report
    $reportPath = "docs/metrics/test-flakiness-report.md"
    $report = @"
# Test Flakiness Report

**Generated**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')  
**Iterations**: $Iterations  
**Configuration**: $Configuration  
**Total Execution Time**: $($duration.ToString('hh\:mm\:ss'))

## Summary

- **Total Unique Tests**: $totalTests
- **Flaky Tests Detected**: $($flakyTests.Count)
- **Flake Rate**: $($flakeRate.ToString('F2'))%
- **Target Flake Rate**: <0.1%

$(if ($flakeRate -lt 0.1) { "✅ **Status**: Meeting target flake rate" } else { "❌ **Status**: Exceeds target flake rate" })

## Iteration Results

| Iteration | Passed | Failed | Skipped | Exit Code |
|-----------|--------|--------|---------|-----------|
"@

    foreach ($iteration in $iterationResults) {
        $status = if ($iteration.Failed -gt 0) { "❌" } else { "✅" }
        $report += "`n| $($iteration.Iteration) | $($iteration.Passed) | $($iteration.Failed) | $($iteration.Skipped) | $($iteration.ExitCode) $status |"
    }
    
    $report += @"


## Flaky Tests

$(if ($flakyTests.Count -eq 0) {
"No flaky tests detected! All tests passed or failed consistently across all $Iterations iterations.
"
} else {
"The following tests exhibited flaky behavior (passed some iterations, failed others):

| Test Name | Passed | Failed | Skipped | Pass Rate | Fail Rate |
|-----------|--------|--------|---------|-----------|-----------|
"
foreach ($test in ($flakyTests | Sort-Object -Property FailRate -Descending)) {
    "`n| ``$($test.Name)`` | $($test.Passed) | $($test.Failed) | $($test.Skipped) | $($test.PassRate.ToString('F1'))% | $($test.FailRate.ToString('F1'))% |"
}
"

### Root Cause Analysis Required

Each flaky test above requires investigation to identify the root cause:

- **Timing issues**: Race conditions, inadequate waits, async handling
- **External dependencies**: Network calls, file system state, environment variables
- **Test isolation**: Shared state between tests, improper cleanup
- **Platform-specific behavior**: OS-dependent code paths

"
})

## Recommendations

$(if ($flakeRate -lt 0.1) {
"The test suite demonstrates excellent stability with a flake rate below the 0.1% target.

**Maintenance Actions**:
- Continue monitoring flake rate in CI/CD
- Add this measurement to regular test health checks
- Investigate any new flaky tests immediately
"
} else {
"The test suite exceeds the target flake rate and requires attention.

**Immediate Actions**:
1. Prioritize fixing flaky tests in order of fail rate (highest first)
2. Add \`[Retry]\` attributes as temporary mitigation for known flaky tests
3. Investigate root causes systematically
4. Re-run this measurement after fixes to verify improvement

**Long-term Actions**:
- Add flakiness monitoring to CI/CD pipeline
- Establish test isolation best practices
- Review test patterns for timing-dependent code
"
})

## Test Execution Performance

- **Average iteration time**: $($($duration.TotalSeconds / $Iterations).ToString('F2'))s
- **Fastest iteration**: $(($iterationResults | Measure-Object -Property Passed -Minimum).Minimum) tests
- **Slowest iteration**: $(($iterationResults | Measure-Object -Property Passed -Maximum).Maximum) tests
- **Consistency**: $(if (($iterationResults | Select-Object -ExpandProperty Passed | Sort-Object -Unique).Count -eq 1) { "Excellent - same test count every run" } else { "Variable - test count varied between runs" })

## Next Steps

1. Review and fix any identified flaky tests
2. Add test result monitoring to CI/CD
3. Re-measure after fixes to verify <0.1% target achieved
4. Integrate flakiness checks into PR validation
"@

    # Ensure directory exists
    $reportDir = Split-Path -Parent $reportPath
    if (-not (Test-Path $reportDir)) {
        New-Item -ItemType Directory -Path $reportDir -Force | Out-Null
    }
    
    # Write report
    $report | Out-File -FilePath $reportPath -Encoding UTF8 -Force
    
    Write-Host "Report written to: $reportPath" -ForegroundColor Green
    Write-Host ""
    
    if ($flakyTests.Count -gt 0) {
        Write-Host "⚠️  Flaky tests detected: $($flakyTests.Count)" -ForegroundColor Yellow
        Write-Host "   Flake rate: $($flakeRate.ToString('F2'))% (target: <0.1%)" -ForegroundColor Yellow
        exit 1
    } else {
        Write-Host "✅ No flaky tests detected!" -ForegroundColor Green
        Write-Host "   Flake rate: 0.00% (target: <0.1%)" -ForegroundColor Green
        exit 0
    }
}
finally {
    Pop-Location
}
