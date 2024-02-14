namespace NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// General options for <see cref="SqlCheckBase{TConfiguration}"/>.
/// </summary>
public interface ISqlCheckOptions
{
    /// <summary>
    /// The connection string for the database to check.
    /// </summary>
    string ConnectionString { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against database.
    /// </summary>
    int Timeout { get; set; }

    /// <summary>
    /// The sql command to execute against the database.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    string Command { get; }
}
