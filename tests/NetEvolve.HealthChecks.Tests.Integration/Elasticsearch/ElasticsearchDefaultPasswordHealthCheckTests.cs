namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Container;

[ClassDataSource<ContainerDefaultPassword>(Shared = SharedType.PerTestSession)]
[InheritsTests]
[TestGroup(nameof(Elasticsearch))]
public sealed class ElasticsearchDefaultPasswordHealthCheckTests : ElasticsearchHealthCheckBaseTests
{
    public ElasticsearchDefaultPasswordHealthCheckTests(ContainerDefaultPassword container)
        : base(container) { }
}
