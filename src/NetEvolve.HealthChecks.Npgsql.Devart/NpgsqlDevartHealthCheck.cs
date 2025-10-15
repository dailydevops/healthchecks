namespace NetEvolve.HealthChecks.Npgsql.Devart;

using global::Devart.Data.PostgreSql;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.SqlHealthCheck;

[GenerateSqlHealthCheck(typeof(PgSqlConnection), typeof(NpgsqlDevartOptions), false)]
internal sealed partial class NpgsqlDevartHealthCheck(IOptionsMonitor<NpgsqlDevartOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<NpgsqlDevartOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}