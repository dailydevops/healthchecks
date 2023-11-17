namespace NetEvolve.HealthChecks.SqlServer.Legacy;

using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class SqlServerLegacyCheck : SqlCheckBase<SqlServerLegacyOptions>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";

    public SqlServerLegacyCheck(IOptionsMonitor<SqlServerLegacyOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override DbConnection CreateConnection(string connectionString) =>
        new SqlConnection(connectionString);
}
