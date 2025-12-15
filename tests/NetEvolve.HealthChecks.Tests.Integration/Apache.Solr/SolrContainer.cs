namespace NetEvolve.HealthChecks.Tests.Integration.Apache.Solr;

using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;

public sealed class SolrContainer : IAsyncInitializer, IAsyncDisposable
{
    private const int SolrPort = 8983;

    private readonly IContainer _container = new ContainerBuilder()
        .WithImage("solr:9")
        .WithPortBinding(SolrPort, true)
        .WithCommand("solr-precreate", "healthchecks")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(SolrPort)))
        .WithLogger(NullLogger.Instance)
        .Build();

    public string BaseUrl => $"http://{_container.Hostname}:{_container.GetMappedPublicPort(SolrPort)}";

    public string Core => "healthchecks";

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
