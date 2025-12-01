namespace NetEvolve.HealthChecks.Kubernetes;

/// <summary>
/// Represents configuration options for the Kubernetes health check.
/// </summary>
/// <remarks>
/// This record provides configuration for the health check that verifies connectivity and proper functioning of a Kubernetes cluster.
/// </remarks>
public sealed record KubernetesOptions
{
    /// <summary>
    /// Gets or sets the key used to resolve the <c>IKubernetes</c> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <c>IKubernetes</c> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// When null or empty, the health check will resolve the <c>IKubernetes</c> using <c>IServiceProvider.GetRequiredService</c>.
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
