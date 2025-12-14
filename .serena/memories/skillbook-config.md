# Configuration Skills

## Skill-Config-001

**Entity Type**: Skill
**Statement**: .serena memories directory should be version controlled for team knowledge sharing
**Atomicity**: 94%
**Category**: Configuration
**Context**: When setting up Serena MCP or AI tool configurations
**Evidence**: Session 2025-12-14 - User corrected assumption about gitignoring .serena
**Tag**: helpful
**Impact**: 8
**Validated**: 1

**Summary**: The .serena directory contains valuable project memories and skillbooks that should be shared across team members. Unlike ephemeral AI tool caches (like .codebase-index.json), .serena memories represent curated knowledge. Version controlling .serena enables cross-session and cross-developer knowledge transfer.

**Pattern - Correct:**

```gitignore
# AI tooling caches (ephemeral, regeneratable)
.codebase-index.json
.codebase-intelligence.json

# Serena config - DO version control
# .serena/  <- Do NOT add this
```

**Pattern - Wrong:**

```gitignore
# Excluding valuable memories
.serena/
```

---

## Skill-Config-002

**Entity Type**: Skill
**Statement**: Use ignorePatterns in .markdownlint-cli2.yaml to exclude AI tool directories from linting
**Atomicity**: 93%
**Category**: Configuration
**Context**: When AI tooling directories contain non-compliant markdown
**Evidence**: Session 2025-12-14 - Added .serena to ignorePatterns
**Tag**: helpful
**Impact**: 7
**Validated**: 1

**Summary**: AI tool directories may contain generated or imported markdown that does not comply with project linting rules. Rather than fixing all files (which may be overwritten), exclude the directory from linting. This keeps lint checks fast and focused on project documentation.

**Configuration:**

```yaml
# .markdownlint-cli2.yaml
ignorePatterns:
  - "node_modules"
  - ".serena" # AI tool memories
  - ".agents/skills" # May contain external skills
```

**When to Apply**:

- Directory contains imported/generated content
- Content is valuable but non-compliant
- Fixing would be overwritten by tooling
- Directory is not primary project documentation
