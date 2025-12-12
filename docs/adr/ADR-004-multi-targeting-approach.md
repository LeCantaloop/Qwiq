# ADR-004: Multi-Targeting Approach

- **Status**: Accepted
- **Date**: 2025-12-06
- **Decision Makers**: Qwiq Development Team
- **Supersedes**: None
- **Superseded by**: None

## Context

Qwiq must support a diverse set of consumers:

- Legacy .NET Framework 4.7.2 applications
- .NET Standard 2.0 libraries
- Modern .NET 8 applications
- Future .NET versions (e.g., .NET 10 LTS)

Additionally, there are platform-specific constraints:

- SOAP client requires Windows + .NET Framework 4.7.2 (TFS Client OM dependency)
- REST client should be cross-platform

### Problem Statement

How can we structure Qwiq to:

1. Support multiple .NET target frameworks
2. Maximize cross-platform compatibility
3. Maintain a single codebase (avoid branching)
4. Leverage modern .NET features where available
5. Keep package/binary size reasonable

### Forces

- **Backward Compatibility**: Many customers still on .NET Framework
- **Modern Features**: .NET 8+ offers performance and language improvements
- **Binary Size**: More TFMs = larger NuGet packages
- **Maintenance**: Multi-targeting adds complexity
- **Conditional Compilation**: `#if` directives can make code harder to read
- **Testing**: Must test each TFM

## Decision

We will use **.NET SDK-style multi-targeting** with carefully chosen target frameworks that balance compatibility, performance, and maintainability.

### Target Framework Strategy

| Project              | Target Frameworks              | Rationale                                       |
| -------------------- | ------------------------------ | ----------------------------------------------- |
| **Qwiq.Core**        | `net472;netstandard2.0;net8.0` | Core interfaces, maximum compatibility          |
| **Qwiq.Client.Rest** | `net472;netstandard2.0;net8.0` | Cross-platform REST client                      |
| **Qwiq.Client.Soap** | `net472`                       | Windows-only (TFS Client OM requires net472)    |
| **Qwiq.Linq**        | `net472;net8.0`                | LINQ provider, skip netstandard2.0 (not needed) |
| **Qwiq.Mapper**      | `net472;net8.0`                | Object mapping, skip netstandard2.0             |
| **Qwiq.Identity**    | `net472;net8.0`                | Identity services                               |
| **Test Projects**    | `net472;net8.0`                | Test both oldest and newest frameworks          |

### Implementation

```xml
<!-- Qwiq.Core.csproj - Cross-platform core -->
<PropertyGroup>
  <TargetFrameworks>net472;netstandard2.0;net8.0</TargetFrameworks>
</PropertyGroup>

<!-- Qwiq.Client.Soap.csproj - Windows-only SOAP -->
<PropertyGroup>
  <TargetFrameworks>net472</TargetFrameworks>
</PropertyGroup>

<!-- Qwiq.Linq.csproj - LINQ provider (skip netstandard2.0) -->
<PropertyGroup>
  <TargetFrameworks>net472;net8.0</TargetFrameworks>
</PropertyGroup>
```

### Conditional Compilation

Use sparingly, only when necessary:

```csharp
#if NET8_0_OR_GREATER
using System.Text.Json; // Modern JSON serialization
#else
using Newtonsoft.Json; // Legacy JSON serialization
#endif

public sealed class RestWorkItemStore
{
#if NET8_0_OR_GREATER
    // Use System.Text.Json for better performance
    private readonly JsonSerializerOptions _jsonOptions;
#else
    // Use Newtonsoft.Json for compatibility
    private readonly JsonSerializerSettings _jsonSettings;
#endif
}
```

### Polyfills for Older Frameworks

```csharp
// src/Qwiq.Core/Compatibility/NullableAttributes.cs
#if NETFRAMEWORK || NETSTANDARD2_0
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue) { }
    }
}
#endif
```

### Future Framework Additions

When .NET 10 LTS is released (November 2025):

```xml
<!-- Phase 1: Add SDK support -->
<PropertyGroup>
  <TargetFrameworks>net472;netstandard2.0;net8.0</TargetFrameworks>
  <!-- Update global.json to .NET 10 SDK -->
</PropertyGroup>

<!-- Phase 2: Add net10.0 TFM after SDK stabilizes -->
<PropertyGroup>
  <TargetFrameworks>net472;netstandard2.0;net8.0;net10.0</TargetFrameworks>
</PropertyGroup>

<!-- Phase 3: Consider removing netstandard2.0 -->
<PropertyGroup>
  <TargetFrameworks>net472;net8.0;net10.0</TargetFrameworks>
</PropertyGroup>
```

**Decision**: Skip .NET 9 (STS - Standard Term Support) and adopt .NET 10 (LTS) directly.

## Consequences

### Positive

1. **Maximum Reach**: Support .NET Framework, .NET Standard, and modern .NET
2. **Performance**: Leverage .NET 8+ improvements where available
3. **Cross-Platform**: REST client works on Windows, Linux, macOS
4. **Single Codebase**: No separate branches for different frameworks
5. **NuGet Compatibility**: Multi-TFM packages "just work" for consumers
6. **Future-Ready**: Easy to add new TFMs (e.g., .NET 10)

### Negative

1. **Build Time**: Multi-targeting increases build duration (3x for 3 TFMs)
2. **Package Size**: Larger `.nupkg` files (multiple DLLs included)
3. **Conditional Complexity**: `#if` directives can obscure code logic
4. **Testing Burden**: Must test all TFM combinations
5. **Tooling Support**: Some analyzers/tools struggle with multi-targeting

### Trade-offs

- **netstandard2.0 vs net8.0**: netstandard2.0 enables broader compatibility but misses modern APIs
- **TFM Count**: More TFMs = better compatibility but higher complexity
- **Conditional Compilation**: Enables optimization but reduces code readability

### Risks

- **API Surface Differences**: Some APIs only available in certain TFMs
  - _Mitigation_: Use polyfills, design lowest-common-denominator APIs
- **Behavioral Differences**: Subtle runtime differences between frameworks
  - _Mitigation_: Comprehensive cross-TFM integration tests
- **Dependency Conflicts**: Package versions may differ by TFM
  - _Mitigation_: Central Package Management, careful version selection

## Alternatives Considered

### Alternative 1: Single Target Framework (.NET 8 Only)

```xml
<!-- ❌ Rejected -->
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
</PropertyGroup>
```

**Rejected because:**

- Breaks backward compatibility for .NET Framework customers
- Forces migration for existing consumers
- Loses market share of legacy applications

### Alternative 2: Separate Packages Per Framework

```
Qwiq.Core.Net472
Qwiq.Core.NetStandard20
Qwiq.Core.Net80
```

**Rejected because:**

- Consumer confusion about which package to use
- Difficult to maintain multiple packages
- NuGet handles multi-targeting natively

### Alternative 3: Target Only .NET Standard 2.0

```xml
<!-- ❌ Rejected -->
<PropertyGroup>
  <TargetFramework>netstandard2.0</TargetFramework>
</PropertyGroup>
```

**Rejected because:**

- Lowest common denominator, can't use modern APIs
- Performance loss (no Span<T>, no modern JSON, etc.)
- Still requires net472 for SOAP client

### Alternative 4: Maximum TFM Coverage

```xml
<!-- ❌ Rejected - Too many TFMs -->
<PropertyGroup>
  <TargetFrameworks>net462;net472;net48;netstandard2.0;netcoreapp3.1;net6.0;net7.0;net8.0</TargetFrameworks>
</PropertyGroup>
```

**Rejected because:**

- Build time explosion
- Testing nightmare
- Unnecessary - NuGet rolls forward automatically

## Implementation Guidelines

### DO

- ✅ Use multi-targeting for cross-platform libraries
- ✅ Target oldest and newest frameworks (net472, net8.0)
- ✅ Use `netstandard2.0` for maximum library compatibility
- ✅ Test all TFMs in CI pipeline
- ✅ Document TFM-specific behavior in XML comments
- ✅ Use polyfills for missing APIs on older frameworks

### DON'T

- ❌ Target every .NET version ever released
- ❌ Use excessive conditional compilation
- ❌ Let TFM-specific code dominate the codebase
- ❌ Assume APIs work identically across all TFMs
- ❌ Forget to test each TFM

## Related Decisions

- [ADR-003: REST vs SOAP Strategy](ADR-003-rest-vs-soap-strategy.md) - SOAP client Windows-only constraint
- [ADR-005: Central Package Management](ADR-005-central-package-management.md) - Dependency version management across TFMs

## References

- [.NET SDK Multi-Targeting](https://learn.microsoft.com/en-us/dotnet/standard/frameworks)
- [.NET Standard Versions](https://learn.microsoft.com/en-us/dotnet/standard/net-standard)
- [Conditional Compilation](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives)
- [.NET Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)

## Revision History

| Date       | Author        | Changes                                          |
| ---------- | ------------- | ------------------------------------------------ |
| 2025-12-06 | Copilot Agent | Initial ADR documenting multi-targeting strategy |
