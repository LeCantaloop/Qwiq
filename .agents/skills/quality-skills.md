# Code Quality Skills

## Skill-Quality-001

**Statement**: Test classes ending in "Collection" trigger CA1711; use abbreviations like "ToWIC"

**Atomicity**: 92%

**Category**: Code Quality

**Context**: When naming test classes for collection extension methods

**Evidence**: Session 39 - Renamed 5 test classes to fix CA1711

**Details**:

- CA1711: "Identifiers should not have incorrect suffix"
- "Collection" is reserved for actual collection types, not test classes
- Analyzer flags: `ToCollectionTests`, `WhereManyCollectionTests`, etc.
- Solution: Use abbreviation (e.g., "ToWIC" for "To Work Item Collection")

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

**How to Choose Abbreviation**:

- ToCollection → ToWIC (work item collection)
- WhereManyCollection → WhereManyWIC
- SelectCollection → SelectWIC

**Application**:

- Review test class names during code review
- Automated: Enable CA1711 in analyzer config
- With `/p:PedanticMode=true` in CI

---

## Skill-Quality-002

**Statement**: Add `#pragma warning disable CA1001` with explanation when test cleanup handles disposal via [TestCleanup]

**Atomicity**: 90%

**Category**: Code Quality

**Context**: When test classes own disposable fields managed by ContextSpecification.Cleanup()

**Evidence**: Session 39 - Added pragma with explanatory comment

**Details**:

- CA1001: "Types that own disposable fields should implement IDisposable"
- Test classes using ContextSpecification base class handle cleanup via [TestCleanup] method
- Base class calls Cleanup() which disposes owned resources
- Pragma warning is appropriate here because cleanup is delegated to base class

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

**When to Apply**:

- Test classes extending ContextSpecification
- That own disposable fields (mocks, contexts, stubs)
- Where cleanup is guaranteed by framework

**Alternatives**:

1. Don't store disposables at class level (create in method)
2. Implement IDisposable on test class
3. Use using blocks in test methods

---

## Skill-Quality-003

**Statement**: Extract inline test arrays to `static readonly` fields to satisfy CA1861

**Atomicity**: 88%

**Category**: Code Quality

**Context**: When using constant arrays in test methods

**Evidence**: Session 39 - Created TestArrays.DefaultTargetIds

**Details**:

- CA1861: "Avoid passing a literal array to a method that expects a 'ReadOnlySpan' or 'ImmutableArray'"
- Allocates new array for each test call
- Solution: Extract to static readonly field for reuse

**Pattern - Wrong** ❌:

```csharp
[TestMethod]
public void Method_DoesWork()
{
    var targetIds = new[] { 1, 2, 3, 4, 5 };
    var result = Method(targetIds); // CA1861: allocates array each time
}
```

**Pattern - Correct** ✅:

```csharp
private static readonly int[] DefaultTargetIds = { 1, 2, 3, 4, 5 };

[TestMethod]
public void Method_DoesWork()
{
    var result = Method(DefaultTargetIds);
}
```

**Shared Test Arrays**:

```csharp
public static class TestArrays
{
    public static readonly int[] DefaultTargetIds = { 1, 2, 3, 4, 5 };
    public static readonly string[] CommonTitles = { "Bug", "Feature", "Task" };
    public static readonly int[] LargeIdSet = { 1, 2, 3, ..., 100 };
}

[TestClass]
public class MyTests
{
    [TestMethod]
    public void Test1() => Method(TestArrays.DefaultTargetIds);

    [TestMethod]
    public void Test2() => Method(TestArrays.LargeIdSet);
}
```

**Benefits**:

- Avoids repeated allocations
- More readable test code
- Follows CA1861 analyzer guidance
- Slight performance improvement in test suites
