namespace NetEvolve.HealthChecks.Tests.Integration.Qdrant;

using System.Threading.Tasks;
using Testcontainers.Qdrant;
using Xunit;

public sealed class QdrantDatabase : IAsyncLifetime
{
    private readonly QdrantContainer _database = new QdrantBuilder().Build();

    public Uri GrpcConnectionString => new Uri(_database.GetGrpcConnectionString());

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
