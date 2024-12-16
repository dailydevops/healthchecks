namespace NetEvolve.HealthChecks.ClickHouse;

using System.Data.Common;
using global::ClickHouse.Client.ADO;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class ClickHouseCheck : SqlCheckBase<ClickHouseOptions>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";

    public ClickHouseCheck(IOptionsMonitor<ClickHouseOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override DbConnection CreateConnection(string connectionString) =>
        new ClickHouseConnection(connectionString);
}
