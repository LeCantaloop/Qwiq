# ADR-003: REST vs SOAP Client Strategy

- **Status**: Accepted
- **Date**: 2025-12-06
- **Decision Makers**: Qwiq Development Team
- **Supersedes**: None
- **Superseded by**: None

## Context

Azure DevOps and Team Foundation Server expose work item tracking through two primary APIs:

1. **REST API** - Modern, cross-platform, HTTP/JSON-based
2. **SOAP API** - Legacy, Windows-only, via TFS Client Object Model

Both APIs have different characteristics, capabilities, and constraints. Qwiq must support customers on various platforms and TFS/Azure DevOps versions.

### Problem Statement

How should Qwiq support both REST and SOAP APIs while:

1. Providing a unified API surface to consumers
2. Allowing consumers to choose the appropriate backend
3. Maintaining feature parity where possible
4. Avoiding code duplication
5. Supporting gradual migration from SOAP to REST

### Forces

- **Platform Support**: REST works everywhere, SOAP requires Windows + .NET Framework 4.7.2
- **Feature Parity**: Some features only exist in one API (e.g., SOAP has richer query capabilities)
- **Performance**: REST is generally faster for simple queries, SOAP better for complex operations
- **Azure DevOps Server Versions**: Older on-premises TFS only supports SOAP
- **Deprecation**: Microsoft is moving away from SOAP, but many customers still use it
- **Developer Experience**: Users want one consistent API regardless of backend

## Decision

We will provide **two separate client implementations** (REST and SOAP) that both implement the same core interfaces, allowing consumers to choose the backend at runtime via factory selection.

### Architecture

```text
┌─────────────────────────────────────────┐
│           Consumer Application           │
└───────────┬─────────────────────────────┘
            │ depends on
            v
┌─────────────────────────────────────────┐
│         Qwiq.Core (Interfaces)          │
│  IWorkItemStore, IWorkItem, ILink, etc. │
└───────────┬─────────────────────────────┘
            │ implemented by
    ┌───────┴────────┐
    v                v
┌──────────┐   ┌──────────┐
│   REST   │   │   SOAP   │
│  Client  │   │  Client  │
└──────────┘   └──────────┘
```

### Implementation

#### REST Client (`Qwiq.Client.Rest`)

```csharp
// Package: Qwiq.Client.Rest
// Target Frameworks: net472, netstandard2.0, net8.0
// Platform: Cross-platform (Windows, Linux, macOS)

namespace Qwiq.Client.Rest
{
    public sealed class WorkItemStoreFactory : IWorkItemStoreFactory
    {
        public static readonly WorkItemStoreFactory Default = new();

        public IWorkItemStore Create(AuthenticationOptions options)
        {
            // REST implementation using HttpClient
        }
    }
}
```

#### SOAP Client (`Qwiq.Client.Soap`)

```csharp
// Package: Qwiq.Client.Soap
// Target Framework: net472 only
// Platform: Windows only (TFS Client OM dependency)

namespace Qwiq.Client.Soap
{
    public sealed class WorkItemStoreFactory : IWorkItemStoreFactory
    {
        public static readonly WorkItemStoreFactory Default = new();

        public IWorkItemStore Create(AuthenticationOptions options)
        {
            // SOAP implementation using TFS Client OM
        }
    }
}
```

### Usage Pattern

```csharp
// Consumers choose backend via factory
var options = new AuthenticationOptions(
    new Uri("https://dev.azure.com/org"),
    AuthenticationTypes.PersonalAccessToken,
    credentialsFactory
);

// Modern Azure DevOps Services / Server - use REST
IWorkItemStore store = Qwiq.Client.Rest.WorkItemStoreFactory.Default.Create(options);

// Legacy TFS on-premises - use SOAP
IWorkItemStore store = Qwiq.Client.Soap.WorkItemStoreFactory.Default.Create(options);

// Both expose identical IWorkItemStore interface
var items = store.Query("SELECT [System.Id] FROM WorkItems");
```

### Client Comparison

| Feature                       | REST Client                    | SOAP Client                |
| ----------------------------- | ------------------------------ | -------------------------- |
| **Platform**                  | Cross-platform                 | Windows only               |
| **Target Frameworks**         | net472, netstandard2.0, net8.0 | net472 only                |
| **Azure DevOps Services**     | ✅ Full support                | ✅ Full support            |
| **Azure DevOps Server 2019+** | ✅ Full support                | ✅ Full support            |
| **TFS 2018 and older**        | ⚠️ Limited                     | ✅ Full support            |
| **Authentication**            | PAT, OAuth, Basic              | Windows, PAT, Basic        |
| **Query Performance**         | Fast for simple queries        | Better for complex queries |
| **Work Item Linking**         | ✅ Supported                   | ✅ Supported               |
| **Field Updates**             | ✅ Supported                   | ✅ Supported               |
| **Batch Operations**          | ✅ Efficient                   | ⚠️ Less efficient          |

## Consequences

### Positive

1. **Choice**: Consumers can choose the right backend for their scenario
2. **Gradual Migration**: SOAP users can migrate to REST at their own pace
3. **Platform Flexibility**: REST enables Linux/macOS support
4. **Future-Proof**: REST is Microsoft's strategic direction
5. **Backward Compatibility**: SOAP supports legacy TFS customers
6. **Performance Optimization**: Use SOAP for complex queries, REST for simple operations

### Negative

1. **Maintenance Burden**: Two implementations to maintain
2. **Feature Parity Challenges**: Not all features available in both APIs
3. **Testing Overhead**: Must test both implementations
4. **Documentation Complexity**: Need to document differences and trade-offs
5. **Binary Size**: Consumers may package both clients unnecessarily

### Trade-offs

- **Code Duplication**: Some logic duplicated vs. shared complexity
- **Abstraction Leaks**: Some behavioral differences can't be fully hidden
- **Versioning**: Need to keep both clients in sync with core interfaces

### Risks

- **SOAP Deprecation**: Microsoft may eventually remove SOAP API
  - _Mitigation_: Provide clear migration guide, encourage REST adoption
- **Behavioral Differences**: Subtle differences between REST and SOAP
  - _Mitigation_: Comprehensive integration tests, document known differences
- **Dependency Hell**: SOAP requires TFS Client OM, which has many transitive dependencies
  - _Mitigation_: Use separate NuGet packages, consumers opt in

## Alternatives Considered

### Alternative 1: REST Only

```csharp
// ❌ Rejected - Provide only REST client
```

**Rejected because:**

- Breaks backward compatibility for TFS 2018 and earlier users
- Some customers locked into Windows-only environments with legacy TFS
- Complex queries sometimes perform better with SOAP

### Alternative 2: Abstraction Layer Over Both

```csharp
// ❌ Rejected - Single unified implementation that wraps both
public class UnifiedWorkItemStore : IWorkItemStore
{
    public UnifiedWorkItemStore()
    {
        if (IsWindowsAndTfs())
            _impl = new SoapWorkItemStore();
        else
            _impl = new RestWorkItemStore();
    }
}
```

**Rejected because:**

- Consumers can't explicitly choose backend
- Binary bloat (includes both clients always)
- Complex feature detection logic
- Harder to test and debug

### Alternative 3: Adapter Pattern with Pluggable Backends

```csharp
// ❌ Rejected - Extensible plugin system
public interface IWorkItemProvider
{
    IWorkItem GetWorkItem(int id);
}

public class WorkItemStore
{
    public WorkItemStore(IWorkItemProvider provider) { }
}
```

**Rejected because:**

- Over-engineered for current needs
- Consumer confusion about which provider to use
- No clear benefit over explicit factory choice

## Migration Strategy

### When to Use REST

✅ **Recommended for:**

- New projects
- Cross-platform applications (Linux, macOS, Docker)
- Azure DevOps Services
- Azure DevOps Server 2019+
- Simple query scenarios
- Microservices and cloud-native architectures

### When to Use SOAP

✅ **Recommended for:**

- Legacy applications on TFS 2018 and earlier
- Complex queries with advanced filtering
- Windows-only environments
- Existing codebases with SOAP dependency
- Applications requiring Windows Authentication

### Deprecation Timeline

| Date  | Milestone                                               |
| ----- | ------------------------------------------------------- |
| 2024  | SOAP client marked as "legacy" in documentation         |
| 2025  | REST client recommended for all new projects            |
| 2026+ | Evaluate SOAP client deprecation based on usage metrics |

## Related Decisions

- [ADR-001: Factory Pattern for WorkItemStore](ADR-001-factory-pattern-workitemstore.md) - Explains how consumers select between REST/SOAP
- [ADR-002: Interface-First Design](ADR-002-interface-first-design.md) - Unified interface enables this strategy
- [ADR-004: Multi-Targeting Approach](ADR-004-multi-targeting-approach.md) - Platform support enables REST cross-platform

## References

- [Azure DevOps REST API Documentation](https://learn.microsoft.com/en-us/rest/api/azure/devops/)
- [TFS Client OM Documentation](https://learn.microsoft.com/en-us/azure/devops/integrate/concepts/dotnet-client-libraries)
- [SOAP to REST Migration Guide](../../docs/SOAP_TO_REST_MIGRATION.md) (future)

## Revision History

| Date       | Author        | Changes                                      |
| ---------- | ------------- | -------------------------------------------- |
| 2025-12-06 | Copilot Agent | Initial ADR documenting dual client strategy |
