# Contract Tests Plan: Mock Fidelity Verification

## Wave 2.3: Contract Tests Between Qwiq.Mocks and REST/SOAP Implementations

**Version:** 1.0
**Date:** December 2025
**Status:** Planning

---

## 1. Executive Summary

### 1.1 Problem Statement

The Qwiq library uses mock implementations (`Qwiq.Mocks`) extensively throughout its test suite to simulate Azure DevOps behavior. If these mocks do not faithfully represent the behavior of the actual REST and SOAP implementations, tests will pass in isolation but fail in production.

### 1.2 Design Goals

1. **Behavioral Fidelity**: Ensure mocks match REST/SOAP behavior for all interface contracts
2. **Exception Parity**: Verify mocks throw the same exceptions under the same conditions
3. **Null Handling Consistency**: Validate null/missing value behavior is identical
4. **Shared Test Fixtures**: Create reusable test infrastructure that runs against all implementations
5. **CI Integration**: Tests should run in CI without requiring actual Azure DevOps connection

---

## 2. Interface Contract Analysis

### 2.1 Core Interfaces to Verify

| Interface              | Mock Class                | REST Class                                | SOAP Class                        | Priority |
| ---------------------- | ------------------------- | ----------------------------------------- | --------------------------------- | -------- |
| `IWorkItem`            | `MockWorkItem`            | `Client.Rest.WorkItem`                    | `Client.Soap.WorkItem`            | P0       |
| `IWorkItemStore`       | `MockWorkItemStore`       | `Client.Rest.WorkItemStore`               | `Client.Soap.WorkItemStore`       | P0       |
| `IFieldDefinition`     | `MockFieldDefinition`     | `Client.Rest.FieldDefinition`             | `Client.Soap.FieldDefinition`     | P1       |
| `IField`               | `MockField`               | `Client.Rest.Field` (via FieldCollection) | `Client.Soap.Field`               | P1       |
| `IFieldCollection`     | `MockFieldCollection`     | `Client.Rest.FieldCollection`             | `Client.Soap.FieldCollection`     | P1       |
| `IWorkItemLinkType`    | `MockWorkItemLinkType`    | `WorkItemLinkType`                        | `Client.Soap.WorkItemLinkType`    | P1       |
| `IWorkItemLinkTypeEnd` | `MockWorkItemLinkTypeEnd` | `Client.Rest.WorkItemLinkTypeEnd`         | `Client.Soap.WorkItemLinkTypeEnd` | P1       |
| `IProject`             | `MockProject`             | `Client.Rest.Project`                     | `Client.Soap.Project`             | P2       |
| `IRelatedLink`         | `MockRelatedLink`         | (via LinkCollection)                      | `Client.Soap.RelatedLink`         | P2       |

### 2.2 Critical Behavioral Contracts

#### 2.2.1 IWorkItem Contract

```text
Property Access Contracts:
- Id: Returns 0 for new items, positive int after save
- Fields[name]: Returns field value, throws FieldDefinitionNotExistException for unknown field
- Type: Never null, always returns associated IWorkItemType
- IsDirty: True when fields modified, false after save
- Links: Returns collection, may be empty but not null

Method Contracts:
- Save(): Throws InvalidOperationException if !IsValid()
- Copy(): Creates new work item with cloned fields
- CreateRelatedLink(): Throws InvalidOperationException if IsNew
- Validate(): Returns empty enumerable if valid, field collection otherwise
```

#### 2.2.2 IWorkItemStore Contract

```text
Query Contracts:
- Query(int id): Returns null for non-existent ID
- Query(IEnumerable<int> ids): Returns empty collection for empty input
- Query(string wiql): Returns matching items, may be empty
- QueryLinks(string wiql): Returns link info enumerable

Exception Contracts:
- Query with malformed WIQL: Should throw appropriate exception
- Query with unauthorized access: Should throw AccessDeniedException
```

#### 2.2.3 IFieldDefinition Contract

```text
Property Contracts:
- Id: Stable identifier, matches CoreFieldRefNames for system fields
- Name: Friendly name, never null
- ReferenceName: System reference name, never null
```

#### 2.2.4 IWorkItemLinkType Contract

```text
Property Contracts:
- ForwardEnd: Never null
- ReverseEnd: Never null, equals ForwardEnd for non-directional links
- IsDirectional: Boolean indicating if forward/reverse are different
- IsActive: Boolean indicating if link type is enabled
- ReferenceName: System reference name matching CoreLinkTypeReferenceNames
```

---

## 3. Contract Test Design

### 3.1 Architecture Pattern: Parameterized Fixtures

The recommended approach uses **parameterized test fixtures** with a common base class. This allows the same tests to run against Mock, REST, and SOAP implementations.

```text
+------------------------------------------+
|     IImplementationProvider<T>           |
|  - CreateWorkItem()                      |
|  - CreateWorkItemStore()                 |
|  - CreateFieldDefinition()               |
+------------------------------------------+
           ^           ^            ^
           |           |            |
    +------+     +-----+     +------+
    |            |           |
+--------+  +--------+  +---------+
| Mock   |  | REST   |  | SOAP    |
|Provider|  |Provider|  |Provider |
+--------+  +--------+  +---------+
```

### 3.2 Test Categories

1. **Contract Tests** (always run): Test behavioral contracts without real services
2. **Fidelity Tests** (run with services): Compare mock behavior to actual REST/SOAP
3. **Exception Contract Tests**: Verify exception types and conditions match

### 3.3 Project Structure

```text
test/
  Qwiq.Contract.Tests/                    # NEW PROJECT
    Qwiq.Contract.Tests.csproj
    Infrastructure/
      IImplementationProvider.cs          # Provider interface
      MockImplementationProvider.cs       # Uses Qwiq.Mocks
      RestImplementationProvider.cs       # Uses REST with WireMock
      SoapImplementationProvider.cs       # Uses SOAP with mocked TFS
      ContractTestBase.cs                 # Base class for all contract tests
    Contracts/
      WorkItem/
        WorkItemPropertyContractTests.cs
        WorkItemFieldAccessContractTests.cs
        WorkItemSaveContractTests.cs
        WorkItemCopyContractTests.cs
        WorkItemLinkContractTests.cs
      WorkItemStore/
        WorkItemStoreQueryContractTests.cs
        WorkItemStoreLinkQueryContractTests.cs
        WorkItemStoreExceptionContractTests.cs
      Field/
        FieldDefinitionContractTests.cs
        FieldCollectionContractTests.cs
        FieldValueContractTests.cs
      Link/
        WorkItemLinkTypeContractTests.cs
        WorkItemLinkTypeEndContractTests.cs
        RelatedLinkContractTests.cs
    Fidelity/                             # Optional: Compare mock vs real
      MockToRestFidelityTests.cs
      MockToSoapFidelityTests.cs
```

### 3.4 WireMock Integration

For REST contract tests without live Azure DevOps, use WireMock to simulate responses:

```csharp
public class RestImplementationProvider : IImplementationProvider<IWorkItem>
{
    private readonly WireMockServer _server;

    public RestImplementationProvider()
    {
        _server = WireMockServer.Start();
        ConfigureWorkItemResponses();
    }

    private void ConfigureWorkItemResponses()
    {
        _server.Given(
            Request.Create()
                .WithPath("/_apis/wit/workitems/*")
                .UsingGet())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBodyFromFile("TestData/workitem-response.json"));
    }
}
```

---

## 4. Detailed Test Cases

### 4.1 IWorkItem Contract Tests

#### 4.1.1 Property Access

| Test Case                              | Mock Expected | REST/SOAP Expected | Notes                   |
| -------------------------------------- | ------------- | ------------------ | ----------------------- |
| `Id_WhenNew_ReturnsZero`               | 0             | 0                  |                         |
| `Id_AfterSave_ReturnsPositiveInt`      | >0            | >0                 |                         |
| `Type_Always_NotNull`                  | Not null      | Not null           |                         |
| `Fields_Always_NotNull`                | Not null      | Not null           |                         |
| `IsDirty_WhenNew_ReturnsFalse`         | false         | false              | VERIFY: Mock may differ |
| `IsDirty_AfterFieldChange_ReturnsTrue` | true          | true               |                         |
| `Links_WhenNew_ReturnsEmptyCollection` | Empty         | Empty              | Not null                |

#### 4.1.2 Field Access

| Test Case                            | Mock Expected | REST/SOAP Expected | Notes            |
| ------------------------------------ | ------------- | ------------------ | ---------------- |
| `Field_ByReferenceName_ReturnsValue` | Value         | Value              |                  |
| `Field_ByFriendlyName_ReturnsValue`  | Value         | Value              | Case-insensitive |
| `Field_NonExistent_ThrowsException`  | Exception     | Exception          | Type must match  |
| `Field_SetValue_UpdatesIsDirty`      | true          | true               |                  |
| `Field_SetSameValue_NotDirty`        | false         | false              | VERIFY           |

#### 4.1.3 Save Operations

| Test Case                          | Mock Expected | REST/SOAP Expected | Notes |
| ---------------------------------- | ------------- | ------------------ | ----- |
| `Save_WhenValid_SetsId`            | Id > 0        | Id > 0             |       |
| `Save_WhenInvalid_ThrowsException` | Exception     | Exception          |       |
| `Save_ClearsIsDirty`               | false         | false              |       |

#### 4.1.4 Link Operations

| Test Case                                   | Mock Expected | REST/SOAP Expected | Notes |
| ------------------------------------------- | ------------- | ------------------ | ----- |
| `CreateRelatedLink_WhenNew_ThrowsException` | InvalidOp     | InvalidOp          |       |
| `CreateRelatedLink_AfterSave_ReturnsLink`   | Link          | Link               |       |
| `Links_Add_UpdatesRelatedLinkCount`         | +1            | +1                 |       |

### 4.2 IWorkItemStore Contract Tests

#### 4.2.1 Query by ID

| Test Case                          | Mock Expected | REST/SOAP Expected | Notes |
| ---------------------------------- | ------------- | ------------------ | ----- |
| `Query_SingleId_ReturnsWorkItem`   | WorkItem      | WorkItem           |       |
| `Query_NonExistentId_ReturnsNull`  | null          | null               |       |
| `Query_MultipleIds_ReturnsAll`     | Collection    | Collection         |       |
| `Query_EmptyIds_ReturnsEmpty`      | Empty         | Empty              |       |
| `Query_NullIds_ThrowsArgumentNull` | ArgNull       | ArgNull            |       |

#### 4.2.2 Query by WIQL

| Test Case                           | Mock Expected | REST/SOAP Expected | Notes           |
| ----------------------------------- | ------------- | ------------------ | --------------- |
| `Query_ValidWiql_ReturnsMatches`    | Collection    | Collection         |                 |
| `Query_NoMatches_ReturnsEmpty`      | Empty         | Empty              |                 |
| `Query_InvalidWiql_ThrowsException` | Exception     | Exception          | Type may differ |

#### 4.2.3 Link Queries

| Test Case                         | Mock Expected | REST/SOAP Expected | Notes |
| --------------------------------- | ------------- | ------------------ | ----- |
| `QueryLinks_ReturnsLinkInfo`      | LinkInfo[]    | LinkInfo[]         |       |
| `QueryLinks_NoLinks_ReturnsEmpty` | Empty         | Empty              |       |

### 4.3 IFieldDefinition Contract Tests

| Test Case                              | Mock Expected | REST/SOAP Expected | Notes                 |
| -------------------------------------- | ------------- | ------------------ | --------------------- |
| `Id_CoreField_MatchesKnownValue`       | Known ID      | Known ID           | Use CoreFieldRefNames |
| `Name_NotNullOrEmpty`                  | Valid         | Valid              |                       |
| `ReferenceName_NotNullOrEmpty`         | Valid         | Valid              |                       |
| `Equals_SameReferenceName_ReturnsTrue` | true          | true               |                       |

### 4.4 IWorkItemLinkType Contract Tests

| Test Case                            | Mock Expected | REST/SOAP Expected | Notes |
| ------------------------------------ | ------------- | ------------------ | ----- |
| `Hierarchy_IsDirectional`            | true          | true               |       |
| `Hierarchy_ForwardEnd_IsChild`       | "Child"       | "Child"            |       |
| `Hierarchy_ReverseEnd_IsParent`      | "Parent"      | "Parent"           |       |
| `Related_IsNotDirectional`           | false         | false              |       |
| `Related_ForwardEquals_Reverse`      | Same          | Same               |       |
| `ForwardEnd_Id_MatchesCoreLinkTypes` | Known         | Known              |       |

### 4.5 Exception Contract Tests

| Scenario               | Expected Exception Type                 | Mock Throws | REST Throws | SOAP Throws |
| ---------------------- | --------------------------------------- | ----------- | ----------- | ----------- |
| Field not found        | `FieldDefinitionNotExistException`      | VERIFY      | VERIFY      | VERIFY      |
| Project not found      | `DeniedOrNotExistException`             | VERIFY      | VERIFY      | VERIFY      |
| Invalid work item type | `WorkItemTypeDeniedOrNotExistException` | VERIFY      | VERIFY      | VERIFY      |
| Access denied          | `AccessDeniedException`                 | VERIFY      | VERIFY      | VERIFY      |
| Invalid WIQL           | Implementation-specific                 | VERIFY      | VERIFY      | VERIFY      |

---

## 5. Implementation Plan

### Phase 1: Infrastructure Setup (2-3 hours)

1. Create `Qwiq.Contract.Tests` project
2. Implement `IImplementationProvider<T>` interface
3. Create `MockImplementationProvider`
4. Create `ContractTestBase` with xUnit theory data sources

### Phase 2: Core Contract Tests (4-6 hours)

1. `WorkItemPropertyContractTests` - All property access contracts
2. `WorkItemFieldAccessContractTests` - Field indexer behavior
3. `FieldDefinitionContractTests` - Field definition contracts
4. `WorkItemLinkTypeContractTests` - Link type contracts

### Phase 3: Store Contract Tests (3-4 hours)

1. `WorkItemStoreQueryContractTests` - Query behaviors
2. `WorkItemStoreExceptionContractTests` - Exception conditions

### Phase 4: WireMock REST Provider (4-6 hours)

1. Create `RestImplementationProvider`
2. Configure WireMock responses for standard scenarios
3. Add REST-specific contract verification

### Phase 5: Exception Fidelity (2-3 hours)

1. Document all exception scenarios
2. Implement exception contract tests
3. Fix mock exception behavior where needed

### Phase 6: Integration (2-3 hours)

1. Add contract tests to CI pipeline
2. Configure test categories for different run modes
3. Document how to run fidelity tests locally

---

## 6. Sample Implementation

### 6.1 IImplementationProvider Interface

```csharp
namespace Qwiq.Contract.Tests.Infrastructure
{
    /// <summary>
    /// Provides implementations of Qwiq interfaces for contract testing.
    /// </summary>
    public interface IImplementationProvider : IDisposable
    {
        /// <summary>
        /// Gets the name of this provider for test reporting.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Creates a new work item store.
        /// </summary>
        IWorkItemStore CreateWorkItemStore();

        /// <summary>
        /// Creates a new work item of the specified type.
        /// </summary>
        IWorkItem CreateWorkItem(string workItemType);

        /// <summary>
        /// Creates a work item with the specified field values.
        /// </summary>
        IWorkItem CreateWorkItem(string workItemType, IDictionary<string, object?> fields);

        /// <summary>
        /// Gets an existing work item by ID.
        /// </summary>
        IWorkItem? GetWorkItem(int id);

        /// <summary>
        /// Gets field definitions for the store.
        /// </summary>
        IFieldDefinitionCollection GetFieldDefinitions();

        /// <summary>
        /// Gets work item link types for the store.
        /// </summary>
        IWorkItemLinkTypeCollection GetWorkItemLinkTypes();
    }
}
```

### 6.2 ContractTestBase

```csharp
namespace Qwiq.Contract.Tests.Infrastructure
{
    public abstract class ContractTestBase : IDisposable
    {
        protected IImplementationProvider Provider { get; }

        protected ContractTestBase(IImplementationProvider provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public void Dispose()
        {
            Provider?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Provides test data for all implementation providers.
    /// </summary>
    public class ImplementationProviderData : TheoryData<IImplementationProvider>
    {
        public ImplementationProviderData()
        {
            Add(new MockImplementationProvider());

            // Only add REST/SOAP providers in appropriate environments
            if (CanRunRestTests())
            {
                Add(new RestImplementationProvider());
            }

            if (CanRunSoapTests())
            {
                Add(new SoapImplementationProvider());
            }
        }

        private static bool CanRunRestTests() =>
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("QWIQ_REST_TESTS"));

        private static bool CanRunSoapTests() =>
            Environment.OSVersion.Platform == PlatformID.Win32NT &&
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("QWIQ_SOAP_TESTS"));
    }
}
```

### 6.3 Example Contract Test

```csharp
namespace Qwiq.Contract.Tests.Contracts.WorkItem
{
    public class WorkItemPropertyContractTests
    {
        [Theory]
        [ClassData(typeof(ImplementationProviderData))]
        public void Id_WhenNew_ReturnsZero(IImplementationProvider provider)
        {
            // Arrange
            using var _ = provider;
            var workItem = provider.CreateWorkItem("Bug");

            // Act
            var id = workItem.Id;

            // Assert
            id.ShouldBe(0, $"Provider: {provider.Name}");
        }

        [Theory]
        [ClassData(typeof(ImplementationProviderData))]
        public void Type_Always_NotNull(IImplementationProvider provider)
        {
            // Arrange
            using var _ = provider;
            var workItem = provider.CreateWorkItem("Bug");

            // Act
            var type = workItem.Type;

            // Assert
            type.ShouldNotBeNull($"Provider: {provider.Name}");
        }

        [Theory]
        [ClassData(typeof(ImplementationProviderData))]
        public void Fields_Always_NotNull(IImplementationProvider provider)
        {
            // Arrange
            using var _ = provider;
            var workItem = provider.CreateWorkItem("Bug");

            // Act
            var fields = workItem.Fields;

            // Assert
            fields.ShouldNotBeNull($"Provider: {provider.Name}");
        }

        [Theory]
        [ClassData(typeof(ImplementationProviderData))]
        public void IsDirty_AfterFieldChange_ReturnsTrue(IImplementationProvider provider)
        {
            // Arrange
            using var _ = provider;
            var workItem = provider.CreateWorkItem("Bug");
            var originalDirty = workItem.IsDirty;

            // Act
            workItem[CoreFieldRefNames.Title] = "Modified Title";

            // Assert
            workItem.IsDirty.ShouldBeTrue($"Provider: {provider.Name}");
        }
    }
}
```

### 6.4 Exception Contract Test

```csharp
namespace Qwiq.Contract.Tests.Contracts.WorkItem
{
    public class WorkItemExceptionContractTests
    {
        [Theory]
        [ClassData(typeof(ImplementationProviderData))]
        public void Field_NonExistent_ThrowsFieldDefinitionNotExistException(
            IImplementationProvider provider)
        {
            // Arrange
            using var _ = provider;
            var workItem = provider.CreateWorkItem("Bug");

            // Act & Assert
            var exception = Should.Throw<Exception>(() =>
            {
                _ = workItem["NonExistent.Field.That.Does.Not.Exist"];
            });

            // The exception should be FieldDefinitionNotExistException or derived
            // or contain it as inner exception
            exception.ShouldSatisfyAllConditions(
                () => exception.ShouldNotBeNull(),
                () => (exception is FieldDefinitionNotExistException ||
                       exception is InvalidOperationException ||
                       exception.InnerException is FieldDefinitionNotExistException)
                      .ShouldBeTrue($"Provider: {provider.Name}, Got: {exception.GetType().Name}")
            );
        }

        [Theory]
        [ClassData(typeof(ImplementationProviderData))]
        public void CreateRelatedLink_WhenNew_ThrowsInvalidOperationException(
            IImplementationProvider provider)
        {
            // Arrange
            using var _ = provider;
            var workItem = provider.CreateWorkItem("Bug");

            // Act & Assert
            Should.Throw<InvalidOperationException>(() =>
            {
                workItem.CreateRelatedLink(999);
            }, $"Provider: {provider.Name}");
        }
    }
}
```

---

## 7. Known Behavioral Differences

Document known differences that are acceptable:

| Behavior                | Mock       | REST            | SOAP            | Resolution                       |
| ----------------------- | ---------- | --------------- | --------------- | -------------------------------- |
| `IsDirty` initial state | false      | N/A (read-only) | false           | Mock behavior is correct         |
| Field ID generation     | Hash-based | Server-assigned | Server-assigned | Document as expected             |
| Exception messages      | Generic    | API-specific    | SOAP-specific   | Test exception type, not message |

---

## 8. CI/CD Integration

### 8.1 Test Categories

```xml
<!-- In test project -->
<PropertyGroup>
  <TestCategories>ContractTest</TestCategories>
</PropertyGroup>
```

### 8.2 GitHub Actions

```yaml
- name: Run Contract Tests (Mock Only)
  run: dotnet test --filter "Category=ContractTest" --logger "trx;LogFileName=contract-tests.trx"

- name: Run Fidelity Tests (With Services)
  if: github.event_name == 'schedule' || github.event.inputs.run_fidelity == 'true'
  env:
    QWIQ_REST_TESTS: "true"
    AZURE_DEVOPS_PAT: ${{ secrets.AZURE_DEVOPS_PAT }}
  run: dotnet test --filter "Category=FidelityTest" --logger "trx;LogFileName=fidelity-tests.trx"
```

---

## 9. Success Criteria

1. All contract tests pass for `MockImplementationProvider`
2. Contract tests identify any mock/implementation divergence
3. Clear documentation of expected vs actual behavior
4. CI runs contract tests on every PR
5. Fidelity tests can run on-demand with real services

---

## 10. Risks and Mitigations

| Risk                                 | Impact | Mitigation                                            |
| ------------------------------------ | ------ | ----------------------------------------------------- |
| Mock behavior differs significantly  | High   | Document differences, decide if mock or impl is wrong |
| REST/SOAP not available in CI        | Medium | WireMock for REST, skip SOAP in non-Windows           |
| Test maintenance overhead            | Medium | Keep tests focused on contracts, not implementation   |
| Breaking changes in Azure DevOps API | Low    | WireMock responses can be versioned                   |

---

## 11. References

- [Azure DevOps REST API](https://docs.microsoft.com/en-us/rest/api/azure/devops/)
- [WireMock.Net Documentation](https://github.com/WireMock-Net/WireMock.Net)
- [Contract Testing Pattern](https://martinfowler.com/bliki/ContractTest.html)
- Existing integration tests: `test/Qwiq.Integration.Tests/`
