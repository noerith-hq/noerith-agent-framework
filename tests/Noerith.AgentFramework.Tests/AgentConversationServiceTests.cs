using Noerith.AgentFramework.Application.Contracts;
using Noerith.AgentFramework.Application.Interfaces;
using Noerith.AgentFramework.Application.Services;
using Xunit;

namespace Noerith.AgentFramework.Tests;

/// <summary>
/// Verifies provider-neutral agent turn orchestration without a live cloud dependency.
/// </summary>
public sealed class AgentConversationServiceTests
{
  /// <summary>
  /// Ensures a successful runtime result is returned and a non-content lifecycle trace is retained.
  /// </summary>
  [Fact]
  public async Task RunAsync_PersistsSucceededTraceAndReturnsProviderResponseAsync()
  {
    // Arrange
    var runtime = new StubAgentRuntime();
    var journal = new RecordingAgentRunJournal();
    var service = new AgentConversationService(runtime, journal);

    // Act
    var result = await service.RunAsync(new AgentTurnRequest("session-1", "What changed?", "correlation-1"));

    // Assert
    Assert.Equal("agent response", result.OutputText);
    Assert.Equal("azure-openai", result.Provider);
    Assert.Equal("session-1", result.SessionId);
    Assert.Equal("correlation-1", runtime.LastRequest?.CorrelationId);

    var trace = Assert.Single(journal.Traces);
    Assert.Equal(AgentRunOutcome.Succeeded, trace.Outcome);
    Assert.Equal(result.RunId, trace.RunId);
    Assert.Equal("provider-run-1", trace.ProviderRunId);
    Assert.Null(trace.ErrorCode);
  }

  /// <summary>
  /// Ensures runtime errors retain a terminal failure trace before being surfaced to the adapter.
  /// </summary>
  [Fact]
  public async Task RunAsync_PersistsFailedTraceBeforeRethrowingAsync()
  {
    // Arrange
    var journal = new RecordingAgentRunJournal();
    var service = new AgentConversationService(new FailingAgentRuntime(), journal);

    // Act
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(
      () => service.RunAsync(new AgentTurnRequest("session-2", "Try provider")));

    // Assert
    Assert.Equal("provider unavailable", exception.Message);
    var trace = Assert.Single(journal.Traces);
    Assert.Equal(AgentRunOutcome.Failed, trace.Outcome);
    Assert.Equal("failing-runtime", trace.Provider);
    Assert.Equal(nameof(InvalidOperationException), trace.ErrorCode);
  }

  /// <summary>
  /// Ensures invalid caller input never reaches a provider adapter.
  /// </summary>
  [Fact]
  public async Task RunAsync_RejectsBlankMessageBeforeInvokingRuntimeAsync()
  {
    // Arrange
    var runtime = new StubAgentRuntime();
    var journal = new RecordingAgentRunJournal();
    var service = new AgentConversationService(runtime, journal);

    // Act and assert
    await Assert.ThrowsAsync<ArgumentException>(
      () => service.RunAsync(new AgentTurnRequest("session-3", " ")));
    Assert.Null(runtime.LastRequest);
    Assert.Empty(journal.Traces);
  }

  private sealed class StubAgentRuntime : IAgentRuntime
  {
    /// <inheritdoc />
    public string Name => "azure-openai";

    /// <summary>
    /// Gets the most recent normalized request.
    /// </summary>
    public AgentRuntimeRequest? LastRequest { get; private set; }

    /// <inheritdoc />
    public Task<AgentRuntimeResponse> RunAsync(
      AgentRuntimeRequest request,
      CancellationToken cancellationToken = default)
    {
      LastRequest = request;
      return Task.FromResult(new AgentRuntimeResponse(
        "provider-run-1",
        "agent response",
        Name,
        []));
    }
  }

  private sealed class FailingAgentRuntime : IAgentRuntime
  {
    /// <inheritdoc />
    public string Name => "failing-runtime";

    /// <inheritdoc />
    public Task<AgentRuntimeResponse> RunAsync(
      AgentRuntimeRequest request,
      CancellationToken cancellationToken = default) =>
      throw new InvalidOperationException("provider unavailable");
  }

  private sealed class RecordingAgentRunJournal : IAgentRunJournal
  {
    /// <summary>
    /// Gets the in-memory trace collection used by the test assertions.
    /// </summary>
    public List<AgentRunTrace> Traces { get; } = [];

    /// <inheritdoc />
    public ValueTask AppendAsync(AgentRunTrace trace, CancellationToken cancellationToken = default)
    {
      Traces.Add(trace);
      return ValueTask.CompletedTask;
    }
  }
}
