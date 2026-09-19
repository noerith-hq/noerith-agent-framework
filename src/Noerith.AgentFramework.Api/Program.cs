using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Noerith.AgentFramework.Api.Services;
using Noerith.AgentFramework.Application.Interfaces;
using Noerith.AgentFramework.Application.Services;
using Noerith.AgentFramework.Persistence;

var host = new HostBuilder()
  .ConfigureFunctionsWorkerDefaults()
  .ConfigureServices(services =>
  {
    services.AddSingleton<IAgentRuntime, AzureOpenAiAgentRuntime>();
    services.AddSingleton<IAgentRunJournal, InMemoryAgentRunJournal>();
    services.AddSingleton<IAgentConversationService, AgentConversationService>();
  })
  .Build();

await host.RunAsync();
