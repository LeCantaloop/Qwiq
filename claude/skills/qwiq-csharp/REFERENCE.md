# QWIQ C# Reference Documentation

## Nullable Reference Types Migration Status

| Project        | Status        | Warnings | Notes                    |
| -------------- | ------------- | -------- | ------------------------ |
| Qwiq.Core      | ✅ Complete   | 0        | Fully annotated          |
| Qwiq.Core.Rest | ⚠️ Partial    | ~42      | In progress              |
| Qwiq.Core.Soap | ⚠️ Needs work | TBD      | Windows-only             |
| Qwiq.Linq      | ⚠️ Needs work | ~128     | Complex expression trees |
| Qwiq.Identity  | ⚠️ Needs work | ~28      | Identity resolution      |
| Qwiq.Mapper    | ⚠️ Needs work | TBD      | Attribute mapping        |

Run `scripts/Count-NullableWarnings.ps1` for current counts.

## Common Nullable Patterns

### Try\* Methods with Out Parameters

```csharp
public bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value)
{
    if (_dictionary.TryGetValue(key, out value))
        return true;
    value = default;
    return false;
}
```

### Nullable Value Type Assertions

```csharp
// ✅ Correct for int?
value.HasValue.ShouldBeFalse();

// ❌ Wrong - ShouldBeNull<T>() doesn't work well with int?
value.ShouldBeNull();
```

### Interface and Implementation Alignment

When changing nullability on an interface:

1. Update the interface signature
2. Update ALL implementations
3. Update all callers that depend on the nullability

## Core Field Reference Names

Use constants from `CoreFieldRefNames` instead of string literals:

```csharp
// ✅ Correct
field.ReferenceName == CoreFieldRefNames.Id

// ❌ Wrong - magic string
field.ReferenceName == "System.Id"
```

## Exception Types Reference

| Exception                   | When to Use                                      |
| --------------------------- | ------------------------------------------------ |
| `ArgumentNullException`     | Parameter is null                                |
| `ArgumentException`         | Parameter is invalid (but not null)              |
| `InvalidOperationException` | Operation invalid for current state              |
| `NotSupportedException`     | Operation not supported (e.g., unsupported LINQ) |
| `PageSizeRangeException`    | PageSize outside 50-200 range                    |
| `AttributeMapException`     | Mapper field/type conversion failure             |

## Compatibility Shims

### NullableAttributes.cs

Location: `src/Qwiq.Core/Compatibility/NullableAttributes.cs`

Provides nullable attributes for `net472` and `netstandard2.0`:

- `MaybeNullWhenAttribute`
- `AllowNullAttribute`
- `NotNullAttribute`
- `NotNullWhenAttribute`
- `DoesNotReturnAttribute`
- `DoesNotReturnIfAttribute`

**Important:** Do NOT use the `Polyfill` NuGet package - it conflicts with VSS Client polyfills.

### IdentityTypeMapper.cs

Location: `src/Qwiq.Core/Compatibility/IdentityTypeMapper.cs`

Compatibility shim for class removed in Microsoft.VisualStudio.Services.Client v19+.
