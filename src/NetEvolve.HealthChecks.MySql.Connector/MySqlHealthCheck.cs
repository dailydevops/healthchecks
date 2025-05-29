namespace NetEvolve.HealthChecks.MySql.Connector;

using System.Data.Common;
using Microsoft.Extensions.Options;
using MySqlConnector;
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
