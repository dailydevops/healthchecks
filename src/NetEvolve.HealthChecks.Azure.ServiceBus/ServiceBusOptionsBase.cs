namespace NetEvolve.HealthChecks.Azure.ServiceBus;

/// <summary>
/// Represents the base configuration options for Azure Service Bus health checks.
/// </summary>
public abstract record ServiceBusOptionsBase
{
    /// <summary>
    /// Gets or sets the client creation mode. Default is <see cref="ClientCreationMode.ServiceProvider"/>.
    /// </summary>
    public ClientCreationMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the Azure Service Bus connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the fully qualified namespace for the Azure Service Bus resource.
    /// </summary>
    public string? FullyQualifiedNamespace { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when connecting and executing tasks against the Service Bus. Default is 100 milliseconds.
    /// </summary>
    public int Timeout { get; set; } = 100;
}
