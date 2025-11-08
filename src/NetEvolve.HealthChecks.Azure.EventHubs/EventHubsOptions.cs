namespace NetEvolve.HealthChecks.Azure.EventHubs;

/// <summary>
/// Represents configuration options for Azure Event Hubs health checks.
/// </summary>
public sealed record EventHubsOptions
{
    /// <summary>
    /// Gets or sets the client creation mode. Default is <see cref="ClientCreationMode.ServiceProvider"/>.
    /// </summary>
    public ClientCreationMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets the Azure Event Hubs connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the fully qualified namespace for the Azure Event Hubs resource.
    /// </summary>
    public string? FullyQualifiedNamespace { get; set; }

    /// <summary>
    /// Gets or sets the name of the Event Hub to check.
    /// </summary>
    public string? EventHubName { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when connecting and executing tasks against Event Hubs. Default is 100 milliseconds.
    /// </summary>
    public int Timeout { get; set; } = 100;
}
