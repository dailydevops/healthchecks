namespace NetEvolve.HealthChecks.SqlEdge;

using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class SqlEdgeCheck : SqlCheckBase<SqlEdgeOptions>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";

    public SqlEdgeCheck(IOptionsMonitor<SqlEdgeOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override DbConnection CreateConnection(string connectionString) =>
        new SqlConnection(connectionString);
}
