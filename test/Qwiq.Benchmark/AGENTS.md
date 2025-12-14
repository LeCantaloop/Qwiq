# Qwiq.Benchmark Component Guide

## Component Overview

**Qwiq.Benchmark** contains performance benchmarks for core Qwiq functionality using BenchmarkDotNet.

## Purpose

- Measure query performance
- Benchmark field access patterns
- Profile LINQ translation overhead
- Track performance regressions over time

## Key Characteristics

- **Target Frameworks**: Benchmark TFMs
- **Framework**: BenchmarkDotNet
- **Test Category**: `[TestCategory("Benchmark")]`
- **Pattern**: Performance measurement, not assertions

## Running Benchmarks

### Run All Benchmarks

```powershell
dotnet run -c Release --project test/Qwiq.Benchmark/Qwiq.Benchmark.csproj
```

### Run Specific Benchmark

```powershell
dotnet run -c Release --project test/Qwiq.Benchmark/Qwiq.Benchmark.csproj -- --filter "*QueryBenchmark*"
```

## Benchmark Structure

```csharp
[MemoryDiagnoser]
public class QueryBenchmark
{
    private MockWorkItemStore? _store;

    [GlobalSetup]
    public void Setup()
    {
        _store = new MockWorkItemStore();
        // Add test data
    }

    [Benchmark]
    public void Query_100_WorkItems()
    {
        var items = _store!.Query("SELECT [System.Id] FROM WorkItems");
        _ = items.ToList();
    }
}
```

## Common Benchmark Categories

- **Query performance**: WIQL query execution
- **Field access**: Reading/writing work item fields
- **LINQ translation**: Expression tree to WIQL conversion
- **Object mapping**: Work item to POCO mapping

## Test Category Exclusion

Benchmarks are excluded from regular test runs:

```powershell
# Excluded via filter
dotnet test --filter "TestCategory!=Benchmark"
```

## Related Components

- **Qwiq.Core** - Core functionality being benchmarked
- **Qwiq.Linq** - LINQ query performance
- **Qwiq.Mocks** - Mock implementations for benchmarks

## Common Mistakes to Avoid

❌ **Don't run in Debug mode** - Always use Release for accurate results

```powershell
# ❌ WRONG: Debug mode
dotnet run --project test/Qwiq.Benchmark

# ✅ CORRECT: Release mode
dotnet run -c Release --project test/Qwiq.Benchmark
```

✅ **Do run on idle machine** - Minimize background processes

✅ **Do use [MemoryDiagnoser]** - Track allocations

✅ **Do baseline results** - Track changes over time
