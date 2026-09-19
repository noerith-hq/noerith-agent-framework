#!/usr/bin/env bash
set -euo pipefail

fail() {
  printf 'ERROR: %s\n' "$*" >&2
  exit 1
}

command -v gh >/dev/null 2>&1 || fail 'GitHub CLI (gh) is required to configure repository labels.'

repository="${1:-}"
if [[ -z "${repository}" ]]; then
  repository="$(gh repo view --json nameWithOwner --jq .nameWithOwner)"
fi

[[ "${repository}" == */* ]] || fail 'Repository must use the OWNER/REPOSITORY format.'

gh label create dependencies \
  --repo "${repository}" \
  --color 0366d6 \
  --description 'Dependency updates and maintenance' \
  --force

gh label create security \
  --repo "${repository}" \
  --color d73a4a \
  --description 'Security-related work or disclosure follow-up' \
  --force

printf 'Ensured dependencies and security labels in %s.\n' "${repository}"
