using System.Collections.Concurrent;
using Noerith.AgentFramework.Application.Contracts;
using Noerith.AgentFramework.Application.Interfaces;

namespace Noerith.AgentFramework.Persistence;

/// <summary>
/// Provides a non-durable journal suitable only for local reference execution and tests.
/// </summary>
public sealed class InMemoryAgentRunJournal : IAgentRunJournal
{
  private readonly ConcurrentQueue<AgentRunTrace> _traces = new();

  /// <summary>
  /// Appends one terminal agent run trace.
  /// </summary>
  public ValueTask AppendAsync(AgentRunTrace trace, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(trace);
    cancellationToken.ThrowIfCancellationRequested();
    _traces.Enqueue(trace);
    return ValueTask.CompletedTask;
  }

  /// <summary>
  /// Returns a point-in-time view of recorded traces for local diagnostics.
  /// </summary>
  public IReadOnlyCollection<AgentRunTrace> Snapshot() => _traces.ToArray();
}
