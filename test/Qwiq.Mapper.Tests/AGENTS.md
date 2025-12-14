# Qwiq.Mapper.Tests Component Guide

## Component Overview

**Qwiq.Mapper.Tests** contains unit tests for the Qwiq.Mapper object mapping functionality, testing attribute-based work item to POCO mapping.

## Purpose

- Test attribute-based field mapping
- Verify WorkItemMapper behavior
- Test mapping strategies (AttributeMapperStrategy, BulkIdentityAwareAttributeMapperStrategy)
- Validate type conversion and validation

## Key Characteristics

- **Target Frameworks**: `net472;net48;net481;net8.0;net9.0;net10.0`
- **Test Framework**: MSTest
- **Pattern**: ContextSpecification
- **Focus**: Object mapping

## Test Categories

### Mapping Tests

```csharp
[TestMethod]
public void Should_map_work_item_to_poco()
{
    var workItem = new MockWorkItem("Bug")
    {
        Id = 123,
        Title = "Test",
        ["System.State"] = "Active"
    };

    var bug = _mapper!.Create<Bug>(new[] { workItem }).Single();

    bug.Id.ShouldBe(123);
    bug.Title.ShouldBe("Test");
    bug.State.ShouldBe("Active");
}
```

### Attribute Tests

- [WorkItemType] attribute recognition
- [FieldDefinition] field mapping
- [IdentityField] identity resolution

### Strategy Tests

- AttributeMapperStrategy behavior
- BulkIdentityAwareAttributeMapperStrategy with identity service
- Custom strategy implementation

## Common Test Patterns

### Testing Field Mapping

```csharp
[WorkItemType("Bug")]
public class TestBug
{
    [FieldDefinition(CoreFieldRefNames.Id)]
    public int? Id { get; set; }

    [FieldDefinition(CoreFieldRefNames.Title)]
    public string? Title { get; set; }
}
```

### Testing Identity Resolution

```csharp
public override void Given()
{
    var identityService = new MockIdentityManagementService();
    identityService.AddIdentity("user@domain.com", "Display Name");

    var strategy = new BulkIdentityAwareAttributeMapperStrategy(identityService);
    _mapper = new WorkItemMapper(strategy);
}
```

## Related Components

- **Qwiq.Mapper** - Component under test
- **Qwiq.Mocks** - Mock work items and identity service
- **Qwiq.Tests.Common** - Test infrastructure
