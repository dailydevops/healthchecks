namespace NetEvolve.HealthChecks.SqlServer;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using System.Data.Common;

internal sealed class SqlServerCheck : SqlCheckBase<SqlServerOptions>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";

    public SqlServerCheck(IOptionsMonitor<SqlServerOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override DbConnection CreateConnection(string connectionString) =>
        new SqlConnection(connectionString);
}
