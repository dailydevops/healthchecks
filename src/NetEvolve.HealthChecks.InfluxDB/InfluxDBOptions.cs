namespace NetEvolve.HealthChecks.InfluxDB;

using global::InfluxDB.Client;

/// <summary>
/// Options for <see cref="InfluxDBHealthCheck"/>
/// </summary>
public sealed record InfluxDBOptions
{
    /// <summary>
    /// Gets or sets the key used to resolve the <c>IInfluxDBClient</c> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <c>IInfluxDBClient</c> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// When null or empty, the health check will resolve the <c>IInfluxDBClient</c> using <c>IServiceProvider.GetRequiredService</c>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against database.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// The ping command to execute against the database.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    public Func<IInfluxDBClient, CancellationToken, Task<bool>> PingAsync { get; internal set; } =
        InfluxDBHealthCheck.DefaultPingAsync;
}
