namespace NetEvolve.HealthChecks.Tests.Integration.Minio;

using System.Threading.Tasks;
using global::Minio;
using global::Minio.DataModel.Args;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Minio;

public sealed class MinioDatabase : IAsyncInitializer, IAsyncDisposable
{
    internal const string BucketName = "test-bucket";

    private readonly MinioContainer _container = new MinioBuilder(
        /*dockerimage*/"minio/minio:RELEASE.2023-01-31T02-24-19Z"
    )
        .WithLogger(NullLogger.Instance)
        .Build();
    private IMinioClient? _client;

    internal string ConnectionString => _container.GetConnectionString();

#pragma warning disable CA2000 // Dispose objects before losing scope
    internal IMinioClient Client =>
        new MinioClient()
            .WithEndpoint(_container.Hostname, _container.GetMappedPublicPort(9000))
            .WithCredentials(_container.GetAccessKey(), _container.GetSecretKey())
            .WithSSL(false)
            .Build();
#pragma warning restore CA2000 // Dispose objects before losing scope

    public async ValueTask DisposeAsync()
    {
        _client?.Dispose();
        await _container.DisposeAsync().ConfigureAwait(false);
    }

#pragma warning disable CA2000 // MinioClient lifetime managed by class, disposed in DisposeAsync
    public async Task InitializeAsync()
    {
        var cancellationToken = TestContext.Current!.Execution.CancellationToken;

        await _container.StartAsync(cancellationToken).ConfigureAwait(false);

        // Create Minio client
        _client = Client;

        // Create a test bucket
        var makeBucketArgs = new MakeBucketArgs().WithBucket(BucketName);
        await _client.MakeBucketAsync(makeBucketArgs, cancellationToken).ConfigureAwait(false);
    }
#pragma warning restore CA2000
}
