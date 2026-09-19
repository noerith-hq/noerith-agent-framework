namespace Noerith.AgentFramework.Application.Contracts;

/// <summary>
/// Carries the normalized output of a provider adapter.
/// </summary>
public sealed record AgentRuntimeResponse(
  string ProviderRunId,
  string OutputText,
  string Provider,
  IReadOnlyList<AgentEvidence> Evidence);
