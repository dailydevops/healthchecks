namespace NetEvolve.HealthChecks.Tests.Unit.Azure.CosmosDB;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.CosmosDB;

[TestGroup($"{nameof(Azure)}.{nameof(CosmosDB)}")]
public sealed class CosmosDbAvailableHealthCheckTests
{
    [Test]
    public void Constructor_Default_Expected()
    {
        // Arrange / Act / Assert
        _ = new CosmosDbAvailableHealthCheck();
    }
}
