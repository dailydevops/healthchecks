namespace NetEvolve.HealthChecks.Tests.Integration.Internals;

using System.Diagnostics;

public record HealthCheckParallelLimit : IParallelLimit
{
    public int Limit
    {
        get
        {
            var limit = Math.Clamp(Environment.ProcessorCount - 1, Math.Min(4, Environment.ProcessorCount - 1), 16);
            Debug.WriteLine($"Parallel Limit: {limit}");
            return limit;
        }
    }
}
