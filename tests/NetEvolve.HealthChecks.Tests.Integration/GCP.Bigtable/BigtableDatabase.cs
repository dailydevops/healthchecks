namespace NetEvolve.HealthChecks.Tests.Integration.GCP.Bigtable;

using System;
using System.Threading.Tasks;
using Google.Cloud.Bigtable.Admin.V2;
using Grpc.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Bigtable;
using TUnit.Core.Interfaces;

public sealed class BigtableDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly BigtableContainer _container = new BigtableBuilder().WithLogger(NullLogger.Instance).Build();

    private BigtableTableAdminClient? _client;

    public const string ProjectId = "test-project";
    public const string InstanceId = "test-instance";

    public BigtableTableAdminClient Client => _client ?? throw new InvalidOperationException("Client not initialized");

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        // Parse endpoint to get host:port
        var fullEndpoint = _container.GetEmulatorEndpoint();
        var uri = new Uri(fullEndpoint);

        // Create Bigtable client configured for emulator
        var clientBuilder = new BigtableTableAdminClientBuilder
        {
            Endpoint = $"{uri.Host}:{uri.Port}",
            ChannelCredentials = ChannelCredentials.Insecure,
        };

        _client = await clientBuilder.BuildAsync().ConfigureAwait(false);

        _ = await _client
            .CreateTableAsync(ProjectId, InstanceId, new Table(), CancellationToken.None)
            .ConfigureAwait(false);
    }
}
