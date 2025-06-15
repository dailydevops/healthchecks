namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Container;

[ClassDataSource<ContainerCustomPassword>(Shared = SharedType.PerTestSession)]
[InheritsTests]
[TestGroup(nameof(Elasticsearch))]
public sealed class ElasticsearchCustomPasswordHealthCheckTests : ElasticsearchHealthCheckBaseTests
{
    public ElasticsearchCustomPasswordHealthCheckTests(ContainerCustomPassword container)
        : base(container) { }
}
