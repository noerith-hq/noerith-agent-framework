namespace Noerith.AgentFramework.Application.Contracts;

/// <summary>
/// Represents one caller-submitted agent turn.
/// </summary>
public sealed record AgentTurnRequest(string SessionId, string Message, string? CorrelationId = null)
{
  /// <summary>
  /// Validates the bounded public contract before a provider is invoked.
  /// </summary>
  public void Validate()
  {
    if (string.IsNullOrWhiteSpace(SessionId))
    {
      throw new ArgumentException("A session ID is required.", nameof(SessionId));
    }

    if (string.IsNullOrWhiteSpace(Message))
    {
      throw new ArgumentException("A message is required.", nameof(Message));
    }
  }
}
