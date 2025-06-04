namespace NetEvolve.HealthChecks.DuckDB;

using System.Data.Common;
using global::DuckDB.NET.Data;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class DuckDBHealthCheck : SqlCheckBase<DuckDBOptions>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";

    public DuckDBHealthCheck(IOptionsMonitor<DuckDBOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override DbConnection CreateConnection(string connectionString) => new DuckDBConnection(connectionString);
}
