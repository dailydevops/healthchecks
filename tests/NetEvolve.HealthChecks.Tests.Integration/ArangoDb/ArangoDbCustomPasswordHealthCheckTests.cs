namespace NetEvolve.HealthChecks.Tests.Integration.ArangoDb;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.ArangoDb.Container;

[ClassDataSource<ContainerCustomPassword>(Shared = SharedType.PerClass)]
[InheritsTests]
[TestGroup(nameof(ArangoDb))]
[TestGroup("Z01TestGroup")]
public sealed class ArangoDbCustomPasswordHealthCheckTests : ArangoDbHealthCheckBaseTests
{
    public ArangoDbCustomPasswordHealthCheckTests(ContainerCustomPassword container)
        : base(container) { }
}
