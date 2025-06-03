namespace NetEvolve.HealthChecks.Odbc;

using System.Data.Common;
using System.Data.Odbc;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class OdbcHealthCheck : SqlCheckBase<OdbcOptions>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";

    public OdbcHealthCheck(IOptionsMonitor<OdbcOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override DbConnection CreateConnection(string connectionString) => new OdbcConnection(connectionString);
}
