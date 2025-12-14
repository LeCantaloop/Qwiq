# Skills Repository

Extracted learnings and atomic skills from Qwiq development sessions (December 2025).

> **Source**: Comprehensive retrospective analysis (2025-12-13)
> **Total Skills**: 22
> **Atomicity Average**: 94%

## Quick Navigation

| Category | Skills | File |
|----------|--------|------|
| Build & CI | 3 | [build-skills.md](build-skills.md) |
| Testing | 3 | [testing-skills.md](testing-skills.md) |
| Code Quality | 3 | [quality-skills.md](quality-skills.md) |
| Strategy | 2 | [strategy-skills.md](strategy-skills.md) |
| Documentation | 2 | [documentation-skills.md](documentation-skills.md) |
| Git | 2 | [git-skills.md](git-skills.md) |
| Markdown | 2 | [markdown-skills.md](markdown-skills.md) |
| Developer Experience | 2 | [devex-skills.md](devex-skills.md) |
| Workflow | 1 | [workflow-skills.md](workflow-skills.md) |
| Installation | 1 | [workflow-skills.md](workflow-skills.md) |
| GitHub | 2 | [github-skills.md](github-skills.md) |

## Skills by Atomicity (Highest First)

| Skill | Atomicity | Category |
|-------|-----------|----------|
| Skill-GitHooks-001 | 99% | Git-Hooks |
| Skill-Markdown-001 | 98% | Markdown |
| Skill-GitHub-001 | 98% | GitHub |
| Skill-Strategic-001 | 97% | Strategy |
| Skill-Git-001 | 97% | Git |
| Skill-DevEx-001 | 97% | DevEx |
| Skill-Build-001 | 96% | Build |
| Skill-Markdown-002 | 96% | Markdown |
| Skill-CI-001 | 95% | CI |
| Skill-Doc-002 | 95% | Documentation |
| Skill-Test-001 | 95% | Testing |
| Skill-Workflow-001 | 95% | Workflow |
| Skill-Test-003 | 94% | Testing |
| Skill-Strategic-002 | 94% | Strategy |
| Skill-Doc-001 | 93% | Documentation |
| Skill-Build-002 | 93% | Build |
| Skill-Quality-001 | 92% | Quality |
| Skill-Issue-001 | 92% | GitHub |
| Skill-Install-001 | 91% | Installation |
| Skill-Git-002 | 91% | Git |
| Skill-Test-002 | 91% | Testing |
| Skill-Quality-002 | 90% | Quality |
| Skill-Quality-003 | 88% | Quality |

## How to Use

### Apply a Skill

When implementing a strategy from this repository, cite it explicitly:

```markdown
**Applying**: Skill-Build-001
**Strategy**: Use CI build flags locally
**Expected**: Match CI analyzer behavior

[Execute command...]

**Result**: Build succeeded with same warnings as CI
**Skill Validated**: Yes
```

### Update Skills

When you discover an improvement or new skill:

1. Add to appropriate category file
2. Follow atomicity requirements (88%+ minimum)
3. Include evidence and context
4. Update this README with new entry

### Deduplication

Before adding a new skill:

1. Search this directory for similar concepts
2. If >70% similar, update existing skill instead
3. If new, add with unique ID

## Atomicity Scoring

Skills are scored 0-100% on clarity and specificity:

| Score | Quality | Example |
|-------|---------|---------|
| 95-100% | Excellent | "Use /m:1 /nodeReuse:false for CI builds" |
| 70-94% | Good | Most skills in this repository |
| 40-69% | Needs Work | Too vague or compound statements |
| <40% | Rejected | Too ambiguous |

**Penalties Applied**:

- Compound statements ("and", "also"): -15% each
- Vague terms ("generally", "sometimes"): -20% each
- Length > 15 words: -5% per extra word

## Document Control

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2025-12-13 | Initial skills repository with 22 atomic skills |

## Related Documents

- `.agents/AGENT-INSTRUCTIONS.md` - Complete skill reference in agent instructions
- `.agents/retrospective/2025-12-13-comprehensive-agent-system-analysis.md` - Full retrospective with skill extraction details
- `.agents/AGENT-SYSTEM.md` - Agent system overview and skill citation protocol
