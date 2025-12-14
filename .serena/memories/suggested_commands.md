# Suggested Commands for Qwiq Development

## Build Commands

```powershell
# Restore tools (nbgv for versioning)
dotnet tool restore

# Standard build
dotnet build Qwiq.sln -c Release

# Strict build (warnings as errors) - used by CI
dotnet build Qwiq.sln -c Release /p:PedanticMode=true

# Flexible build (warnings allowed) - for diagnosing analyzer issues
dotnet build Qwiq.sln -c Release /p:PedanticMode=false

# Single-threaded build (avoids Windows file locking issues)
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false

# Clean build
dotnet clean Qwiq.sln -c Release
```

## Test Commands

```powershell
# Run unit tests (excludes integration tests requiring servers)
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

# Run a single test class
dotnet test Qwiq.sln -c Release --no-build --filter "FullyQualifiedName~ClassName"

# Run tests with code coverage
dotnet test Qwiq.sln -c Release --settings coverage.runsettings

# Run all tests (including integration - requires server connection)
dotnet test Qwiq.sln -c Release --no-build
```

## Linting and Formatting

```powershell
# Fix markdown issues
npx markdownlint-cli2 --fix "**/*.md"

# Check markdown issues (no fix)
npx markdownlint-cli2 "**/*.md"

# Fix C# formatting
dotnet format Qwiq.sln

# Fix general formatting (markdown, JSON, YAML)
dotnet pprettier --write .

# Check formatting (no fix)
dotnet pprettier --check .
```

## Git Commands

```powershell
# Standard git operations
git status
git diff
git log --oneline -10
git branch -a
git fetch origin
git pull origin develop
git push origin <branch>

# Create conventional commit
git commit -m "fix(scope): description"
git commit -m "feat(scope): description"
git commit -m "refactor(scope): description"
```

## GitHub CLI

```powershell
# PR operations
gh pr create --title "Title" --body "Description"
gh pr view
gh pr checks
gh pr merge

# Issue operations
gh issue list
gh issue view <number>
```

## Versioning (Nerdbank.GitVersioning)

```powershell
# Get current version
dotnet nbgv get-version

# Get specific version field
dotnet nbgv get-version -v AssemblyInformationalVersion
```

## Mutation Testing (Stryker)

```powershell
# Run mutation testing
dotnet stryker
```

## Utility Commands (Windows)

```powershell
# List directory contents
ls
dir

# Find files
Get-ChildItem -Recurse -Filter "*.cs"

# Search in files
Select-String -Path "*.cs" -Pattern "pattern"

# Remove files/directories
rm -Recurse -Force <path>
```
