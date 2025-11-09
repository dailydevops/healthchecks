namespace NetEvolve.HealthChecks.Dapr;

using Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Options for <see cref="DaprHealthCheck"/>
/// </summary>
public sealed record DaprOptions
{
    /// <summary>
    /// Gets or sets the key used to resolve the <c>DaprClient</c> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <c>DaprClient</c> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// When null or empty, the health check will resolve the <c>DaprClient</c> using <c>IServiceProvider.GetRequiredService</c>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Timeout, which is granted when checking the Dapr sidecar. If the timeout is exceeded, the health check is classified as <see cref="HealthStatus.Degraded"/>.
    /// </summary>
    public int Timeout { get; set; } = 100;
}
