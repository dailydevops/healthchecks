namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Container;

using System;
using System.Threading.Tasks;
using Testcontainers.Elasticsearch;

public abstract class ContainerBase : IAsyncInitializer, IAsyncDisposable
{
    private readonly ElasticsearchContainer _container;

    protected ContainerBase(string? password)
    {
        var builder = new ElasticsearchBuilder(
            /*dockerimage*/"elasticsearch:9.3.0"
        );

        if (!string.IsNullOrWhiteSpace(password))
        {
            _ = builder.WithPassword(password);
        }

        _container = builder.Build();
        Password = password;
    }

    public string? Username => Password is not null ? ElasticsearchBuilder.DefaultUsername : null;

    public string? Password { get; }

    public Uri ConnectionString => new Uri(_container.GetConnectionString());

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
