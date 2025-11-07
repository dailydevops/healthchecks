namespace NetEvolve.HealthChecks.Tests.Integration.OpenSearch;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.OpenSearch;

public sealed class OpenSearchContainer : IAsyncInitializer, IAsyncDisposable
{
    private readonly Testcontainers.OpenSearch.OpenSearchContainer _database = new OpenSearchBuilder()
        .WithLogger(NullLogger.Instance)
        .Build();

    public string GetConnectionString() => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
