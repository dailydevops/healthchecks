namespace NetEvolve.HealthChecks.Dapr;

using Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Options for <see cref="DaprHealthCheck"/>
/// </summary>
public sealed class DaprOptions
{
    /// <summary>
    /// Timeout, which is granted when checking the Dapr sidecar. If the timeout is exceeded, the health check is classified as <see cref="HealthStatus.Degraded"/>.
    /// </summary>
    public int Timeout { get; set; } = 100;
}
