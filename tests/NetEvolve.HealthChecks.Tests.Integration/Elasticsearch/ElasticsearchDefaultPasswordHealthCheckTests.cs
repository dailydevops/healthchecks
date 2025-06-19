namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Cluster;

[ClassDataSource<ClusterDefaultPassword>(Shared = SharedType.PerTestSession)]
[InheritsTests]
[TestGroup(nameof(Elasticsearch))]
public sealed class ElasticsearchDefaultPasswordHealthCheckTests : ElasticsearchHealthCheckBaseTests
{
    public ElasticsearchDefaultPasswordHealthCheckTests(ClusterDefaultPassword cluster)
        : base(cluster) { }
}
