using Noerith.AgentFramework.Application.Contracts;

namespace Noerith.AgentFramework.Application.Interfaces;

/// <summary>
/// Provides a provider-neutral boundary for an agent runtime.
/// </summary>
public interface IAgentRuntime
{
  /// <summary>
  /// Gets the stable identifier used in lifecycle traces.
  /// </summary>
  string Name { get; }

  /// <summary>
  /// Invokes the underlying agent provider for one normalized turn.
  /// </summary>
  Task<AgentRuntimeResponse> RunAsync(AgentRuntimeRequest request, CancellationToken cancellationToken = default);
}
