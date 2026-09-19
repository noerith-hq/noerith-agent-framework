#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${repo_root}"

usage() {
  cat <<'USAGE'
Usage:
  ./scripts/initialize.sh --name "Project name" --description "Short description" --problem "Problem statement" [--status "Incubating"]
USAGE
}

fail() {
  printf 'ERROR: %s\n' "$*" >&2
  exit 1
}

project_name=''
project_description=''
project_problem=''
project_status='Incubating'

while [[ $# -gt 0 ]]; do
  case "$1" in
    --name)
      [[ $# -ge 2 ]] || fail '--name requires a value'
      project_name="$2"
      shift 2
      ;;
    --description)
      [[ $# -ge 2 ]] || fail '--description requires a value'
      project_description="$2"
      shift 2
      ;;
    --problem)
      [[ $# -ge 2 ]] || fail '--problem requires a value'
      project_problem="$2"
      shift 2
      ;;
    --status)
      [[ $# -ge 2 ]] || fail '--status requires a value'
      project_status="$2"
      shift 2
      ;;
    --help|-h)
      usage
      exit 0
      ;;
    *)
      usage >&2
      fail "Unknown argument: $1"
      ;;
  esac
done

[[ -f .noerith-template ]] || fail 'This repository has already been initialized or is not a NOERITH starter template.'

for value in "${project_name}" "${project_description}" "${project_problem}" "${project_status}"; do
  [[ -n "${value}" ]] || fail 'Name, description, problem, and status must not be empty.'
  [[ "${value}" != *$'\n'* ]] || fail 'Values must be single-line text.'
done

escape_sed_replacement() {
  printf '%s' "$1" | sed -e 's/[\\&|]/\\&/g'
}

escaped_name="$(escape_sed_replacement "${project_name}")"
escaped_description="$(escape_sed_replacement "${project_description}")"
escaped_problem="$(escape_sed_replacement "${project_problem}")"
escaped_status="$(escape_sed_replacement "${project_status}")"

replace_in_file() {
  local file="$1"
  local temporary_file="${file}.tmp"
  sed \
    -e "1s|^# NOERITH Repository Starter$|# ${escaped_name}|" \
    -e "s|^> Secure, reusable project foundation for NOERITH repositories\.$|> ${escaped_description}|" \
    -e "s|[{][{]PROJECT_NAME[}][}]|${escaped_name}|g" \
    -e "s|[{][{]PROJECT_DESCRIPTION[}][}]|${escaped_description}|g" \
    -e "s|[{][{]PROJECT_PROBLEM[}][}]|${escaped_problem}|g" \
    -e "s|[{][{]PROJECT_STATUS[}][}]|${escaped_status}|g" \
    "${file}" > "${temporary_file}"
  mv "${temporary_file}" "${file}"
}

replace_in_file README.md
replace_in_file docs/architecture.md

rm -f -- .noerith-template

placeholder_pattern='\{\{PROJECT_[A-Z_]+\}\}'
placeholder_hits="$(find . -type f ! -path './.git/*' -exec grep -nE "${placeholder_pattern}" {} + 2>/dev/null || true)"
if [[ -n "${placeholder_hits}" ]]; then
  printf '%s\n' "${placeholder_hits}" >&2
  fail 'Required project placeholders remain after initialization.'
fi

printf 'Initialized %s. Run ./scripts/configure-labels.sh and ./ci/verify.sh before opening the first pull request.\n' "${project_name}"
