namespace NetEvolve.HealthChecks.DB2;

using System.Data.Common;
using IBM.Data.Db2;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class DB2HealthCheck : SqlCheckBase<DB2Options>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1 FROM SYSIBM.SYSDUMMY1;";

    public DB2Check(IOptionsMonitor<DB2Options> optionsMonitor)
        : base(optionsMonitor) { }

    protected override DbConnection CreateConnection(string connectionString) => new DB2Connection(connectionString);
}
