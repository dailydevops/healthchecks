namespace NetEvolve.HealthChecks.Tests.Unit.Azure.CosmosDB;

using NetEvolve.Extensions.TUnit;

[TestGroup($"{nameof(Azure)}.{nameof(CosmosDB)}")]
public sealed class CosmosDbAvailableHealthCheckTests
{
    // Note: The health check uses source generators and requires a service provider
    // The constructor tests are not applicable as the class is instantiated by DI
}
