# ADR-010: SOAP Client Deprecation Strategy

**Status**: Proposed  
**Date**: 2025-12-12  
**Deciders**: Architecture Team  
**Context**: Wave 4 - Test Quality & Coverage Excellence (W4.4)

## Context and Problem Statement

The Qwiq library provides two client implementations for accessing Azure DevOps/TFS work items:

1. **REST client** (`Qwiq.Client.Rest`) - Modern HTTP/JSON API
2. **SOAP client** (`Qwiq.Client.Soap`) - Legacy SOAP/XML API

As part of Wave 4 test quality improvements and the transition to production v11.0.0 for 100+ team members, we need to determine the long-term strategy for the SOAP client.

**Key Questions**:

- Should the SOAP client be maintained, deprecated, or enhanced?
- What is the migration path for SOAP users?
- How does this decision impact test coverage targets and maintenance burden?

## Decision Drivers

### Technical Factors

1. **Platform Constraints**:

   - SOAP client requires `net472` target framework (Windows SDK dependency)
   - Cannot be used in Kubernetes containers (Linux-only deployment target)
   - Depends on `Microsoft.TeamFoundationServer.ExtendedClient` which is Windows-only

2. **Code Complexity**:

   - **47 source files**, **~2,296 lines of code** across `Qwiq.Client.Soap` and `Qwiq.Identity.Soap`
   - Parallel implementation of all core interfaces (IWorkItemStore, IWorkItem, etc.)
   - Mapper implementations for SOAP-to-Core type conversions

3. **Test Coverage**:

   - **0% automated test coverage** for SOAP client (integration tests require TFS instance)
   - Integration tests exist but are excluded from CI (require Windows + credentials)
   - Testing SOAP requires on-premises TFS or Azure DevOps Server with Windows auth

4. **Azure DevOps API Evolution**:
   - Microsoft prioritizes REST API development
   - SOAP API is legacy/maintenance mode (no new features since ~2015)
   - Azure DevOps Services (cloud) supports both, but REST is recommended

### Business Factors

1. **User Base**:

   - Unknown number of SOAP-only users
   - Target deployment: Kubernetes containers (REST only)
   - 100+ team members planned for production (container-based)

2. **Maintenance Burden**:

   - Parallel codebase increases complexity
   - Every feature/fix requires dual implementation
   - Test coverage gap creates risk

3. **Migration Path**:
   - REST API has feature parity with SOAP for work item operations
   - Authentication differs but is manageable (PAT vs Windows)

## Considered Options

### Option 1: Deprecate SOAP Client (Recommended)

**Status**: Deprecate in v11.0.0, remove in v12.0.0

**Rationale**:

- Kubernetes deployment requires REST client
- Microsoft recommends REST API for new development
- Reduces maintenance burden by 2,296 LOC
- Allows focusing test coverage efforts on REST client (currently 0%)

**Migration Path**:

1. **v11.0.0** (Current release):

   - Mark SOAP packages as deprecated (add `<deprecated>` to NuGet metadata)
   - Add obsolete warnings to SOAP public APIs
   - Document migration guide (SOAP → REST)
   - Continue shipping SOAP packages for backward compatibility

2. **v11.x** (6-month window):

   - Monitor NuGet download stats for SOAP packages
   - Provide migration support
   - No new features for SOAP

3. **v12.0.0** (Breaking change release):
   - Remove SOAP packages entirely
   - Remove `Qwiq.Client.Soap` and `Qwiq.Identity.Soap` projects
   - Clean up parallel implementations

**Pros**:

- ✅ Aligns with container deployment strategy
- ✅ Reduces maintenance burden significantly
- ✅ Focuses testing efforts on actively developed code path
- ✅ Follows Microsoft's recommended API direction

**Cons**:

- ❌ Breaking change for SOAP-only users (mitigated by migration window)
- ❌ Requires user action to migrate

### Option 2: Maintain SOAP Client Indefinitely

**Status**: Not recommended

**Rationale**: Keep SOAP client for backward compatibility

**Pros**:

- ✅ No breaking changes for existing users
- ✅ Supports on-premises TFS scenarios

**Cons**:

- ❌ Cannot be deployed in Kubernetes (primary deployment target)
- ❌ Perpetual dual implementation burden
- ❌ Test coverage remains at 0% (integration tests impractical in CI)
- ❌ Diverges from Microsoft's API strategy

### Option 3: Enhance SOAP Client (Test Coverage)

**Status**: Not recommended

**Rationale**: Invest in SOAP test coverage and feature parity

**Pros**:

- ✅ Improves quality metrics
- ✅ No user migration required

**Cons**:

- ❌ High investment for legacy technology
- ❌ Testing requires Windows + TFS infrastructure
- ❌ Still cannot deploy in Kubernetes
- ❌ Contradicts "production v11.0.0" goal (containers)

## Decision

Adopt Option 1: Deprecate SOAP Client.

### Implementation Plan

#### Phase 1: v11.0.0 Deprecation (Current Release)

1. **NuGet Metadata**:

   ```xml
   <PropertyGroup>
     <PackageDeprecated>true</PackageDeprecated>
     <PackageDeprecationMessage>
       The SOAP client is deprecated and will be removed in v12.0.0.
       Migrate to Qwiq.Client.Rest for continued support and container deployment compatibility.
       See migration guide: https://github.com/rjmurillo/Qwiq/blob/master/docs/SOAP-TO-REST-MIGRATION.md
     </PackageDeprecationMessage>
   </PropertyGroup>
   ```

2. **Code Annotations**:

   - Add `[Obsolete]` to all public SOAP APIs with migration guidance
   - Update XML docs with deprecation notices

3. **Documentation**:
   - Create `docs/SOAP-TO-REST-MIGRATION.md` migration guide
   - Update `README.md` to recommend REST client
   - Add deprecation notice to package README files

#### Phase 2: Monitor & Support (v11.x - 6 months)

1. **Metrics**:

   - Track `Qwiq.Client.Soap` NuGet downloads
   - Monitor GitHub issues for SOAP migration questions
   - Survey known users about migration timeline

2. **Support**:
   - Answer migration questions promptly
   - Provide code samples for common scenarios
   - No new features, only critical bug fixes

#### Phase 3: Removal (v12.0.0)

1. **Code Removal**:

   - Delete `src/Qwiq.Client.Soap` project
   - Delete `src/Qwiq.Identity.Soap` project
   - Remove SOAP-related tests
   - Clean up shared infrastructure no longer needed

2. **Documentation**:
   - Archive SOAP documentation
   - Update v11→v12 migration guide with SOAP removal details

### Migration Guide (Summary)

| SOAP                                    | REST                                      | Notes                                |
| --------------------------------------- | ----------------------------------------- | ------------------------------------ |
| `Qwiq.Client.Soap.WorkItemStoreFactory` | `Qwiq.Client.Rest.WorkItemStoreFactory`   | Factory pattern unchanged            |
| Windows Authentication                  | Personal Access Token (PAT)               | Create PAT in Azure DevOps           |
| `AuthenticationTypes.Windows`           | `AuthenticationTypes.PersonalAccessToken` | Update AuthenticationOptions         |
| TFS URL                                 | Azure DevOps URL                          | REST supports both on-prem and cloud |

**Example Migration**:

```csharp
// BEFORE (SOAP)
var options = new AuthenticationOptions(
    new Uri("https://tfs.contoso.com/DefaultCollection"),
    AuthenticationTypes.Windows,
    credentialsFactory
);
var store = Qwiq.Client.Soap.WorkItemStoreFactory.Default.Create(options);

// AFTER (REST)
var options = new AuthenticationOptions(
    new Uri("https://dev.azure.com/contoso"),  // Or on-prem URL
    AuthenticationTypes.PersonalAccessToken,
    () => new NetworkCredential("", pat)
);
var store = Qwiq.Client.Rest.WorkItemStoreFactory.Default.Create(options);
```

## Consequences

### Positive

1. **Reduced Complexity**: ~2,300 LOC removed from maintenance burden
2. **Focused Testing**: Test coverage efforts focus on actively used code path (REST)
3. **Container Compatibility**: Full alignment with Kubernetes deployment strategy
4. **API Modernization**: Users migrate to Microsoft-recommended API
5. **Clear Direction**: Eliminates ambiguity about which client to use

### Negative

1. **User Migration**: SOAP users must update code (mitigated by 6-month window + guide)
2. **Backward Compatibility**: Breaking change in v12.0.0 (expected for major version)
3. **On-Premises TFS**: Users must configure PATs instead of Windows auth

### Neutral

1. **TFS Support**: REST API supports on-premises TFS (Server 2017+)
2. **Feature Parity**: REST has equivalent functionality for work item operations

## Validation

### Success Criteria

- [ ] SOAP deprecation notices published in v11.0.0
- [ ] Migration guide created and reviewed
- [ ] Zero unresolved SOAP migration issues after 6 months
- [ ] SOAP packages removed in v12.0.0
- [ ] Test coverage focused on REST client (target: 70%)

### Monitoring

- NuGet download metrics for `Qwiq.Client.Soap` (track deprecation adoption)
- GitHub issues tagged with `soap-migration`
- User survey results (if available)

## Related Decisions

- **ADR-007**: REST Client Testability - Prioritize REST for testing investment
- **ADR-008**: WireMock Offline REST Testing - Enables offline REST testing (not feasible for SOAP)
- **W3.1**: TFM Expansion - Expanded .NET support excludes SOAP (net472 only)
- **W4.5**: Test Improvement Plan - Focus on REST coverage (0% → 60%+)

## References

- [Microsoft REST API Documentation](https://learn.microsoft.com/en-us/rest/api/azure/devops/)
- [Azure DevOps SOAP API (Legacy)](https://learn.microsoft.com/en-us/previous-versions/azure/devops/integrate/overview)
- [Qwiq GitHub Issues - SOAP Client](https://github.com/rjmurillo/Qwiq/labels/soap-client)
- [Wave 4 Test Quality Plan](../../.agents/WAVE4-TEST-IMPROVEMENT-PLAN.md)
