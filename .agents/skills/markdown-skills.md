# Markdown Skills

## Skill-Markdown-001

**Statement**: Always add language identifier to code fences; use 'text' for pseudo-code or diagrams

**Atomicity**: 98%

**Category**: Markdown

**Context**: When writing markdown documentation in agent files

**Evidence**: Session Linting Automation - Fixed 92+ MD040 violations

**Details**:

- Markdown linter rule MD040 requires language identifier on code blocks
- Improves readability and enables syntax highlighting
- 'text' is catch-all for diagrams, pseudo-code, tool output
- Prevents rendering issues and linting failures

**Pattern - Wrong** ❌:

```markdown
` ` `function hello() {
  console.log("Hello");
}` ` `
```

**Pattern - Correct** ✅:

```markdown
\`\`\`javascript
function hello() {
console.log("Hello");
}
\`\`\`
```

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

**Real Examples**:

```markdown
# Pseudo-code example

\`\`\`text
FOR each build error:
IF is_analyzer_error:
FIX analyzer issue
ELSE:
FIX syntax error
END IF
END FOR
\`\`\`

# Workflow diagram

\`\`\`text
developer → pre-commit hook → git add → CI pipeline → deploy
↓
auto-fix lint
\`\`\`

# Generic type syntax (triggers MD033 without code block)

\`\`\`text
Example: IEnumerable<T> where T : IDisposable
\`\`\`
```

**Common Violations**:

| Pattern                     | Rule  | Fix                            |
| --------------------------- | ----- | ------------------------------ |
| Missing identifier          | MD040 | Add language ID                |
| Generic types `<T>` in text | MD033 | Wrap in code block with `text` |
| Command output              | MD040 | Use `text` or `powershell`     |
| Diagram with arrows         | MD040 | Use `text`                     |

**Enforcement**:

- Pre-commit hook: `markdownlint-cli2 --fix`
- CI pipeline: `markdownlint-cli2` (verify-only)
- Documentation standards: Section in AGENT-INSTRUCTIONS.md

---

## Skill-Markdown-002

**Statement**: Generic type syntax like `ArrayPool<T>` triggers MD033; use code blocks instead

**Atomicity**: 96%

**Category**: Markdown

**Context**: When documenting .NET code with generic types in markdown

**Evidence**: Session Linting Automation - Multiple files flagged for inline HTML

**Details**:

- Markdown linter rule MD033 flags inline HTML
- Generic type syntax `<T>` looks like HTML tag to linter
- Solution: Wrap in backticks or code block
- Improves readability and linting compliance

**Pattern - Wrong** ❌:

```markdown
Use IEnumerable<T> to iterate over items.
This method works with ArrayPool<byte> instances.
```

Error: `MD033/no-inline-html - Inline HTML [Context: 'IEnumerable']`

**Pattern - Correct** ✅:

```markdown
Use `IEnumerable<T>` to iterate over items.
This method works with `ArrayPool<byte>` instances.
```

**For Complex Examples**:

```markdown
Method signature:

\`\`\`csharp
public class Repository<T> where T : IDisposable
{
public IEnumerable<T> GetAll();
}
\`\`\`
```

**Real Examples in Documentation**:

```markdown
# Work Item Types

The REST client works with `IWorkItem<T>` implementations.

**Generic Constraints**:

- `T : IWorkItem` - Must implement work item interface
- `T : IDisposable` - Should be disposable
- `T : IEquatable<T>` - Should support equality

**Common Types**:

- `Bug` - Inherits from `IWorkItem<Bug>`
- `Feature` - Inherits from `IWorkItem<Feature>`
- `Task` - Inherits from `IWorkItem<Task>`
```

**Quick Fix**:

If you get `MD033` error:

1. Identify the `<T>` pattern in text
2. Wrap in backticks: `` `<T>` ``
3. Or move to code block with language identifier
4. Run linter to verify

**Prevention**:

- Always use backticks for generic syntax in prose
- Use code blocks for complete type signatures
- Run `markdownlint-cli2 --fix` before committing
- CI will catch any remaining issues

**Note**: This skill overlaps with MD040 - ensure all code examples have language identifiers

---

## Skill-Markdown-003

**Statement**: Use ul/li in MD033 allowed_elements for markdown tables containing lists

**Atomicity**: 95%

**Category**: Markdown

**Context**: When configuring markdownlint for documentation with complex tables

**Evidence**: Session markdown-lint-incident 2025-12-14 - ~280 MD033 errors from HTML lists in tables

**Details**:

- Markdown tables don't support native list syntax inside cells
- Common pattern: Use HTML `<ul><li>` elements for multi-item cells
- MD033 rule flags all inline HTML by default
- Solution: Allowlist `ul` and `li` elements in markdownlint config

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

**Pattern - Triggers MD033** ❌:

```markdown
| Agent   | Capabilities                           |
| ------- | -------------------------------------- |
| analyst | <ul><li>Research</li><li>RCA</li></ul> |
```

Error: `MD033/no-inline-html - Inline HTML [Context: 'ul']`

**Pattern - After Config Fix** ✅:

Same markdown, but with `ul` and `li` in allowed_elements - no error.

**When to Apply**:

- Agent documentation with capability matrices
- Feature comparison tables
- Any table needing multi-item cells
- Documentation heavy in structured tables

**Alternative Approaches**:

1. Use `<br>` for simple line breaks (already allowed)
2. Use separate rows for each item (verbose)
3. Use comma-separated text (loses structure)
4. Use nested tables (complex)

**Recommendation**: Allow `ul`/`li` in config - cleanest solution for table lists
