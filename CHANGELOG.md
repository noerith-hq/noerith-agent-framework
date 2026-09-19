# Changelog

All notable changes to this project are documented in this file.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and versions follow [Semantic Versioning](https://semver.org/).

## [Unreleased]

### Added

- Initial project foundation from `noerith-starter`.

### Changed

- Split release validation from publication so write access is granted only to the publish job.
- Check both YAML workflow extensions and local composite actions for immutable action references.
- Present a polished starter README while preserving project-specific initialization.

### Security

- Reject broad or unexpected GitHub Actions write permissions.
- Use the hardened, immutable revision of the NOERITH reusable CI workflow.
