namespace NetEvolve.HealthChecks.Tests.Integration.GCP.Firestore;

using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Firestore;
using TUnit.Core.Interfaces;

public sealed class FirestoreDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly FirestoreContainer _database = new FirestoreBuilder()
        .WithLogger(NullLogger.Instance)
        .WithWaitStrategy(
            Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request.ForPath("/").ForPort(8080))
        )
        .Build();

    public string ProjectId => "test-project";

    public string EmulatorHost => _database.GetEmulatorEndpoint();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
