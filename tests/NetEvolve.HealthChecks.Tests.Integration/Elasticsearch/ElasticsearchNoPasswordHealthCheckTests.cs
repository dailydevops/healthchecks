namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Cluster;

[ClassDataSource<ClusterNoPassword>(Shared = SharedType.PerTestSession)]
[InheritsTests]
[TestGroup(nameof(Elasticsearch))]
public sealed class ElasticsearchNoPasswordHealthCheckTests : ElasticsearchHealthCheckBaseTests
{
    public ElasticsearchNoPasswordHealthCheckTests(ClusterNoPassword cluster)
        : base(cluster) { }
}
