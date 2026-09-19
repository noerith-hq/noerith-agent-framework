# Project setup

## First five minutes

1. Create a repository from `noerith-starter`, or clone/copy it if template creation is unavailable on the current GitHub plan.
2. Run `./scripts/initialize.sh` with the project name, description, and problem statement.
3. Run `./scripts/configure-labels.sh` after authenticating the GitHub CLI; GitHub template generation does not copy repository labels.
4. Record the first technical decision in `docs/decisions/`.
5. Run `./ci/verify.sh` locally and open a pull request.
6. Add the project ecosystem to `.github/dependabot.yml` once a package manager is selected.
7. Configure a `main` branch ruleset for public repositories; template generation does not copy repository rulesets or merge settings.

## Security baseline

- Keep credentials outside Git history.
- Use GitHub environment secrets only when a deployment requires them.
- Keep CI permissions read-only unless a specific job needs a narrow write permission.
- Require pull requests, successful CI, and resolved conversations on `main`; block branch deletion and force-pushes.
- Review major dependency updates manually.
- Report vulnerabilities through the organization security policy.
- Verify Dependabot alerts and security updates after creating a private repository. They are supported on the current Free plan, but repository settings are not copied by a template.

## GitHub Free boundaries

This starter does not rely on paid GitHub features. If branch or ruleset enforcement is unavailable for a private repository, use the documented pull-request workflow as the operating standard and record exceptions in the pull request.

Secret Scanning and Push Protection are enabled for the public `noerith-starter` repository. Do not assume those controls are available for a private repository on GitHub Free: verify the repository settings and skip unsupported controls rather than upgrading the organization.
