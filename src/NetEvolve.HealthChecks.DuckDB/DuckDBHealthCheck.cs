namespace NetEvolve.HealthChecks.DuckDB;

using global::DuckDB.NET.Data;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(DuckDBConnection), typeof(DuckDBOptions), true)]
internal sealed partial class DuckDBHealthCheck(IOptionsMonitor<DuckDBOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<DuckDBOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
