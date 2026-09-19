namespace Noerith.AgentFramework.Application.Contracts;

/// <summary>
/// Records non-content lifecycle metadata for an agent turn.
/// </summary>
public sealed record AgentRunTrace(
  string RunId,
  string SessionId,
  string CorrelationId,
  string Provider,
  string? ProviderRunId,
  DateTimeOffset StartedAtUtc,
  DateTimeOffset CompletedAtUtc,
  AgentRunOutcome Outcome,
  string? ErrorCode = null);
