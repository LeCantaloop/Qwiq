# Qwiq.Mapper.Benchmark.Tests Component Guide

## Component Overview

**Qwiq.Mapper.Benchmark.Tests** contains performance benchmarks for work item mapping operations using BenchmarkDotNet.

## Purpose

- Measure object mapping performance
- Benchmark different mapping strategies
- Profile attribute reflection overhead
- Track mapper performance over time

## Key Characteristics

- **Framework**: BenchmarkDotNet
- **Test Category**: `[TestCategory("Benchmark")]`
- **Focus**: Work item to POCO mapping performance

## Running Benchmarks

```powershell
dotnet run -c Release --project test/Qwiq.Mapper.Benchmark.Tests
```

## Common Benchmarks

### Mapping Strategy Performance

```csharp
[Benchmark]
public void AttributeMapperStrategy()
{
    var strategy = new AttributeMapperStrategy();
    var mapper = new WorkItemMapper(strategy);
    _ = mapper.Create<Bug>(_workItems).ToList();
}

[Benchmark]
public void BulkIdentityAwareStrategy()
{
    var strategy = new BulkIdentityAwareAttributeMapperStrategy(_identityService);
    var mapper = new WorkItemMapper(strategy);
    _ = mapper.Create<Bug>(_workItems).ToList();
}
```

### Reflection vs Cached Mapping

Measure cost of attribute reflection vs cached property access.

## Related Components

- **Qwiq.Mapper** - Mapping functionality under test
- **Qwiq.Mocks** - Mock work items
- **Qwiq.Benchmark** - General benchmarks

## Common Mistakes to Avoid

❌ **Don't benchmark in Debug mode** - Use Release

✅ **Do benchmark with realistic work item counts** - 10, 100, 1000 items

✅ **Do measure both cold and warm paths** - Reflection caching impact
