namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Container;

using System;
using System.Threading.Tasks;
using Testcontainers.Elasticsearch;

public abstract class ContainerBase : IAsyncInitializer, IAsyncDisposable
{
    private readonly ElasticsearchContainer _container;

    protected ContainerBase(string? password)
    {
        var builder = new ElasticsearchBuilder().WithImage("elasticsearch:9.0.2");

        if (!string.IsNullOrWhiteSpace(password))
        {
            _ = builder.WithPassword(password);
        }

        _container = builder.Build();
        Password = password;
    }

    public string? Username => Password is not null ? ElasticsearchBuilder.DefaultUsername : null;

    public string? Password { get; }

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
