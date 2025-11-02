namespace NetEvolve.HealthChecks.Npgsql;

using global::Npgsql;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(NpgsqlConnection), typeof(NpgsqlOptions), true)]
internal sealed partial class NpgsqlHealthCheck(IOptionsMonitor<NpgsqlOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<NpgsqlOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
