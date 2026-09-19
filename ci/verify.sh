#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${repo_root}"

fail() {
  printf 'ERROR: %s\n' "$*" >&2
  exit 1
}

require_file() {
  [[ -f "$1" ]] || fail "Missing required file: $1"
}

require_contains() {
  local needle="$1"
  local file="$2"
  grep -Fq -- "${needle}" "${file}" || fail "Expected ${file} to contain: ${needle}"
}

required_files=(
  README.md
  LICENSE
  NOTICE
  CHANGELOG.md
  .editorconfig
  .gitattributes
  .gitignore
  .github/dependabot.yml
  .github/workflows/ci.yml
  .github/workflows/release.yml
  ci/verify.sh
  docs/architecture.md
  docs/decisions/README.md
  docs/PROJECT_SETUP.md
  scripts/initialize.sh
  scripts/configure-labels.sh
  src/README.md
  tests/README.md
)

for file in "${required_files[@]}"; do
  require_file "${file}"
done

[[ -x ci/verify.sh ]] || fail 'ci/verify.sh must be executable'
[[ -x scripts/initialize.sh ]] || fail 'scripts/initialize.sh must be executable'
[[ -x scripts/configure-labels.sh ]] || fail 'scripts/configure-labels.sh must be executable'

for heading in \
  '## Status' \
  '## Problem' \
  '## Getting started' \
  '## Local development' \
  '## Testing' \
  '## Architecture' \
  '## Security' \
  '## Contributing' \
  '## License'; do
  require_contains "${heading}" README.md
done

require_contains 'pull_request:' .github/workflows/ci.yml
require_contains 'push:' .github/workflows/ci.yml
require_contains 'noerith-hq/.github/.github/workflows/secure-ci.yml@fb0f1e4e8946380d2ef121de0bbb004b2786de15' .github/workflows/ci.yml
require_contains 'contents: read' .github/workflows/ci.yml
require_contains 'workflow_dispatch:' .github/workflows/release.yml
require_contains 'contents: write' .github/workflows/release.yml
require_contains "github.ref == 'refs/heads/main'" .github/workflows/release.yml
require_contains 'SEMVER_PATTERN=' .github/workflows/release.yml
require_contains 'GH_TOKEN: ${{ github.token }}' .github/workflows/release.yml
require_contains '--repo "${GITHUB_REPOSITORY}"' .github/workflows/release.yml

if grep -Eq '^[[:space:]]*[A-Za-z0-9_-]+:[[:space:]]*write([[:space:]#]|$)' .github/workflows/ci.yml; then
  fail 'CI must not request write permissions'
fi

workflow_roots=(.github/workflows)
if [[ -d .github/actions ]]; then
  workflow_roots+=(.github/actions)
fi

while IFS= read -r workflow_file; do
  while IFS= read -r line; do
    if [[ "${line}" =~ ^[[:space:]]*permissions:[[:space:]]*(read-all|write-all)([[:space:]#]|$) ]]; then
      fail "Broad workflow permissions are not allowed in ${workflow_file}: ${BASH_REMATCH[1]}"
    fi

    if [[ "${line}" =~ ^[[:space:]]*permissions:[[:space:]]*\{.*write.*\} ]]; then
      fail "Inline write permissions are not allowed in ${workflow_file}"
    fi

    if [[ "${line}" =~ ^[[:space:]]*([A-Za-z0-9_-]+):[[:space:]]*write([[:space:]#]|$) ]]; then
      permission="${BASH_REMATCH[1]}"
      if [[ "${workflow_file}" != '.github/workflows/release.yml' || "${permission}" != 'contents' ]]; then
        fail "Unexpected write permission in ${workflow_file}: ${permission}"
      fi
    fi

    if [[ "${line}" =~ ^[[:space:]]*-?[[:space:]]*uses:[[:space:]]*([^[:space:]#]+) ]]; then
      reference="${BASH_REMATCH[1]}"
      if [[ "${reference}" == ./* ]]; then
        continue
      fi
      sha="${reference##*@}"
      [[ "${sha}" =~ ^[0-9a-fA-F]{40}$ ]] || fail "Unpinned action or reusable workflow in ${workflow_file}: ${reference}"
    fi
  done < "${workflow_file}"
done < <(find "${workflow_roots[@]}" -type f \( -name '*.yml' -o -name '*.yaml' \) -print)

secret_pattern='gh[pousr]_[A-Za-z0-9]{20,}|github_pat_[A-Za-z0-9_]{20,}|AKIA[0-9A-Z]{16}|-----BEGIN [A-Z ]*PRIVATE KEY-----'
secret_hits="$(find . -type f ! -path './.git/*' ! -name LICENSE -exec grep -nE "${secret_pattern}" {} + 2>/dev/null || true)"
if [[ -n "${secret_hits}" ]]; then
  printf '%s\n' "${secret_hits}" >&2
  fail 'Potential credential material found in the repository'
fi

placeholder_pattern='\{\{PROJECT_[A-Z_]+\}\}'
if [[ -f .noerith-template ]]; then
  require_contains 'NOERITH starter template sentinel.' .noerith-template
else
  placeholder_hits="$(find . -type f ! -path './.git/*' -exec grep -nE "${placeholder_pattern}" {} + 2>/dev/null || true)"
  if [[ -n "${placeholder_hits}" ]]; then
    printf '%s\n' "${placeholder_hits}" >&2
    fail 'Required project placeholders remain after initialization'
  fi
fi

printf 'NOERITH starter checks passed.\n'
