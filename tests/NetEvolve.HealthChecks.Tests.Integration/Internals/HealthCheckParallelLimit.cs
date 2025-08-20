namespace NetEvolve.HealthChecks.Tests.Integration.Internals;

public record HealthCheckParallelLimit : IParallelLimit
{
    public int Limit => Math.Clamp(Environment.ProcessorCount, Math.Min(4, Environment.ProcessorCount), 6);
}
