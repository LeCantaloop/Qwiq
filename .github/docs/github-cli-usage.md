# GitHub CLI Usage for Agents

This document provides examples of how GitHub Copilot agents can use the GitHub CLI (`gh`) via the `copilot-setup-steps.yml` workflow.

## Environment Setup

The `copilot-setup-steps.yml` workflow includes:

```yaml
env:
  GH_TOKEN: ${{ github.token }}
```

This enables `gh` CLI commands without authentication prompts.

**Security Note:** For principle of least privilege, only the `copilot-setup-steps.yml` workflow has GH_TOKEN enabled. Other workflows do not have GitHub CLI access unless specifically required.

## Common Use Cases

### 1. Monitor Workflow Execution

When an agent makes changes and triggers a workflow:

```bash
# List recent runs of main workflow
gh run list --workflow=main.yml --limit 5

# Watch a specific run
gh run watch <run-id>

# View run status
gh run view <run-id>
```

### 2. Debug Failed Workflows

When a workflow fails, the agent can:

```bash
# View logs from failed run
gh run view <run-id> --log

# View logs for specific job
gh run view <run-id> --log --job=<job-id>

# Download all logs for analysis
gh run download <run-id>
```

### 3. Check PR Status

After creating a PR:

```bash
# View PR details
gh pr view <pr-number>

# Check all CI/CD checks
gh pr checks <pr-number>

# Watch checks in real-time
gh pr checks <pr-number> --watch

# View specific check logs
gh pr checks <pr-number> --required
```

### 4. Verify Changes

Before and after making changes:

```bash
# Get current workflow status
gh workflow view main.yml

# List recent failures
gh run list --workflow=main.yml --status=failure --limit 3

# Compare run results
gh run view <run-id-before>
gh run view <run-id-after>
```

## Agent Workflow Pattern

Recommended pattern for agents making changes:

1. **Make changes** to code/workflows
2. **Trigger workflow** via push/PR
3. **Get run ID**: `gh run list --workflow=main.yml --limit 1 --json databaseId --jq '.[0].databaseId'`
4. **Monitor execution**: `gh run watch <run-id>`
5. **If failure, get logs**: `gh run view <run-id> --log`
6. **Analyze and fix**: Review logs, make corrections
7. **Verify fix**: Repeat until success

## Example: Complete Monitoring Script

```bash
#!/bin/bash
# Monitor the most recent workflow run

WORKFLOW="main.yml"
RUN_ID=$(gh run list --workflow=$WORKFLOW --limit 1 --json databaseId --jq '.[0].databaseId')

echo "Monitoring run: $RUN_ID"
gh run watch $RUN_ID

if [ $? -eq 0 ]; then
  echo "✅ Workflow succeeded!"
  gh run view $RUN_ID
else
  echo "❌ Workflow failed. Fetching logs..."
  gh run view $RUN_ID --log
fi
```

## Available Permissions

The `github.token` provides permissions based on the workflow's `permissions:` block:

- **Read**: Repository contents, actions, workflows
- **Write**: Varies by workflow (check individual workflow permissions)

For elevated permissions (like approving PRs), workflows use PAT secrets instead.

## Troubleshooting

### "Resource not accessible by integration"

The automatic token doesn't have permission. Check:

1. Workflow's `permissions:` block
2. Whether the operation requires a PAT secret
3. Repository settings for Actions permissions

### "gh: command not found"

The `gh` CLI is pre-installed on all GitHub-hosted runners (ubuntu-latest, windows-latest, macos-latest).

## Related Documentation

- [GitHub CLI Manual](https://cli.github.com/manual/)
- [GitHub Actions Automatic Token](https://docs.github.com/en/actions/security-guides/automatic-token-authentication)
- [yaml.instructions.md](../instructions/yaml.instructions.md) - Workflow guidelines
- [copilot-instructions.md](../copilot-instructions.md) - Repository instructions
