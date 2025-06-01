global using NetEvolve.HealthChecks.Tests.Integration.Internals;
global using TUnit.Core.Interfaces;

[assembly: ParallelLimiter<HealthCheckParallelLimit>]
[assembly: TUnit.Core.Executors.InvariantCulture]
