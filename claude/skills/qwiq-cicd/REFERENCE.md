# QWIQ CI/CD Reference Documentation

## Workflow Files

| File                         | Purpose            | Triggers                    |
| ---------------------------- | ------------------ | --------------------------- |
| `.github/workflows/main.yml` | Primary build/test | Push to develop/master, PRs |

## Runner Requirements

| Runner           | Required For                      |
| ---------------- | --------------------------------- |
| `windows-latest` | All jobs (net472 SOAP dependency) |

**Note:** Do NOT use `windows-2019` - it is retired.

## Build Flags Reference

### Always Use in CI

```yaml
/p:Deterministic=true
/p:UseSharedCompilation=false
/nodeReuse:false
```

### Optional Debugging

```yaml
/bl:./artifacts/logs/build.binlog  # Binary log for debugging
/v:detailed                         # Verbose output
```

## Test Filter Syntax

```yaml
# Exclude multiple categories
--filter "TestCategory!=localOnly&TestCategory!=Benchmark"

# Include specific category
--filter "TestCategory=UnitTest"

# By name pattern
--filter "FullyQualifiedName~WorkItem"
```

## Secrets and Variables

| Secret/Variable | Purpose                          |
| --------------- | -------------------------------- |
| `GITHUB_TOKEN`  | Auto-provided, for releases      |
| `NUGET_API_KEY` | NuGet publishing (if configured) |

## Conventional Commits for CI

CI may enforce commit message format:

```text
<type>(<scope>): <description>

Types: fix, feat, refactor, docs, test, chore, style, build, ci
Scopes: core, rest, soap, linq, mapper, identity, ci
```

## Artifact Paths

With `ArtifactsPath` enabled:

| Artifact     | Path                                |
| ------------ | ----------------------------------- |
| Packages     | `artifacts/package/Release/*.nupkg` |
| Binlogs      | `artifacts/logs/Release/*.binlog`   |
| Test results | `artifacts/TestResults/**/*.trx`    |

## Troubleshooting Checklist

- [ ] Using `windows-latest` runner?
- [ ] `fetch-depth: 0` for versioning?
- [ ] `dotnet tool restore` before build?
- [ ] Using `global-json-file` not `dotnet-version`?
- [ ] Test filters exclude integration tests?
- [ ] Artifacts uploaded with `if: always()`?
