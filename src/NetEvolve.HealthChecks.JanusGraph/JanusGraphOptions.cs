namespace NetEvolve.HealthChecks.JanusGraph;

using Gremlin.Net.Driver;

/// <summary>
/// Options for <see cref="JanusGraphHealthCheck"/>
/// </summary>
public sealed record JanusGraphOptions
{
    /// <summary>
    /// Gets or sets the key used to resolve the <see cref="IGremlinClient"/> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <see cref="IGremlinClient"/> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// When null or empty, the health check will resolve the <see cref="IGremlinClient"/> using <c>IServiceProvider.GetRequiredService</c>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against database.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// The command to execute against the database.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    public Func<IGremlinClient, CancellationToken, Task<object>> CommandAsync { get; internal set; } =
        JanusGraphHealthCheck.DefaultCommandAsync;
}
