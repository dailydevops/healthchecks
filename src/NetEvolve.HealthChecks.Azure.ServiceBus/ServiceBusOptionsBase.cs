namespace NetEvolve.HealthChecks.Azure.ServiceBus;

public abstract class ServiceBusOptionsBase
{
    /// <summary>
    /// Gets or sets the client creation mode, default is <see cref="ClientCreationMode.ServiceProvider"/>.
    /// </summary>
    public ClientCreationMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the azure service bus connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the fully qualified namespace.
    /// </summary>
    public string FullyQualifiedNamespace { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against database.
    /// Default is 100 milliseconds.
    /// </summary>
    public int Timeout { get; set; } = 100;
}
