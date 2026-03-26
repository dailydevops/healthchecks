namespace NetEvolve.HealthChecks.Tests.Integration.GCP.PubSub;

using System;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.PubSub;
using TUnit.Core.Interfaces;

public sealed class PubSubEmulator : IAsyncInitializer, IAsyncDisposable
{
    private readonly PubSubContainer _container = new PubSubBuilder(
        /*dockerimage*/"gcr.io/google.com/cloudsdktool/google-cloud-cli:446.0.1"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    private PublisherServiceApiClient? _client;

    public const string ProjectId = "test-project";

    public PublisherServiceApiClient Client => _client ?? throw new InvalidOperationException("Client not initialized");

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        var endpoint = _container.GetEmulatorEndpoint();

        var clientBuilder = new PublisherServiceApiClientBuilder
        {
            Endpoint = endpoint,
            ChannelCredentials = ChannelCredentials.Insecure,
        };

        _client = await clientBuilder.BuildAsync().ConfigureAwait(false);
        _ = await _client
            .CreateTopicAsync(new TopicName(ProjectId, "healthcheck"), CancellationToken.None)
            .ConfigureAwait(false);
    }
}
