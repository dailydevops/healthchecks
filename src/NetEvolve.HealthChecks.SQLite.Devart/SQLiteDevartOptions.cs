namespace NetEvolve.HealthChecks.SQLite.Devart;

/// <summary>
/// Options for <see cref="SQLiteDevartHealthCheck"/>
/// </summary>
public sealed record SQLiteDevartOptions
{
    /// <summary>
    /// Gets or sets the connection string for the SQLite database to check.
    /// </summary>
    /// <value>
    /// The connection string used to establish a connection to the SQLite database.
    /// </value>
    public string ConnectionString { get; set; } = default!;

    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when connecting and executing tasks against the SQLite database.
    /// </summary>
    /// <value>
    /// The timeout in milliseconds. Default value is 100 milliseconds.
    /// </value>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets the SQL command to execute against the SQLite database.
    /// </summary>
    /// <value>
    /// The SQL command to execute. Default value is defined by <see cref="SQLiteDevartHealthCheck.DefaultCommand"/>.
    /// </value>
    /// <remarks>
    /// This property is for internal use only.
    /// </remarks>
    public string Command { get; internal set; } = SQLiteDevartHealthCheck.DefaultCommand;
}
