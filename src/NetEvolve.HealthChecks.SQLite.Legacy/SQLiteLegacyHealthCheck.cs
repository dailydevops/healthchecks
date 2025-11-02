namespace NetEvolve.HealthChecks.SQLite.Legacy;

using System.Data.SQLite;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.Attributes;

[GenerateSqlHealthCheck(typeof(SQLiteConnection), typeof(SQLiteLegacyOptions), true)]
internal sealed partial class SQLiteLegacyHealthCheck(IOptionsMonitor<SQLiteLegacyOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<SQLiteLegacyOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";
}
