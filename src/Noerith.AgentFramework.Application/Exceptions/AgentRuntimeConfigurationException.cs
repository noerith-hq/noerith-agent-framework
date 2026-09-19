namespace Noerith.AgentFramework.Application.Exceptions;

/// <summary>
/// Signals that a provider adapter cannot run because required runtime configuration is absent or invalid.
/// </summary>
public sealed class AgentRuntimeConfigurationException(string message) : InvalidOperationException(message);
