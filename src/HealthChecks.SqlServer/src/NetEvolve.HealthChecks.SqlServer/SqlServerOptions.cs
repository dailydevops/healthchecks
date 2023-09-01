﻿namespace NetEvolve.HealthChecks.SqlServer;

using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Options for <see cref="SqlServerCheck"/>
/// </summary>
public class SqlServerOptions : ISqlCheckOptions
{
    /// <summary>
    /// The connection string for the SQL Server database to check.
    /// </summary>
    public string ConnectionString { get; set; } = default!;

    /// <summary>
    /// The timeout to use when connecting and executing tasks against SQL Server database.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// The sql command to execute against the SQL Server database.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    public string Command { get; internal set; } = SqlServerCheck.DefaultCommand;
}
