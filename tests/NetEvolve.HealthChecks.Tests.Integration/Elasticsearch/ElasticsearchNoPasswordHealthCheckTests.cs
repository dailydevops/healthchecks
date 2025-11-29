namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Container;

[ClassDataSource<ContainerNoPassword>(Shared = SharedType.PerClass)]
[InheritsTests]
[TestGroup(nameof(Elasticsearch))]
[TestGroup("Z04TestGroup")]
public sealed class ElasticsearchNoPasswordHealthCheckTests : ElasticsearchHealthCheckBaseTests
{
    public ElasticsearchNoPasswordHealthCheckTests(ContainerNoPassword cluster)
        : base(cluster) { }
}
