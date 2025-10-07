namespace NetEvolve.HealthChecks.Tests.Integration.GCP.Firestore;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Firestore;
using TUnit.Core.Interfaces;

public sealed class FirestoreDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly FirestoreContainer _database = new FirestoreBuilder().WithLogger(NullLogger.Instance).Build();

    public string ProjectId => _database.GetProjectId();

    public string EmulatorHost => _database.GetEmulatorEndpoint();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
