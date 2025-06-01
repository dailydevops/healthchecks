namespace NetEvolve.HealthChecks.Tests.Integration.Qdrant;

using System.Threading.Tasks;
using Testcontainers.Qdrant;
using TUnit.Core.Interfaces;

public sealed class QdrantDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly QdrantContainer _database = new QdrantBuilder().Build();

    public Uri GrpcConnectionString => new Uri(_database.GetGrpcConnectionString());

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
