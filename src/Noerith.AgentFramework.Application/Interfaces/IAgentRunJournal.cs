using Noerith.AgentFramework.Application.Contracts;

namespace Noerith.AgentFramework.Application.Interfaces;

/// <summary>
/// Persists non-content lifecycle metadata for an agent run.
/// </summary>
public interface IAgentRunJournal
{
  /// <summary>
  /// Appends one terminal lifecycle record.
  /// </summary>
  ValueTask AppendAsync(AgentRunTrace trace, CancellationToken cancellationToken = default);
}
