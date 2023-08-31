#if USE_SQL_HEALTHCHECK
namespace NetEvolve.HealthChecks.Abstractions;

internal interface ISqlCheckOptions
{
    /// <summary>
    /// The connection string for the SQL Server database to check.
    /// </summary>
    string ConnectionString { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against SQL Server database.
    /// </summary>
    int Timeout { get; set; }

    /// <summary>
    /// The sql command to execute against the SQL Server database.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    string Command { get; }
}
#endif
