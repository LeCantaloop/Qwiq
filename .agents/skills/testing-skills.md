# Testing Skills

## Skill-Test-001

**Statement**: WireMock.Net OWIN hosting deadlocks on .NET Framework 4.7.2; use dedicated net8.0+ test project

**Atomicity**: 95%

**Category**: Testing

**Context**: When implementing WireMock-based tests

**Evidence**: Sessions multiple - WireMock moved from Integration.Tests to dedicated Qwiq.WireMock.Tests

**Details**:

- WireMock.Net uses OWIN for HTTP server emulation
- OWIN hosting deadlocks when running on .NET Framework 4.7.2
- Solution: Create separate test project targeting modern .NET
- Allows WireMock tests to run without deadlock issues

**Implementation Pattern**:

```xml
<!-- Qwiq.WireMock.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="WireMock.Net" Version="1.5.40" />
  </ItemGroup>
</Project>
```

**When to Apply**:

- Integrating WireMock into .NET Framework legacy projects
- Need offline HTTP mocking without deadlock risk
- Creating isolated test infrastructure

**Alternative**: Use REST API mocking libraries designed for .NET Framework if WireMock isn't critical

---

## Skill-Test-002

**Statement**: Capture real HTTP traffic with Fiddler system proxy for WireMock stubs; SDK bypasses WireMock Cloud recording

**Atomicity**: 91%

**Category**: Testing

**Context**: When creating realistic API test stubs

**Evidence**: WireMock implementation - HAR capture with Convert-HarToWireMock.ps1

**Details**:

- Azure DevOps SDK connects directly to service, bypassing proxy recording
- WireMock Cloud recording mode doesn't capture SDK requests
- Solution: Use Fiddler as system-level proxy to capture all traffic
- Parse HAR files to extract realistic JSON responses

**Implementation Workflow**:

1. **Configure Fiddler**:
   - Enable system-level proxy capture
   - Capture all HTTPS traffic

2. **Capture Traffic**:
   - Run your application/tests
   - Perform operations that generate HTTP requests
   - Export as HAR (HTTP Archive) format

3. **Parse HAR to WireMock**:

   ```powershell
   .\scripts\Convert-HarToWireMock.ps1 -HarFilePath "captured.har" -OutputPath "stubs.json"
   ```

4. **Validate Stubs**:
   - Verify JSON format is valid
   - Check that response content matches expectations
   - Update test assertions to match captured data

**Benefits**:

- Real API responses instead of mocked/guessed JSON
- Captures correct header formats and response structures
- Ensures test assertions match actual service behavior

---

## Skill-Test-003

**Statement**: IdentityDescriptor must be string format in captured stubs, not object serialization

**Atomicity**: 94%

**Category**: Testing

**Context**: When stubbing Azure DevOps API responses

**Evidence**: WireMock implementation - VssConnection handshake requirements

**Details**:

- Azure DevOps SDK expects IdentityDescriptor as specific string format
- Common mistake: Serializing as JSON object loses required format
- Correct format: `"descriptor": "Microsoft.IdentityModel.Claims.ClaimsIdentity;00020100039B8083@Live.com"`
- VssConnection handshake fails if format is wrong

**Correct Format**:

```json
{
  "descriptor": "Microsoft.IdentityModel.Claims.ClaimsIdentity;00020100039B8083@Live.com",
  "displayName": "Azure DevOps User",
  "url": "https://vssps.dev.azure.com/...",
  "id": "00000000-0000-0000-0000-000000000000",
  "uniqueName": "user@example.com"
}
```

**Wrong Format** ❌:

```json
{
  "descriptor": {
    "identityType": "Microsoft.IdentityModel.Claims.ClaimsIdentity",
    "id": "00020100039B8083@Live.com"
  }
}
```

**When to Apply**:

- Creating WireMock stubs for Azure DevOps API responses
- Debugging VssConnection handshake failures
- Validating captured HAR files before converting to stubs

**Note**: Always capture real traffic with Fiddler rather than manually constructing IdentityDescriptor format
