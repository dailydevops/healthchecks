namespace NetEvolve.HealthChecks.Minio;

using System.Threading.Tasks;
using global::Minio;
using global::Minio.DataModel.Args;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(MinioOptions))]
internal sealed partial class MinioHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        MinioOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IMinioClient>()
            : _serviceProvider.GetRequiredKeyedService<IMinioClient>(options.KeyedService);

        var commandTask = options.CommandAsync.Invoke(client, options.BucketName!, cancellationToken);

        var (isTimelyResponse, result) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "The Minio command did not return a valid result.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultCommandAsync(
        IMinioClient client,
        string bucketName,
        CancellationToken cancellationToken
    )
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
        return await client.BucketExistsAsync(bucketExistsArgs, cancellationToken).ConfigureAwait(false);
    }
}
