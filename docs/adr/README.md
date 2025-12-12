# Architecture Decision Records (ADRs)

This directory contains Architecture Decision Records (ADRs) that document key architectural decisions made for the Qwiq project.

## What is an ADR?

An Architecture Decision Record (ADR) captures an important architectural decision made along with its context and consequences.

## ADR Format

Each ADR follows this structure:

- **Status**: Proposed, Accepted, Deprecated, Superseded
- **Context**: The issue motivating this decision
- **Decision**: The change being proposed or has been made
- **Consequences**: What becomes easier or more difficult

## Index of ADRs

- [ADR-001: Factory Pattern for WorkItemStore](ADR-001-factory-pattern-workitemstore.md)
- [ADR-002: Interface-First Design](ADR-002-interface-first-design.md)
- [ADR-003: REST vs SOAP Client Strategy](ADR-003-rest-vs-soap-strategy.md)
- [ADR-004: Multi-Targeting Approach](ADR-004-multi-targeting-approach.md)
- [ADR-005: Central Package Management](ADR-005-central-package-management.md)
- [ADR-006: Nullable Reference Types Migration](ADR-006-nullable-reference-types.md)
- [ADR-007: REST Client Testability](ADR-007-rest-client-testability.md)
- [ADR-008: WireMock-Based Offline REST Client Testing](ADR-008-wiremock-offline-rest-testing.md)
- [ADR-009: Polyfill Strategy for ArgumentNullException.ThrowIfNull](ADR-009-polyfill-argument-null-exception.md)

## Creating a New ADR

1. Copy the template structure from existing ADRs
2. Number sequentially (ADR-XXX)
3. Use kebab-case for filenames: `ADR-XXX-short-description.md`
4. Update this README with a link to the new ADR
5. Mark status as "Proposed" initially
6. Update to "Accepted" after team review

## References

- [Michael Nygard's ADR format](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions)
- [ADR GitHub Organization](https://adr.github.io/)
