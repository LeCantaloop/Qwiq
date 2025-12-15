# CLAUDE.md

This file provides guidance to Claude Code (<https://claude.ai/code>) when working with code in this repository.

## Repository Overview

QWIQ (**Q**uick **W**ork **I**tem **Q**uery) is a .NET library providing a simplified API for querying Azure DevOps / Team Foundation Server work items. It wraps the TFS Client OM with cleaner interfaces, factory patterns, and mock support.

## Build Commands

```powershell
# Restore tools (nbgv for versioning)
dotnet tool restore

# Build solution
dotnet build Qwiq.sln -c Release

# Strict build (warnings as errors) - used by CI
dotnet build Qwiq.sln -c Release /p:PedanticMode=true

# Flexible build (warnings allowed) - for diagnosing analyzer issues
dotnet build Qwiq.sln -c Release /p:PedanticMode=false

# Single-threaded build (avoids Windows file locking issues)
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false

# CI build (use PowerShell, not bash - see CRITICAL note below)
pwsh -NoProfile -NonInteractive -Command "dotnet build Qwiq.sln -c Release /p:ContinuousIntegrationBuild=true /p:UseSharedCompilation=false /m:1 /nodeReuse:false"
```

> **CRITICAL**: When running from Claude Code or non-PowerShell shells, always wrap dotnet commands in `pwsh -Command "..."`. The .NET 10 SDK parses MSBuild switches (`/m:1`, `/nodeReuse:false`) incorrectly when invoked through bash, interpreting them as separate arguments instead of build properties. This causes cryptic "MSB1008: Only one project can be specified" errors.

## Test Commands

```powershell
# Run unit tests (excludes integration tests requiring servers)
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

# Run a single test class
dotnet test Qwiq.sln -c Release --no-build --filter "FullyQualifiedName~ClassName"

# Run tests with code coverage
dotnet test Qwiq.sln -c Release --settings coverage.runsettings
```

## Linting Commands

```powershell
# Fix markdown issues
npx markdownlint-cli2 --fix "**/*.md"

# Fix C# formatting
dotnet format

# Fix general formatting (markdown, JSON)
dotnet pprettier --write .
```

## Architecture Overview

### Two Client Implementations

Both implement `IWorkItemStore` via factory pattern (`WorkItemStoreFactory.Default.Create(options)`):

| Client   | Project          | Use Case               | Platform              |
| -------- | ---------------- | ---------------------- | --------------------- |
| **REST** | `Qwiq.Core.Rest` | Modern Azure DevOps    | Cross-platform        |
| **SOAP** | `Qwiq.Core.Soap` | Legacy TFS on-premises | Windows-only (net472) |

### LINQ Provider

Translates C# LINQ to WIQL queries:

- `Query<T>` → `WiqlQueryProvider` → `QueryRewriter` → `WiqlTranslator` → WIQL string
- `IFieldMapper` maps .NET property names to TFS field reference names
- Special extension methods: `AsOf()`, `WasEver()`, `InGroup()`

### Mapper System

Converts `IWorkItem` to POCOs using attributes:

```csharp
[WorkItemType("Bug")]
public class Bug
{
    [FieldDefinition("System.Id")]
    public int? Id { get; set; }

    [FieldDefinition("System.Title")]
    public string Title { get; set; }
}
```

### Mock System

`Qwiq.Mocks` provides in-memory implementations for unit testing (`MockWorkItemStore`, `MockWorkItem`, etc.).

## Key Patterns

### Factory Pattern

All stores created via factories, never direct construction:

```csharp
IWorkItemStore store = WorkItemStoreFactory.Default.Create(options);
```

### Null Validation

Use runtime checks, not JetBrains annotations:

```csharp
public void Method(SomeType param)
{
    if (param == null) throw new ArgumentNullException(nameof(param));
}
```

### Test Pattern (ContextSpecification)

```csharp
[TestClass]
public class Given_context : ContextSpecification
{
    public override void Given() { /* Arrange */ }
    public override void When() { /* Act */ }

    [TestMethod]
    public void Then_behavior() { /* Assert with Shouldly */ }
}
```

## Project Structure

| Directory            | Contents                          |
| -------------------- | --------------------------------- |
| `src/Qwiq.Core`      | Core interfaces and abstractions  |
| `src/Qwiq.Core.Rest` | REST API client (cross-platform)  |
| `src/Qwiq.Core.Soap` | SOAP client (Windows/net472 only) |
| `src/Qwiq.Linq`      | LINQ-to-WIQL query provider       |
| `src/Qwiq.Mapper`    | Object mapping layer              |
| `src/Qwiq.Identity`  | Identity management               |
| `test/Qwiq.Mocks`    | Mock implementations for testing  |

## Configuration Files

| File                       | Purpose                                        |
| -------------------------- | ---------------------------------------------- |
| `Directory.Build.props`    | Shared MSBuild properties, package metadata    |
| `Directory.Packages.props` | Central Package Management (all versions here) |
| `global.json`              | Pins .NET SDK version                          |
| `.editorconfig`            | Code style AND analyzer severity configuration |

### Reproducible Builds

The project uses `DotNet.ReproducibleBuilds` (.NET Foundation package) for deterministic builds:

- Auto-detects 11 CI platforms (GitHub Actions, Azure Pipelines, GitLab CI, Jenkins, etc.)
- Automatically sets `ContinuousIntegrationBuild=true` on CI systems
- Configures `Deterministic=true`, `PublishRepositoryUrl=true`, `EmbedUntrackedSources=true`
- Our explicit `DebugType=portable` overrides the package default for `.snupkg` symbol packages

## Critical Notes

1. **Windows required** for SOAP projects (net472 + TFS Client OM dependency)
2. **Central Package Management** - add versions to `Directory.Packages.props`, not individual csproj files
3. **Nullable enabled** - use `?` suffix for nullable types, runtime null checks for validation
4. **Never commit artifacts/** - build outputs are gitignored
5. **Conventional commits** - use `fix(scope):`, `feat(scope):`, `refactor:`, etc.

## Agent System

This repository uses a coordinated multi-agent system for development. Agents are located in your VS Code prompts directory (`%APPDATA%\Code\User\prompts\`).

### Quick Reference

| Agent          | Use When                                      |
| -------------- | --------------------------------------------- |
| `orchestrator` | Complex multi-step tasks needing coordination |
| `implementer`  | Writing C# code and tests                     |
| `analyst`      | Research and investigation                    |
| `architect`    | Design decisions and ADRs                     |
| `planner`      | Breaking down work into tasks                 |
| `critic`       | Validating plans before implementation        |
| `qa`           | Test strategy and verification                |

### Memory (cloudmcp-manager)

Agents use `cloudmcp-manager` tools for cross-session memory:

```text
cloudmcp-manager/memory-search_nodes   # Find context
cloudmcp-manager/memory-create_entities # Store knowledge
cloudmcp-manager/memory-add_observations # Update existing
```

### Output Directories

Agent artifacts go to `.agents/`:

- `.agents/analysis/` - Research reports
- `.agents/architecture/` - ADRs
- `.agents/planning/` - Plans, PRDs, tasks
- `.agents/qa/` - Test strategies

### Full Documentation

- `.agents/AGENT-SYSTEM.md` - Complete agent catalog and workflows
- `.agents/AGENT-INSTRUCTIONS.md` - Task execution protocol

## Detailed Documentation

See `.github/copilot-instructions.md` for comprehensive guidance including:

- Full architecture details and class relationships
- Exception handling patterns
- Test categories and integration test setup
- CI/CD pipeline details
- Troubleshooting patterns
- Agent system routing heuristics

## Additional Agent Instructions

> Appended by install-claude-repo.ps1

### Extended Agent System Overview

This repository provides a coordinated multi-agent system for software development. Specialized agents handle different responsibilities with explicit handoff protocols and persistent memory using `cloudmcp-manager`.

## Agent Catalog

| Agent                      | Purpose                        | When to Use                  |
| -------------------------- | ------------------------------ | ---------------------------- |
| **csharp-expert**          | Production code, .NET patterns | Writing/reviewing C# code    |
| **csharp-pod**             | Design patterns, architecture  | Before writing code          |
| **analyst**                | Research, root cause analysis  | Investigating issues         |
| **architect**              | ADRs, design governance        | Technical decisions          |
| **planner**                | Milestones, work packages      | Breaking down epics          |
| **critic**                 | Plan validation                | Before implementation        |
| **qa**                     | Test strategy, verification    | After implementation         |
| **create-explainer**       | PRDs, feature docs             | Documenting features         |
| **generate-tasks**         | Atomic task breakdown          | After PRD created            |
| **feature-request-review** | Review feature requests        | Evaluating proposals         |
| **high-level-advisor**     | Strategic decisions            | Major direction choices      |
| **independent-thinker**    | Challenge assumptions          | Getting unfiltered feedback  |
| **memory**                 | Cross-session context          | Retrieving/storing knowledge |
| **skillbook**              | Skill management               | Managing learned strategies  |
| **retrospective**          | Learning extraction            | After task completion        |

## Standard Workflows

**Feature Development:**

```text
analyst → architect → planner → critic → csharp-expert → qa → retrospective
```

**Quick Fix:**

```text
csharp-expert → qa
```

**Strategic Decision:**

```text
independent-thinker → high-level-advisor → generate-tasks
```

## Invocation Examples

```python
# Research before implementation
Task(subagent_type="analyst", prompt="Investigate why X fails")

# Design review before coding
Task(subagent_type="csharp-pod", prompt="Review design for feature X")

# Implementation
Task(subagent_type="csharp-expert", prompt="Implement feature X per plan")

# Plan validation (required before implementation)
Task(subagent_type="critic", prompt="Validate plan at .agents/planning/...")

# Code review after writing
Task(subagent_type="csharp-pod", prompt="Review code quality")
Task(subagent_type="csharp-expert", prompt="Review implementation")

# Extract learnings
Task(subagent_type="retrospective", prompt="Analyze what we learned")
```

## Memory Protocol

All agents use `cloudmcp-manager` for cross-session memory:

```python
# Search for context
mcp__cloudmcp-manager__memory-search_nodes(query="[topic]")

# Store learnings
mcp__cloudmcp-manager__memory-add_observations(...)
mcp__cloudmcp-manager__memory-create_entities(...)
```

## Skill Citation

When applying learned strategies, cite skills:

```markdown
**Applying**: Skill-Build-001
**Strategy**: Use /m:1 /nodeReuse:false for CI builds
**Expected**: Avoid file locking errors

[Execute...]

**Result**: Build succeeded
**Skill Validated**: Yes
```

## Output Directories

Agents save artifacts to `.agents/`:

- `analysis/` - Analyst findings
- `architecture/` - ADRs
- `planning/` - Plans and PRDs
- `critique/` - Plan reviews
- `qa/` - Test strategies and reports
- `retrospective/` - Learning extractions

## Best Practices

1. **Memory First**: Retrieve context before multi-step reasoning
2. **Document Outputs**: Save artifacts to appropriate directories
3. **Clear Handoffs**: Announce next agent and purpose
4. **Store Learnings**: Update memory at milestones
5. **Test Everything**: No skipping hard tests
6. **Commit Atomically**: Small, conventional commits

<!-- Appended by install-claude-repo.ps1 -->

## Claude Code Agent Instructions

This file provides instructions for Claude Code when using the agent system.

### Agent System Overview

This repository provides a coordinated multi-agent system for software development. Specialized agents handle different responsibilities with explicit handoff protocols and persistent memory using `cloudmcp-manager`.

### Agent Catalog

| Agent                    | Purpose                                       | When to Use                               |
| ------------------------ | --------------------------------------------- | ----------------------------------------- |
| **orchestrator**         | Task coordination                             | Complex multi-step tasks                  |
| **implementer**          | Production code, .NET patterns                | Writing/reviewing C# code                 |
| **analyst**              | Research, root cause analysis, feature review | Investigating issues, evaluating requests |
| **architect**            | ADRs, design governance                       | Technical decisions                       |
| **planner**              | Milestones, work packages                     | Breaking down epics                       |
| **critic**               | Plan validation                               | Before implementation                     |
| **qa**                   | Test strategy, verification                   | After implementation                      |
| **explainer**            | PRDs, feature docs                            | Documenting features                      |
| **task-generator**       | Atomic task breakdown                         | After PRD created                         |
| **high-level-advisor**   | Strategic decisions                           | Major direction choices                   |
| **independent-thinker**  | Challenge assumptions                         | Getting unfiltered feedback               |
| **memory**               | Cross-session context                         | Retrieving/storing knowledge              |
| **skillbook**            | Skill management                              | Managing learned strategies               |
| **retrospective**        | Learning extraction                           | After task completion                     |
| **devops**               | CI/CD pipelines                               | Build automation, deployment              |
| **roadmap**              | Strategic vision                              | Epic definition, prioritization           |
| **security**             | Vulnerability assessment                      | Threat modeling, secure coding            |
| **pr-comment-responder** | PR review handler                             | Addressing bot/human review comments      |

### Standard Workflows

**Feature Development:**

```text
analyst -> architect -> planner -> critic -> implementer -> qa -> retrospective
```

**Quick Fix:**

```text
implementer -> qa
```

**Strategic Decision:**

```text
independent-thinker -> high-level-advisor -> task-generator
```

### Invocation Examples

```python
# Research before implementation
Task(subagent_type="analyst", prompt="Investigate why X fails")

# Design review before coding
Task(subagent_type="architect", prompt="Review design for feature X")

# Implementation
Task(subagent_type="implementer", prompt="Implement feature X per plan")

# Plan validation (required before implementation)
Task(subagent_type="critic", prompt="Validate plan at .agents/planning/...")

# Code review after writing
Task(subagent_type="architect", prompt="Review code quality")
Task(subagent_type="implementer", prompt="Review implementation")

# Extract learnings
Task(subagent_type="retrospective", prompt="Analyze what we learned")
```

### Memory Protocol

All agents use `cloudmcp-manager` for cross-session memory:

```python
# Search for context
mcp__cloudmcp-manager__memory-search_nodes(query="[topic]")

# Store learnings
mcp__cloudmcp-manager__memory-add_observations(...)
mcp__cloudmcp-manager__memory-create_entities(...)
```

### Skill Citation

When applying learned strategies, cite skills:

```markdown
**Applying**: Skill-Build-001
**Strategy**: Use /m:1 /nodeReuse:false for CI builds
**Expected**: Avoid file locking errors

[Execute...]

**Result**: Build succeeded
**Skill Validated**: Yes
```

### Output Directories

Agents save artifacts to `.agents/`:

- `analysis/` - Analyst findings
- `architecture/` - ADRs
- `planning/` - Plans and PRDs
- `critique/` - Plan reviews
- `qa/` - Test strategies and reports
- `retrospective/` - Learning extractions

### Best Practices

1. **Memory First**: Retrieve context before multi-step reasoning
2. **Document Outputs**: Save artifacts to appropriate directories
3. **Clear Handoffs**: Announce next agent and purpose
4. **Store Learnings**: Update memory at milestones
5. **Test Everything**: No skipping hard tests
6. **Commit Atomically**: Small, conventional commits

### Utilities

#### Fix Markdown Fences

When generating or fixing markdown with code blocks, use the fix-markdown-fences utility to repair malformed closing fences automatically.

**Location**: `.agents/utilities/fix-markdown-fences/SKILL.md`

**Problem**: Closing fences should never have language identifiers (e.g., ` ` `text). This utility detects and fixes them:

```markdown
<!-- Wrong -->

` ` `python
def hello():
    pass
` ` `python

<!-- Correct -->

` ` `python
def hello():
    pass
` ` `
```

**Usage**:

```bash
# PowerShell
pwsh .agents/utilities/fix-markdown-fences/fix_fences.ps1

# Python
python .agents/utilities/fix-markdown-fences/fix_fences.py
```

**Benefits**:

- Prevents token waste from repeated fence fixing cycles
- Validates markdown before committing
- Handles edge cases (nested indentation, multiple blocks, unclosed blocks)
- Supports batch processing of multiple files

**Reference**: See `.agents/utilities/fix-markdown-fences/SKILL.md` for implementation details and additional language versions.

- When generating or reviewing code, use codebase-context MCP tools (`mcp__codebase-context__get_team_patterns`, `mcp__codebase-context__search_codebase`) to check team patterns first.
- When cloudmcp-manager memory functions fail, use Serena memory tools as fallback:
  - Primary functions: `memory-add_observations`, `memory-create_entities`, `memory-create_relations`, `memory-delete_entities`, `memory-delete_observations`, `memory-delete_relations`, `memory-open_nodes`, `memory-read_graph`, `memory-search_nodes`
  - Fallback functions: `write_memory`, `read_memory`, `list_memories`, `delete_memory`, `edit_memory`
