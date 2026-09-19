#pragma warning disable OPENAI001 // Azure OpenAI v1 client APIs used by the official MAF OpenAI sample are marked experimental.

using System.ClientModel.Primitives;
using Azure.Core;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using Noerith.AgentFramework.Application.Contracts;
using Noerith.AgentFramework.Application.Exceptions;
using Noerith.AgentFramework.Application.Interfaces;
using OpenAI;
using OpenAI.Responses;

namespace Noerith.AgentFramework.Api.Services;

/// <summary>
/// Implements the provider boundary with Microsoft Agent Framework and Azure OpenAI's v1 route.
/// </summary>
public sealed class AzureOpenAiAgentRuntime(IConfiguration configuration) : IAgentRuntime
{
  private const string AzureAiScope = "https://ai.azure.com/.default";
  private const string DefaultInstructions =
    "You are the NOERITH reference agent. Be concise, state uncertainty clearly, and do not invent sources.";

  private readonly IConfiguration _configuration = configuration;

  /// <inheritdoc />
  public string Name => "azure-openai";

  /// <inheritdoc />
  public async Task<AgentRuntimeResponse> RunAsync(
    AgentRuntimeRequest request,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(request);

    var endpoint = GetEndpoint();
    var deploymentName = GetRequiredSetting("AZURE_OPENAI_DEPLOYMENT_NAME");
    var agent = CreateAgent(endpoint, deploymentName);
    var response = await agent.RunAsync(request.Message, cancellationToken: cancellationToken);

    return new AgentRuntimeResponse(
      string.IsNullOrWhiteSpace(response.ResponseId) ? request.RunId : response.ResponseId,
      response.Text ?? string.Empty,
      Name,
      []);
  }

  private AIAgent CreateAgent(Uri endpoint, string deploymentName)
  {
    var client = new OpenAIClient(
      new BearerTokenPolicy(CreateCredential(), AzureAiScope),
      new OpenAIClientOptions { Endpoint = endpoint });

    return client
      .GetResponsesClient()
      .AsAIAgent(
        model: deploymentName,
        name: "noerith-reference-agent",
        instructions: DefaultInstructions);
  }

  private TokenCredential CreateCredential()
  {
    var credentialMode = _configuration["NOERITH_AGENT_CREDENTIAL"] ?? "default";

    return credentialMode.ToLowerInvariant() switch
    {
      "default" => new DefaultAzureCredential(),
      "managed-identity" => CreateManagedIdentityCredential(),
      _ => throw new AgentRuntimeConfigurationException(
        "NOERITH_AGENT_CREDENTIAL must be either 'default' or 'managed-identity'."),
    };
  }

  private Uri GetEndpoint()
  {
    var endpoint = GetRequiredSetting("AZURE_OPENAI_ENDPOINT");

    if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri))
    {
      throw new AgentRuntimeConfigurationException("AZURE_OPENAI_ENDPOINT must be an absolute URI.");
    }

    return endpointUri;
  }

  private ManagedIdentityCredential CreateManagedIdentityCredential()
  {
    var clientId = _configuration["AZURE_CLIENT_ID"];
    return string.IsNullOrWhiteSpace(clientId)
      ? new ManagedIdentityCredential(new ManagedIdentityCredentialOptions())
      : new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(clientId));
  }

  private string GetRequiredSetting(string name)
  {
    var value = _configuration[name];

    if (string.IsNullOrWhiteSpace(value))
    {
      throw new AgentRuntimeConfigurationException($"{name} is required to invoke the agent runtime.");
    }

    return value.Trim();
  }
}
