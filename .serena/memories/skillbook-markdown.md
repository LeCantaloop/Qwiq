# Markdown Skills

## Skill-Markdown-001

**Entity Type**: Skill
**Statement**: Always add language identifier to code fences; use 'text' for pseudo-code or diagrams
**Atomicity**: 98%
**Category**: Markdown
**Context**: When writing markdown documentation in agent files
**Evidence**: Session Linting Automation - Fixed 92+ MD040 violations
**Tag**: helpful
**Impact**: 9
**Validated**: 2

**Summary**: Markdown linter rule MD040 requires language identifier on code blocks. Improves readability and enables syntax highlighting. 'text' is catch-all for diagrams, pseudo-code, tool output. Prevents rendering issues and linting failures.

**Language Identifiers by Content Type**:

| Content             | Identifier    | Example                      |
| ------------------- | ------------- | ---------------------------- |
| C# code             | `csharp`      | Class definitions, methods   |
| PowerShell commands | `powershell`  | Build scripts, setup         |
| Bash/shell          | `bash`        | Linux commands               |
| JSON data           | `json`        | Configuration, API responses |
| XML/YAML            | `xml`, `yaml` | Configuration files          |
| Markdown examples   | `markdown`    | Documentation samples        |
| **Pseudo-code**     | `text`        | Algorithm descriptions       |
| **Diagrams**        | `text`        | ASCII art, flowcharts        |
| **Tool output**     | `text`        | Command output, logs         |
| **Workflows**       | `text`        | → arrows, process flows      |

---

## Skill-Markdown-002

**Entity Type**: Skill
**Statement**: Generic type syntax like `ArrayPool<T>` triggers MD033; use code blocks instead
**Atomicity**: 96%
**Category**: Markdown
**Context**: When documenting .NET code with generic types in markdown
**Evidence**: Session Linting Automation - Multiple files flagged for inline HTML
**Tag**: helpful
**Impact**: 8
**Validated**: 2

**Summary**: Markdown linter rule MD033 flags inline HTML. Generic type syntax `<T>` looks like HTML tag to linter. Solution: Wrap in backticks or code block. Improves readability and linting compliance.

**Pattern - Correct** ✅:

```markdown
Use `IEnumerable<T>` to iterate over items.
This method works with `ArrayPool<byte>` instances.
```

**Quick Fix**:

If you get `MD033` error:

1. Identify the `<T>` pattern in text
2. Wrap in backticks: `` `<T>` ``
3. Or move to code block with language identifier
4. Run linter to verify

---

## Skill-Markdown-003

**Entity Type**: Skill
**Statement**: Use ul/li in MD033 allowed_elements for markdown tables containing lists
**Atomicity**: 95%
**Category**: Markdown
**Context**: When configuring markdownlint for documentation with complex tables
**Evidence**: Session markdown-lint-incident 2025-12-14 - ~280 MD033 errors from HTML lists in tables
**Tag**: helpful
**Impact**: 9
**Validated**: 1

**Summary**: Markdown tables don't support native list syntax inside cells. Common pattern: Use HTML `<ul><li>` elements for multi-item cells. MD033 rule flags all inline HTML by default. Solution: Allowlist `ul` and `li` elements in markdownlint config.

**Configuration**:

```yaml
# .markdownlint-cli2.yaml
MD033:
  allowed_elements:
    - details
    - summary
    - br
    - sup
    - sub
    - ul # Add for table cell lists
    - li # Add for table cell lists
```

---

## Skill-Markdown-004

**Entity Type**: Skill
**Statement**: Bold text followed by content on next line triggers MD036; use proper headings
**Atomicity**: 95%
**Category**: Markdown
**Context**: When writing or validating markdown documentation
**Evidence**: Session 2025-12-14 - Multiple .serena memory files flagged for pseudo-headings
**Tag**: helpful
**Impact**: 7
**Validated**: 1

**Summary**: Markdown linter rule MD036 detects emphasis used instead of heading. Pattern: **Bold text** on its own line followed by content looks like a heading but uses emphasis syntax. Solution: Convert to proper heading syntax (## or ###). Improves document structure and accessibility.

**Pattern - Wrong:**

```markdown
**Section Title**

This is the content under the section.
```

**Pattern - Correct:**

```markdown
## Section Title

This is the content under the section.
```

**Quick Fix**:

1. Find lines with only bold text (`**text**`)
2. Check if followed by content (looks like a section)
3. Replace `**text**` with `## text` or appropriate heading level
4. Run linter to verify
