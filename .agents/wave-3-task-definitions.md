# Wave 3: Qwiq Modernization Task Definitions

**Status Legend:**
- 📋 FUTURE - Not yet started, planned for Wave 3
- 📋 DEFERRED - Moved from Wave 2 to Wave 3
- ⏸️ BLOCKED - Cannot proceed until dependencies resolved

---

## W3.1: .NET 10 SDK Upgrade

**Status:** 📋 FUTURE
**Effort:** S (Small)
**Priority:** P1 (High)
**Dependencies:** .NET 10 GA release (November 2025)

### Description
Upgrade the repository to target .NET 10 SDK when available.

### Implementation Notes
- Update `global.json` to pin .NET 10.0.100+ SDK
- Update CI/CD workflows to use .NET 10
- Test compatibility with existing multi-targeting (net472, netstandard2.0, net8.0)
- No breaking changes expected for existing TFMs

### Acceptance Criteria
- [ ] `global.json` specifies .NET 10 SDK version
- [ ] CI builds successfully on .NET 10 SDK
- [ ] All existing target frameworks build and test pass
- [ ] No degradation in build performance

### Files to Modify
- `global.json`
- `.github/workflows/main.yml`

---

## W3.1a: Add net10.0 Target Framework Moniker

**Status:** 📋 FUTURE
**Effort:** M (Medium)
**Priority:** P2 (Medium)
**Dependencies:** W3.1 (SDK upgrade), .NET 10 GA

### Description
Add `net10.0` as a target framework to all projects currently targeting `net8.0`.

### Implementation Notes
- Update multi-targeting strings: `net472;netstandard2.0;net8.0;net10.0`
- Verify no API breakages or platform-specific issues
- Update package metadata to reflect net10.0 support
- Test projects: Add net10.0 alongside net472;net8.0
- Consider: net10.0-only features if applicable

### Acceptance Criteria
- [ ] All `net8.0` projects also target `net10.0`
- [ ] NuGet packages include `net10.0` binaries
- [ ] Tests pass on both net8.0 and net10.0
- [ ] No net10.0-specific runtime issues
- [ ] Package readme updated to list net10.0 support

### Files to Modify
- `src/Qwiq.Core/Qwiq.Core.csproj`
- `src/Qwiq.Core.Rest/Qwiq.Core.Rest.csproj`
- `src/Qwiq.Linq/Qwiq.Linq.csproj`
- `src/Qwiq.Mapper/Qwiq.Mapper.csproj`
- `src/Qwiq.Identity/Qwiq.Identity.csproj`
- `test/*/test.csproj` (all test projects)
- `docs/package-readme/*.md` (update supported frameworks)

---

## W3.3: ARM64 Testing Infrastructure

**Status:** 📋 FUTURE
**Effort:** M (Medium)
**Priority:** P3 (Low)
**Dependencies:** None

### Description
Add ARM64 testing to CI/CD to ensure compatibility with ARM-based systems (Apple Silicon, Azure ARM VMs).

### Implementation Notes
- GitHub Actions supports `macos-14` (M1) and `windows-arm64` runners
- Add ARM64 test jobs in CI workflow (parallel to x64 jobs)
- May need to exclude net472 on macOS-arm64 (Mono compatibility check)
- REST client should work fine on ARM64
- SOAP client (net472) is Windows-only, test on windows-arm64 if available

### Acceptance Criteria
- [ ] CI includes ARM64 test jobs for macOS (net8.0/net10.0)
- [ ] CI includes ARM64 test jobs for Windows (all TFMs) if runners available
- [ ] All tests pass on ARM64 architecture
- [ ] No architecture-specific runtime issues
- [ ] Documentation notes ARM64 support status

### Files to Modify
- `.github/workflows/main.yml` (add ARM64 matrix)
- `README.md` (note ARM64 support)

---

## W3.4: SOAP Client Deprecation Plan

**Status:** 📋 FUTURE
**Effort:** S (Small)
**Priority:** P2 (Medium)
**Dependencies:** None

### Description
Create a formal deprecation timeline and strategy for the SOAP client (`Qwiq.Core.Soap`, `Qwiq.Identity.Soap`).

### Implementation Notes
- SOAP client only supports net472 (Windows-only)
- REST client is feature-complete and modern
- Azure DevOps Server 2019+ supports REST API
- Deprecation timeline suggestion:
  - Wave 3: Mark `[Obsolete]` with guidance
  - Wave 4: Remove from default build (opt-in only)
  - Wave 5: Complete removal
- Document migration path in W3.6

### Acceptance Criteria
- [ ] Deprecation document created in `docs/deprecation/soap-client.md`
- [ ] Timeline with clear milestones (Obsolete → Optional → Removed)
- [ ] Communication plan (release notes, README, migration guide)
- [ ] `[Obsolete]` attributes added to SOAP entry points
- [ ] Package release notes include deprecation notice

### Files to Create
- `docs/deprecation/soap-client.md`

### Files to Modify
- `src/Qwiq.Core.Soap/WorkItemStoreFactory.cs` (add `[Obsolete]`)
- `src/Qwiq.Identity.Soap/IdentityManagementService.cs` (add `[Obsolete]`)
- `README.md` (add deprecation notice)

---

## W3.5: API Compatibility Policy Document

**Status:** 📋 FUTURE
**Effort:** S (Small)
**Priority:** P2 (Medium)
**Dependencies:** None

### Description
Document the API compatibility and versioning policy for Qwiq.

### Implementation Notes
- Define semantic versioning commitment (breaking changes → major bump)
- Specify supported .NET versions and deprecation timeline
- Document experimental vs. stable API surface
- Clarify backward compatibility guarantees
- Reference NuGet package versioning strategy
- Include guidance on consuming preview releases

### Acceptance Criteria
- [ ] Policy document created: `docs/api-compatibility-policy.md`
- [ ] Covers semantic versioning commitment
- [ ] Defines "breaking change" for Qwiq context
- [ ] Lists supported .NET versions and support lifecycle
- [ ] Specifies deprecation process (`[Obsolete]` → removal timeline)
- [ ] Linked from main README

### Files to Create
- `docs/api-compatibility-policy.md`

### Files to Modify
- `README.md` (link to policy)

---

## W3.6: SOAP → REST Migration Guide

**Status:** 📋 FUTURE
**Effort:** M (Medium)
**Priority:** P2 (Medium)
**Dependencies:** W3.4 (Deprecation Plan)

### Description
Create comprehensive migration guide for developers moving from SOAP to REST client.

### Implementation Notes
- Side-by-side code examples (SOAP vs. REST)
- Authentication differences (credentials, PAT, OAuth)
- API behavior differences (if any)
- Performance characteristics comparison
- Known limitations or edge cases
- Troubleshooting common migration issues
- Include identity service migration

### Acceptance Criteria
- [ ] Migration guide created: `docs/migration/soap-to-rest.md`
- [ ] Code examples for common scenarios (connection, query, identity)
- [ ] Authentication migration guidance
- [ ] API compatibility matrix (if differences exist)
- [ ] Troubleshooting section
- [ ] Linked from README and deprecation notice

### Files to Create
- `docs/migration/soap-to-rest.md`

### Files to Modify
- `README.md` (link to migration guide)
- `docs/deprecation/soap-client.md` (reference guide)

---

## W3.7: Performance Baseline Establishment

**Status:** 📋 FUTURE
**Effort:** M (Medium)
**Priority:** P3 (Low)
**Dependencies:** None

### Description
Establish performance baseline benchmarks for key operations using BenchmarkDotNet.

### Implementation Notes
- Expand existing `Qwiq.Benchmark` project
- Key scenarios:
  - Work item query (WIQL execution)
  - Work item retrieval (single, batch)
  - LINQ query translation
  - Identity resolution (single, bulk)
  - Object mapping performance
- Run benchmarks on standard hardware (CI or dedicated machine)
- Store baseline results in repository (`docs/benchmarks/baseline-*.md`)
- Compare SOAP vs. REST performance

### Acceptance Criteria
- [ ] Benchmarks added to `Qwiq.Benchmark` for all key scenarios
- [ ] Baseline results documented in `docs/benchmarks/`
- [ ] Performance regression detection strategy defined
- [ ] CI job runs benchmarks (optional: on-demand or scheduled)
- [ ] Baseline includes SOAP vs. REST comparison

### Files to Modify
- `test/Qwiq.Benchmark/` (add benchmark classes)
- `docs/benchmarks/baseline-v2.x.md` (new file)

---

## W3.8: Observability Overhaul (Deferred from W2.1 + W2.9)

**Status:** 📋 DEFERRED
**Effort:** L (Large)
**Priority:** P2 (Medium)
**Dependencies:** None (but should complete Wave 2 first)

### Description
Modernize logging and diagnostics infrastructure. Replace `System.Diagnostics.Trace` with `Microsoft.Extensions.Logging.ILogger<T>` and add OpenTelemetry support for distributed tracing.

### Implementation Notes
- **Phase 1: ILogger Migration**
  - Replace all `Trace.TraceError/Warning/Information` calls with `ILogger<T>`
  - Add `Microsoft.Extensions.Logging.Abstractions` package
  - Create `QwiqLoggerExtensions` for common log patterns
  - Maintain backward compatibility: provide default NullLogger if not injected

- **Phase 2: OpenTelemetry**
  - Add `System.Diagnostics.DiagnosticSource` package
  - Create `QwiqDiagnostics` static class with `ActivitySource`
  - Instrument key operations (queries, HTTP calls, identity resolution)
  - Correlation ID propagation across calls

- **Phase 3: Structured Logging**
  - Use strongly-typed log messages with `LoggerMessage.Define`
  - Add context properties (work item IDs, query text, correlation IDs)
  - Ensure no sensitive data (tokens, passwords) in logs

- **Requires Separate PRD**: This is a significant architectural change requiring:
  - Dependency injection strategy (optional ILogger parameters)
  - Breaking change analysis (constructor signatures)
  - Migration guide for consumers
  - Performance impact assessment

### Acceptance Criteria
- [ ] All `System.Diagnostics.Trace` calls replaced with `ILogger<T>`
- [ ] `QwiqDiagnostics.ActivitySource` created and instrumented
- [ ] OpenTelemetry traces emitted for HTTP requests, queries, identity calls
- [ ] Correlation IDs propagate through call chains
- [ ] Backward compatibility maintained (optional ILogger injection)
- [ ] Performance impact < 5% overhead
- [ ] Documentation: observability guide created
- [ ] Tests verify log output and Activity creation

### Files to Create
- `src/Qwiq.Core/Diagnostics/QwiqDiagnostics.cs`
- `src/Qwiq.Core/Diagnostics/QwiqLoggerExtensions.cs`
- `docs/observability.md`

### Files to Modify
- `Directory.Packages.props` (add Microsoft.Extensions.Logging.Abstractions, System.Diagnostics.DiagnosticSource)
- `src/Qwiq.Core/Qwiq.Core.csproj`
- `src/Qwiq.Core.Rest/` (instrument HTTP calls)
- `src/Qwiq.Core.Soap/` (instrument SOAP calls)
- `src/Qwiq.Linq/` (instrument query translation)
- `src/Qwiq.Identity/` (instrument identity resolution)
- All files currently using `Trace.*` methods

### Breaking Changes
- Constructor signatures may change if ILogger is required (mitigate with optional parameters)
- Consumers may need to register ILogger in their DI container (provide guidance)

### References
- OpenTelemetry .NET: https://opentelemetry.io/docs/languages/net/
- ILogger best practices: https://learn.microsoft.com/en-us/dotnet/core/extensions/logging

---

## W3.9: IConfiguration Support (Deferred from W2.8)

**Status:** 📋 DEFERRED
**Effort:** M (Medium)
**Priority:** P3 (Low - Nice to Have)
**Dependencies:** None

### Description
Enable Qwiq to read connection options and credentials from configuration providers (`appsettings.json`, environment variables, Key Vault, etc.).

### Implementation Notes
- Add `Microsoft.Extensions.Configuration.Abstractions` package
- Create `QwiqOptions` class for DI registration:
  ```csharp
  public class QwiqOptions
  {
      public string? BaseUrl { get; set; }
      public string? AuthenticationType { get; set; }
      public string? PersonalAccessToken { get; set; }
      // ... other options
  }
  ```
- Add extension methods for service registration:
  ```csharp
  services.AddQwiq(Configuration.GetSection("Qwiq"));
  ```
- Support multiple named configurations (e.g., dev, prod)
- Integration with existing `AuthenticationOptions` class
- Ensure secrets (PATs, passwords) are not logged

### Acceptance Criteria
- [ ] `QwiqOptions` class created with configuration properties
- [ ] `IServiceCollection` extension methods for DI registration
- [ ] Configuration binding works from `appsettings.json`
- [ ] Environment variables override file-based configuration
- [ ] Secrets properly masked in logs and error messages
- [ ] Sample configurations documented
- [ ] Tests verify configuration binding

### Files to Create
- `src/Qwiq.Core/Configuration/QwiqOptions.cs`
- `src/Qwiq.Core/Configuration/QwiqServiceCollectionExtensions.cs`
- `docs/configuration.md`

### Files to Modify
- `Directory.Packages.props` (add Microsoft.Extensions.Configuration.Abstractions, Microsoft.Extensions.Options.ConfigurationExtensions)
- `src/Qwiq.Core/Qwiq.Core.csproj`
- `README.md` (add configuration example)

### Sample Configuration
```json
{
  "Qwiq": {
    "BaseUrl": "https://dev.azure.com/myorg",
    "AuthenticationType": "PersonalAccessToken",
    "PersonalAccessToken": "***" // From environment variable or Key Vault
  }
}
```

---

## W3.10: NuGet Package Signing (Deferred from W2.12)

**Status:** 📋 DEFERRED
**Effort:** M (Medium)
**Priority:** P3 (Low)
**Dependencies:** Azure Key Vault, Code Signing Certificate
**Blocked By:** Interactive setup required (Azure subscription, certificate purchase)

### Description
Sign NuGet packages with a code signing certificate to provide authenticity and integrity verification.

### Implementation Notes
- **Prerequisites (BLOCKED until completed):**
  - Azure subscription with Key Vault access
  - Purchase EV code signing certificate (Sectigo, DigiCert, etc.)
  - Import certificate to Azure Key Vault
  - Configure GitHub secrets for Key Vault access

- **Implementation:**
  - Use NuGet Sign tool or Azure SignTool
  - Integrate signing into CI/CD pipeline (pack → sign → push)
  - Sign packages before publishing to NuGet.org
  - Timestamp signatures for long-term validity

- **CI/CD Integration:**
  ```yaml
  - name: Sign NuGet packages
    run: |
      dotnet tool install --global AzureSignTool
      AzureSignTool sign --azure-key-vault-url ${{ secrets.AZURE_KEY_VAULT_URL }} \
        --azure-key-vault-certificate ${{ secrets.AZURE_KEY_VAULT_CERT_NAME }} \
        --azure-key-vault-client-id ${{ secrets.AZURE_CLIENT_ID }} \
        --azure-key-vault-client-secret ${{ secrets.AZURE_CLIENT_SECRET }} \
        ./artifacts/**/*.nupkg
  ```

### Acceptance Criteria
- [ ] Code signing certificate obtained and imported to Azure Key Vault
- [ ] GitHub secrets configured for Key Vault access
- [ ] CI/CD pipeline signs packages before publishing
- [ ] Published packages show "Signed" badge on NuGet.org
- [ ] Signature verification passes: `nuget verify -Signatures package.nupkg`
- [ ] Documentation updated with signing information

### Files to Modify
- `.github/workflows/main.yml` (add signing step)
- `README.md` (note that packages are signed)

### Prerequisites Checklist
- [ ] Azure subscription created
- [ ] Code signing certificate purchased (cost: ~$300-500/year)
- [ ] Certificate imported to Azure Key Vault
- [ ] Azure AD app registration for GitHub authentication
- [ ] GitHub repository secrets configured:
  - `AZURE_KEY_VAULT_URL`
  - `AZURE_KEY_VAULT_CERT_NAME`
  - `AZURE_CLIENT_ID`
  - `AZURE_CLIENT_SECRET` (or use OIDC federation)

### References
- NuGet Package Signing: https://learn.microsoft.com/en-us/nuget/create-packages/sign-a-package
- AzureSignTool: https://github.com/vcsjones/AzureSignTool

---

## Summary

### Task Priority Matrix

| Priority | Tasks | Effort |
|----------|-------|--------|
| **P1 (High)** | W3.1 | S |
| **P2 (Medium)** | W3.1a, W3.4, W3.5, W3.6, W3.8 | M-L |
| **P3 (Low)** | W3.3, W3.7, W3.9, W3.10 | M |

### Dependency Graph

```
W3.1 (.NET 10 SDK) → W3.1a (net10.0 TFM)
W3.4 (SOAP Deprecation) → W3.6 (Migration Guide)
Wave 2 Completion → W3.8 (Observability)
Azure Setup → W3.10 (Package Signing)
```

### Effort Breakdown

- **Small (S):** W3.1, W3.4, W3.5 — ~1-2 days each
- **Medium (M):** W3.1a, W3.3, W3.6, W3.7, W3.9, W3.10 — ~3-5 days each
- **Large (L):** W3.8 — ~10-15 days (requires PRD)

### Blocked Items

- **W3.10:** Requires Azure subscription and certificate purchase (interactive, non-development work)

### Recommended Sequencing

1. **Phase 1 (Q1 2026):** W3.1, W3.1a, W3.4, W3.5 — SDK upgrade and deprecation notices
2. **Phase 2 (Q2 2026):** W3.6, W3.7, W3.8 — Migration guides and observability overhaul
3. **Phase 3 (Q3 2026):** W3.3, W3.9 — Nice-to-haves (ARM64, configuration)
4. **Phase 4 (Q4 2026):** W3.10 — Package signing (if prerequisites met)

---

**Document Version:** 1.0
**Last Updated:** December 5, 2025
**Owner:** @rjmurillo
