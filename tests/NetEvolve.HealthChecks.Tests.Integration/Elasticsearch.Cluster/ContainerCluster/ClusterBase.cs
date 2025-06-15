namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Cluster.ContainerCluster;

using System.Collections.Generic;
using NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Container;

public abstract class ClusterBase<TContainer> : IContainerCluster
    where TContainer : ContainerBase, new()
{
    private readonly IEnumerable<ContainerBase> _containers = [];

    protected ClusterBase(int containersAmount = 2)
    {
        for (var i = 0; i < containersAmount; i++)
        {
            _containers = _containers.Append(new TContainer());
        }
    }

    public IEnumerable<string> ConnectionStrings => _containers.Select(container => container.ConnectionString);

    public string Username =>
        _containers.FirstOrDefault()?.Username ?? throw new InvalidOperationException("No containers available.");

    public string Password =>
        _containers.FirstOrDefault()?.Password ?? throw new InvalidOperationException("No containers available.");

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(_containers.Select(container => container.DisposeAsync().AsTask())).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public async Task InitializeAsync() =>
        await Task.WhenAll(_containers.Select(container => container.InitializeAsync())).ConfigureAwait(false);
}
