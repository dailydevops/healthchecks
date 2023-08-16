namespace NetEvolve.HealthChecks.SqlServer;

internal class SqlServerHealthCheckOptions
{
    public string ConnectionString { get; internal set; } = default!;

    public int Timeout { get; internal set; } = 100;
}
