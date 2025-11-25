namespace NetEvolve.HealthChecks.Tests.Integration.Minio;

using System.Threading.Tasks;
using global::Minio;
using global::Minio.DataModel.Args;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Minio;

public sealed class MinioDatabase : IAsyncInitializer, IAsyncDisposable
{
    internal const string AccessKey = "minioadmin";
    internal const string SecretKey = "minioadmin";
    internal const string BucketName = "test-bucket";

    private readonly MinioContainer _container = new MinioBuilder().WithLogger(NullLogger.Instance).Build();
#pragma warning disable TUnit0023 // Member should be disposed within a clean up method
#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private IMinioClient _client = default!;
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
#pragma warning restore TUnit0023 // Member should be disposed within a clean up method

    internal string ConnectionString => _container.GetConnectionString();

    internal IMinioClient Client => _client;

    public async ValueTask DisposeAsync()
    {
        _client?.Dispose();
        await _container.DisposeAsync().ConfigureAwait(false);
    }

#pragma warning disable CA2000 // Dispose objects before losing scope - handled by DisposeAsync
    public async Task InitializeAsync()
    {
        var cancellationToken = TestContext.Current!.Execution.CancellationToken;

        await _container.StartAsync(cancellationToken).ConfigureAwait(false);

        // Create Minio client
        var minioClientBuilder = new MinioClient()
            .WithEndpoint(_container.Hostname, _container.GetMappedPublicPort(9000))
            .WithCredentials(AccessKey, SecretKey)
            .WithSSL(false);

        _client = minioClientBuilder.Build();

        // Create a test bucket
        var makeBucketArgs = new MakeBucketArgs().WithBucket(BucketName);
        await _client.MakeBucketAsync(makeBucketArgs, cancellationToken).ConfigureAwait(false);
    }
#pragma warning restore CA2000
}
