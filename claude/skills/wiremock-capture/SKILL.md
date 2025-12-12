---
name: wiremock-capture
description: HTTP traffic capture and WireMock stub generation for QWIQ integration tests. Use when creating offline test fixtures, recording API responses, or converting HAR files to WireMock stubs.
---

# WireMock Capture Skill

## Purpose

This skill helps capture HTTP traffic from Azure DevOps API calls and convert them to WireMock stubs for offline testing. This enables integration tests to run without live Azure DevOps access.

## When To Use

- Creating offline test fixtures for integration tests
- Recording API responses for new test scenarios
- Converting Fiddler/Charles HAR exports to WireMock format
- Setting up deterministic test data

## Instructions

### 1. Capture Traffic with Proxy

```powershell
# Start capture (requires proxy running)
./scripts/Capture-WireMockTraffic.ps1

# With Fiddler on default port
./scripts/Capture-WireMockTraffic.ps1 -UseFiddler

# Custom proxy port
./scripts/Capture-WireMockTraffic.ps1 -ProxyPort 8080
```

### 2. Prerequisites for Capture

You need one of:

- **Fiddler**: Enable "Decrypt HTTPS traffic" in Tools > Options > HTTPS
- **Charles Proxy**: Enable SSL proxying
- **WireMock**: Start in proxy mode

The proxy's root certificate must be trusted by your system.

### 3. Convert HAR to WireMock

After capturing traffic, export as HAR and convert:

```powershell
./scripts/Convert-HarToWireMock.ps1 -HarFile "./recordings/capture.har" -OutputFile "./stubs/azure-devops.json"
```

### 4. Key Endpoints Captured

The converter focuses on these Azure DevOps API endpoints:

- `/_apis/connectionData` - Connection info
- `/_apis/resourceAreas` - API discovery
- `/_apis/wit/wiql` - WIQL queries
- `/_apis/wit/workItems` - Work item operations
- `/_apis/wit/workitemsbatch` - Batch operations
- `/_apis/wit/workItemTypes` - Type definitions
- `/_apis/wit/fields` - Field definitions
- `/_apis/projects` - Project info

### 5. Output Location

Captured traffic goes to:

```text
WireMockRecordings/
└── {timestamp}/
    └── stubs.json
```

### 6. Using Stubs in Tests

Place generated stubs in test project and configure WireMock server:

```csharp
var server = WireMockServer.Start();
server.ReadStaticMappings("path/to/stubs");
```

## Helper Scripts

### scripts/Capture-WireMockTraffic.ps1

**Purpose:** Run integration tests while capturing HTTP traffic

**Input:**

- `-ProxyPort`: Proxy port (default: 8888)
- `-OutputPath`: Output directory (default: `./WireMockRecordings`)
- `-UseFiddler`: Use Fiddler default port (8866)

**Output:**

- Timestamped folder with captured traffic
- Sets `HTTP_PROXY` and `HTTPS_PROXY` environment variables

**Prerequisites:**

- Proxy server running (Fiddler, Charles, or WireMock)
- HTTPS decryption enabled
- Proxy certificate trusted

### scripts/Convert-HarToWireMock.ps1

**Purpose:** Convert HAR file to WireMock stub mappings

**Input:**

- `-HarFile`: Path to HAR file (required)
- `-OutputFile`: Output JSON path (default: `./wiremock-stubs.json`)
- `-FilterHost`: Host to filter (default: `qwiq-sandbox.visualstudio.com`)

**Output:**

- JSON file with WireMock stub mappings
- Deduplicates requests by method + path
- Filters to key Azure DevOps endpoints

**Usage:**

```powershell
./scripts/Convert-HarToWireMock.ps1 -HarFile "./capture.har" -OutputFile "./stubs.json"
```

## Examples

### Example 1: Recording New Test Scenario

**User goal:** Create offline stubs for a new query type

**Process:**

1. Start Fiddler with HTTPS decryption
2. Run `./scripts/Capture-WireMockTraffic.ps1 -UseFiddler`
3. Execute the integration test that makes the API calls
4. Stop capture, export HAR from Fiddler
5. Run `./scripts/Convert-HarToWireMock.ps1 -HarFile ./export.har`
6. Review and commit generated stubs

**Expected output:** WireMock stubs for the new scenario

### Example 2: Updating Existing Stubs

**User goal:** Update stubs after API response format changed

**Process:**

1. Delete old stubs
2. Re-capture traffic with current API
3. Convert to WireMock format
4. Run tests against new stubs to verify
5. Commit updated stubs

**Expected output:** Tests pass with updated stubs

## Troubleshooting

### No traffic captured

**Causes:**

1. Proxy not running or wrong port
2. HTTPS decryption not enabled
3. Proxy certificate not trusted

**Fix:** Verify proxy is running, enable HTTPS decryption, trust root certificate.

### HAR file empty or missing entries

**Cause:** Filter host doesn't match actual requests

**Fix:** Check `-FilterHost` parameter matches your Azure DevOps URL.

### WireMock stubs don't match requests

**Cause:** Request URLs or headers differ from recording

**Fix:** Check WireMock logs for match failures, adjust stub patterns.

## Related Resources

- See [../sandbox-validation/SKILL.md](../sandbox-validation/SKILL.md) for sandbox environment setup
- See [../qwiq-testing/SKILL.md](../qwiq-testing/SKILL.md) for integration test patterns
- See [../qwiq-cicd/SKILL.md](../qwiq-cicd/SKILL.md) for running tests in CI
