namespace Noerith.AgentFramework.Application.Contracts;

/// <summary>
/// Describes evidence that an agent runtime can attach to a result.
/// </summary>
public sealed record AgentEvidence(string SourceId, string Title, Uri? Uri = null);
