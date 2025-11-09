namespace NetEvolve.HealthChecks.MariaDb;

/// <summary>
/// Options for <see cref="MariaDbHealthCheck"/>.
/// </summary>
public sealed record MariaDbOptions
{
    /// <summary>
    /// Gets or sets the connection string for the MariaDb database to check.
    /// </summary>
    /// <value>
    /// The connection string used to establish a connection to the MariaDb database.
    /// </value>
    public string ConnectionString { get; set; } = default!;

    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when connecting and executing tasks against the MariaDb database.
    /// </summary>
    /// <value>
    /// The timeout in milliseconds. Default value is 100 milliseconds.
    /// </value>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets the SQL command to execute against the MariaDb database.
    /// </summary>
    /// <value>
    /// The SQL command to execute. Default value is defined by <see cref="MariaDbHealthCheck.DefaultCommand"/>.
    /// </value>
    /// <remarks>
    /// This property is for internal use only.
    /// </remarks>
    public string Command { get; internal set; } = MariaDbHealthCheck.DefaultCommand;
}
