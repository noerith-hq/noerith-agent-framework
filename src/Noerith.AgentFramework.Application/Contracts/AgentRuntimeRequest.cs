namespace Noerith.AgentFramework.Application.Contracts;

/// <summary>
/// Carries a normalized turn from the application layer to a provider adapter.
/// </summary>
public sealed record AgentRuntimeRequest(
  string RunId,
  string SessionId,
  string Message,
  string CorrelationId,
  DateTimeOffset StartedAtUtc);
