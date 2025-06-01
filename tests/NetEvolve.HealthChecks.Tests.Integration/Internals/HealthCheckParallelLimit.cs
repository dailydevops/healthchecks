namespace NetEvolve.HealthChecks.Tests.Integration.Internals;

public record HealthCheckParallelLimit : IParallelLimit
{
    public int Limit => 4;
}
