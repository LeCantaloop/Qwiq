# Fix Markdown Code Fence Closings

This document describes a tool for repairing malformed markdown code fences. The core issue: "closing fences sometimes include language identifiers" when they should remain plain.

## Key Problem

Closing markdown fences incorrectly formatted like ` ```python` instead of ` ``` ` break parsing and cause rendering failures.

## Main Algorithm

The solution tracks code block state line-by-line:

- **Opening fence detection**: Identifies lines matching the pattern of opening backticks with language tags
- **Malformed closing detection**: Catches closing fences that erroneously include language identifiers
- **Proper closing insertion**: Adds correctly formatted closing fences before problematic lines
- **Unclosed block handling**: Appends closing fences at end-of-file if needed

## Implementation Options

Three implementations are provided:

1. **Python** (recommended): Full-featured with both single-string and batch file processing
2. **Bash**: Quick pattern-matching for identifying potential issues
3. **PowerShell**: Directory traversal with automated fixing capability

All versions preserve indentation levels from opening fences and handle mixed line endings.

## Practical Use Cases

This tool applies when: generating markdown with code blocks, validating documentation before commits, fixing "bleeding" code blocks that merge into surrounding content, or batch-correcting existing markdown files with rendering issues.

## Additional Tools

### add_fence_language.py

Companion script that adds default language specifier (`text`) to code fences that don't have one. This fixes MD040 linting errors.

Usage:

```bash
python add_fence_language.py .agents --pattern "**/*.md"
```
