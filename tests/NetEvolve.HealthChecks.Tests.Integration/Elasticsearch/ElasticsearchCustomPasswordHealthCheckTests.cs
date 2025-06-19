namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Cluster;

[ClassDataSource<ClusterCustomPassword>(Shared = SharedType.PerTestSession)]
[InheritsTests]
[TestGroup(nameof(Elasticsearch))]
public sealed class ElasticsearchCustomPasswordHealthCheckTests : ElasticsearchHealthCheckBaseTests
{
    public ElasticsearchCustomPasswordHealthCheckTests(ClusterCustomPassword cluster)
        : base(cluster) { }
}
