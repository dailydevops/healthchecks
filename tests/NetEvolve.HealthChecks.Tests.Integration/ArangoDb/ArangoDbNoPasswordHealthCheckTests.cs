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
}
