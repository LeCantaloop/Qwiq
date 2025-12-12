# Session Handoff: LINQ Test Failures Investigation

**Date**: December 5, 2025  
**Session**: Addressing PR #58 Test Failures  
**Status**: 🔴 **BLOCKED** - 4 LINQ/Mapper tests failing, root cause identified

---

## Executive Summary

PR review identified test failures in LINQ query translation. Investigation revealed that recent changes to the codebase (likely C# 14 polyfills or .NET 10 SDK upgrade) have broken the `Contains` method handling in LINQ expression trees. **4 tests are failing** across LINQ and Mapper test suites.

**Critical**: These failures existed BEFORE this session started (not introduced by recent analyzer work).

---

## Failing Tests

### LINQ Tests (2 failures)
1. **`when_a_query_has_a_field_that_should_be_in_a_list_of_string_array_values.it_is_translated_to_an_in_operator`**
   - Test: `_values.Contains(item.Tags)` where `_values` is `string[]`
   - Expected: `SELECT * FROM WorkItems WHERE (([Tags] IN ('person1', 'person2')))`
   - Error: `Specified method is not supported`

2. **`when_an_ienumerable_contains_constants_with_special_wiql_characters.each_string_is_escaped`**
   - Test: `_values.Contains(item.AssignedTo)` where `_values` is `string[]`
   - Expected: Proper escaping of special characters in IN clause
   - Error: `Specified method is not supported`

### Mapper Tests (2 failures)
3. **`when_a_where_clause_includes_an_empty_contains_clause.the_query_should_not_be_run`**
   - Test: `list.Contains(item.IntField)` where `list` is `int[]`
   - Expected: Query optimization (don't run query for empty array)
   - Error: `Specified method is not supported`

4. **`when_a_where_clause_includes_an_empty_contains_clause.the_query_should_return_the_empty_result_set`**
   - Same test as #3, different assertion

---

## Root Cause Analysis

### Problem
The `QueryRewriter.VisitMethodCall` in `src/Qwiq.Linq/Visitors/QueryRewriter.cs` is not recognizing `array.Contains()` calls in expression trees.

### Why It's Failing
1. **Original code** (line 99-105) checked for:
   ```csharp
   if (node.Method.DeclaringType == typeof(Enumerable) && node.Method.Name == "Contains")
   ```

2. **The issue**: When you write `array.Contains(value)` in C#, the compiler generates a call to an extension method, but the representation in expression trees can vary:
   - .NET 8 SDK: Uses `Enumerable.Contains`
   - .NET 9+ SDK: May use `MemoryExtensions.Contains` or other optimizations
   - Recent codebase changes (C# 14 polyfills?): May have changed how this is represented

3. **Attempts made**:
   - Added check for `System.MemoryExtensions` ✅
   - Added check for 2-arg vs 1-arg patterns ✅
   - Made permissive to accept ALL non-string Contains ✅
   - **STILL FAILING** ❌

### Critical Finding
Tests fail with BOTH .NET 8.0.416 SDK and .NET 10.0.100 SDK, ruling out SDK version as the cause. The issue was introduced by recent codebase changes, likely:
- Commit `9d53546`: "refactor: use ArgumentNullException.ThrowIfNull across source files"
- Commit `9c9d85d`: "refactor(core): rewrite polyfills using C# 14 extension blocks"
- Commit `d97d0d4`: "feat(core): rewrite ArgumentNullException polyfill using C# 14 extension blocks"

---

## Investigation Attempts

### What Was Tried
1. ✅ Added support for `MemoryExtensions.Contains`
2. ✅ Added pattern matching for 2-arg extension methods
3. ✅ Added pattern matching for 1-arg instance-style calls  
4. ✅ Made logic permissive to accept all non-Collection/HashSet/List Contains
5. ✅ Added debug logging (file-based)
6. ✅ Modified ContextSpecification to write full exception to file
7. ✅ Tested with .NET 8 SDK (8.0.416) - still fails
8. ✅ Tested with .NET 10 SDK (10.0.100) - still fails

### What Didn't Work
- **Debug logging never executed**: The `/tmp/contains-debug.txt` file was never created, meaning `VisitMethodCall` with `node.Method.Name == "Contains"` is never being reached
- **Exception file not created**: Modified ContextSpecification to write exception details, but file was never created
- **Permissive logic didn't help**: Even accepting ALL Contains methods (except string) didn't fix it

### Critical Insight
The fact that debug logging was never executed suggests:
1. The Contains method call is being transformed/inlined before reaching `VisitMethodCall`, OR
2. An exception is thrown earlier in the expression tree traversal, OR
3. The method name is not "Contains" in the expression tree representation

---

## Current Code State

### Files Modified This Session
1. **`src/Qwiq.Linq/Visitors/QueryRewriter.cs`**
   - Cleaned up all debug code
   - Updated Contains handling to be more permissive
   - Still not working

2. **`src/Qwiq.Core/IdentitySearchFactor.cs`**
   - Fixed: Use `<see cref="..." />` for type references ✅

3. **`Directory.Packages.props`**
   - Removed obsolete comment about Microsoft.CodeAnalysis.NetAnalyzers ✅

4. **`test/Qwiq.Tests.Common/ContextSpecification.cs`**
   - Added exception logging to file (for debugging)
   - Should be reverted or removed

---

## Recommended Next Steps

### Option 1: Git Bisect (Recommended)
Use `git bisect` to find the exact commit that broke the tests:
```bash
git bisect start
git bisect bad HEAD
git bisect good a89f97b  # Last known good commit before modernization
# Test at each step with:
dotnet test test/Qwiq.Linq.Tests/Qwiq.Linq.UnitTests.csproj -c Release --framework net8.0
```

### Option 2: Revert Polyfill Changes
The C# 14 extension blocks in polyfills might be causing issues:
1. Check if reverting `src/Qwiq.Core/Compatibility/ArgumentNullExceptionPolyfill.cs` fixes tests
2. The extension block syntax might affect expression tree compilation

### Option 3: Deep Debugging
Add console output or System.Diagnostics.Trace at multiple points:
1. **Start of VisitMethodCall**: Log every method call being visited
2. **Before Contains check**: Verify we're even reaching that code
3. **In Visit(node.Arguments[0])**: Check if recursion is failing

Example debugging code:
```csharp
protected override Expression VisitMethodCall(MethodCallExpression node)
{
    System.IO.File.AppendAllText("/tmp/visit-methods.txt", 
        $"Method: {node.Method.Name}, Type: {node.Method.DeclaringType?.FullName}\n");
    
    // ... rest of method
}
```

### Option 4: Check Base Branch
Test if the issue exists on the base branch (`feat/modernize-2` or `develop`):
```bash
git stash
git checkout feat/modernize-2
dotnet test test/Qwiq.Linq.Tests/Qwiq.Linq.UnitTests.csproj -c Release --framework net8.0
```

---

## Code Changes Made

### QueryRewriter.cs - Current State
```csharp
// Handle Contains method calls
if (node.Method.Name == "Contains")
{
    var declaringType = node.Method.DeclaringType;
    
    // String.Contains - substring matching
    if (declaringType == typeof(string))
    {
        var subject = Visit(node.Object);
        var target = Visit(node.Arguments[0]);
        return new ContainsExpression(node.Type, subject!, target!);
    }
    
    // Collection Contains - membership testing
    // Exclude Collection<T>, HashSet<T>, List<T> (unsupported)
    var isUnsupportedCollection = 
        declaringType?.Name == "Collection`1" ||
        declaringType?.Name == "HashSet`1" ||
        declaringType?.Name == "List`1";
    
    if (!isUnsupportedCollection)
    {
        Expression subject, target;
        
        if (node.Arguments.Count == 2)
        {
            // Extension method: Contains(source, value)
            subject = Visit(node.Arguments[1]);
            target = Visit(node.Arguments[0]);
        }
        else if (node.Arguments.Count == 1)
        {
            // Instance syntax: source.Contains(value)
            subject = Visit(node.Arguments[0]);
            target = Visit(node.Object!);
        }
        else
        {
            goto unknown_method;
        }
        
        return new InExpression(node.Type, subject, target);
    }
}

unknown_method:
// Unknown method call
throw new NotSupportedException($"The method '{node.Method.Name}' is not supported");
```

---

## Testing Commands

### Run Failing Tests
```bash
# LINQ tests
dotnet test test/Qwiq.Linq.Tests/Qwiq.Linq.UnitTests.csproj \
  -c Release --no-build --framework net8.0

# Specific failing test
dotnet test test/Qwiq.Linq.Tests/Qwiq.Linq.UnitTests.csproj \
  -c Release --no-build --framework net8.0 \
  --filter "FullyQualifiedName~when_a_query_has_a_field_that_should_be_in_a_list_of_string_array_values"

# Mapper tests  
dotnet test test/Qwiq.Mapper.Tests/Qwiq.Mapper.UnitTests.csproj \
  -c Release --no-build --framework net8.0 \
  --filter "FullyQualifiedName~when_a_where_clause_includes_an_empty_contains_clause"
```

### Build Commands
```bash
# Rebuild from scratch
dotnet clean Qwiq.sln -c Release
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false

# Just LINQ
dotnet build src/Qwiq.Linq/Qwiq.Linq.csproj -c Release /m:1 /nodeReuse:false
```

---

## Success Criteria

Tests pass when:
```
=== LINQ Tests ===
Passed!  - Failed:     0, Passed:    34, Skipped:     0, Total:    34

=== Mapper Tests ===  
Passed!  - Failed:     0, Passed:     2, Skipped:     0, Total:     2
```

---

## Files to Review/Revert

### Files Modified by Previous Sessions (Suspects)
1. `src/Qwiq.Core/Compatibility/ArgumentNullExceptionPolyfill.cs`
   - C# 14 extension blocks - might affect expression tree compilation
   
2. All files changed in commit `9d53546`
   - Replaced `if (x == null) throw` with `ArgumentNullException.ThrowIfNull`
   - Check if any changes affected LINQ or expression tree code

3. `Directory.Build.props` or `global.json`
   - LangVersion set to "preview" for C# 14
   - May affect how expressions are compiled

### Files Modified This Session (Safe)
1. `src/Qwiq.Core/IdentitySearchFactor.cs` - Documentation fix ✅
2. `Directory.Packages.props` - Comment removal ✅
3. `src/Qwiq.Linq/Visitors/QueryRewriter.cs` - Attempted fix (not working)
4. `test/Qwiq.Tests.Common/ContextSpecification.cs` - Debug code (revert)

---

## Cleanup Needed

Before ending session:
1. ✅ Remove debug code from QueryRewriter.cs (DONE)
2. ⬜ Revert or remove debug code from ContextSpecification.cs
3. ⬜ Commit current state with clear message about unresolved issue
4. ⬜ Update TODO document
5. ⬜ Create this handoff document ✅

---

## Context for Next Session

### What You Need to Know
1. **The tests were passing before recent polyfill changes**
2. **The issue is NOT related to .NET SDK version** (tested with both 8 and 10)
3. **The Contains handler code is likely never being reached**
4. **Expression trees may be compiled differently with C# 14 extension blocks**

### Quick Start
```bash
cd /home/runner/work/Qwiq/Qwiq

# Reproduce the issue
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
dotnet test test/Qwiq.Linq.Tests/Qwiq.Linq.UnitTests.csproj -c Release --no-build --framework net8.0

# Expected: 2 failures in LINQ tests
# Check this document for detailed analysis
```

### Priority Actions
1. Use git bisect to find breaking commit (highest priority)
2. Compare expression tree representation before/after
3. Check if C# 14 extension blocks affect expression compilation
4. Consider reverting polyfill changes temporarily to confirm

---

## Document Version

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | Dec 5, 2025 | Copilot Session | Initial handoff document |

**Location**: `.agents/sessions/session-handoff-test-failures.md`
