namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Cluster;

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

    public string? Username => _firstContainer?.Username;

    public string? Password => _firstContainer?.Password;

    public async ValueTask DisposeAsync()
    {
        await Parallel
            .ForEachAsync(
                _containers,
                TestContext.Current!.CancellationToken,
                async (container, _) => await container.DisposeAsync().AsTask().ConfigureAwait(false)
            )
            .ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public async Task InitializeAsync() =>
        await Parallel
            .ForEachAsync(
                _containers,
                TestContext.Current!.CancellationToken,
                async (container, _) => await container.InitializeAsync().ConfigureAwait(false)
            )
            .ConfigureAwait(false);

    private ContainerBase? _firstContainer => _containers.FirstOrDefault();
}
