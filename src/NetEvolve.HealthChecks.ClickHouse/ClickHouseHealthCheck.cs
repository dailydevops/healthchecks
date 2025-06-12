namespace NetEvolve.HealthChecks.ClickHouse;

using global::ClickHouse.Client.ADO;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.SqlHealthCheck;

[GenerateSqlHealthCheck(typeof(ClickHouseConnection), typeof(ClickHouseOptions), true)]
internal sealed partial class ClickHouseHealthCheck(IOptionsMonitor<ClickHouseOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<ClickHouseOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
