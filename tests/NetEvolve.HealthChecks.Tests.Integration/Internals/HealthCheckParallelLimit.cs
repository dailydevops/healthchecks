namespace NetEvolve.HealthChecks.Tests.Integration.Internals;

public record HealthCheckParallelLimit : IParallelLimit
{
    public int Limit => Math.Clamp(Environment.ProcessorCount, 4, 6);
}
