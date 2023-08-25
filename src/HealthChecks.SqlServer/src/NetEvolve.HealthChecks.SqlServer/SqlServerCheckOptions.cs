namespace NetEvolve.HealthChecks.SqlServer;

public class SqlServerCheckOptions
{
    public string ConnectionString { get; internal set; } = default!;

    public int Timeout { get; internal set; } = 100;
}
