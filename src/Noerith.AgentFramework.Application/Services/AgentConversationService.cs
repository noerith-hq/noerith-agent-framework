using Noerith.AgentFramework.Application.Contracts;
using Noerith.AgentFramework.Application.Interfaces;

namespace Noerith.AgentFramework.Application.Services;

/// <summary>
/// Coordinates one agent turn while retaining a provider-neutral lifecycle trace.
/// </summary>
public sealed class AgentConversationService(
  IAgentRuntime runtime,
  IAgentRunJournal runJournal) : IAgentConversationService
{
  private readonly IAgentRuntime _runtime = runtime;
  private readonly IAgentRunJournal _runJournal = runJournal;

  /// <inheritdoc />
  public async Task<AgentTurnResult> RunAsync(
    AgentTurnRequest request,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(request);
    request.Validate();

    var runId = Guid.NewGuid().ToString("N");
    var startedAtUtc = DateTimeOffset.UtcNow;
    var correlationId = string.IsNullOrWhiteSpace(request.CorrelationId)
      ? runId
      : request.CorrelationId.Trim();
    var runtimeRequest = new AgentRuntimeRequest(
      runId,
      request.SessionId.Trim(),
      request.Message.Trim(),
      correlationId,
      startedAtUtc);

    AgentRuntimeResponse runtimeResponse;

    try
    {
      runtimeResponse = await _runtime.RunAsync(runtimeRequest, cancellationToken);
    }
    catch (OperationCanceledException)
    {
      await _runJournal.AppendAsync(
        CreateTrace(
          runtimeRequest,
          _runtime.Name,
          null,
          AgentRunOutcome.Cancelled,
          "cancelled"),
        CancellationToken.None);
      throw;
    }
    catch (Exception exception)
    {
      await _runJournal.AppendAsync(
        CreateTrace(
          runtimeRequest,
          _runtime.Name,
          null,
          AgentRunOutcome.Failed,
          exception.GetType().Name),
        CancellationToken.None);
      throw;
    }

    var completedAtUtc = DateTimeOffset.UtcNow;
    await _runJournal.AppendAsync(
      new AgentRunTrace(
        runtimeRequest.RunId,
        runtimeRequest.SessionId,
        runtimeRequest.CorrelationId,
        runtimeResponse.Provider,
        runtimeResponse.ProviderRunId,
        runtimeRequest.StartedAtUtc,
        completedAtUtc,
        AgentRunOutcome.Succeeded),
      cancellationToken);

    return new AgentTurnResult(
      runtimeRequest.RunId,
      runtimeRequest.SessionId,
      runtimeResponse.OutputText,
      runtimeResponse.Provider,
      completedAtUtc,
      runtimeResponse.Evidence);
  }

  private static AgentRunTrace CreateTrace(
    AgentRuntimeRequest request,
    string provider,
    string? providerRunId,
    AgentRunOutcome outcome,
    string errorCode) =>
    new(
      request.RunId,
      request.SessionId,
      request.CorrelationId,
      provider,
      providerRunId,
      request.StartedAtUtc,
      DateTimeOffset.UtcNow,
      outcome,
      errorCode);
}
