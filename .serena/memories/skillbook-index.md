# Skillbook Index

## Overview

This is the master index of all skills stored in the Qwiq repository's skillbook. Skills are organized by category and stored in separate memory files for efficient retrieval.

**Total Skills**: 36
**Categories**: 12
**Storage Method**: Serena memory tools (cloudmcp-manager was full)

## Quick Access by Category

### Build & CI Skills (9 skills)

**File**: `skillbook-build-ci.md`

- **Skill-Build-001** (96%): Use CI flags for local builds to match CI analyzer strictness
- **Skill-Build-002** (93%): Set BuildInParallel=false for Windows multi-framework builds
- **Skill-Build-003** (94%): Use git clean -fdx before builds when file locks persist
- **Skill-Build-PKG-001** (92%): Use .NET Foundation packages to replace 4+ manual build properties
- **Skill-Build-CFG-002** (87%): Override package defaults only when format requirements differ
- **Skill-CI-001** (95%): CI should verify lint rules without auto-fix
- **Skill-CI-002** (92%): CI must validate all files, not just changes
- **Skill-CI-003** (88%): Run baseline validation when enabling hooks
- **Skill-CI-004** (92%): Renovate pinGitHubActionDigests preset automates SHA updates

### Developer Experience & Git Hooks (2 skills)

**File**: `skillbook-devex-githooks.md`

- **Skill-DevEx-001** (97%): Pre-commit hooks should auto-fix issues then verify
- **Skill-GitHooks-001** (99%): Auto-fix hooks must re-stage modified files with git add

### Documentation Skills (3 skills)

**File**: `skillbook-documentation.md`

- **Skill-Doc-001** (93%): Split documentation files when approaching 25,000 token limit
- **Skill-Doc-002** (95%): Update HANDOFF.md at session end
- **Skill-Doc-CFG-001** (84%): Remove superfluous comments from config files when purpose is self-evident

### GitHub & Git Skills (4 skills)

**File**: `skillbook-github-git.md`

- **Skill-GitHub-001** (98%): GitHub @copilot cannot be assigned to issues; use comment mentions
- **Skill-Issue-001** (92%): Include suggested fix code in bug reports
- **Skill-Git-001** (97%): Use git checkout -- [file] to restore overwritten files
- **Skill-Git-002** (91%): Use git mv to preserve history during reorganization

### Markdown Skills (4 skills)

**File**: `skillbook-markdown.md`

- **Skill-Markdown-001** (98%): Always add language identifier to code fences
- **Skill-Markdown-002** (96%): Generic type syntax like `ArrayPool<T>` triggers MD033
- **Skill-Markdown-003** (95%): Use ul/li in MD033 allowed_elements for tables
- **Skill-Markdown-004** (95%): Bold text as pseudo-heading triggers MD036

### Configuration Skills (2 skills)

**File**: `skillbook-config.md`

- **Skill-Config-001** (94%): .serena memories should be version controlled for team knowledge
- **Skill-Config-002** (93%): Use ignorePatterns in markdownlint for AI tool directories

### Code Quality Skills (3 skills)

**File**: `skillbook-quality.md`

- **Skill-Quality-001** (92%): Test classes ending in "Collection" trigger CA1711
- **Skill-Quality-002** (90%): Add pragma warning disable CA1001 for test cleanup
- **Skill-Quality-003** (88%): Extract inline test arrays to static readonly fields

### Strategy Skills (3 skills)

**File**: `skillbook-strategy.md`

- **Skill-Strategic-001** (97%): Verify deployment scale before declaring maintenance mode
- **Skill-Strategic-002** (94%): SOAP clients cannot deploy to Kubernetes; deprecate for REST
- **Skill-Proj-CTX-001** (88%): Revalidate priority context when deployment scope/security requirements change

### Testing Skills (3 skills)

**File**: `skillbook-testing.md`

- **Skill-Test-001** (95%): WireMock.Net deadlocks on .NET Framework 4.7.2
- **Skill-Test-002** (91%): Capture real HTTP traffic with Fiddler for WireMock stubs
- **Skill-Test-003** (94%): IdentityDescriptor must be string format in stubs

### Workflow & Installation Skills (4 skills)

**File**: `skillbook-workflow-install.md`

- **Skill-Workflow-001** (95%): Check for installation scripts before manual file operations
- **Skill-Install-001** (91%): Installation scripts may replace config files; backup first
- **Skill-Workflow-002** (90%): AI agents should run lint validation before commit
- **Skill-Agent-WF-001** (90%): Multi-agent Epic workflow produces zero-rework PRDs and atomic task lists

## Skills by Atomicity Score

### Excellent (95-100%)

- Skill-GitHooks-001 (99%)
- Skill-GitHub-001 (98%)
- Skill-Markdown-001 (98%)
- Skill-DevEx-001 (97%)
- Skill-Git-001 (97%)
- Skill-Strategic-001 (97%)
- Skill-Build-001 (96%)
- Skill-Markdown-002 (96%)
- Skill-CI-001 (95%)
- Skill-Doc-002 (95%)
- Skill-Markdown-003 (95%)
- Skill-Markdown-004 (95%)
- Skill-Test-001 (95%)
- Skill-Workflow-001 (95%)

### Good (90-94%)

- Skill-Build-003 (94%)
- Skill-Strategic-002 (94%)
- Skill-Test-003 (94%)
- Skill-Config-001 (94%)
- Skill-Config-002 (93%)
- Skill-Build-002 (93%)
- Skill-Doc-001 (93%)
- Skill-CI-002 (92%)
- Skill-CI-004 (92%)
- Skill-Issue-001 (92%)
- Skill-Quality-001 (92%)
- Skill-Git-002 (91%)
- Skill-Install-001 (91%)
- Skill-Test-002 (91%)
- Skill-Quality-002 (90%)
- Skill-Workflow-002 (90%)

### Acceptable (70-89%)

- Skill-CI-003 (88%)
- Skill-Quality-003 (88%)

## Skills by Impact (10-point scale)

### Critical Impact (9-10)

- Skill-CI-002 (10): All files validation in CI
- Skill-Doc-002 (10): HANDOFF.md updates
- Skill-Strategic-001 (10): Verify deployment scale
- Skill-Build-001 (9): CI flags for local builds
- Skill-DevEx-001 (9): Auto-fix pre-commit hooks
- Skill-GitHooks-001 (9): Re-stage fixed files
- Skill-CI-001 (9): CI verify without auto-fix
- Skill-CI-003 (9): Baseline validation for hooks
- Skill-Git-001 (9): Restore overwritten files
- Skill-Markdown-001 (9): Language identifiers
- Skill-Markdown-003 (9): ul/li in MD033
- Skill-Strategic-002 (9): SOAP deprecation
- Skill-Test-001 (9): WireMock .NET Framework issue
- Skill-Test-003 (9): IdentityDescriptor format
- Skill-Install-001 (9): Backup before install scripts

### High Impact (7-8)

- Skill-CI-004 (8): Renovate SHA pinning automation
- Skill-Build-002 (8): Windows multi-framework builds
- Skill-Doc-001 (8): Split large documentation
- Skill-Issue-001 (8): Suggested fix in bug reports
- Skill-Git-002 (8): git mv for history preservation
- Skill-Markdown-002 (8): Generic types trigger MD033
- Skill-Quality-002 (8): pragma warning CA1001
- Skill-Test-002 (8): Fiddler for traffic capture
- Skill-Workflow-001 (8): Check for install scripts
- Skill-Workflow-002 (8): Agent lint validation
- Skill-Build-003 (7): git clean for file locks
- Skill-GitHub-001 (7): Copilot not assignable
- Skill-Quality-001 (7): Collection suffix CA1711
- Skill-Quality-003 (7): Extract test arrays

## Search by Context

### Build & Compilation

- Skill-Build-001: Before pushing changes
- Skill-Build-002: CS0006 metadata errors
- Skill-Build-003: File lock errors
- Skill-CI-001: CI pipeline configuration
- Skill-CI-002: CI lint checks
- Skill-CI-003: Enabling git hooks
- Skill-CI-004: GitHub Actions supply chain security

### Git Operations

- Skill-Git-001: File overwritten
- Skill-Git-002: Directory reorganization
- Skill-GitHooks-001: Auto-fix hooks
- Skill-GitHub-001: GitHub issue assignment
- Skill-Issue-001: Bug reporting

### Documentation

- Skill-Doc-001: Large files
- Skill-Doc-002: Session end
- Skill-Markdown-001: Writing markdown
- Skill-Markdown-002: .NET generic types
- Skill-Markdown-003: Markdown tables with lists

### Code Quality

- Skill-Quality-001: Test class naming
- Skill-Quality-002: Test disposable fields
- Skill-Quality-003: Test constant arrays

### Testing

- Skill-Test-001: WireMock integration
- Skill-Test-002: Creating API stubs
- Skill-Test-003: Azure DevOps API stubs

### Workflow

- Skill-Workflow-001: External tool installation
- Skill-Install-001: Running install scripts
- Skill-Workflow-002: Agent-generated markdown
- Skill-DevEx-001: Linting hooks

### Strategy

- Skill-Strategic-001: Project decisions
- Skill-Strategic-002: Container deployment

## Usage Instructions

### Searching for Skills

```bash
# List all skillbook memories
mcp__serena__list_memories

# Read a specific category
mcp__serena__read_memory --memory_file_name "skillbook-build-ci.md"

# Search for skills by keyword
grep -r "analyzer" .serena/memories/skillbook-*.md
```

### Citing Skills in Work

When applying a skill:

```markdown
**Applying**: Skill-Build-001
**Strategy**: Use /p:ContinuousIntegrationBuild=true for local builds
**Expected**: Avoid "works locally, fails in CI" situations

[Execute...]

**Result**: Build passed with all analyzers enabled
**Skill Validated**: Yes
```

### Adding New Skills

1. Create skill in appropriate category file
2. Follow skill format:

   - Entity Type: Skill
   - Statement (atomic, actionable)
   - Atomicity percentage
   - Category
   - Context
   - Evidence
   - Tag (helpful/harmful/neutral)
   - Impact (1-10)
   - Validated count
   - Summary

3. Update this index with new skill

## Maintenance

**Last Updated**: 2025-12-14
**Total Skills**: 31
**Storage Location**: `.serena/memories/skillbook-*.md`
**Index File**: `skillbook-index.md`

**Validation Status**:

- ✅ All 26 skills from `.agents/skills/*.md` files converted to memory
- ✅ Organized by category into 8 memory files
- ✅ Index created for navigation
- ✅ Searchable and retrievable via Serena memory tools
