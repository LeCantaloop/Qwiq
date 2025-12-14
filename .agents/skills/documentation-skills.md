# Documentation Skills

## Skill-Doc-001

**Statement**: Split documentation files when approaching 25,000 token AI agent limit

**Atomicity**: 93%

**Category**: Documentation

**Context**: When documentation files become too large for single-context processing

**Evidence**: Session 31 - Split 45,651-token file into 4 files

**Details**:

- Large documentation files exceed AI agent context windows
- Token limit for multi-agent systems typically 25,000-30,000 tokens
- File at 45,651 tokens forced split into 4 separate files
- Proactive splitting prevents context loss mid-analysis

**Symptoms of Over-Sizing**:

- File > 2000 lines
- Estimated tokens > 25,000 (rough: 1 line ≈ 2 tokens)
- Single document covering multiple distinct topics
- Difficult to update without affecting unrelated sections

**Splitting Strategy**:

```text
modernize-TODO.md (45,651 tokens) → split into:
├── modernize-TODO-index.md (navigation + metrics overview)
├── modernize-wave1.md (Wave 0-1 tasks)
├── modernize-wave2.md (Wave 2 tasks)
└── modernize-wave3-5.md (Waves 3-5 tasks)
```

**Splitting Guidelines**:

1. **By Wave/Phase**: Group related work units
2. **By Topic**: One concept per file (e.g., architecture ADRs)
3. **Index File**: Create navigation doc with links
4. **Cross-References**: Use links to reference from other files

**Example Structure**:

```markdown
# Documentation Set Index

- [Overview & Navigation](README.md)
- [Wave 1 Tasks](wave1.md)
- [Wave 2 Tasks](wave2.md)
- [Architecture Decisions](../architecture/)
- [Session Log](../sessions/)
```

**Implementation Checklist**:

- [ ] Identify natural boundaries (topics, phases, categories)
- [ ] Create index/navigation file
- [ ] Split content into separate files
- [ ] Update all cross-references
- [ ] Verify no content loss
- [ ] Commit with clear message

**Benefits**:

- Agents can process one file completely
- Easier to navigate and search
- Parallel editing without conflicts
- Better git history (smaller diffs)

---

## Skill-Doc-002

**Statement**: Update HANDOFF.md at session end with build/test status, completed work, and next steps

**Atomicity**: 95%

**Category**: Documentation

**Context**: Every session end

**Evidence**: 40+ sessions following handoff protocol

**Details**:

- HANDOFF.md is cross-session knowledge transfer document
- Must be updated EVERY session before ending work
- Contains critical context for next agent/session
- Failure to update causes context loss

**Required HANDOFF.md Sections**:

```markdown
# Handoff Document

> **Last Updated**: YYYY-MM-DD HH:MM by [Agent/Session]
> **Current Phase**: [Phase name]
> **Branch**: [git branch]

## Current State

**Build Status**: ✅ Passing | ❌ Failing
**Test Status**: ✅ X/Y Passing | ❌ Failing

**Last Commit**: [SHA] - [message]

## What Was Completed

- [x] Task 1 - Brief description
- [x] Task 2 - Brief description
- [ ] Task 3 - Partially done / blocked

## What's Next

The next session should:
1. Specific action
2. Specific action
3. etc.

## Blockers & Concerns

| Issue | Impact | Mitigation |
|-------|--------|-----------|
| [Issue] | [Impact] | [Action] |

## Quick Verification

\`\`\`powershell
# Verify state commands
git log --oneline -5
dotnet build Qwiq.sln -c Release /p:ContinuousIntegrationBuild=true ...
dotnet test Qwiq.sln -c Release --no-build ...
\`\`\`

## Session History

| Date | Phase | Tasks | Status |
|------|-------|-------|--------|
| YYYY-MM-DD | Phase | W2.5 | ✅ Complete |
```

**Best Practices**:

1. **Be Specific**: Don't write "fixed bugs" - list which bugs
2. **Document Decisions**: Why was something done this way?
3. **Flag Blockers Early**: Don't hide problems for next session
4. **Provide Commands**: Include exact commands to verify state
5. **Update Every Session**: Even if minimal work, update the timestamp

**Verification Protocol**:

Before marking session complete, next session should:

1. Read HANDOFF.md
2. Run verification commands
3. Confirm build/tests pass
4. Understand what's next

**Common Mistakes**:

- ❌ Forgetting to update HANDOFF.md
- ❌ Writing vague descriptions
- ❌ Not listing blockers
- ❌ Outdated session history
- ❌ Missing verification commands

**Enforcement**: HANDOFF.md update is part of session end checklist (MANDATORY)
