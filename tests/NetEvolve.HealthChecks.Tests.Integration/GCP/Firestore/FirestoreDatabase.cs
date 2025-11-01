namespace NetEvolve.HealthChecks.Tests.Integration.GCP.Firestore;

using System;
using System.Threading.Tasks;
using global::Google.Cloud.Firestore;
using global::Google.Cloud.Firestore.V1;
using global::Grpc.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Firestore;
using TUnit.Core.Interfaces;

public sealed class FirestoreDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly FirestoreContainer _container = new FirestoreBuilder().WithLogger(NullLogger.Instance).Build();

    private FirestoreClient? _client;
    private FirestoreDb? _database;

    public const string ProjectId = "test-project";

    public FirestoreDb Database => _database ?? throw new InvalidOperationException("Database not initialized");

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync().ConfigureAwait(false);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        // Parse endpoint to get host:port
        var fullEndpoint = _container.GetEmulatorEndpoint();
        var uri = new Uri(fullEndpoint);
        var hostPort = $"{uri.Host}:{uri.Port}";

        // Create Firestore client configured for emulator
        var clientBuilder = new FirestoreClientBuilder
        {
            Endpoint = hostPort,
            ChannelCredentials = ChannelCredentials.Insecure,
        };

        _client = await clientBuilder.BuildAsync().ConfigureAwait(false);
        _database = await FirestoreDb.CreateAsync(ProjectId, _client).ConfigureAwait(false);
    }
}
