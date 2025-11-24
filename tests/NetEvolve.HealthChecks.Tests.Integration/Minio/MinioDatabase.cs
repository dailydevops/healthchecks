namespace NetEvolve.HealthChecks.Tests.Integration.Minio;

using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Minio;

public sealed class MinioDatabase : IAsyncInitializer, IAsyncDisposable
{
    internal const string AccessKey = "minioadmin";
    internal const string SecretKey = "minioadmin";
    internal const string BucketName = "test-bucket";

    private readonly MinioContainer _container = new MinioBuilder().WithLogger(NullLogger.Instance).Build();

    internal string ConnectionString => _container.GetConnectionString();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        var cancellationToken = TestContext.Current!.Execution.CancellationToken;

        await _container.StartAsync(cancellationToken).ConfigureAwait(false);

        // Create a test bucket
        using var s3Client = new AmazonS3Client(
            AccessKey,
            SecretKey,
            new AmazonS3Config { ServiceURL = ConnectionString, ForcePathStyle = true }
        );

        var putBucketRequest = new PutBucketRequest { BucketName = BucketName };
        _ = await s3Client.PutBucketAsync(putBucketRequest, cancellationToken).ConfigureAwait(false);
    }
}
