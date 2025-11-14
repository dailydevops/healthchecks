namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Container;

[ClassDataSource<ContainerNoPassword>(Shared = InstanceSharedType.Elasticsearch)]
[InheritsTests]
[TestGroup(nameof(Elasticsearch))]
[TestGroup("Z01TestGroup")]
public sealed class ElasticsearchCustomPasswordHealthCheckTests : ElasticsearchHealthCheckBaseTests
{
    public ElasticsearchCustomPasswordHealthCheckTests(ContainerNoPassword cluster)
        : base(cluster) { }
}
