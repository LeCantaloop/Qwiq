# WireMock Stub Mappings

This directory contains Azure DevOps REST API stub mappings for offline testing.

## Current Status: Ready for Testing

The stubs in this folder were captured from real Azure DevOps traffic using Fiddler and contain the exact JSON format required by the VssConnection SDK.

## Stub Files

- `azure-devops-stubs.json` - Main stub mappings file with real Azure DevOps responses
- `azure-devops-stubs-extracted.json` - Backup of extracted stubs (same content)

## Captured Endpoints

The following endpoints are captured and mocked:

| Endpoint                       | Method | Description                                     |
| ------------------------------ | ------ | ----------------------------------------------- |
| `/_apis/connectionData`        | GET    | VssConnection handshake with authenticated user |
| `/_apis/projects`              | GET    | Project information                             |
| `/_apis/wit/wiql`              | POST   | WIQL query execution                            |
| `/_apis/wit/workItems`         | GET    | Work item retrieval                             |
| `/WIT/_apis/wit/workItemTypes` | GET    | Work item type definitions                      |

## Using the Stubs

The stubs can be loaded into WireMock using the `LoadStubsFromFile` extension method:

```csharp
var server = WireMockServer.Start();
server.LoadStubsFromFile(AzureDevOpsWireMockExtensions.GetDefaultStubsFilePath());
```

Or specify a custom path:

```csharp
server.LoadStubsFromFile(@"path\to\azure-devops-stubs.json");
```

## Capturing New Traffic

If you need to capture new traffic (e.g., for additional endpoints):

### Step 1: Capture with Fiddler

1. **Install and configure Fiddler**:

   - Download from https://www.telerik.com/fiddler
   - Enable HTTPS decryption: Tools > Options > HTTPS > Decrypt HTTPS traffic
   - Trust the Fiddler root certificate

2. **Run the REST integration tests**:

   ```powershell
   dotnet test test\Qwiq.Integration.Tests --filter "TestCategory=REST"
   ```

3. **Export as HAR**:
   - In Fiddler: File > Export Sessions > All Sessions > HTTPArchive v1.2
   - Save to `.agents/qwiq.har`

### Step 2: Convert HAR to WireMock

Run the conversion script:

```powershell
.\scripts\Convert-HarToWireMock.ps1 `
    -HarFile ".agents\qwiq.har" `
    -OutputFile "test\Qwiq.Integration.Tests\WireMock\Stubs\azure-devops-stubs.json"
```

The script will:

- Parse the HAR file
- Filter for relevant Azure DevOps API endpoints
- Deduplicate identical requests
- Convert to WireMock stub format
- Save to the output file

## Known Issues

### IdentityDescriptor Format

The `IdentityDescriptor` type requires a specific JSON format. The SDK's custom JSON converter expects:

```json
{
  "descriptor": "Microsoft.IdentityModel.Claims.ClaimsIdentity;00020100039B8083@Live.com"
}
```

Not the object format:

```json
{
  "descriptor": {
    "identityType": "Microsoft.IdentityModel.Claims.ClaimsIdentity",
    "identifier": "..."
  }
}
```

The captured stubs contain the correct string format.

### User Identity

The stubs contain real user identity information (Richard Murillo) from the qwiq-sandbox environment. This is expected and required for the SDK to function correctly.

## WireMock Cloud

Stubs are also available in WireMock Cloud:

- Mock API ID: `4d822`
- Name: `qwiq-sandbox.visualstudio.com`
