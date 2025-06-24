namespace NetEvolve.HealthChecks.Tests.Integration.ArangoDb;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.ArangoDb;
using NetEvolve.HealthChecks.Tests.Integration.ArangoDb.Container;

[ClassDataSource<ContainerNoPassword>(Shared = InstanceSharedType.ArangoDb)]
[InheritsTests]
[TestGroup(nameof(ArangoDb))]
public sealed class ArangoDbNoPasswordHealthCheckTests : ArangoDbHealthCheckBaseTests
{
    public ArangoDbNoPasswordHealthCheckTests(ContainerNoPassword container)
        : base(container) { }

    [Test]
    public async Task AddArangoDb_UseConfigurationWithoutCredentials_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddArangoDb("TestContainerNoCredentialsHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    {
                        "HealthChecks:ArangoDb:TestContainerNoCredentialsHealthy:Mode",
                        $"{ArangoDbClientCreationMode.Internal}"
                    },
                    {
                        "HealthChecks:ArangoDb:TestContainerNoCredentialsHealthy:TransportAddress",
                        _container.TransportAddress
                    },
                    { "HealthChecks:ArangoDb:TestContainerNoCredentialsHealthy:Timeout", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
