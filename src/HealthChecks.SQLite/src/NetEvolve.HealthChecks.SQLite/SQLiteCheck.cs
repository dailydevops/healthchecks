﻿namespace NetEvolve.HealthChecks.SQLite;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using System.Data.Common;

internal sealed class SQLiteCheck : SqlCheckBase<SQLiteOptions>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";

    public SQLiteCheck(IOptionsMonitor<SQLiteOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override DbConnection CreateConnection(string connectionString) =>
        new SqliteConnection(connectionString);
}
