namespace NetEvolve.HealthChecks.SqlServer;

/// <summary>
/// Options for <see cref="SqlServerHealthCheck"/>
/// </summary>
public class SqlServerHealthCheckOptions
{
    /// <summary>
    /// The connection string for the SQL Server database to check.
    /// </summary>
    public string ConnectionString { get; internal set; } = default!;

    /// <summary>
    /// The timeout to use when connecting and executing tasks against SQL Server database.
    /// </summary>
    public int Timeout { get; internal set; } = 100;
}
