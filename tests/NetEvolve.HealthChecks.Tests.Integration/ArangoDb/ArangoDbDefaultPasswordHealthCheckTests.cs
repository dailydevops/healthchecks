namespace NetEvolve.HealthChecks.Tests.Integration.ArangoDb;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.ArangoDb.Container;

[ClassDataSource<ContainerDefaultPassword>(Shared = InstanceSharedType.ArangoDb)]
[InheritsTests]
[TestGroup(nameof(ArangoDb))]
public sealed class ArangoDbDefaultPasswordHealthCheckTests : ArangoDbHealthCheckBaseTests
{
    public ArangoDbDefaultPasswordHealthCheckTests(ContainerDefaultPassword container)
        : base(container) { }
}
