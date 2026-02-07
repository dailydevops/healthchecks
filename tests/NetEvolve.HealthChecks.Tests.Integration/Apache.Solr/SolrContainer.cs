namespace NetEvolve.HealthChecks.Tests.Integration.Apache.Solr;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;

public sealed class SolrContainer : IAsyncInitializer, IAsyncDisposable
{
    private const string SolrCore = "healthchecks";
    private const int SolrPort = 8983;

    private readonly IContainer _container = new ContainerBuilder(
        /*dockerimage*/"solr:9.10.1"
    )
        .WithPortBinding(SolrPort, assignRandomHostPort: true)
        .WithCommand("solr-precreate", SolrCore)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(@"o\.a\.s\.c\.SolrCore Registered new searcher"))
        .WithLogger(NullLogger.Instance)
        .Build();

    [SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "As designed.")]
    public string Url => $"http://{_container.Hostname}:{_container.GetMappedPublicPort(SolrPort)}/solr/{SolrCore}";

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
