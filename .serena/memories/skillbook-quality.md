# Code Quality Skills

## Skill-Quality-001

**Entity Type**: Skill
**Statement**: Test classes ending in "Collection" trigger CA1711; use abbreviations like "ToWIC"
**Atomicity**: 92%
**Category**: Code Quality
**Context**: When naming test classes for collection extension methods
**Evidence**: Session 39 - Renamed 5 test classes to fix CA1711
**Tag**: helpful
**Impact**: 7
**Validated**: 1

**Summary**: CA1711: "Identifiers should not have incorrect suffix". "Collection" is reserved for actual collection types, not test classes. Analyzer flags: `ToCollectionTests`, `WhereManyCollectionTests`, etc. Solution: Use abbreviation (e.g., "ToWIC" for "To Work Item Collection").

**Pattern**:

```csharp
// WRONG - triggers CA1711
[TestClass]
public class ToCollectionTests
{
    // Extension method tests for IEnumerable<T>.ToCollection()
}

// CORRECT - abbreviation avoids conflict
[TestClass]
public class ToWICTests
{
    // Extension method tests for IEnumerable<T>.ToCollection()
}
```

**Other Names to Avoid**:

- Classes ending in: Dictionary, List, Queue, Stack, Set, Enumerable
- Use abbreviations when testing extension methods on these types

---

## Skill-Quality-002

**Entity Type**: Skill
**Statement**: Add `#pragma warning disable CA1001` with explanation when test cleanup handles disposal via [TestCleanup]
**Atomicity**: 90%
**Category**: Code Quality
**Context**: When test classes own disposable fields managed by ContextSpecification.Cleanup()
**Evidence**: Session 39 - Added pragma with explanatory comment
**Tag**: helpful
**Impact**: 8
**Validated**: 1

**Summary**: CA1001: "Types that own disposable fields should implement IDisposable". Test classes using ContextSpecification base class handle cleanup via [TestCleanup] method. Base class calls Cleanup() which disposes owned resources. Pragma warning is appropriate here because cleanup is delegated to base class.

**Pattern**:

```csharp
[TestClass]
public class GivenSomeContext : ContextSpecification
{
#pragma warning disable CA1001
// Explanation: Disposal is handled by ContextSpecification.Cleanup()
// which is called via [TestCleanup] MSTest attribute
    private IDisposable _mockStore;
#pragma warning restore CA1001

    public override void Given()
    {
        _mockStore = CreateMockStore();
    }

    // Cleanup handled by base class - no need for Dispose
}
```

---

## Skill-Quality-003

**Entity Type**: Skill
**Statement**: Extract inline test arrays to `static readonly` fields to satisfy CA1861
**Atomicity**: 88%
**Category**: Code Quality
**Context**: When using constant arrays in test methods
**Evidence**: Session 39 - Created TestArrays.DefaultTargetIds
**Tag**: helpful
**Impact**: 7
**Validated**: 1

**Summary**: CA1861: "Avoid passing a literal array to a method that expects a 'ReadOnlySpan' or 'ImmutableArray'". Allocates new array for each test call. Solution: Extract to static readonly field for reuse.

**Pattern - Correct** ✅:

```csharp
private static readonly int[] DefaultTargetIds = { 1, 2, 3, 4, 5 };

[TestMethod]
public void Method_DoesWork()
{
    var result = Method(DefaultTargetIds);
}
```

**Benefits**:

- Avoids repeated allocations
- More readable test code
- Follows CA1861 analyzer guidance
- Slight performance improvement in test suites
