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

#pragma warning disable CS0618 // Type or member is obsolete
    protected override DbConnection CreateConnection(string connectionString) => new SqlConnection(connectionString);
#pragma warning restore CS0618 // Type or member is obsolete
}
