---
name: sandbox-validation
description: Integration test environment validation for QWIQ. Use when setting up the qwiq-sandbox Azure DevOps environment, verifying test prerequisites, or troubleshooting integration test failures.
---

# Sandbox Validation Skill

## Purpose

This skill helps validate and troubleshoot the qwiq-sandbox Azure DevOps environment used for integration testing. It ensures the sandbox has the required work items, queries, and configuration for tests to pass.

## When To Use

- Setting up integration test environment
- Troubleshooting integration test failures
- Verifying sandbox configuration after changes
- Onboarding new contributors to run integration tests

## Instructions

### 1. Run Validation Script

```powershell
# Using environment variable
$env:AZURE_DEVOPS_PAT = "your-pat-here"
./scripts/Validate-SandboxEnvironment.ps1

# Using parameter
./scripts/Validate-SandboxEnvironment.ps1 -PersonalAccessToken "your-pat-here"
```

### 2. What Gets Validated

The script checks:

- ✅ Connection to Azure DevOps organization
- ✅ Existence of WIT project
- ✅ Required work items (IDs 1-7)
- ✅ Work Item 1 assigned to test user
- ✅ Work Item 5 has attachments
- ✅ Shared Queries folder exists

### 3. Required PAT Scopes

Your Personal Access Token needs:

- Work Items (Read & Write)
- Project and Team (Read)
- Identity (Read)

### 4. Sandbox Environment Details

| Setting      | Value                                    |
| ------------ | ---------------------------------------- |
| Organization | `https://qwiq-sandbox.visualstudio.com/` |
| Project      | `WIT`                                    |
| Project ID   | `0a4c0240-1a67-45de-93db-fc1de9f54ffb`   |
| Test User    | `rjmurillo@msn.com`                      |

### 5. Test Work Items

| ID  | Type       | Purpose                    |
| --- | ---------- | -------------------------- |
| 1   | Bug        | Basic work item tests      |
| 2   | Task       | Child of ID 3 (hierarchy)  |
| 3   | User Story | Parent for hierarchy tests |
| 4   | Bug        | Mapper tests               |
| 5   | Bug        | Work item with links       |
| 6   | Task       | Second child of ID 3       |

### 6. Troubleshooting Failures

**Connection failed:**

- Verify PAT is valid and not expired
- Check network connectivity to visualstudio.com
- Ensure PAT has required scopes

**Work item not found:**

- Work item may have been deleted
- Check if ID matches `TestData.cs` constants
- May need to recreate work item in sandbox

**Assignment check failed:**

- Work Item 1 must be assigned to test user
- Update assignment in Azure DevOps UI

## Helper Scripts

### scripts/Validate-SandboxEnvironment.ps1

**Purpose:** Validate qwiq-sandbox environment for integration tests

**Input:**

- `-PersonalAccessToken`: PAT with required scopes (or `$env:AZURE_DEVOPS_PAT`)
- `-Organization`: Override URL (default: `https://qwiq-sandbox.visualstudio.com`)
- `-Project`: Override project (default: `WIT`)

**Output:**

- Console: Pass/fail status for each check
- Exit code: 0 = all passed, 1 = failures, 2 = script error

**Usage:**

```powershell
# Basic validation
./scripts/Validate-SandboxEnvironment.ps1 -PersonalAccessToken $pat

# Custom organization
./scripts/Validate-SandboxEnvironment.ps1 -PersonalAccessToken $pat -Organization "https://myorg.visualstudio.com"
```

## Examples

### Example 1: First-Time Setup

**User goal:** Verify sandbox is ready for integration tests

**Process:**

1. Create PAT in Azure DevOps with required scopes
2. Set `$env:AZURE_DEVOPS_PAT = "your-pat"`
3. Run `./scripts/Validate-SandboxEnvironment.ps1`
4. Fix any failures reported
5. Run integration tests

**Expected output:** All checks pass, ready to run tests

### Example 2: Debugging Test Failure

**User goal:** Integration test fails with "work item not found"

**Process:**

1. Run validation script to check work item exists
2. If missing, check `TestData.cs` for expected ID
3. Either recreate work item or update `TestData.cs`
4. Re-run validation to confirm fix
5. Re-run failing test

**Expected output:** Work item exists, test passes

## Related Resources

- See [../qwiq-testing/SKILL.md](../qwiq-testing/SKILL.md) for testing patterns and test categories
- See [../qwiq-testing/REFERENCE.md](../qwiq-testing/REFERENCE.md) for test data constants
- See [../wiremock-capture/SKILL.md](../wiremock-capture/SKILL.md) for creating offline test fixtures
- See `test/Qwiq.Integration.Tests/TestData.cs` for work item IDs
