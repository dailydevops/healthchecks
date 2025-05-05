namespace NetEvolve.HealthChecks.Npgsql;

using System.Data.Common;
using global::Npgsql;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class NpgsqlCheck : SqlCheckBase<NpgsqlOptions>
{
    /// <summary>
    /// The default sql command.
    /// </summary>
    public const string DefaultCommand = "SELECT 1;";

    public NpgsqlCheck(IOptionsMonitor<NpgsqlOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override DbConnection CreateConnection(string connectionString) => new NpgsqlConnection(connectionString);
}
