namespace NetEvolve.HealthChecks.Oracle;

using System.Data.Common;
using global::Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class OracleHealthCheck : SqlCheckBase<OracleOptions>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1 FROM dual";

    public OracleHealthCheck(IOptionsMonitor<OracleOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override DbConnection CreateConnection(string connectionString) => new OracleConnection(connectionString);
}
