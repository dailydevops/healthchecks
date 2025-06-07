namespace NetEvolve.HealthChecks.ArangoDb;

using ArangoDBNetStandard;

/// <summary>
/// Options for <see cref="ArangoDbHealthCheck"/>
/// </summary>
public sealed record ArangoDbOptions
{
    /// <summary>
    /// Gets or sets the key used to resolve the <c>IConnection</c> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <c>IConnection</c> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// When null or empty, the health check will resolve the <c>IConnection</c> using <c>IServiceProvider.GetRequiredService</c>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against database.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// The command to execute against the database.
    /// Returns <see langword="true"/> if successful, <see langword="false"/> otherwise.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    public Func<ArangoDBClient, CancellationToken, Task<bool>> CommandAsync { get; internal set; } =
        ArangoDbHealthCheck.DefaultCommandAsync;
}
