# ADR 0001: Keep Microsoft Agent Framework behind a provider-neutral runtime boundary

- Status: accepted
- Date: 2026-09-19
- Owner: NOERITH

## Context

The project should adopt Microsoft Agent Framework while preserving the ability to test the application
without a cloud account and to change hosting/provider choices later. SkyAI's `develop` architecture
separates public entry points, application contracts, persistence, and direct provider SDK interaction;
this reference needs that same direction of dependency without reusing SkyAI implementation code.

## Decision

`Application` defines `IAgentRuntime`, `IAgentConversationService`, turn contracts, and lifecycle traces.
Only `Api` references Microsoft Agent Framework and Azure OpenAI SDKs through
`AzureOpenAiAgentRuntime`. `Persistence` implements an in-memory, non-content run journal for the first
milestone. The default integration uses stable `Microsoft.Agents.AI` and
`Microsoft.Agents.AI.OpenAI` packages. A Foundry-specific adapter will be evaluated separately because
the .NET Foundry package is prerelease at this baseline.

## Alternatives considered

1. Put Microsoft Agent Framework calls directly in the HTTP function. Rejected because it makes testing,
   traces, provider migration, and future orchestration needlessly coupled to the host.
2. Copy or fork SkyAI agent services. Rejected because SkyAI has product-specific domain, data,
   authorization, and provider lifecycle contracts that do not belong in this public reference project.
3. Start with a Foundry-hosted agent. Deferred because it requires an approved Azure environment,
   identity, deployment plan, and cost boundary.

## Consequences

- Unit tests run without Azure credentials or model usage.
- A production integration must replace the in-memory journal, introduce application identity and
  authorization, and use a managed identity with least privilege.
- Adding retrieval, tools, or multi-agent workflows requires source ownership, evaluation, and
  human-review design before those capabilities are enabled.
