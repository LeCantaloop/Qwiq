# Copilot Agent Prompts

> **Purpose**: Standard prompts for starting and ending Copilot agent sessions.
> Copy and paste these prompts to ensure consistent agent behavior.

---

## Session Start Prompt

Use this prompt to start a new phase:

```
Read the contents of the .agents directory completely before starting any work:

1. FIRST: Read `.agents/AGENT-INSTRUCTIONS.md` - this governs how you must operate
2. SECOND: Read `.agents/HANDOFF.md` - this contains context from the previous session
3. THIRD: Read `.agents/modernize-TODO.md` - this contains your task details

**Current Branch**: `feat/modernize-3` (commit: 767b30f0)
**Target**: Production v11.0.0 Release for 100+ team members

Execute all tasks specified for the current phase comprehensively, following the AGENT-INSTRUCTIONS.md protocol:
- Create a session log file at `.agents/sessions/YYYY-MM-DD-phase-XX.md`
- Complete the pre-flight checklist
- Work incrementally with small commits
- Check off tasks in modernize-TODO.md as you complete them
- Update your session log with decisions, challenges, and resolutions

Upon completion of each task, immediately update:
- The checkbox in modernize-TODO.md
- Your session log with details

Use tools like `dotnet format` as leverage. Make small commits with conventional commit messages.
```

---

## Session End Prompt

Use this prompt before ending ANY session:

```
Before this session ends, complete the mandatory finalization checklist from AGENT-INSTRUCTIONS.md:

1. DOCUMENTATION UPDATES:
   - [ ] All completed tasks are checked off in `.agents/modernize-TODO.md`
   - [ ] Session log at `.agents/sessions/YYYY-MM-DD-phase-XX.md` is complete with:
     - What was done for each task
     - Decisions made and rationale
     - Challenges encountered and resolutions
     - Files changed and commits made
   - [ ] `.agents/HANDOFF.md` is updated with:
     - Current state (build/test status)
     - What was completed
     - What's next for the following session
     - Any blockers or concerns
     - Verification commands

2. GIT OPERATIONS:
   - [ ] All documentation files are staged: `git add .agents/`
   - [ ] All changes are committed
   - [ ] Force-add session logs if needed: `git add -f .agents/sessions/*.md`

3. VERIFICATION:
   - [ ] Lint clean: `dotnet pprettier --write . && dotnet format`
   - [ ] Build passes: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`
   - [ ] Tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`

The next Copilot session will have NO context except what is in the checked-in documentation. Ensure that agent has everything needed to continue smoothly.
```

---

## Phase-Specific Start Prompts

### Phase 2A: Release Automation

```
Read the contents of the .agents directory completely before starting any work:

1. FIRST: Read `.agents/AGENT-INSTRUCTIONS.md` - this governs how you must operate
2. SECOND: Read `.agents/HANDOFF.md` - this contains context from the previous session
3. THIRD: Read `.agents/modernize-TODO.md` - find Phase 2A tasks

Execute Phase 2A tasks in this priority order:
1. W2.5 - Create Architecture Decision Records (HIGH)
2. W2.2 - Create API Compatibility Baselines (CRITICAL - before any API changes)
3. W2.15 - Pin GitHub Actions by SHA + Dependabot/Renovate (CRITICAL)
4. W2.18 - Enable Package Validation (HIGH)
5. W2.11 - Create Release Workflow (CRITICAL)

Follow the AGENT-INSTRUCTIONS.md protocol:
- Create session log: `.agents/sessions/YYYY-MM-DD-phase-2a.md`
- Work incrementally with small commits
- Check off tasks immediately when complete
- Update session log with decisions and challenges

Use `dotnet format` after code changes. Make small commits with conventional messages.
```

### Phase 2B: Supply Chain Security

```
Read the contents of the .agents directory completely before starting any work:

1. FIRST: Read `.agents/AGENT-INSTRUCTIONS.md` - this governs how you must operate
2. SECOND: Read `.agents/HANDOFF.md` - this contains context from the previous session
3. THIRD: Read `.agents/modernize-TODO.md` - find Phase 2B tasks

Execute Phase 2B tasks:
1. W2.17 - SLSA Provenance Generation (CRITICAL)
2. W2.13 - SBOM Generation - dual pipeline (HIGH)
3. W2.14 - Dependency Review Action (HIGH)

Follow the AGENT-INSTRUCTIONS.md protocol. Work incrementally. Update documentation.
```

### Phase 2C: Testing Enhancements

```
Read the contents of the .agents directory completely before starting any work:

1. FIRST: Read `.agents/AGENT-INSTRUCTIONS.md` - this governs how you must operate
2. SECOND: Read `.agents/HANDOFF.md` - this contains context from the previous session
3. THIRD: Read `.agents/modernize-TODO.md` - find Phase 2C tasks

Execute Phase 2C tasks:
1. W2.16 Phase 1 - REST Unit Tests with WireMock.Net (HIGH)
2. W2.3 - Contract Tests for REST/SOAP Parity (Low)
3. W2.4 - Benchmark CI Integration (Low)

Note: W2.16 uses WireMock.Net for HTTP mocking and Moq 4.16.0 + Moq.Analyzers 0.4.0 for SOAP tests.

Follow the AGENT-INSTRUCTIONS.md protocol. Work incrementally. Update documentation.
```

### Phase 2D: Security Hardening

```
Read the contents of the .agents directory completely before starting any work:

1. FIRST: Read `.agents/AGENT-INSTRUCTIONS.md` - this governs how you must operate
2. SECOND: Read `.agents/HANDOFF.md` - this contains context from the previous session
3. THIRD: Read `.agents/modernize-TODO.md` - find Phase 2D tasks

Execute Phase 2D tasks:
1. W2.19 - CodeQL Advanced Security (integrated into main build, not separate workflow)
2. W2.20 - Secrets Scanning (Medium)

Note: W2.19 should be integrated into main.yml, not a separate workflow, to avoid duplicate builds.

Follow the AGENT-INSTRUCTIONS.md protocol. Work incrementally. Update documentation.
```

### Phase 2E: Documentation

```
Read the contents of the .agents directory completely before starting any work:

1. FIRST: Read `.agents/AGENT-INSTRUCTIONS.md` - this governs how you must operate
2. SECOND: Read `.agents/HANDOFF.md` - this contains context from the previous session
3. THIRD: Read `.agents/modernize-TODO.md` - find Phase 2E tasks

Execute Phase 2E tasks:
1. W2.7 - Update CONTRIBUTING.md (Medium)

Follow the AGENT-INSTRUCTIONS.md protocol. Work incrementally. Update documentation.
```

---

## Troubleshooting Prompts

### If Agent Forgets Documentation

```
STOP. You have not updated the documentation as required.

Complete these mandatory steps from AGENT-INSTRUCTIONS.md:

1. Check off ALL completed tasks in `.agents/modernize-TODO.md`
2. Update your session log at `.agents/sessions/YYYY-MM-DD-phase-XX.md` with:
   - What was done
   - Decisions made
   - Challenges and resolutions
3. Update `.agents/HANDOFF.md` with current state and next steps
4. Commit all documentation: `git add .agents/ && git commit -m "docs: update session documentation"`

The next session has NO context except documentation. Make it complete.
```

### If Agent Loses Context

```
You appear to have lost context. Re-read these files in order:

1. `.agents/AGENT-INSTRUCTIONS.md` - Process instructions
2. `.agents/HANDOFF.md` - Previous session context
3. `.agents/modernize-TODO.md` - Task details
4. `.agents/sessions/` - Recent session logs

Then continue with the current phase tasks.
```

### If Build/Tests Fail

```
Build or tests are failing. Before continuing:

1. Run: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false 2>&1 | Select-String "error"`
2. Identify the specific error
3. Check recent commits: `git log --oneline -5`
4. If needed, revert: `git revert HEAD`

Document the issue and resolution in your session log.
```

---

## Document Control

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2025-12-06 | Initial prompts |
