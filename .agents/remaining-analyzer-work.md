# Remaining Analyzer Violations - Completion Plan

**Date**: December 5, 2025  
**Status**: 705 of 931 violations fixed (76% complete)  
**Remaining**: 226 violations

---

## Executive Summary

This document outlines the remaining 226 analyzer violations that need to be addressed to achieve zero suppression debt. These violations are more complex than the ones already fixed and require either API design decisions, performance optimizations, or breaking changes.

---

## Progress Summary

### Completed (705 violations)

- ✅ CA1510 (152): Use `ArgumentNullException.ThrowIfNull`
- ✅ CA1860 (42): Use `Count > 0` instead of `Any()`
- ✅ CA1305 (38): String formatting with `CultureInfo`
- ✅ CA1825 (6): Use `Array.Empty<T>()`
- ✅ CA2208 (12 of 16): Fix `ArgumentException` parameter order
- ✅ Auto-fixed via `dotnet format` (656): Various violations

### Approach Used

1. Created polyfill for `ArgumentNullException.ThrowIfNull` for net472/netstandard2.0
2. Ran `dotnet format` to auto-fix violations
3. Manually fixed remaining violations by category
4. Validated with build and tests after each category

---

## Remaining Violations Breakdown

| Rule   | Count | Category      | Complexity | Breaking Change |
| ------ | ----- | ------------- | ---------- | --------------- |
| CA1847 | 24    | Performance   | Low        | No              |
| CA1863 | 20    | Performance   | Medium     | No              |
| CA1512 | 20    | Quality       | Low        | No              |
| CA1859 | 18    | Performance   | Medium     | Potentially     |
| CA1711 | 18    | Naming        | High       | **Yes**         |
| CA1200 | 18    | Documentation | Low        | No              |
| CA1806 | 12    | Quality       | Low        | No              |
| CA1720 | 12    | Naming        | High       | **Yes**         |
| CA1716 | 12    | Naming        | High       | **Yes**         |
| CA1036 | 6     | Design        | Medium     | No              |
| CA2208 | 4     | Quality       | Low        | No              |

---

## Detailed Fix Plan

### Priority 1: Low-Hanging Fruit (No Breaking Changes)

#### CA1200: Avoid cref with prefix (18 violations)

**Complexity**: Low  
**Breaking**: No  
**Effort**: ~30 minutes

**Issue**: XML documentation uses `M:` or `T:` prefixes in cref tags.

**Example**:

```csharp
// ❌ Wrong
/// <seealso cref="M:System.String.Compare"/>

// ✅ Correct
/// <seealso cref="String.Compare"/>
```

**Fix**: Remove prefixes from all cref tags in XML documentation.

**Files affected**: Check with:

```bash
grep -r "cref=\"[MTF]:" src/ --include="*.cs"
```

---

#### CA1512: Use ThrowIfNullOrEmpty (20 violations)

**Complexity**: Low  
**Breaking**: No  
**Effort**: ~1 hour

**Issue**: Use `ArgumentException.ThrowIfNullOrEmpty` for string validation.

**Example**:

```csharp
// ❌ Old pattern
if (string.IsNullOrEmpty(value))
    throw new ArgumentException("Value cannot be null or empty", nameof(value));

// ✅ New pattern
ArgumentException.ThrowIfNullOrEmpty(value);
```

**Note**: Requires polyfill for net472/netstandard2.0 (similar to CA1510).

**Action**: Create polyfill in `Compatibility/ArgumentExceptionPolyfill.cs`

---

#### CA1847: Use string.Contains(char) (24 violations)

**Complexity**: Low  
**Breaking**: No  
**Effort**: ~30 minutes

**Issue**: Use `string.Contains(char)` instead of `string.Contains(string)` for single characters.

**Example**:

```csharp
// ❌ Less efficient
if (str.Contains("\n"))

// ✅ More efficient
if (str.Contains('\n'))
```

**Fix**: Replace string literals with char literals where appropriate.

---

#### CA1806: Check TryParse return value (12 violations)

**Complexity**: Low  
**Breaking**: No  
**Effort**: ~30 minutes

**Issue**: `TryParse` called but return value not checked.

**Files**:

- `IdentityFieldValue.cs` (lines 257, 271)

**Example**:

```csharp
// ❌ Wrong - ignores success
Guid.TryParse(value, out var result);

// ✅ Correct - check success
if (Guid.TryParse(value, out var result))
{
    // Use result
}
```

---

#### CA2208: ArgumentException parameter fixes (4 remaining)

**Complexity**: Low  
**Breaking**: No  
**Effort**: ~15 minutes

**Issue**: Remaining cases where parameter name is passed as message.

**Action**: Add descriptive messages to remaining ArgumentException constructors.

---

### Priority 2: Performance Optimizations

#### CA1863: Cache CompositeFormat (20 violations)

**Complexity**: Medium  
**Breaking**: No  
**Effort**: ~2 hours

**Issue**: Repeated string formatting should cache the `CompositeFormat`.

**Example**:

```csharp
// ❌ Repeated format parsing
for (int i = 0; i < 1000; i++)
{
    string.Format("Item {0}: {1}", i, value);
}

// ✅ Cache the format
private static readonly CompositeFormat ItemFormat =
    CompositeFormat.Parse("Item {0}: {1}");

for (int i = 0; i < 1000; i++)
{
    string.Format(CultureInfo.InvariantCulture, ItemFormat, i, value);
}
```

**Note**: `CompositeFormat` is .NET 8+ only. Requires evaluation of performance benefit vs. complexity.

**Decision needed**: Is this worth the complexity for this codebase?

---

#### CA1859: Use concrete types for performance (18 violations)

**Complexity**: Medium  
**Breaking**: Potentially  
**Effort**: ~3 hours

**Issue**: Fields/properties declared as interface types when concrete types would perform better.

**Example**:

```csharp
// ❌ Less efficient
private readonly IDictionary<string, int> _map;

// ✅ More efficient
private readonly Dictionary<string, int> _map;
```

**Considerations**:

- Changes internal implementation details
- May affect mockability in tests
- Review each case for actual performance impact

**Decision needed**: Is the performance gain worth reduced flexibility?

---

### Priority 3: Design/Quality Improvements

#### CA1036: Implement comparison operators (6 violations)

**Complexity**: Medium  
**Breaking**: No (additive)  
**Effort**: ~1 hour

**Issue**: Types implementing `IComparable` should also implement comparison operators.

**Example**:

```csharp
public class IdentityDescriptor : IComparable<IdentityDescriptor>
{
    public int CompareTo(IdentityDescriptor? other) { /* ... */ }

    // Add these:
    public static bool operator ==(IdentityDescriptor? left, IdentityDescriptor? right);
    public static bool operator !=(IdentityDescriptor? left, IdentityDescriptor? right);
    public static bool operator <(IdentityDescriptor? left, IdentityDescriptor? right);
    public static bool operator <=(IdentityDescriptor? left, IdentityDescriptor? right);
    public static bool operator >(IdentityDescriptor? left, IdentityDescriptor? right);
    public static bool operator >=(IdentityDescriptor? left, IdentityDescriptor? right);
}
```

**Files**: `IdentityDescriptor.cs`

---

### Priority 4: Breaking Changes (Requires Major Version)

#### CA1711: Rename types ending in Collection/Flags (18 violations)

**Complexity**: High  
**Breaking**: **YES - Public API**  
**Effort**: ~4 hours + migration guide

**Issue**: Type names ending in "Collection" or "Flags" violate naming guidelines.

**Examples**:

- `ITeamProjectCollection` → Consider `ITeamProjectCollectionClient` or `ITeamProjectRepository`
- `SaveFlags` → Consider `SaveOptions`
- `WorkItemCopyFlags` → Consider `WorkItemCopyOptions`

**Impact**:

- Public API changes
- Requires major version bump
- Needs migration guide for consumers

**Decision needed**: Worth breaking API for naming guidelines?

**Recommendation**: Defer to v3.0 release or suppress with justification.

---

#### CA1716: Reserved keyword conflict (12 violations)

**Complexity**: High  
**Breaking**: **YES - Public API**  
**Effort**: ~2 hours

**Issue**: Member names conflict with reserved keywords in other languages.

**Example**:

```csharp
// ❌ Conflicts with VB.NET "When" keyword
public interface IContextSpecification
{
    void When();  // Problematic in VB.NET
}

// ✅ Alternative
public interface IContextSpecification
{
    void Act();  // Or OnSetup, Execute, etc.
}
```

**Files**:

- `IContextSpecification.cs`
- `ContextSpecification.cs`

**Impact**:

- Used throughout all test files
- Would require updating hundreds of tests
- Test pattern is well-established in codebase

**Decision needed**: Is VB.NET interop important enough?

**Recommendation**: Suppress with justification - test internal API, established pattern.

---

#### CA1720: Identifier contains type name (12 violations)

**Complexity**: High  
**Breaking**: **YES - Public API**  
**Effort**: ~3 hours

**Issue**: Identifiers like `Guid` contain type names.

**Example**:

```csharp
// ❌ Contains type name
public interface IProject
{
    Guid Guid { get; }
}

// ✅ Alternatives
public interface IProject
{
    Guid Id { get; }
    Guid Identifier { get; }
    Guid ProjectId { get; }
}
```

**Files**:

- `IProject.cs`
- `Project.cs`

**Impact**:

- Public API change
- May conflict with existing usage patterns
- "Guid" is descriptive in context of Azure DevOps where GUIDs are important

**Decision needed**: Is this worth the breaking change?

**Recommendation**: Suppress with justification - matches Azure DevOps terminology.

---

## Recommended Execution Plan

### Phase 1: Quick Wins (No Breaking Changes) - 2-3 hours

1. CA1200 (18): Remove cref prefixes
2. CA1847 (24): Use char in Contains()
3. CA1806 (12): Check TryParse return
4. CA2208 (4): Fix remaining ArgumentException
5. CA1512 (20): Add ThrowIfNullOrEmpty polyfill and use it

**Total**: 78 violations fixed, ~148 remaining

---

### Phase 2: Performance Review - 3-5 hours

1. CA1863 (20): Evaluate CompositeFormat benefit
2. CA1859 (18): Review concrete vs interface types
3. CA1036 (6): Add comparison operators

**Decision point**: Some may be suppressed if cost > benefit

---

### Phase 3: Breaking Changes Decision - Design Review Required

1. CA1711 (18): Collection/Flags naming
2. CA1716 (12): Keyword conflicts
3. CA1720 (12): Type names in identifiers

**Options**:

- a) Fix in v3.0 with migration guide
- b) Suppress with justification
- c) Hybrid: Fix some, suppress others

---

## Suppression Guidelines

When suppressing violations, follow this pattern:

```csharp
// Suppress with clear justification
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "Well-established test pattern. VB.NET interop not a priority for internal test API.")]
public virtual void When() { }
```

**or** in `.editorconfig`:

```editorconfig
# CA1716: Test framework uses 'When' method - established pattern, internal API
dotnet_diagnostic.CA1716.severity = none
```

---

## Success Metrics

### Quantitative

- [ ] 0 analyzer violations (or 100% justified suppressions)
- [ ] Build with `TreatWarningsAsErrors=true` succeeds
- [ ] All tests pass
- [ ] No regressions in behavior

### Qualitative

- [ ] All suppressions have clear justifications
- [ ] Breaking changes documented in migration guide
- [ ] Performance improvements validated with benchmarks
- [ ] Code review approved

---

## Tools and Commands

### Count remaining violations by category

```bash
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false 2>&1 | \
  grep "error CA" | \
  sed 's/.*error \(CA[0-9]*\):.*/\1/' | \
  sort | uniq -c | sort -rn
```

### Auto-fix with dotnet format

```bash
dotnet format Qwiq.sln --severity error --verbosity diagnostic
```

### Find specific violation instances

```bash
# Example: Find CA1200 violations
dotnet build Qwiq.sln -c Release 2>&1 | grep "error CA1200"
```

---

## Dependencies

### Polyfills Needed

1. ✅ `ArgumentNullException.ThrowIfNull` - Already created
2. ⬜ `ArgumentException.ThrowIfNullOrEmpty` - Needs creation for CA1512
3. ⬜ `CompositeFormat` - May need polyfill or suppress for CA1863

### NuGet Packages

No additional packages required.

---

## Risk Assessment

| Risk                                      | Probability | Impact | Mitigation                                      |
| ----------------------------------------- | ----------- | ------ | ----------------------------------------------- |
| Breaking changes cause consumer issues    | High        | High   | Defer to major version, provide migration guide |
| Performance optimizations introduce bugs  | Medium      | Medium | Thorough testing, benchmarking                  |
| Over-optimization reduces maintainability | Medium      | Medium | Suppress when cost > benefit                    |
| Time investment not justified             | Low         | Low    | Focus on non-breaking changes first             |

---

## Timeline Estimate

| Phase                       | Duration                     | Violations Fixed                            |
| --------------------------- | ---------------------------- | ------------------------------------------- |
| Phase 1: Quick Wins         | 2-3 hours                    | 78                                          |
| Phase 2: Performance Review | 3-5 hours                    | 44 (if all done)                            |
| Phase 3: Breaking Changes   | Design decision + 8-10 hours | 42                                          |
| Documentation & Testing     | 2-3 hours                    | -                                           |
| **Total**                   | **15-21 hours**              | **164 (remaining 62 may need suppression)** |

---

## Next Session Checklist

- [ ] Review this document
- [ ] Decide on breaking changes approach
- [ ] Start with Phase 1 (quick wins)
- [ ] Create ThrowIfNullOrEmpty polyfill
- [ ] Fix CA1200, CA1847, CA1806, CA2208, CA1512
- [ ] Test and validate
- [ ] Move to Phase 2 if time permits

---

## Document Control

| Version | Date        | Changes                                    |
| ------- | ----------- | ------------------------------------------ |
| 1.0     | Dec 5, 2025 | Initial completion plan after 76% progress |
