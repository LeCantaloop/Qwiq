# Build & CI Skills

## Skill-Build-001

**Statement**: Use `/p:ContinuousIntegrationBuild=true /p:UseSharedCompilation=false /m:1 /nodeReuse:false` for local builds to match CI analyzer strictness

**Atomicity**: 96%

**Category**: Build

**Context**: Before pushing changes that passed local builds

**Evidence**: Session 39 - Fixed CA1711, CA1001, CA1861 errors that passed locally but failed CI

**Details**:

- Local builds without CI flags don't enable strict analyzer rules
- CI pipeline enables these flags by default
- Mismatch causes "works locally, fails in CI" situations
- Always use full CI build command before pushing

**Application Example**:

```powershell
# WRONG - may pass but CI will fail
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false

# CORRECT - matches CI behavior
dotnet build Qwiq.sln -c Release /p:ContinuousIntegrationBuild=true /p:UseSharedCompilation=false /m:1 /nodeReuse:false
```

**Common Errors Caught**:

- CA1711: Identifier contains type name (e.g., classes ending in "Collection")
- CA1001: Types that own disposable fields
- CA1861: Avoid passing initialization data to const arrays

---

## Skill-Build-002

**Statement**: Set `BuildInParallel=false` and `ProduceReferenceAssembly=false` for Windows multi-framework builds

**Atomicity**: 93%

**Category**: Build

**Context**: When experiencing CS0006 "metadata file not found" errors in multi-targeting

**Evidence**: Session CS0006-fix - Directory.Build.props lines 88-96

**Details**:

- Inner-build parallelism causes reference assembly conflicts on Windows
- Multi-targeting builds (.NET Framework + .NET Core) need special handling
- Error manifests as "The metadata file X cannot be found"
- Infrastructure fix in Directory.Build.props prevents race conditions

**Application**:

Add to Directory.Build.props:

```xml
<!-- Workaround for Windows multi-framework builds -->
<PropertyGroup Condition="'$(OS)' == 'Windows_NT'">
  <BuildInParallel>false</BuildInParallel>
  <ProduceReferenceAssembly>false</ProduceReferenceAssembly>
</PropertyGroup>
```

**When to Apply**:

- Multi-targeting projects (.NET Framework + .NET 6+)
- Windows CI agents experiencing CS0006 errors
- Full solution builds with net472 + modern framework targets

---

## Skill-CI-001

**Statement**: CI should verify lint rules without auto-fix to catch commits bypassing pre-commit hooks

**Atomicity**: 95%

**Category**: CI

**Context**: When configuring CI pipeline for markdown-heavy repositories

**Evidence**: Session Linting Automation - Added check-only step to main.yml

**Details**:

- Pre-commit hooks auto-fix issues locally (developer experience)
- CI should verify rules WITHOUT auto-fixing (catch bypasses)
- Prevents developers from bypassing local hooks
- Two-layer approach: auto-fix locally, verify in CI

**Implementation Pattern**:

```yaml
# In CI workflow (main.yml)
- name: Verify markdown linting
  run: |
    npm install -g markdownlint-cli2
    # Use --no-fix to catch issues
    npx markdownlint-cli2 "**/*.md"
  env:
    SKIP_AUTOFIX: 1
```

**Contrast with Local**:

```bash
# Local pre-commit hook - AUTO-FIXES
npx markdownlint-cli2 --fix "**/*.md"

# CI verification - CHECK ONLY
npx markdownlint-cli2 "**/*.md"
```

**Benefits**:

- Local: Eliminates friction, auto-fixes common issues
- CI: Catches intentional bypass attempts
- Clear separation of concerns
