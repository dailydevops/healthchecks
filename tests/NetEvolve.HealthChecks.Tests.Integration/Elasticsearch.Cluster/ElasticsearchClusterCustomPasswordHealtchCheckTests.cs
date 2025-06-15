namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Cluster;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Cluster.ContainerCluster;

[ClassDataSource<ClusterCustomPassword>(Shared = SharedType.PerTestSession)]
[InheritsTests]
[TestGroup($"{nameof(Elasticsearch)}.{nameof(Cluster)}")]
public sealed class ElasticsearchClusterCustomPasswordHealthCheckTests : ElasticsearchClusterHealthCheckBaseTests
{
    public ElasticsearchClusterCustomPasswordHealthCheckTests(ClusterCustomPassword cluster)
        : base(cluster) { }
}
