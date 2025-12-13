# ADR-009: Polyfill Strategy for ArgumentNullException.ThrowIfNull

## Status

Accepted (Revised)

## Context

- The repository targets multiple TFMs: `net472`, `netstandard2.0`, and `net8.0`.
- Modern guard APIs like `ArgumentNullException.ThrowIfNull` are only available in .NET 6+.
- Earlier experiments tried C# 14 extension members syntax (`extension(ArgumentNullException)`) patterned after the SimonCropp/Polyfill project, but the current .NET 10 SDK (10.0.100) and cross-assembly usage led to ambiguity and build failures for `net472` consumers.
- File-level locks (e.g., Microsoft Defender) complicated incremental builds; stability and determinism were prioritized.

### What we tried (and why it initially failed)

- **C# 14 extension members syntax** (inspired by SimonCropp/Polyfill):
  - Implemented `Polyfill.extension(ArgumentNullException).ThrowIfNull` with global usings.
  - Worked in-assembly but caused **CS0121 ambiguous call** errors across projects/assemblies targeting `net472`.
  - Extension members for static types are not yet reliable across assembly boundaries in the current toolchain.
- **Making Polyfill public/internal**:
  - Tried toggling visibility to reduce ambiguity; issues persisted because multiple linked copies still surfaced competing extension candidates.
- **GlobalUsings for Polyfills**:
  - Reduced call-site noise but did not eliminate ambiguity when the same extension surfaced from multiple referenced assemblies.

### The Solution: `[Embedded]` Attribute

After further research (see Andrew Lock's article on the `[Embedded]` attribute), we discovered that the **`Microsoft.CodeAnalysis.EmbeddedAttribute`** solves the cross-assembly ambiguity problem:

- The `[Embedded]` attribute marks types as invisible outside the current compilation unit
- This prevents `InternalsVisibleTo` from leaking polyfill types across assemblies
- Each project gets its own isolated copy of the polyfill, eliminating CS0121 ambiguity
- Available in Roslyn 4.14+ (.NET 10 SDK), but can be polyfilled for older frameworks

### Why not ship the SimonCropp Polyfill package directly?

- The repo already ships custom compatibility shims (e.g., `NullableAttributes.cs`) to avoid conflicts with VSS Client polyfills.
- Adding the package would risk namespace/type clashes with existing TFS/VSS dependencies.

## Decision

- **Use C# 14 extension members syntax** with the `[Embedded]` attribute to provide `ArgumentNullException.ThrowIfNull` for `net472` and `netstandard2.0`.
- **Create `EmbeddedAttribute.cs`** polyfill for older frameworks that don't have `Microsoft.CodeAnalysis.EmbeddedAttribute`.
- **Apply `[global::Microsoft.CodeAnalysis.EmbeddedAttribute]`** to the `Polyfill` partial class to prevent cross-assembly visibility.
- **Link polyfill files** into each project that needs them (each project gets its own embedded copy).
- **Retain** other compatibility shims (e.g., `CallerArgumentExpressionAttribute`, `ArgumentOutOfRangeExceptionPolyfill`) using the same pattern.

## Consequences

### Positive

- **Eliminates CS0121 ambiguity** - The `[Embedded]` attribute prevents type leakage across assemblies.
- **Modern API surface** - Code uses `ArgumentNullException.ThrowIfNull(param)` consistently across all TFMs.
- **Automatic parameter name capture** - `CallerArgumentExpression` provides parameter names without `nameof()`.
- **Consistent with .NET patterns** - Same API as .NET 6+ for easier migration.
- **No external dependencies** - Self-contained polyfills avoid package conflicts.

### Negative / Trade-offs

- **File linking required** - Each project must link the polyfill files (adds csproj complexity).
- **Partial class coordination** - The `[Embedded]` attribute must be applied exactly once to the partial class.
- **Preview feature dependency** - C# 14 extension members are still in preview (though stable in SDK 10.0.100).

## Implementation Notes

### Files Created/Modified

**New polyfill files** in `src/Qwiq.Core/Compatibility/`:

- `EmbeddedAttribute.cs` - Polyfill for `Microsoft.CodeAnalysis.EmbeddedAttribute`
- `ArgumentNullExceptionPolyfill.cs` - Provides `ThrowIfNull` with `[Embedded]` attribute
- `ArgumentOutOfRangeExceptionPolyfill.cs` - Provides `ThrowIfZero`, `ThrowIfEqual`, etc.
- `CallerArgumentExpressionAttribute.cs` - Required for parameter name capture

**Project files updated** with polyfill links:

- `src/Qwiq.Core/Qwiq.Core.csproj` (source files)
- `src/Qwiq.Linq/Qwiq.Linq.csproj`
- `src/Qwiq.Identity/Qwiq.Identity.csproj`
- `src/Qwiq.Mapper/Qwiq.Mapper.csproj`
- `src/Qwiq.Core.Rest/Qwiq.Client.Rest.csproj`
- `test/Qwiq.Mocks/Qwiq.Mocks.csproj`

### Key Implementation Details

```xml
<!-- Example project file polyfill links -->
<ItemGroup>
  <Compile Include="..\Qwiq.Core\Compatibility\EmbeddedAttribute.cs" Link="Compatibility\EmbeddedAttribute.cs" />
  <Compile Include="..\Qwiq.Core\Compatibility\CallerArgumentExpressionAttribute.cs" Link="Compatibility\CallerArgumentExpressionAttribute.cs" />
  <Compile Include="..\Qwiq.Core\Compatibility\ArgumentNullExceptionPolyfill.cs" Link="Compatibility\ArgumentNullExceptionPolyfill.cs" />
</ItemGroup>
```

```csharp
// EmbeddedAttribute.cs - makes types invisible outside compilation
#if NETFRAMEWORK || NETSTANDARD2_0
namespace Microsoft.CodeAnalysis
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class EmbeddedAttribute : System.Attribute { }
}
#endif

// ArgumentNullExceptionPolyfill.cs - the actual polyfill
#if !NET6_0_OR_GREATER
[global::Microsoft.CodeAnalysis.EmbeddedAttribute]
static partial class Polyfill
{
    extension(ArgumentNullException)
    {
        public static void ThrowIfNull([NotNull] object? argument,
            [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument is null) throw new ArgumentNullException(paramName);
        }
    }
}
#endif
```

### Build Configuration

- **Build flags**: `Directory.Build.rsp` enforces `/m:1 /nodeReuse:false` for stability on Windows
- **Defender exclusions** recommended to reduce file-lock noise during builds

## Alternatives Considered

1. **Traditional null checks** (`if (param == null) throw new ArgumentNullException(nameof(param))`)

   - Initially adopted as fallback when extension syntax failed
   - Rejected after discovering `[Embedded]` attribute solution
   - More verbose and loses `CallerArgumentExpression` benefits

2. **Ship SimonCropp/Polyfill NuGet** and rely on its source generation/targets.

   - Rejected: risk of conflicts with VSS/TFS client polyfills and existing custom shims.

3. **Namespace-based isolation** (different namespaces per project)
   - Rejected: still requires `using` statements and doesn't prevent `InternalsVisibleTo` leakage.

## Future Work

- Monitor C# extension members feature stability in future SDK releases
- Consider consolidating polyfill files into a shared source package if pattern proves stable
- Evaluate if `[Embedded]` attribute becomes unnecessary in future Roslyn versions

## References

- Andrew Lock: "Behind the implementation of the [Embedded] attribute in .NET 10" - <https://andrewlock.net/behind-the-implementation-of-the-embedded-attribute-in-net-10/>
- SimonCropp/Polyfill `ArgumentNullExceptionPolyfill.cs` (commit e78ac432695270075490e9fbee211e25e4fccb70)
- C# Extension Members (C# 14 proposal): <https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-14.0/extensions>
- Qwiq repository compatibility shims (e.g., `NullableAttributes.cs`, `CallerArgumentExpressionAttribute.cs`)
