# Tests

Tests cover application contracts without invoking a cloud provider. They use a fake `IAgentRuntime` and
an in-memory recording journal, so they are deterministic and safe to run in CI.

Live Azure OpenAI/Foundry smoke tests are intentionally not part of this baseline because they require
an approved environment, identity, deployment, and cost boundary.
