﻿namespace NetEvolve.HealthChecks.Firebird;

/// <summary>
/// Options for <see cref="FirebirdHealthCheck"/>
/// </summary>
public sealed record FirebirdOptions
{
    /// <summary>
    /// Gets or sets the connection string for the Firebird database to check.
    /// </summary>
    /// <value>
    /// The connection string used to establish a connection to the Firebird database.
    /// </value>
    public string ConnectionString { get; set; } = default!;

    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when connecting and executing tasks against the Firebird database.
    /// </summary>
    /// <value>
    /// The timeout in milliseconds. Default value is 100 milliseconds.
    /// </value>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets the SQL command to execute against the Firebird database.
    /// </summary>
    /// <value>
    /// The SQL command to execute. Default value is defined by <see cref="FirebirdHealthCheck.DefaultCommand"/>.
    /// </value>
    /// <remarks>
    /// This property is for internal use only.
    /// </remarks>
    public string Command { get; internal set; } = FirebirdHealthCheck.DefaultCommand;
}
