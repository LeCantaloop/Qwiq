# Markdown Linting Requirements

> **Purpose**: Define markdown linting standards for agent templates and documentation to prevent recurring linting failures during commits.

---

## Overview

This document specifies markdown formatting requirements for the vs-code-agents repository. These standards apply to:

- Agent template files (`.claude/agents/*.md`, `.github/agents/*.md`)
- Documentation files (`*.md`)
- Generated documentation from agents

Following these standards prevents common linting errors that waste time and tokens when agents must repeatedly fix issues during commits.

---

## Quick Reference

| Rule | Problem | Solution |
|------|---------|----------|
| **MD040** | Code block missing language identifier | Add language identifier after opening fence |
| **MD033** | Generic type syntax interpreted as HTML | Wrap generic types in inline code backticks |
| **MD036** | Emphasis used as heading | Use proper heading syntax (`##`) |
| **MD025** | Multiple H1 headings | Use only one `#` heading per document |
| **MD031** | No blank line around code block | Add blank lines before and after code fences |
| **MD058** | No blank line around table | Add blank lines before and after tables |

---

## Code Block Language Identifiers (MD040)

### Rule

Every fenced code block MUST have a language identifier on the opening fence.

### Why This Matters

- Without a language identifier, `markdownlint` reports MD040 violations
- This was the most frequent violation (92+ occurrences in one audit)
- Agents often generate code blocks without language identifiers

### Language Identifier Reference

| Content Type | Language ID | Example Use Case |
|--------------|-------------|------------------|
| C# code | `csharp` | Code examples, implementation |
| PowerShell commands | `powershell` | Windows shell commands |
| Bash commands | `bash` | Unix/Linux shell commands |
| JSON data | `json` | Configuration, API responses |
| YAML configuration | `yaml` | CI/CD, config files |
| Markdown templates | `markdown` | Documentation examples |
| Plain text | `text` | Pseudo-code, ASCII diagrams |
| Workflow diagrams | `text` | Arrow diagrams (A -> B -> C) |
| Tool calls | `text` | cloudmcp-manager calls |
| Python code | `python` | Python examples |
| JavaScript | `javascript` | JS examples |
| TypeScript | `typescript` | TS examples |
| XML | `xml` | XML configuration |
| SQL | `sql` | Database queries |
| Diff output | `diff` | Code comparisons |

### Examples

**INCORRECT** - triggers MD040:

````markdown
```
public class Example
{
    public void Method() { }
}
```
````

**CORRECT** - with language identifier:

````markdown
```csharp
public class Example
{
    public void Method() { }
}
```
````

**CORRECT** - text for pseudo-code or diagrams:

````markdown
```text
User Request -> Agent Processing -> Output Generation -> Validation
```
````

**CORRECT** - text for tool calls:

````markdown
```text
mcp__cloudmcp-manager__memory-search_nodes(query="markdown linting")
```
````

### Decision Tree

When choosing a language identifier:

1. Is it code in a specific programming language? Use that language ID (`csharp`, `python`, `javascript`)
2. Is it a shell command? Use `powershell` (Windows) or `bash` (Unix)
3. Is it structured data? Use `json`, `yaml`, or `xml`
4. Is it a workflow diagram, pseudo-code, or plain output? Use `text`
5. Is it a markdown example? Use `markdown`

---

## Generic Type Syntax (MD033)

### Rule

Generic type syntax (e.g., `ArrayPool<T>`, `Span<byte>`, `List<int>`) MUST be wrapped in inline code backticks.

### Why This Matters

- Angle brackets `< >` are interpreted as HTML tags by markdownlint
- MD033 flags inline HTML usage unless explicitly allowed
- This frequently occurs in .NET documentation

### Examples

**INCORRECT** - triggers MD033:

```markdown
Use ArrayPool<T> for efficient buffer management.
The Span<byte> type provides memory-safe access.
```

**CORRECT** - wrapped in inline code:

```markdown
Use `ArrayPool<T>` for efficient buffer management.
The `Span<byte>` type provides memory-safe access.
```

### Common .NET Generic Types to Watch

- `ArrayPool<T>`
- `Span<T>`, `ReadOnlySpan<T>`
- `Memory<T>`, `ReadOnlyMemory<T>`
- `List<T>`, `Dictionary<TKey, TValue>`
- `IEnumerable<T>`, `ICollection<T>`
- `Task<T>`, `ValueTask<T>`
- `Func<T>`, `Action<T>`
- `Nullable<T>`

---

## Heading Rules (MD025, MD036)

### Single H1 Heading (MD025)

**Rule**: Each document MUST have exactly one H1 (`#`) heading, which serves as the document title.

**INCORRECT**:

```markdown
# Main Title

## Section One

# Another Main Heading

## Section Two
```

**CORRECT**:

```markdown
# Main Title

## Section One

## Another Section

## Section Two
```

### No Emphasis as Headings (MD036)

**Rule**: Do not use emphasis (`_text_` or `**text**`) as standalone headings.

**INCORRECT**:

```markdown
_Note: This is important_

Some content here.

**Warning**

More content.
```

**CORRECT**:

```markdown
> **Note**: This is important

Some content here.

### Warning

More content.
```

For inline emphasis that introduces a paragraph, use bold followed by a colon:

```markdown
**Note:** This is an inline callout that does not trigger MD036.
```

---

## Spacing Rules (MD031, MD058)

### Blank Lines Around Code Blocks (MD031)

**Rule**: Fenced code blocks MUST be surrounded by blank lines.

**INCORRECT**:

````markdown
Some text
```csharp
var x = 1;
```
More text
````

**CORRECT**:

````markdown
Some text

```csharp
var x = 1;
```

More text
````

### Blank Lines Around Tables (MD058)

**Rule**: Tables MUST be surrounded by blank lines.

**INCORRECT**:

```markdown
Some text
| Column 1 | Column 2 |
|----------|----------|
| Data 1   | Data 2   |
More text
```

**CORRECT**:

```markdown
Some text

| Column 1 | Column 2 |
|----------|----------|
| Data 1   | Data 2   |

More text
```

---

## Testing Locally

### Install markdownlint-cli2

```bash
npm install -g markdownlint-cli2
```

### Check for Violations

```bash
# Check all markdown files
npx markdownlint-cli2 "**/*.md"

# Check specific directory
npx markdownlint-cli2 ".claude/agents/*.md"

# Check single file
npx markdownlint-cli2 "README.md"
```

### Auto-Fix Violations

```bash
# Auto-fix all fixable issues
npx markdownlint-cli2 --fix "**/*.md"

# Auto-fix specific directory
npx markdownlint-cli2 --fix ".claude/agents/*.md"
```

### What Can Be Auto-Fixed

| Rule | Auto-Fixable | Manual Fix Required |
|------|--------------|---------------------|
| MD031 | Yes | - |
| MD058 | Yes | - |
| MD040 | No | Add language identifier |
| MD033 | No | Wrap in backticks |
| MD036 | No | Change to proper heading |
| MD025 | No | Restructure document |

---

## Configuration

### markdownlint-cli2.yaml

Place this configuration in the repository root:

```yaml
config:
  default: true

  # Heading style - use ATX style (#)
  MD003:
    style: atx

  # Line length - disabled, let prettier handle wrapping
  MD013: false

  # Multiple headings with same content - allow in different nesting
  MD024:
    siblings_only: true

  # Single title/top-level heading - allow front matter title
  MD025:
    front_matter_title: ""

  # Inline HTML - allow for details/summary and common patterns
  MD033:
    allowed_elements:
      - details
      - summary
      - br
      - sup
      - sub

  # Emphasis used instead of heading
  MD036: true

  # Fenced code blocks should have a language specified
  MD040: true

  # First line should be a top-level heading - not always applicable
  MD041: false

  # Code block style - use fenced
  MD046:
    style: fenced

  # Code fence style - use backticks
  MD048:
    style: backtick

globs:
  - "**/*.md"

ignores:
  - "**/node_modules/**"
  - "**/.git/**"
```

---

## Integration with Agent Templates

### Agent Prompt Additions

Include these instructions in agent system prompts:

```markdown
## Markdown Standards

When generating or updating markdown:

1. **Code blocks**: Always include language identifier
   - Use `csharp`, `powershell`, `json`, `yaml`, `text` as appropriate
   - Use `text` for pseudo-code, diagrams, and tool calls

2. **Generic types**: Wrap in inline code backticks
   - Write `ArrayPool<T>` not ArrayPool<T>

3. **Headings**: Use only one H1 (`#`) per document
   - Use H2+ (`##`, `###`) for sections

4. **Spacing**: Add blank lines before and after:
   - Code blocks
   - Tables
```

### Pre-Commit Hook Integration

The pre-commit hook should:

1. Run `markdownlint-cli2 --fix` on staged `.md` files
2. Re-stage any files that were modified
3. Fail only if unfixable errors remain

```bash
# Example pre-commit hook snippet
for file in $(git diff --cached --name-only --diff-filter=ACM | grep '\.md$'); do
    npx markdownlint-cli2 --fix "$file"
    git add "$file"
done

# Check for remaining errors
npx markdownlint-cli2 $(git diff --cached --name-only --diff-filter=ACM | grep '\.md$')
```

---

## Common Patterns to Avoid

### Pattern 1: Workflow Diagrams Without Language

**Incorrect**:

````markdown
```
Request → Processing → Response
```
````

**Correct**:

````markdown
```text
Request -> Processing -> Response
```
````

### Pattern 2: Agent Tool Calls Without Language

**Incorrect**:

````markdown
```
mcp__cloudmcp-manager__memory-search_nodes(query="topic")
```
````

**Correct**:

````markdown
```text
mcp__cloudmcp-manager__memory-search_nodes(query="topic")
```
````

### Pattern 3: Pseudo-Code Without Language

**Incorrect**:

````markdown
```
IF condition THEN
  do something
ELSE
  do something else
END IF
```
````

**Correct**:

````markdown
```text
IF condition THEN
  do something
ELSE
  do something else
END IF
```
````

### Pattern 4: Command Output Without Language

**Incorrect**:

````markdown
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```
````

**Correct**:

````markdown
```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
```
````

---

## Checklist for New Documents

Before committing any markdown file:

- [ ] Only one H1 (`#`) heading (the title)
- [ ] All code blocks have language identifiers
- [ ] Generic types wrapped in inline code backticks
- [ ] Blank lines before and after code blocks
- [ ] Blank lines before and after tables
- [ ] No emphasis (`_text_`) used as standalone headings
- [ ] Ran `npx markdownlint-cli2 --fix` on the file
- [ ] Verified no remaining errors with `npx markdownlint-cli2`

---

## Related Documents

- `.markdownlint-cli2.yaml` - Repository linting configuration
- `.editorconfig` - Editor formatting rules
- `AGENT-INSTRUCTIONS.md` - Agent execution protocol
- `CONTRIBUTING.md` - Contribution guidelines

---

## Document History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2025-12-13 | Initial specification |
