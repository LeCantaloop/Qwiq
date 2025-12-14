# Task Completion Checklist

When completing a task in the Qwiq project, follow this checklist:

## Before Committing

### 1. Build Successfully

```powershell
dotnet build Qwiq.sln -c Release
```

### 2. Run Unit Tests

```powershell
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
```

### 3. Format Code

```powershell
# C# formatting
dotnet format Qwiq.sln

# Markdown/JSON/YAML formatting
dotnet pprettier --write .
```

### 4. Lint Markdown (if changed)

```powershell
npx markdownlint-cli2 --fix "**/*.md"
```

### 5. Check for Analyzer Warnings

```powershell
# Strict mode - CI uses this
dotnet build Qwiq.sln -c Release /p:PedanticMode=true
```

## Commit Guidelines

### Conventional Commit Format

```text
type(scope): short description

[optional body]

[optional footer]
```

### Types

- `fix`: Bug fix
- `feat`: New feature
- `refactor`: Code refactoring (no functional change)
- `docs`: Documentation only
- `test`: Test changes
- `chore`: Build/tooling changes

### Example

```powershell
git commit -m "fix(linq): correct WIQL translation for InGroup operator"
```

## Pull Request Checklist

1. ✅ All tests pass
2. ✅ No new analyzer warnings
3. ✅ Code is formatted
4. ✅ Markdown is linted
5. ✅ Conventional commit messages
6. ✅ PR description explains the change
7. ✅ Breaking changes documented (if any)

## CI Build Notes

The CI build uses:

- `/m:1 /nodeReuse:false` - Single-threaded to avoid file locking
- `/p:PedanticMode=true` - Warnings as errors
- Test filter excludes integration tests

## Agent Output Directories

When working with agents, save artifacts to:

- `.agents/analysis/` - Research reports
- `.agents/architecture/` - ADRs
- `.agents/planning/` - Plans, PRDs, tasks
- `.agents/qa/` - Test strategies

## Memory Updates

After significant learning or task completion:

1. Use `write_memory` to store new knowledge
2. Use `edit_memory` to update existing memories
3. Consider running `retrospective` agent for learning extraction
