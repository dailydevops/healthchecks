namespace NetEvolve.HealthChecks.Azure.EventHubs;

/// <summary>
/// Represents configuration options for Azure Event Hub health checks.
/// </summary>
public sealed record EventHubOptions : EventHubsOptionsBase
{
    /// <summary>
    /// Gets or sets the name of the Event Hub to check for existence.
    /// </summary>
    public string? EventHubName { get; set; }
}