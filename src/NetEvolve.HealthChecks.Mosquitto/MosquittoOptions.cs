namespace NetEvolve.HealthChecks.Mosquitto;

/// <summary>
/// Configuration options for the <see cref="MosquittoHealthCheck"/>.
/// </summary>
public sealed record MosquittoOptions
{
    /// <summary>
    /// Gets or sets the key used to resolve the <c>IMqttClient</c> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <c>IMqttClient</c> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// When null or empty, the health check will resolve the <c>IMqttClient</c> using <c>IServiceProvider.GetRequiredService</c>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    /// <remarks>
    /// The minimum value is -1 (Timeout.Infinite). Default value is 100 milliseconds.
    /// </remarks>
    public int Timeout { get; set; } = 100;
}
