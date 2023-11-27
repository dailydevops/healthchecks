#if USE_SQL_HEALTHCHECK
namespace NetEvolve.HealthChecks.Abstractions;

using Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// General options for <see cref="SqlCheckBase{TConfiguration}"/>.
/// </summary>
internal interface ISqlCheckOptions
{
    /// <summary>
    /// The connection string for the database to check.
    /// </summary>
    string ConnectionString { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against database.
    /// If the timeout is exceeded, the health check is classified as <see cref="HealthStatus.Degraded"/>.
    /// </summary>
    int Timeout { get; set; }

    /// <summary>
    /// The sql command to execute against the database.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    string Command { get; }
}
#endif
