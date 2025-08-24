namespace NetEvolve.HealthChecks.SQLite.Devart;

using global::Devart.Data.SQLite;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.SqlHealthCheck;

[GenerateSqlHealthCheck(typeof(SQLiteConnection), typeof(SQLiteDevartOptions), true)]
internal sealed partial class SQLiteDevartHealthCheck(IOptionsMonitor<SQLiteDevartOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<SQLiteDevartOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}