namespace NetEvolve.HealthChecks.Tests.Integration.ArangoDb;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.ArangoDb.Container;

[ClassDataSource<ContainerNoPassword>(Shared = SharedType.PerTestSession)]
[InheritsTests]
[TestGroup(nameof(ArangoDb))]
public sealed class ArangoDbNoPasswordHealthCheckTests : ArangoDbHealthCheckBaseTests
{
    public ArangoDbNoPasswordHealthCheckTests(ContainerNoPassword container)
        : base(container) { }
}
