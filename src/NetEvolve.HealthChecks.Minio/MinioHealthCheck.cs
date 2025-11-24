namespace NetEvolve.HealthChecks.Minio;

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(MinioOptions))]
internal sealed partial class MinioHealthCheck
{
    private static async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        MinioOptions options,
        CancellationToken cancellationToken
    )
    {
        using var client = CreateClient(options);

        var (isTimelyResponse, response) = await client
            .ListObjectsV2Async(
                new ListObjectsV2Request { BucketName = options.BucketName, MaxKeys = 1 },
                cancellationToken
            )
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            return HealthCheckUnhealthy(
                failureStatus,
                name,
                $"Unexpected HTTP status code: {response.HttpStatusCode}."
            );
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    private static AmazonS3Client CreateClient(MinioOptions options)
    {
        var config = new AmazonS3Config
        {
            ServiceURL = options.ServiceUrl,
            ForcePathStyle = true, // Required for Minio
        };

        var credentials = options.GetCredentials();

        return (credentials is not null, options.RegionEndpoint is not null) switch
        {
            (true, true) => new AmazonS3Client(credentials, options.RegionEndpoint),
            (true, false) => new AmazonS3Client(credentials, config),
            (false, true) => new AmazonS3Client(options.RegionEndpoint),
            _ => new AmazonS3Client(config),
        };
    }
}
