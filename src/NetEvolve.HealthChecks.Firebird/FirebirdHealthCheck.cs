namespace NetEvolve.HealthChecks.Firebird;

using System.Data.Common;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class FirebirdHealthCheck : SqlCheckBase<FirebirdOptions>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1 FROM RDB$DATABASE;";

    public FirebirdHealthCheck(IOptionsMonitor<FirebirdOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override DbConnection CreateConnection(string connectionString) => new FbConnection(connectionString);
}
