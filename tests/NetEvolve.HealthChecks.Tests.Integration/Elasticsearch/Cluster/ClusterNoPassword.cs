namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Cluster;

using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Container;

public sealed class ClusterNoPassword : ClusterBase<ContainerNoPassword>
{
    public ClusterNoPassword()
        : base() { }
}
