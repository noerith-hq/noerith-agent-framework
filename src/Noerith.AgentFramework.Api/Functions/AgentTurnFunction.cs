using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Noerith.AgentFramework.Application.Contracts;
using Noerith.AgentFramework.Application.Exceptions;
using Noerith.AgentFramework.Application.Interfaces;

namespace Noerith.AgentFramework.Api.Functions;

/// <summary>
/// Exposes a deliberately bounded HTTP adapter for one agent turn.
/// </summary>
public sealed class AgentTurnFunction(
  IAgentConversationService agentConversationService,
  ILogger<AgentTurnFunction> logger)
{
  private readonly IAgentConversationService _agentConversationService = agentConversationService;
  private readonly ILogger<AgentTurnFunction> _logger = logger;

  /// <summary>
  /// Runs a single agent turn through the application contract.
  /// </summary>
  [Function(nameof(RunAsync))]
  public async Task<HttpResponseData> RunAsync(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "agent/turns")] HttpRequestData request,
    CancellationToken cancellationToken)
  {
    AgentTurnRequest? turnRequest = await request.ReadFromJsonAsync<AgentTurnRequest>(cancellationToken: cancellationToken);

    if (turnRequest is null)
    {
      return await CreateErrorResponseAsync(request, HttpStatusCode.BadRequest, "invalid_request", cancellationToken);
    }

    try
    {
      var result = await _agentConversationService.RunAsync(turnRequest, cancellationToken);
      var response = request.CreateResponse(HttpStatusCode.OK);
      await response.WriteAsJsonAsync(result, cancellationToken);
      return response;
    }
    catch (ArgumentException)
    {
      return await CreateErrorResponseAsync(request, HttpStatusCode.BadRequest, "invalid_request", cancellationToken);
    }
    catch (AgentRuntimeConfigurationException)
    {
      return await CreateErrorResponseAsync(request, HttpStatusCode.ServiceUnavailable, "agent_runtime_unavailable", cancellationToken);
    }
    catch (OperationCanceledException)
    {
      throw;
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Agent turn failed for correlation ID {CorrelationId}.", turnRequest.CorrelationId);
      return await CreateErrorResponseAsync(request, HttpStatusCode.BadGateway, "agent_runtime_failed", cancellationToken);
    }
  }

  private static async Task<HttpResponseData> CreateErrorResponseAsync(
    HttpRequestData request,
    HttpStatusCode statusCode,
    string code,
    CancellationToken cancellationToken)
  {
    var response = request.CreateResponse(statusCode);
    await response.WriteAsJsonAsync(new { error = code }, cancellationToken);
    return response;
  }
}
