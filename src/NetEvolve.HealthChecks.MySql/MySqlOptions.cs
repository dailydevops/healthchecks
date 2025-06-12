namespace NetEvolve.HealthChecks.MySql;

/// <summary>
/// Options for <see cref="MySqlHealthCheck"/>
/// </summary>
public sealed record MySqlOptions
{
    /// <summary>
    /// Gets or sets the connection string for the MySQL database to check.
    /// </summary>
    /// <value>
    /// The connection string used to establish a connection to the MySQL database.
    /// </value>
    public string ConnectionString { get; set; } = default!;

    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when connecting and executing tasks against the MySQL database.
    /// </summary>
    /// <value>
    /// The timeout in milliseconds. Default value is 100 milliseconds.
    /// </value>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the SQL command to execute against the MySQL database.
    /// </summary>
    /// <value>
    /// The SQL command to execute. Default value is defined by <see cref="MySqlHealthCheck.DefaultCommand"/>.
    /// </value>
    /// <remarks>
    /// This property is for internal use only.
    /// </remarks>
    public string Command { get; internal set; } = MySqlHealthCheck.DefaultCommand;
}
