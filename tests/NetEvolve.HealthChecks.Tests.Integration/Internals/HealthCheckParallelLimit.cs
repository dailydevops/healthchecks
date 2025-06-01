namespace NetEvolve.HealthChecks.Tests.Integration.Internals;

public record HealthCheckParallelLimit : IParallelLimit
{
    public int Limit => Math.Min(4, Math.Max(4, Environment.ProcessorCount - 1));
}
