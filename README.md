# NOERITH Agent Framework Reference

> Reference architecture for evidence-first AI agents built with Microsoft Agent Framework.

## Status

Incubating

## Problem

Provide a secure, testable starting point for integrating Microsoft Agent Framework without coupling to SkyAI internals.

## Getting started

This project is a small, runnable reference implementation. It transfers architectural lessons from
SkyAI's `develop` branch while keeping the codebase, contracts, data model, and credentials entirely
independent. Read [the reference boundary](docs/skyai-reference-boundary.md) before reusing a pattern.

```bash
dotnet restore Noerith.AgentFramework.slnx
dotnet build Noerith.AgentFramework.slnx --configuration Release --no-restore
dotnet test Noerith.AgentFramework.slnx --configuration Release --no-build --no-restore
```

The HTTP entry point is `POST /api/agent/turns`. It deliberately exposes one bounded agent-turn
contract rather than a complete product API.

## Local development

Prerequisites:

- .NET SDK 10.0.201 or a compatible .NET 10 SDK.
- Azure Functions Core Tools v4 only when running the local HTTP host.
- Azure CLI authentication only when invoking the real Azure OpenAI provider.

Create a local configuration file without committing it:

```bash
cp src/Noerith.AgentFramework.Api/local.settings.sample.json \
  src/Noerith.AgentFramework.Api/local.settings.json
```

Set `AZURE_OPENAI_ENDPOINT` to an Azure OpenAI v1 endpoint and
`AZURE_OPENAI_DEPLOYMENT_NAME` to an existing deployment. Then authenticate locally with `az login` and
run:

```bash
cd src/Noerith.AgentFramework.Api
func start
```

No test, build, or CI job contacts Azure. The runtime creates a Microsoft Agent Framework agent only
when an authenticated caller invokes the turn endpoint with valid provider configuration.

## Testing

Run the complete local verification contract:

```bash
./ci/verify.sh
```

The tests use deterministic fakes for the agent runtime and run journal. A separate live-provider smoke
test is intentionally deferred until an Azure environment, an identity, and a cost boundary are approved.

## Architecture

Read and maintain [the architecture overview](docs/architecture.md). Record significant technical choices in [architecture decisions](docs/decisions/README.md).

The first delivery uses a stable Microsoft Agent Framework core/OpenAI provider integration. A Foundry
adapter remains a gated follow-up because its .NET integration package is prerelease at this baseline.

## Security

Report vulnerabilities through the organization [security policy](https://github.com/noerith-hq/.github/blob/main/SECURITY.md). Never commit secrets; use a secure secret-management service when a deployment is introduced.

`POST /api/agent/turns` uses Azure Functions `Function` authorization as a safe local/reference default.
Before exposing it to end users, integrate a real application identity boundary (for example Microsoft
Entra ID), authorization policy, rate limits, structured audit retention, and a managed identity.

## Contributing

Follow the NOERITH [contribution guide](https://github.com/noerith-hq/.github/blob/main/CONTRIBUTING.md), [code of conduct](https://github.com/noerith-hq/.github/blob/main/CODE_OF_CONDUCT.md), and [support policy](https://github.com/noerith-hq/.github/blob/main/SUPPORT.md).

## Development conventions

- Default branch: `main`.
- Feature branches: `feat/<short-name>`.
- Fix branches: `fix/<short-name>`.
- Security branches: `security/<short-name>`.
- Use Conventional Commits and Semantic Versioning.
- Use pull requests for normal changes; review major dependency updates manually.
- Owner bypass is reserved for an emergency only.

> GitHub Free note: organization rulesets can be configured but may not be enforced for private repositories. Follow this pull-request convention even where GitHub cannot enforce it.

## License

This starter is licensed under the [Apache License 2.0](LICENSE). Confirm the appropriate license before applying it to a product repository.
