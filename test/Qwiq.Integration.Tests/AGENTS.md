# Qwiq.Integration.Tests Component Guide

## Component Overview

**Qwiq.Integration.Tests** contains integration tests that connect to real Azure DevOps / TFS servers to verify end-to-end functionality.

## Purpose

- Test REST and SOAP client implementations against real servers
- Verify actual Azure DevOps / TFS behavior
- Validate authentication flows
- Compare REST vs SOAP API differences

## Key Characteristics

- **Target Frameworks**: `net472` (supports both REST and SOAP)
- **Test Framework**: MSTest
- **Test Categories**: `REST`, `SOAP`, `IntegrationTests`
- **Sandbox**: qwiq-sandbox Azure DevOps organization

## ⚠️ Critical: Interactive Authentication

**All integration tests present an interactive login dialog** for credential input. This makes them:

✅ **Suitable for**: Local development, manual testing
❌ **Not suitable for**: Headless CI/CD, automated pipelines

## Test Environment

### Sandbox Organization

- **Organization URL**: `https://qwiq-sandbox.visualstudio.com/`
- **Project**: `WIT` (GUID: `0a4c0240-1a67-45de-93db-fc1de9f54ffb`)
- **Test User**: Richard Murillo (`rjmurillo@msn.com`)

### Test Work Items

Use `TestData` constants from `Qwiq.Tests.Common`:

| ID  | Type       | Title                | Constant                     |
| --- | ---------- | -------------------- | ---------------------------- |
| 1   | Bug        | Integration Test     | TestData.BasicWorkItemId     |
| 2   | Task       | Child Task           | TestData.HierarchyChildId    |
| 3   | User Story | Parent Story         | TestData.HierarchyParentId   |
| 4   | Bug        | Bug for Mapper       | TestData.MapperBugId         |
| 5   | Bug        | Work Item with Links | TestData.WorkItemWithLinksId |

## Test Categories

### [TestCategory("REST")]

Tests REST API client implementation:

```csharp
[TestMethod]
[TestCategory("REST")]
public void Should_query_work_items_via_rest()
{
    var store = CreateRestStore();
    var items = store.Query($"SELECT [System.Id] FROM WorkItems WHERE [System.Id] = {TestData.BasicWorkItemId}");
    items.ShouldHaveSingleItem();
}
```

### [TestCategory("SOAP")]

Tests SOAP API client implementation (Windows only, requires domain auth):

```csharp
[TestMethod]
[TestCategory("SOAP")]
public void Should_query_work_items_via_soap()
{
    var store = CreateSoapStore();
    var items = store.Query($"SELECT [System.Id] FROM WorkItems WHERE [System.Id] = {TestData.BasicWorkItemId}");
    items.ShouldHaveSingleItem();
}
```

### [TestCategory("IntegrationTests")]

General integration tests (may use either REST or SOAP).

## Running Integration Tests

### Run REST Tests Only

```powershell
dotnet test test/Qwiq.Integration.Tests/Qwiq.IntegrationTests.csproj --filter "TestCategory=REST"
```

### Run Excluding SOAP

```powershell
dotnet test test/Qwiq.Integration.Tests/Qwiq.IntegrationTests.csproj --filter "TestCategory!=SOAP"
```

### All Integration Tests

```powershell
dotnet test test/Qwiq.Integration.Tests/Qwiq.IntegrationTests.csproj --logger "console;verbosity=detailed"
```

**Note**: Will prompt for credentials via interactive dialog.

## Authentication

### REST Authentication

Uses Personal Access Token (PAT):

```csharp
var options = new AuthenticationOptions(
    new Uri(TestData.OrganizationUrl),
    AuthenticationTypes.PersonalAccessToken,
    () => new NetworkCredential("PAT", pat)
);
var store = Qwiq.Client.Rest.WorkItemStoreFactory.Default.Create(options);
```

### SOAP Authentication

Uses Windows authentication or MSA account:

```csharp
var options = new AuthenticationOptions(
    new Uri(TestData.OrganizationUrl),
    AuthenticationTypes.Windows,
    () => CredentialCache.DefaultNetworkCredentials
);
var store = Qwiq.Client.Soap.WorkItemStoreFactory.Default.Create(options);
```

## API Comparison Tests

Some tests compare REST and SOAP behavior to document known differences:

```csharp
[TestMethod]
public void REST_returns_additional_classification_fields()
{
    var restStore = CreateRestStore();
    var soapStore = CreateSoapStore();

    var restItem = restStore.GetWorkItem(TestData.BasicWorkItemId);
    var soapItem = soapStore.GetWorkItem(TestData.BasicWorkItemId);

    // REST returns System.AreaLevel1-7, SOAP does not
    restItem.Fields.Contains("System.AreaLevel1").ShouldBeTrue();
    soapItem.Fields.Contains("System.AreaLevel1").ShouldBeFalse();
}
```

## Common Test Patterns

### Using TestData Constants

```csharp
// ✅ CORRECT: Use TestData constants
var workItem = store.GetWorkItem(TestData.BasicWorkItemId);
var parent = store.GetWorkItem(TestData.HierarchyParentId);

// ❌ WRONG: Hardcoded IDs
var workItem = store.GetWorkItem(1);
```

### Organization URL Configuration

```csharp
// ✅ CORRECT: Organization-level URL
new Uri(TestData.OrganizationUrl)  // https://qwiq-sandbox.visualstudio.com/

// ❌ WRONG: Project-level URL
new Uri("https://qwiq-sandbox.visualstudio.com/WIT")
```

## Environment Variables

Optional overrides for test environment:

| Variable               | Purpose                   | Default                                  |
| ---------------------- | ------------------------- | ---------------------------------------- |
| `QWIQ_TEST_URL`        | Override organization URL | `https://qwiq-sandbox.visualstudio.com/` |
| `QWIQ_PROJECT_GUID`    | Override project GUID     | `0a4c0240-1a67-45de-93db-fc1de9f54ffb`   |
| `AZURE_DEVOPS_EXT_PAT` | PAT for authentication    | (prompts if not set)                     |

## Common Mistakes to Avoid

❌ **Don't run in CI without handling interactive auth** - Will hang waiting for input

❌ **Don't hardcode test work item IDs** - Use TestData constants

❌ **Don't use project URL as collection URI** - Use organization URL

❌ **Don't assume single test user** - Only one user (Richard Murillo) in sandbox

✅ **Do use TestData constants** - Centralized, documented values

✅ **Do run locally for verification** - Integration tests require real connectivity

✅ **Do document API differences** - REST vs SOAP behavior

## Related Components

- **Qwiq.Core.Rest** - REST client under test
- **Qwiq.Core.Soap** - SOAP client under test
- **Qwiq.Tests.Common** - TestData constants
- **Qwiq.WireMock.Tests** - HTTP traffic recording for mocking
