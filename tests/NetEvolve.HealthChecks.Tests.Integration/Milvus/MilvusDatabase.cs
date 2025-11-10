namespace NetEvolve.HealthChecks.Tests.Integration.Milvus;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Milvus;
using TUnit.Core.Interfaces;

public sealed class MilvusDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly MilvusContainer _database = new MilvusBuilder().WithLogger(NullLogger.Instance).Build();

    public string GrpcConnectionString => _database.GetEndpoint().ToString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
