namespace Noerith.AgentFramework.Application.Contracts;

/// <summary>
/// Defines the terminal outcome recorded for an agent run.
/// </summary>
public enum AgentRunOutcome
{
  /// <summary>
  /// The provider returned an agent response.
  /// </summary>
  Succeeded,

  /// <summary>
  /// The caller cancelled the turn before completion.
  /// </summary>
  Cancelled,

  /// <summary>
  /// The provider or orchestration failed before a result was returned.
  /// </summary>
  Failed,
}
