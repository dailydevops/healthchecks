namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Container;

using Testcontainers.Elasticsearch;

public sealed class ContainerDefaultPassword : ContainerBase
{
    public ContainerDefaultPassword()
        : base(ElasticsearchBuilder.DefaultPassword) { }
}
