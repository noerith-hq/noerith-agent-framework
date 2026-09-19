# Source

The reference follows the SkyAI-style boundary split without copying SkyAI code:

- `Noerith.AgentFramework.Api`: Azure Functions host and the Microsoft Agent Framework provider adapter.
- `Noerith.AgentFramework.Application`: contracts and orchestration free of provider SDKs.
- `Noerith.AgentFramework.Persistence`: run-journal implementation.

Only the API project can depend on Microsoft Agent Framework or Azure provider SDKs.
