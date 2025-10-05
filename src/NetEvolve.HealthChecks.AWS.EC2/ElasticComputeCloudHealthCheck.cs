namespace NetEvolve.HealthChecks.AWS.EC2;

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.EC2;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class ElasticComputeCloudHealthCheck(IOptionsMonitor<ElasticComputeCloudOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<ElasticComputeCloudOptions>(optionsMonitor)
{
    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        ElasticComputeCloudOptions options,
        CancellationToken cancellationToken
    )
    {
        using var client = CreateClient(options);

        var (isTimelyResponse, response) = await client.
            DescribeInstanceStatusAsync(cancellationToken)
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

        if (response.InstanceStatuses is null)
        {
            return HealthCheckUnhealthy(failureStatus, name, "No instance statuses returned.");
        }

        var isHealthy = response.InstanceStatuses.Any(x => x.InstanceState.Code == 16);
        if (!isHealthy)
        {
            return HealthCheckUnhealthy(failureStatus, name, "No running instances found.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    private static AmazonEC2Client CreateClient(ElasticComputeCloudOptions options)
    {
        var config = new AmazonEC2Config { ServiceURL = options.ServiceUrl };

        var credentials = options.GetCredentials();

        return (credentials is not null, options.RegionEndpoint is not null) switch
        {
            (true, true) => new AmazonEC2Client(credentials, options.RegionEndpoint),
            (true, false) => new AmazonEC2Client(credentials, config),
            (false, true) => new AmazonEC2Client(options.RegionEndpoint),
            _ => new AmazonEC2Client(config),
        };
    }
}
