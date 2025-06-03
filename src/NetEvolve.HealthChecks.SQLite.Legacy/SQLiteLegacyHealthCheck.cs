namespace NetEvolve.HealthChecks.SQLite.Legacy;

using System.Data.Common;
using System.Data.SQLite;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class SQLiteLegacyHealthCheck : SqlCheckBase<SQLiteLegacyOptions>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";

    public SQLiteLegacyHealthCheck(IOptionsMonitor<SQLiteLegacyOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override DbConnection CreateConnection(string connectionString) => new SQLiteConnection(connectionString);
}
