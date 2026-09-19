namespace Noerith.AgentFramework.Application.Contracts;

/// <summary>
/// Represents the provider-neutral result of a completed agent turn.
/// </summary>
public sealed record AgentTurnResult(
  string RunId,
  string SessionId,
  string OutputText,
  string Provider,
  DateTimeOffset CompletedAtUtc,
  IReadOnlyList<AgentEvidence> Evidence);
