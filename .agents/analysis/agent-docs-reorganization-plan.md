# Agent Documentation Reorganization Plan

**Date**: 2025-12-13
**Author**: Analyst Agent
**Status**: Research Complete - Ready for Review

---

## Executive Summary

The `.agents/` directory currently contains 75 files across various subdirectories. Many files are misplaced relative to the expected agent workflow structure. This plan provides a comprehensive reorganization strategy to align documentation with the standard agent output categories.

---

## Current State Inventory

### Root-Level Files (23 files)

| File                                  | Current Purpose                         | Proposed Category                       |
| ------------------------------------- | --------------------------------------- | --------------------------------------- |
| `AGENT-SYSTEM.md`                     | Agent catalog, workflows, memory system | **Keep at root** (system documentation) |
| `AGENT-INSTRUCTIONS.md`               | Task execution protocol                 | **Keep at root** (system documentation) |
| `HANDOFF.md`                          | Session-to-session context              | **Keep at root** (critical handoff)     |
| `PROMPTS.md`                          | Reusable agent prompts                  | **Keep at root** (system documentation) |
| `modernize-TODO-index.md`             | Wave summary, navigation, metrics       | `planning/`                             |
| `modernize-wave1.md`                  | Wave 0-1 task tracking                  | `planning/`                             |
| `modernize-wave2.md`                  | Wave 2 task tracking                    | `planning/`                             |
| `modernize-wave3-5.md`                | Waves 3-5 task tracking                 | `planning/`                             |
| `modernize-explainer.md`              | PRD for modernization effort            | `planning/`                             |
| `wave-3-task-definitions.md`          | Wave 3 detailed tasks                   | `planning/`                             |
| `WAVE2-TASK-DEFINITIONS.md`           | Wave 2 detailed tasks                   | `planning/`                             |
| `WAVE4-TASKS.md`                      | Wave 4 task list                        | `planning/`                             |
| `W4-COVERAGE-PLAN.md`                 | Coverage improvement plan               | `planning/`                             |
| `WAVE4-TEST-IMPROVEMENT-PLAN.md`      | Test quality roadmap                    | `planning/`                             |
| `WAVE4-CRAP-SCORE-REDUCTION-PLAN.md`  | CRAP score reduction plan               | `planning/`                             |
| `TASKS-cs0006-fix.md`                 | CS0006 fix task plan                    | `planning/`                             |
| `TASKS-fix-linux-ci-build.md`         | Linux CI fix task plan                  | `planning/`                             |
| `analyzer-debt-inventory.md`          | Analyzer violations inventory           | `analysis/`                             |
| `remaining-analyzer-work.md`          | Remaining analyzer work plan            | `analysis/`                             |
| `CS8xxx-baseline.md`                  | Nullable reference type baseline        | `metrics/`                              |
| `WIREMOCK-IMPLEMENTATION-COMPLETE.md` | WireMock implementation summary         | `retrospective/`                        |
| `contract-tests-plan.md`              | Contract testing plan                   | `planning/`                             |
| `explainer-rest-soap-unit-tests.md`   | REST/SOAP test PRD                      | `planning/`                             |

### Subdirectory Contents

| Directory        | Current Files                                 | Expected Purpose                   |
| ---------------- | --------------------------------------------- | ---------------------------------- |
| `analysis/`      | `.gitkeep` only                               | Analyst findings, research reports |
| `architecture/`  | `.gitkeep` only                               | ADRs (but ADRs are in `docs/adr/`) |
| `critique/`      | `.gitkeep` only                               | Plan reviews, validations          |
| `metrics/`       | `crap-score-baseline.md`                      | Metrics and measurements           |
| `planning/`      | `.gitkeep` only                               | Plans, PRDs, task definitions      |
| `plans/`         | `git-hooks-plan.md`                           | Should merge with `planning/`      |
| `qa/`            | `.gitkeep` only                               | Test strategies, reports           |
| `retrospective/` | `2025-12-13-session-40-agent-installation.md` | Learning extractions               |
| `sessions/`      | 32 session log files                          | Session logs                       |

---

## Proposed Directory Structure

```text
.agents/
├── AGENT-SYSTEM.md              # Keep: Agent catalog, workflows
├── AGENT-INSTRUCTIONS.md        # Keep: Task execution protocol
├── HANDOFF.md                   # Keep: Session-to-session context
├── PROMPTS.md                   # Keep: Reusable agent prompts
│
├── analysis/                    # Analyst findings, research reports
│   ├── analyzer-debt-inventory.md
│   ├── remaining-analyzer-work.md
│   └── agent-docs-reorganization-plan.md (this file)
│
├── architecture/                # ADRs (note: primary ADRs in docs/adr/)
│   └── .gitkeep                 # ADR links or summaries only
│
├── critique/                    # Plan reviews, validations
│   └── .gitkeep
│
├── metrics/                     # Metrics and measurements
│   ├── crap-score-baseline.md
│   └── CS8xxx-baseline.md       # Move from root
│
├── planning/                    # Plans, PRDs, task definitions
│   ├── modernize-TODO-index.md
│   ├── modernize-wave1.md
│   ├── modernize-wave2.md
│   ├── modernize-wave3-5.md
│   ├── modernize-explainer.md
│   ├── wave-3-task-definitions.md
│   ├── WAVE2-TASK-DEFINITIONS.md
│   ├── WAVE4-TASKS.md
│   ├── W4-COVERAGE-PLAN.md
│   ├── WAVE4-TEST-IMPROVEMENT-PLAN.md
│   ├── WAVE4-CRAP-SCORE-REDUCTION-PLAN.md
│   ├── TASKS-cs0006-fix.md
│   ├── TASKS-fix-linux-ci-build.md
│   ├── contract-tests-plan.md
│   ├── explainer-rest-soap-unit-tests.md
│   └── git-hooks-plan.md        # Move from plans/
│
├── qa/                          # Test strategies, reports
│   └── .gitkeep
│
├── retrospective/               # Learning extractions
│   ├── 2025-12-13-session-40-agent-installation.md
│   └── WIREMOCK-IMPLEMENTATION-COMPLETE.md  # Move from root
│
└── sessions/                    # Session logs (32 files)
    ├── .gitkeep
    └── YYYY-MM-DD-*.md          # All session logs
```

---

## Reorganization Actions

### Phase 1: Move Root Files to Appropriate Subdirectories

| Action | Source                                | Destination      | Rationale                    |
| ------ | ------------------------------------- | ---------------- | ---------------------------- |
| Move   | `modernize-TODO-index.md`             | `planning/`      | Task tracking and navigation |
| Move   | `modernize-wave1.md`                  | `planning/`      | Wave task definitions        |
| Move   | `modernize-wave2.md`                  | `planning/`      | Wave task definitions        |
| Move   | `modernize-wave3-5.md`                | `planning/`      | Wave task definitions        |
| Move   | `modernize-explainer.md`              | `planning/`      | PRD document                 |
| Move   | `wave-3-task-definitions.md`          | `planning/`      | Task definitions             |
| Move   | `WAVE2-TASK-DEFINITIONS.md`           | `planning/`      | Task definitions             |
| Move   | `WAVE4-TASKS.md`                      | `planning/`      | Task definitions             |
| Move   | `W4-COVERAGE-PLAN.md`                 | `planning/`      | Coverage plan                |
| Move   | `WAVE4-TEST-IMPROVEMENT-PLAN.md`      | `planning/`      | Test improvement plan        |
| Move   | `WAVE4-CRAP-SCORE-REDUCTION-PLAN.md`  | `planning/`      | CRAP score plan              |
| Move   | `TASKS-cs0006-fix.md`                 | `planning/`      | Task plan                    |
| Move   | `TASKS-fix-linux-ci-build.md`         | `planning/`      | Task plan                    |
| Move   | `contract-tests-plan.md`              | `planning/`      | Contract test plan           |
| Move   | `explainer-rest-soap-unit-tests.md`   | `planning/`      | PRD document                 |
| Move   | `analyzer-debt-inventory.md`          | `analysis/`      | Analysis document            |
| Move   | `remaining-analyzer-work.md`          | `analysis/`      | Analysis document            |
| Move   | `CS8xxx-baseline.md`                  | `metrics/`       | Metrics baseline             |
| Move   | `WIREMOCK-IMPLEMENTATION-COMPLETE.md` | `retrospective/` | Implementation retrospective |

### Phase 2: Merge Duplicate Directories

| Action | Source                    | Destination | Rationale                 |
| ------ | ------------------------- | ----------- | ------------------------- |
| Merge  | `plans/`                  | `planning/` | Single location for plans |
| Move   | `plans/git-hooks-plan.md` | `planning/` | Consolidate plans         |
| Delete | `plans/`                  | N/A         | Empty after merge         |

### Phase 3: Update Internal References

Files that reference moved documents need updates:

1. **HANDOFF.md** - References `modernize-TODO-index.md`, wave files
2. **PROMPTS.md** - References `modernize-TODO-index.md`, wave files
3. **AGENT-INSTRUCTIONS.md** - References `modernize-TODO-index.md`, wave files
4. **Session logs** - May reference root-level files
5. **modernize-TODO-index.md** - Navigation links to wave files

---

## Files to Keep at Root

These files should remain at the `.agents/` root level:

| File                    | Reason                                                    |
| ----------------------- | --------------------------------------------------------- |
| `AGENT-SYSTEM.md`       | Core agent system documentation, referenced by all agents |
| `AGENT-INSTRUCTIONS.md` | Operational protocol, must be immediately accessible      |
| `HANDOFF.md`            | Critical session handoff, needs high visibility           |
| `PROMPTS.md`            | Reusable prompts, frequently accessed                     |

---

## Files to Archive or Remove

| File             | Action | Rationale                             |
| ---------------- | ------ | ------------------------------------- |
| `plans/.gitkeep` | Remove | Directory will be deleted after merge |

---

## Naming Conventions

### Proposed Naming Standards

| Category      | Pattern                            | Example                                  |
| ------------- | ---------------------------------- | ---------------------------------------- |
| Analysis      | `NNN-[topic]-analysis.md`          | `001-analyzer-debt-analysis.md`          |
| Planning      | `[scope]-[type].md`                | `wave4-tasks.md`, `coverage-plan.md`     |
| Metrics       | `[metric]-baseline.md`             | `crap-score-baseline.md`                 |
| Sessions      | `YYYY-MM-DD-session-NN-[topic].md` | `2025-12-13-session-40-agent-install.md` |
| Retrospective | `YYYY-MM-DD-[scope].md`            | `2025-12-13-wiremock-implementation.md`  |

### Files to Rename (Optional)

| Current Name                         | Proposed Name                    | Rationale                     |
| ------------------------------------ | -------------------------------- | ----------------------------- |
| `WAVE4-TASKS.md`                     | `wave4-tasks.md`                 | Consistent lowercase          |
| `W4-COVERAGE-PLAN.md`                | `wave4-coverage-plan.md`         | Consistent naming             |
| `WAVE4-TEST-IMPROVEMENT-PLAN.md`     | `wave4-test-improvement-plan.md` | Consistent lowercase          |
| `WAVE4-CRAP-SCORE-REDUCTION-PLAN.md` | `wave4-crap-score-plan.md`       | Consistent lowercase, shorter |
| `TASKS-cs0006-fix.md`                | `task-cs0006-fix.md`             | Consistent naming             |
| `TASKS-fix-linux-ci-build.md`        | `task-linux-ci-fix.md`           | Consistent naming             |

---

## Impact Assessment

### Breaking Changes

Moving the following files will require updating references:

| File                      | Impact | Affected Files                                |
| ------------------------- | ------ | --------------------------------------------- |
| `modernize-TODO-index.md` | High   | HANDOFF.md, PROMPTS.md, AGENT-INSTRUCTIONS.md |
| `modernize-wave*.md`      | High   | HANDOFF.md, PROMPTS.md, AGENT-INSTRUCTIONS.md |
| `modernize-explainer.md`  | Medium | modernize-TODO-index.md                       |

### Low-Risk Moves

These files have fewer references:

- `analyzer-debt-inventory.md` - Standalone analysis
- `remaining-analyzer-work.md` - Standalone analysis
- `CS8xxx-baseline.md` - Metrics baseline
- `WIREMOCK-IMPLEMENTATION-COMPLETE.md` - Retrospective
- Task definition files - Referenced within planning context

---

## Implementation Recommendation

### Option A: Full Reorganization (Recommended)

**Pros:**

- Clean, consistent structure
- Matches expected agent workflow
- Easier navigation for new agents

**Cons:**

- Requires reference updates
- One-time disruption

**Effort**: 2-3 hours including reference updates

### Option B: Minimal Reorganization

Move only non-referenced files, keep wave files at root.

**Pros:**

- Minimal disruption
- No reference updates needed

**Cons:**

- Inconsistent structure
- Root remains cluttered

### Option C: Symbolic Links

Keep files at root, create symbolic links in proper directories.

**Pros:**

- No breaking changes
- Gradual transition possible

**Cons:**

- Platform-dependent (Windows symlinks have issues)
- Maintenance overhead

---

## Recommended Execution Order

1. **Create analysis directory content** (move analyzer docs)
2. **Create metrics directory content** (move baseline docs)
3. **Merge plans/ into planning/** (move git-hooks-plan.md)
4. **Move planning documents** (all task/wave/PRD files)
5. **Move retrospective content** (WIREMOCK-IMPLEMENTATION-COMPLETE.md)
6. **Update references** (HANDOFF.md, PROMPTS.md, AGENT-INSTRUCTIONS.md)
7. **Delete empty plans/ directory**
8. **Validate all links work**

---

## Validation Checklist

After reorganization:

- [ ] All files accessible at new locations
- [ ] Internal links updated and working
- [ ] HANDOFF.md references correct paths
- [ ] PROMPTS.md references correct paths
- [ ] AGENT-INSTRUCTIONS.md references correct paths
- [ ] Session logs can reference planning docs
- [ ] No orphaned files at root level
- [ ] Git history preserved (use `git mv`)

---

## Session Log Files Summary

The `sessions/` directory contains 32 session logs covering:

- December 5-13, 2025 development sessions
- Phase progression from Wave 1 through Wave 4
- Various topics: CI fixes, coverage, testing, WireMock, agent installation

**Status**: Sessions directory is well-organized. No changes needed.

---

## Architecture Directory Note

The `architecture/` directory is currently empty (`.gitkeep` only). The actual ADRs are stored in `docs/adr/` (9 ADR files). Consider:

1. **Option 1**: Keep `architecture/` empty, document that ADRs live in `docs/adr/`
2. **Option 2**: Add a `README.md` linking to `docs/adr/`
3. **Option 3**: Create agent-specific architecture docs here

**Recommendation**: Option 2 - Add a README.md with links to ADRs.

---

## Document Control

| Version | Date       | Author        | Changes                     |
| ------- | ---------- | ------------- | --------------------------- |
| 1.0     | 2025-12-13 | Analyst Agent | Initial reorganization plan |
