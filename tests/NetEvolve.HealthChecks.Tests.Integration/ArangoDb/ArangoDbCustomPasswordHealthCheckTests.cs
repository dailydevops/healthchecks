namespace NetEvolve.HealthChecks.Tests.Integration.ArangoDb;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.ArangoDb.Container;

[ClassDataSource<ContainerCustomPassword>(Shared = SharedType.PerTestSession)]
[InheritsTests]
[TestGroup(nameof(ArangoDb))]
public sealed class ArangoDbCustomPasswordHealthCheckTests : ArangoDbHealthCheckBaseTests
{
    public ArangoDbCustomPasswordHealthCheckTests(ContainerCustomPassword container)
        : base(container) { }
}
