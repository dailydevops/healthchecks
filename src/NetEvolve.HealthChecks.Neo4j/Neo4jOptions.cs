namespace NetEvolve.HealthChecks.Neo4j;

using global::Neo4j.Driver;

/// <summary>
/// Options for <see cref="Neo4jHealthCheck"/>
/// </summary>
#pragma warning disable S101 // Types should be named in PascalCase
public sealed record Neo4jOptions
#pragma warning restore S101 // Types should be named in PascalCase
{
    /// <summary>
    /// Gets or sets the key used to resolve the <c>IDriver</c> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <c>IDriver</c> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// When null or empty, the health check will resolve the <c>IDriver</c> using <c>IServiceProvider.GetRequiredService</c>.
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
    public Func<IDriver, CancellationToken, Task<bool>> CommandAsync { get; internal set; } =
        Neo4jHealthCheck.DefaultCommandAsync;
}
