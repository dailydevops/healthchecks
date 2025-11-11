namespace NetEvolve.HealthChecks.GCP.PubSub;

/// <summary>
/// Options for <see cref="PubSubHealthCheck"/>
/// </summary>
public sealed record PubSubOptions
{
    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when executing tasks against the Pub/Sub service.
    /// </summary>
    /// <value>
    /// The timeout in milliseconds. Default value is 100 milliseconds.
    /// </value>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the keyed service name for retrieving the <see cref="Google.Cloud.PubSub.V1.PublisherServiceApiClient"/> instance.
    /// </summary>
    /// <value>
    /// The keyed service name, or <c>null</c> if using the default service registration.
    /// </value>
    public string? KeyedService { get; set; }
}
