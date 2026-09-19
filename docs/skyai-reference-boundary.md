# SkyAI architecture reference boundary

## Verified source reference

This project consulted `GlobalAISolutions/SkyAI` branch `develop` at commit
`4d9a3a495168c8c95606b54c5149edf37baca12e` on 2026-09-19. The branch uses a .NET 10 Azure Functions
isolated-worker host, separates API/application/persistence responsibilities, registers services through
DI, and isolates direct AI-provider SDK interaction behind an application interface.

## Patterns adapted

- Explicit host-to-application boundary: the Function maps HTTP input to a narrow contract.
- Application-owned interfaces and DTOs: orchestration is testable without an HTTP host or provider SDK.
- Provider adapter: Microsoft Agent Framework and Azure OpenAI live in `Api`, implementing
  `IAgentRuntime` rather than leaking through the whole solution.
- Lifecycle evidence: a terminal run trace records correlation and outcome without retaining user content.
- Security posture: every HTTP function declares authorization explicitly; local configuration stays
  outside version control.

## Explicit exclusions

No SkyAI source code, package feeds, secrets, endpoint routes, customer data, database schema, test
fixtures, identity/permission model, proprietary business rules, or production configuration is copied
into this repository. Names and contracts in this project are newly authored for NOERITH.

## Deliberate simplifications

SkyAI is a mature multi-domain service. This reference starts with one function, a single agent turn,
an in-memory non-content journal, and no retrieval/tool/action capability. Persistence, streaming,
evaluation, tools, and hosted Foundry deployment remain gated follow-up work rather than unverified
scaffolding.
