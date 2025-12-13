# Session Log: Linting Gaps Analysis and Wave 2 Task Updates

**Date**: December 11, 2025
**Session**: 25
**Branch**: `chore/modernize-wave-2`
**Focus**: Analyze PR #65 bot feedback for linting gaps and update Wave 2 tasks

## Summary

Analyzed 112 PR review comments from bots (coderabbitai, Copilot, github-advanced-security) to identify:

1. Linting configuration gaps that could prevent future violations
2. Code quality issues that should become Wave 2 tasks
3. False positives or intentional design choices

## Linting Gaps Identified

### Markdown Linting Violations (by rule)

| Rule  | Count | Description                                            |
| ----- | ----- | ------------------------------------------------------ |
| MD031 | 15    | Fenced code blocks should be surrounded by blank lines |
| MD040 | 6     | Fenced code blocks should have a language specified    |
| MD034 | 2     | Bare URLs used (should be wrapped)                     |
| MD036 | 1     | Emphasis used instead of heading                       |
| MD058 | 3     | Tables should be surrounded by blank lines             |
| MD022 | 2     | Headings should be surrounded by blank lines           |

### Root Cause

The repository had PackedPrettier installed (`dotnet pprettier`) but lacked configuration files:

- No `.prettierrc` for Prettier formatting rules
- No `.markdownlint-cli2.yaml` for markdownlint rules
- No markdown-specific rules in `.editorconfig`

## Configuration Files Created

### `.prettierrc`

```json
{
  "$schema": "https://json.schemastore.org/prettierrc",
  "proseWrap": "always",
  "printWidth": 120,
  "tabWidth": 2,
  "useTabs": false,
  "endOfLine": "auto"
}
```

### `.markdownlint-cli2.yaml`

Key rules enabled:

- MD031: Blank lines around fenced code blocks
- MD040: Language identifiers on code blocks
- MD034: No bare URLs
- MD058: Blank lines around tables

### `.prettierignore`

Excludes:

- Build outputs (`artifacts/`, `bin/`, `obj/`)
- Verify snapshot files (`*.verified.*`, `*.received.*`)
- WireMock recordings (JSON structure must be preserved)

### `.editorconfig` Updates

Added sections for:

- Markdown files (indent_size: 2, max_line_length: 120)
- YAML files (indent_size: 2)
- JSON files (indent_size: 2)

## Expert Review Feedback

Used `csharp-expert` subagent to validate generated tasks. Key recommendations:

### Effort Estimate Adjustments

| Task  | Original | Revised | Reason                                |
| ----- | -------- | ------- | ------------------------------------- |
| W2.22 | 1-2h     | 2-3h    | Digest lookup & validation time       |
| W2.24 | 4-6h     | 6-8h    | Metadata ripple effects               |
| W2.25 | 2-3h     | 8-12h   | TDD requirement + multi-target checks |
| W2.26 | 0.5h     | 1-2h    | Trace usage across TFMs               |
| W2.27 | 1h       | 3-4h    | Requires audit + fuzzing/unit tests   |
| W2.29 | 0.5h     | 1-2h    | Guard placement affects contracts     |

### Priority Elevations

| Task  | Original | Revised | Reason                              |
| ----- | -------- | ------- | ----------------------------------- |
| W2.27 | Low      | Medium  | Serialization safety                |
| W2.29 | Medium   | HIGH    | Null safety in service entry points |

## Documentation Updates

1. **CONTRIBUTING.md**: Added "Formatting and Linting" section with commands and configuration table
2. **copilot-instructions.md**: Updated Key Configuration Files table and added Formatting and Linting Tools section
3. **markdown.instructions.md**: Added Linting Configuration section and updated validation checklist

## Files Changed

### Created

- `.prettierrc` - Prettier configuration
- `.prettierignore` - Prettier ignore patterns
- `.markdownlint-cli2.yaml` - Markdownlint configuration
- `.agents/sessions/2025-12-11-linting-gaps-analysis.md` - This session log

### Modified

- `.editorconfig` - Added markdown/YAML/JSON sections
- `.agents/modernize-TODO.md` - Updated effort estimates per expert review
- `CONTRIBUTING.md` - Added formatting/linting documentation
- `.github/copilot-instructions.md` - Added linting tools documentation
- `.github/instructions/markdown.instructions.md` - Added linting configuration section

## Wave 2 Tasks Status

Tasks W2.21-W2.31 were already added in Session 24. This session:

1. Validated task completeness with expert review
2. Updated effort estimates based on expert feedback
3. Created the linting configuration files that W2.21 specifies

### W2.21 Status

The task specifies creating `.prettierrc` and `.markdownlint-cli2.yaml`. These files have now been created, but the task should remain open until:

- [ ] CI workflow includes markdown linting step
- [ ] All existing markdown files pass linting
- [ ] Documentation updated in CONTRIBUTING.md ✅

## Next Steps

1. **Immediate**: Run `dotnet pprettier --check "**/*.md"` to identify files needing fixes
2. **Short-term**: Add markdown linting to CI workflow (part of W2.21)
3. **Medium-term**: Fix all markdown files to pass linting
4. **Long-term**: Complete remaining W2.22-W2.31 tasks

## Validation

- [x] Linting configuration files created
- [x] Documentation updated
- [x] Expert review feedback incorporated
- [x] Effort estimates updated in TODO
- [ ] CI workflow updated (pending)
- [ ] All markdown files pass linting (pending)
