# Qwiq.Linq.Tests Component Guide

## Component Overview

**Qwiq.Linq.Tests** contains unit tests for the Qwiq.Linq LINQ query provider, verifying LINQ-to-WIQL translation.

## Purpose

- Test LINQ expression translation to WIQL
- Verify query provider behavior
- Test field mapping
- Validate WIQL extension methods (AsOf, WasEver, InGroup)

## Key Characteristics

- **Target Frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
- **Test Framework**: MSTest
- **Pattern**: ContextSpecification
- **Focus**: Query translation and WIQL generation

## Test Categories

### Translation Tests

Verify LINQ operators translate to correct WIQL:

```csharp
[TestMethod]
public void Should_translate_where_clause()
{
    var query = _store!.Query<WorkItem>()
        .Where(wi => wi.State == "Active");

    var wiql = WiqlTranslator.Translate(query.Expression);
    wiql.ShouldContain("WHERE [System.State] = 'Active'");
}
```

### Operator Tests

- Where clauses (equality, comparison, Contains)
- OrderBy / ThenBy
- Take (TOP N)
- Extension methods (AsOf, WasEver, InGroup)

### Field Mapper Tests

```csharp
[TestMethod]
public void Should_map_property_to_field()
{
    var mapper = new SimpleFieldMapper();
    mapper.GetFieldName("Title").ShouldBe(CoreFieldRefNames.Title);
}
```

## Common Test Patterns

### Testing WIQL Generation

```csharp
public override void When()
{
    _wiql = WiqlTranslator.Translate(_query!.Expression);
}

[TestMethod]
public void Then_generates_valid_wiql()
{
    _wiql.ShouldContain("SELECT");
    _wiql.ShouldContain("FROM WorkItems");
}
```

### Testing Unsupported Operations

```csharp
[TestMethod]
public void Should_throw_for_unsupported_operation()
{
    Should.Throw<NotSupportedException>(() =>
    {
        var query = _store!.Query<WorkItem>()
            .Where(wi => wi.Title.ToUpper().Contains("BUG"));
        query.ToList();
    });
}
```

## Related Components

- **Qwiq.Linq** - Component under test
- **Qwiq.Mocks** - Mock work item store
- **Qwiq.Tests.Common** - Test infrastructure
