namespace NetEvolve.HealthChecks.MySql;

using System.Data.Common;
using global::MySql.Data.MySqlClient;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class MySqlHealthCheck : SqlCheckBase<MySqlOptions>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";

    public MySqlHealthCheck(IOptionsMonitor<MySqlOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override DbConnection CreateConnection(string connectionString) => new MySqlConnection(connectionString);
}
