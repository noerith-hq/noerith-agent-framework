# NOERITH Repository Starter

> Secure, reusable project foundation for NOERITH repositories.

## Status

{{PROJECT_STATUS}}

## Problem

{{PROJECT_PROBLEM}}

## Getting started

This repository was created from the NOERITH starter. Before the first delivery, initialize it with project-specific information:

```bash
./scripts/initialize.sh \
  --name "My Project" \
  --description "One-sentence product description" \
  --problem "The user problem this project addresses"
```

The initializer replaces the required project placeholders and removes the template sentinel. Do not add credentials, API keys, customer data, or production configuration to this repository.

## Local development

This starter is technology-neutral. Select the application stack in the first architecture decision, then document the local setup and run commands here.

## Testing

Run the baseline contract locally:

```bash
./ci/verify.sh
```

Add stack-specific lint, build, and test commands as the project takes shape.

## Architecture

Read and maintain [the architecture overview](docs/architecture.md). Record significant technical choices in [architecture decisions](docs/decisions/README.md).

## Security

Report vulnerabilities through the organization [security policy](https://github.com/noerith-hq/.github/blob/main/SECURITY.md). Never commit secrets; use a secure secret-management service when a deployment is introduced.

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
