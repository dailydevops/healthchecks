namespace NetEvolve.HealthChecks.Firebird;

using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;
using SourceGenerator.SqlHealthCheck;

[GenerateSqlHealthCheck(typeof(FbConnection), typeof(FirebirdOptions), true)]
internal sealed partial class FirebirdHealthCheck(IOptionsMonitor<FirebirdOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<FirebirdOptions>(optionsMonitor)
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1 FROM RDB$DATABASE;";
}
