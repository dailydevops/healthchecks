namespace NetEvolve.HealthChecks.Db2.Devart;

/// <summary>
/// Options for <see cref="Db2DevartHealthCheck"/>
/// </summary>
public sealed record Db2DevartOptions
{
    /// <summary>
    /// Gets or sets the connection string for the IBM DB2 database to check.
    /// </summary>
    /// <value>
    /// The connection string used to establish a connection to the IBM DB2 database.
    /// </value>
    public string ConnectionString { get; set; } = default!;

    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when connecting and executing tasks against the IBM DB2 database.
    /// </summary>
    /// <value>
    /// The timeout in milliseconds. Default value is 100 milliseconds.
    /// </value>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets the SQL command to execute against the IBM DB2 database.
    /// </summary>
    /// <value>
    /// The SQL command to execute. Default value is defined by <see cref="Db2DevartHealthCheck.DefaultCommand"/>.
    /// </value>
    /// <remarks>
    /// This property is for internal use only.
    /// </remarks>
    public string Command { get; internal set; } = Db2DevartHealthCheck.DefaultCommand;
}