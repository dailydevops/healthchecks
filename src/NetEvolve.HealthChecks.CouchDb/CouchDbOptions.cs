namespace NetEvolve.HealthChecks.CouchDb;

using MyCouch;

/// <summary>
/// Options for <see cref="CouchDbHealthCheck"/>
/// </summary>
public sealed record CouchDbOptions
{
    /// <summary>
    /// Gets or sets the connection string for the CouchDb server.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the database name to connect to.
    /// </summary>
    public string? DatabaseName { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against database.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// The command to execute against the database.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    public Func<MyCouchClient, CancellationToken, Task<bool>> CommandAsync { get; internal set; } =
        CouchDbHealthCheck.DefaultCommandAsync;
}
