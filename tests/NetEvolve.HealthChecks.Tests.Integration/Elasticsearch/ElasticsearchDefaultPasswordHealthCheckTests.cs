namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Container;

[ClassDataSource<ContainerDefaultPassword>(Shared = InstanceSharedType.Elasticsearch)]
[InheritsTests]
[TestGroup(nameof(Elasticsearch))]
[TestGroup("Z04TestGroup")]
public sealed class ElasticsearchDefaultPasswordHealthCheckTests : ElasticsearchHealthCheckBaseTests
{
    public ElasticsearchDefaultPasswordHealthCheckTests(ContainerDefaultPassword cluster)
        : base(cluster, isCluster: true) { }
}
