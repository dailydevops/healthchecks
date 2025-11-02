namespace NetEvolve.HealthChecks.ClickHouse;

using global::ClickHouse.Client.ADO;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(ClickHouseConnection), typeof(ClickHouseOptions), true)]
internal sealed partial class ClickHouseHealthCheck
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
