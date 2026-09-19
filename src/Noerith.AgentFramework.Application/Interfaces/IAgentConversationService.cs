using Noerith.AgentFramework.Application.Contracts;

namespace Noerith.AgentFramework.Application.Interfaces;

/// <summary>
/// Coordinates a caller turn, a provider adapter, and lifecycle tracing.
/// </summary>
public interface IAgentConversationService
{
  /// <summary>
  /// Runs one normalized agent turn.
  /// </summary>
  Task<AgentTurnResult> RunAsync(AgentTurnRequest request, CancellationToken cancellationToken = default);
}
