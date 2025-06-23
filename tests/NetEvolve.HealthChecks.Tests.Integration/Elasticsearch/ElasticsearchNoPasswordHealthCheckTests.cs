namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Container;

[ClassDataSource<ContainerNoPassword>(Shared = InstanceSharedType.Elasticsearch)]
[InheritsTests]
[TestGroup(nameof(Elasticsearch))]
public sealed class ElasticsearchNoPasswordHealthCheckTests : ElasticsearchHealthCheckBaseTests
{
    public ElasticsearchNoPasswordHealthCheckTests(ContainerNoPassword cluster)
        : base(cluster) { }
}
