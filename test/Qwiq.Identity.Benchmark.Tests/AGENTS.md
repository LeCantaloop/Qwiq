# Qwiq.Identity.Benchmark.Tests Component Guide

## Component Overview

**Qwiq.Identity.Benchmark.Tests** contains performance benchmarks for identity resolution operations using BenchmarkDotNet.

## Purpose

- Measure identity service performance
- Benchmark bulk vs individual resolution
- Profile identity caching strategies
- Track identity lookup performance

## Key Characteristics

- **Framework**: BenchmarkDotNet
- **Test Category**: `[TestCategory("Benchmark")]`
- **Focus**: Identity resolution performance

## Running Benchmarks

```powershell
dotnet run -c Release --project test/Qwiq.Identity.Benchmark.Tests
```

## Common Benchmarks

### Individual vs Bulk Resolution

```csharp
[Benchmark]
public void ReadIdentities_Individual()
{
    foreach (var user in _users)
    {
        _service.ReadIdentity(user);
    }
}

[Benchmark]
public void ReadIdentities_Bulk()
{
    _service.ReadIdentities(_users);
}
```

### Caching Strategies

Benchmark different identity caching approaches.

## Related Components

- **Qwiq.Identity** - Identity services under test
- **Qwiq.Mocks** - MockIdentityManagementService
- **Qwiq.Benchmark** - General benchmarks

## Common Mistakes to Avoid

❌ **Don't benchmark in Debug mode** - Use Release

✅ **Do use realistic data volumes** - Match production usage

✅ **Do measure both individual and bulk** - Show performance difference
