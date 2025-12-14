# Plan Critique: Security Shift-Left Gap Retrospective

**Document**: `.agents/retrospective/security-shift-left-gap.md`
**Review Date**: 2025-12-13
**Critic Agent**: Plan Validation Specialist

---

## Verdict

### NEEDS REVISION

---

## Summary

The retrospective provides a thorough post-mortem of a real security incident (CWE-78 shell injection in pre-commit hooks) and correctly identifies that the agent system was deployed but not operationalized. However, the document conflates analysis with solution, proposes process overhead without cost acknowledgment, and lacks enforcement mechanisms that would prevent recurrence.

The retrospective is approximately 800 lines and proposes 7 new documentation artifacts, 15+ checkboxes, and multiple decision trees. This creates significant compliance burden without evidence that such overhead will be followed in practice.

---

## Strengths

1. **Genuine Incident Analysis**: The document addresses a real security vulnerability that was caught late (by GitHub Copilot bot, not the agent system).

2. **Honest Self-Assessment**: Correctly identifies that the agent system "exists as documentation, not as operational practice" (line 424).

3. **Detailed Timeline**: Provides forensic reconstruction of the session timeline showing exactly when agents should have been invoked.

4. **Concrete Vulnerability Details**: Links to specific PRs, identifies CWE-78, and documents the fix (separate document at `.agents/security/shell-injection-fix.md`).

5. **Root Cause Identification**: The five causes (lines 108-195) are legitimate observations about process gaps.

---

## Issues Found

### Critical (Must Fix)

- [ ] **No Enforcement Mechanism**: The retrospective proposes checklists, matrices, and decision trees but no mechanism to enforce them. Line 546 mentions "CI gate: Commits to these files must have security review comment" but this is aspirational, not implemented. Without enforcement, this becomes documentation debt.

- [ ] **Cost-Benefit Analysis Missing**: The "Correct Process for CI/Hook Changes" (lines 291-378) estimates 45 minutes for a workflow change that originally took ~5 minutes. The retrospective does not acknowledge this 9x increase in effort or when such overhead is justified.

- [ ] **No ADR for Architectural Decision**: Per architect feedback, introducing mandatory agent review gates is an architectural decision that should be captured in an ADR. The retrospective skips this step despite advocating for process rigor.

- [ ] **Success Metrics Undefined**: Lines 801-814 list action items but no success metrics. How do we know "shift left is working"? The retrospective proposes measuring "% of infrastructure changes with security review" (line 805) but doesn't define target, baseline, or measurement method.

### Important (Should Fix)

- [ ] **Solution Complexity vs. Problem Severity**: The vulnerability was in a pre-commit hook affecting developer machines, severity HIGH. The proposed solution creates 7 new documentation artifacts and 15+ mandatory checkboxes. This may be disproportionate response.

- [ ] **Agent System Simplification Not Explored**: Factor 4 (lines 273-288) identifies "Agent System Complexity May Exceed Practical Usage" with 15 different agent types. The retrospective acknowledges this but proposes adding MORE process (decision trees, matrices) rather than simplifying the system.

- [ ] **Existing Documentation Duplication**: Lines 496-522 propose an "Agent Invocation Matrix" but `AGENT-SYSTEM.md` already has a Routing Heuristics table (lines 211-226). The retrospective doesn't reconcile these.

- [ ] **Time Estimates Unrealistic**: Line 295 estimates "Change Assessment (Should take 2 minutes)" followed by DevOps Review (10 min), Security Review (15 min), Architect Review (10 min), Implementation (5 min), QA (5 min) = 47 minutes. Real-world experience suggests this will be skipped under deadline pressure.

### Minor (Consider)

- [ ] **Markdown Formatting**: Some code blocks use `bash` language identifier for content that is actually pseudo-code workflow descriptions (lines 453-494). Should use `text` or `markdown`.

- [ ] **Document Length**: At 814 lines, this is the longest document in `.agents/retrospective/`. Consider splitting into incident report vs. process improvement proposal.

- [ ] **Inconsistent Terminology**: Uses "devops agent" (lines 59, 306, 468) but `AGENT-SYSTEM.md` lists it as "DevOps" with capital letters and describes it for "CI/CD pipelines" (line 52-53).

---

## Questions for Planner/Retrospective Author

1. **Enforcement Strategy**: How will the proposed checklists be enforced? Git hooks? CI checks? Honor system? If honor system, what evidence suggests developers will follow them when the existing agent documentation wasn't consulted?

2. **Velocity Impact Acknowledgment**: The architect correctly notes that velocity impact should be explicitly acknowledged. Is a 9x time increase acceptable for ALL infrastructure changes, or only for security-critical ones?

3. **Simplification Alternative**: Would reducing the agent catalog from 15 to 5 "core" agents (implementer, architect, security, qa, orchestrator) be more effective than adding process gates?

4. **Automatic Detection**: Instead of manual checklists, could file-pattern detection (e.g., changes to `.githooks/*` or `.github/workflows/*` automatically trigger security review) replace human decision-making?

5. **Asynchronous Review Option**: The retrospective assumes blocking reviews. Would asynchronous security review (comment-based, post-commit) be acceptable for lower-risk infrastructure changes?

6. **Baseline Measurement**: Before implementing process changes, do we have a baseline of "infrastructure changes without security review"? Without baseline, we cannot measure improvement.

---

## Alternative Approaches Not Explored

### 1. Agent System Simplification

**Current**: 15 agent types documented in AGENT-SYSTEM.md
**Alternative**: Reduce to 5 core agents with clear, non-overlapping responsibilities

| Core Agent      | Replaces                     | Responsibility       |
| --------------- | ---------------------------- | -------------------- |
| **builder**     | implementer, csharp-expert   | All code writing     |
| **reviewer**    | critic, qa, security         | All code review      |
| **designer**    | architect, planner           | All design decisions |
| **researcher**  | analyst, independent-thinker | All investigation    |
| **coordinator** | orchestrator                 | All routing          |

**Benefit**: Simpler mental model, easier to remember which agent to invoke
**Cost**: Loss of specialization

### 2. Automatic Change Detection

**Current Proposal**: Manual checklist asking "Does this affect build/CI/infrastructure?"
**Alternative**: Git hook or CI job that detects file patterns and automatically invokes appropriate review

```yaml
# .github/security-review-trigger.yml
triggers:
  - pattern: ".githooks/*"
    required_review: security
  - pattern: ".github/workflows/*"
    required_review: security, devops
  - pattern: "*.csproj"
    required_review: architect
```

**Benefit**: No human decision point, cannot be skipped
**Cost**: Implementation effort, potential false positives

### 3. Post-Commit Security Review

**Current Proposal**: Blocking pre-implementation review
**Alternative**: Non-blocking post-commit review with required sign-off before merge

**Benefit**: Maintains velocity for low-risk changes, security still catches issues before merge
**Cost**: Issues found later in cycle (but still before production)

### 4. Risk-Tiered Process

**Current Proposal**: Same process for all infrastructure changes
**Alternative**: Tiered process based on risk assessment

| Risk Tier    | Triggers              | Process                                 |
| ------------ | --------------------- | --------------------------------------- |
| **Critical** | Auth, crypto, hooks   | Full security review, ADR required      |
| **High**     | CI/CD, build config   | Security review, architect consultation |
| **Medium**   | Dependencies, tooling | Self-review with checklist              |
| **Low**      | Documentation         | No additional review                    |

**Benefit**: Right-sized process for risk level
**Cost**: Risk assessment still requires human judgment

---

## Success Metrics (Proposed)

If this retrospective is approved and implemented, success should be measured by:

| Metric                                        | Baseline             | Target                    | Measurement Method                |
| --------------------------------------------- | -------------------- | ------------------------- | --------------------------------- |
| Security issues caught by agent system        | 0/1 (0%)             | >80%                      | Track source of security findings |
| Infrastructure changes with documented review | Unknown              | >90%                      | Commit message audit              |
| Time to implement infrastructure changes      | ~5 min               | <30 min (with review)     | Sample timing                     |
| Agent system invocation rate                  | Low (estimated <10%) | >50% for eligible changes | Session log analysis              |
| False positive rate (unnecessary reviews)     | Unknown              | <20%                      | Developer feedback                |

**Note**: Without baseline measurements, improvement cannot be quantified.

---

## Recommendations

### Must Address Before Approval

1. **Add Enforcement Section**: Describe how checklists will be enforced (CI gates, git hooks, or explicit decision to rely on honor system with rationale).

2. **Acknowledge Velocity Cost**: Add section acknowledging the time cost of full agent review process and when it is justified.

3. **Create ADR**: Document the architectural decision to require agent gates for infrastructure changes (ADR-011 or similar).

4. **Define Success Metrics**: Add measurable success criteria with baseline, target, and measurement method.

### Should Address

1. **Explore Simplification**: Add section evaluating whether reducing agent count would be more effective than adding process.

2. **Consider Automation**: Evaluate automatic file-pattern detection as alternative to manual checklists.

3. **Reconcile with Existing Docs**: Either update AGENT-SYSTEM.md routing heuristics OR explain why new matrix is needed.

### Could Address

1. **Split Document**: Consider separating incident report from process improvement proposal.

2. **Time-Box Pilot**: Propose piloting the new process on one change type before full rollout.

---

## Approval Conditions

This retrospective may be **APPROVED** when:

1. [ ] Enforcement mechanism is specified (not just "CI gate should exist")
2. [ ] Velocity impact is acknowledged with explicit cost-benefit statement
3. [ ] An ADR is created for the architectural decision (or explicit statement that no ADR is needed with rationale)
4. [ ] Success metrics are defined with baseline, target, and measurement method
5. [ ] Duplicate documentation concern is addressed (new matrix vs. existing routing heuristics)

---

## Handoff Options

| Target            | Condition                  | Outcome                                              |
| ----------------- | -------------------------- | ---------------------------------------------------- |
| **retrospective** | Revise with above feedback | Updated document addressing critical issues          |
| **architect**     | Create ADR                 | ADR-011-Agent-Gates-for-Infrastructure               |
| **planner**       | Implementation planning    | Task breakdown for enforcement mechanisms            |
| **implementer**   | Build automation           | File-pattern detection for automatic review triggers |

---

## Document Control

| Version | Date       | Author       | Changes          |
| ------- | ---------- | ------------ | ---------------- |
| 1.0     | 2025-12-13 | Critic Agent | Initial critique |

---

## Appendix: Architect Feedback Integration

The architect's review (referenced in user request) identified these gaps:

| Architect Concern                | Addressed in Retrospective?  | Critique Assessment                                  |
| -------------------------------- | ---------------------------- | ---------------------------------------------------- |
| No ADR                           | No                           | **Critical Gap** - architectural decisions need ADRs |
| No enforcement                   | Partially (mentions CI gate) | **Critical Gap** - aspirational, not implemented     |
| Weak operationalization          | Yes (extensively discussed)  | **Strength** - problem well-identified               |
| Velocity impact not acknowledged | No                           | **Critical Gap** - 9x time increase not discussed    |

The architect's feedback is valid and should be incorporated before this retrospective is committed as actionable guidance.
