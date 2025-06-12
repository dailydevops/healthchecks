namespace NetEvolve.HealthChecks.ClickHouse;

/// <summary>
/// Options for <see cref="ClickHouseHealthCheck"/>.
/// </summary>
public sealed record ClickHouseOptions
{
    /// <summary>
    /// Gets or sets the connection string for the ClickHouse database to check.
    /// </summary>
    /// <value>
    /// The connection string used to establish a connection to the ClickHouse database.
    /// </value>
    public string ConnectionString { get; set; } = default!;

    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when connecting and executing tasks against the ClickHouse database.
    /// </summary>
    /// <value>
    /// The timeout in milliseconds. Default value is 100 milliseconds.
    /// </value>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets the SQL command to execute against the ClickHouse database.
    /// </summary>
    /// <value>
    /// The SQL command to execute. Default value is defined by <see cref="ClickHouseHealthCheck.DefaultCommand"/>.
    /// </value>
    /// <remarks>
    /// This property is for internal use only.
    /// </remarks>
    public string Command { get; internal set; } = ClickHouseHealthCheck.DefaultCommand;
}
