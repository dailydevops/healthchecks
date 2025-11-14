namespace NetEvolve.HealthChecks.Tests.Integration.ArangoDb;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.ArangoDb.Container;

[ClassDataSource<ContainerNoPassword>(Shared = InstanceSharedType.ArangoDb)]
[InheritsTests]
[TestGroup(nameof(ArangoDb))]
[TestGroup("Z01TestGroup")]
public sealed class ArangoDbNoPasswordHealthCheckTests : ArangoDbHealthCheckBaseTests
{
    public ArangoDbNoPasswordHealthCheckTests(ContainerNoPassword container)
        : base(container) { }
}
