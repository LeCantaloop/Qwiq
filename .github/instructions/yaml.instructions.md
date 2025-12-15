---
applyTo: "**/*.{yml,yaml}"
---

# YAML File Instructions

> **MANDATORY**: You MUST follow these instructions when editing any YAML file in this repository.

## Quick Reference

- Primary YAML files: GitHub Actions workflows in `.github/workflows/`
- **Preferred**: `ubuntu-latest` (faster, cheaper)
- **Use `windows-latest` when**: Building net472 targets (avoids mono on Linux)
- Never use `windows-2019` (retired)
- Prefer `global-json-file: ./global.json` over `dotnet-version`
- Include `dotnet tool restore` for Nerdbank.GitVersioning

## Context Loading

When working on YAML files, you MUST:

1. Read this entire instruction file before making changes
2. Understand the CI/CD pipeline structure
3. Test workflows in a branch before merging
4. Complete the Validation Checklist before submitting

## GitHub Actions Workflows

### Standard .NET Workflow Structure

```yaml
name: Build

on:
  push:
    branches: [develop, master]
  pull_request:
    branches: [develop, master]

jobs:
  build:
    runs-on: windows-latest # Required for net472/SOAP projects

    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0 # Required for Nerdbank.GitVersioning

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          global-json-file: ./global.json # Use pinned SDK version

      - name: Restore tools
        run: dotnet tool restore # For nbgv

      - name: Restore packages
        run: dotnet restore Qwiq.sln

      - name: Build
        run: dotnet build Qwiq.sln -c Release /p:Deterministic=true

      - name: Test
        run: dotnet test Qwiq.sln --no-build -c Release --filter "TestCategory!=localOnly&TestCategory!=Benchmark"
```

### Key Requirements

| Requirement           | Reason                                              |
| --------------------- | --------------------------------------------------- |
| `ubuntu-latest`       | Preferred for non-build workflows (faster, cheaper) |
| `windows-latest`      | Required for net472 builds (avoids mono on Linux)   |
| `fetch-depth: 0`      | Nerdbank.GitVersioning needs full history           |
| `dotnet tool restore` | Restores nbgv from dotnet tool manifest             |
| `global-json-file`    | Uses pinned SDK version from repository             |

### GitHub CLI (gh) Support

The `copilot-setup-steps.yml` workflow includes `GH_TOKEN: ${{ github.token }}` to enable GitHub CLI commands and API access. This allows:

**For GitHub Agents/Copilot:**

- View workflow run logs and status
- Monitor action execution in real-time
- Query PR information and comments
- Check CI/CD pipeline status

**Common gh commands available:**

```bash
# View workflow runs
gh run list --workflow=main.yml

# Get workflow run status
gh run view <run-id>

# View workflow logs
gh run view <run-id> --log

# Check PR status
gh pr view <pr-number>

# List PR checks
gh pr checks <pr-number>
```

**Usage in workflow steps:**

```yaml
jobs:
  setup:
    env:
      GH_TOKEN: ${{ github.token }} # Available to all steps in copilot-setup-steps.yml

    steps:
      - name: Monitor other workflows
        run: gh run list --workflow=main.yml --limit 5
```

**Note:** For security and principle of least privilege, GH_TOKEN is only enabled in `copilot-setup-steps.yml`. Other workflows do not have GitHub CLI access unless specifically required.

### Deterministic Builds

Include these flags for reproducible builds:

```yaml
run: dotnet build /p:Deterministic=true /p:UseSharedCompilation=false /nodeReuse:false
```

### Test Filters

Use filters to exclude integration tests:

```yaml
run: dotnet test --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
```

Consider using `.runsettings` files for complex filters.

## Validation Checklist

Before submitting changes, verify:

- [ ] YAML syntax is valid
- [ ] Workflow runs successfully in branch
- [ ] All required steps are present
- [ ] Secrets are properly referenced (not hardcoded)
- [ ] Matrix strategies are correct if used

## Validation Evidence Requirements

Include in your PR description:

```markdown
## Workflow Validation

- [x] YAML syntax validated
- [x] Workflow tested in branch
- [x] All jobs completed successfully

## CI Evidence

Link to workflow run: [#123](link)
```

## Common Mistakes to AVOID

```yaml
# ❌ WRONG: Retired runner
runs-on: windows-2019

# ✅ CORRECT: Current runner
runs-on: windows-latest

# ❌ WRONG: Hardcoded SDK version
dotnet-version: '8.0.100'

# ✅ CORRECT: Use global.json
global-json-file: ./global.json

# ❌ WRONG: Missing fetch-depth for versioning
- uses: actions/checkout@v4

# ✅ CORRECT: Full history for GitVersioning
- uses: actions/checkout@v4
  with:
    fetch-depth: 0
```

## Decision Trees

### When Modifying Workflows

1. Is this a critical path change? → Test in branch first
2. Does this affect build/test? → Verify all jobs pass
3. Does this add secrets? → Use repository secrets, never hardcode

### When to Stop and Ask

- Adding new workflows
- Modifying deployment steps
- Changing secret handling
- Major restructuring of CI/CD pipeline

## Related Instruction Files

- [shell.instructions.md](shell.instructions.md) - For PowerShell scripts in workflows
- [generic.instructions.md](generic.instructions.md) - For multi-file changes
