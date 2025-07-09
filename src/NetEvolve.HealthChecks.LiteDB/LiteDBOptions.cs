namespace NetEvolve.HealthChecks.LiteDB;

public record LiteDBOptions
{
    public string ConnectionString { get; set; }
    public string CollectionName { get; set; }
    public double Timeout { get; set; } = 100;
}
