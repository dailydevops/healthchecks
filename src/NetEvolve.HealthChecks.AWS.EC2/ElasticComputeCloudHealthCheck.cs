namespace NetEvolve.HealthChecks.AWS.EC2;

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.EC2;
using Amazon.EC2.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(ElasticComputeCloudOptions))]
internal sealed partial class ElasticComputeCloudHealthCheck
{
    private static async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        ElasticComputeCloudOptions options,
        CancellationToken cancellationToken
    )
    {
        using var client = CreateClient(options);

        var request = new DescribeKeyPairsRequest { KeyNames = [options.KeyName] };

        var (isTimelyResponse, response) = await client
            .DescribeKeyPairsAsync(request, cancellationToken)
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

        if (response.KeyPairs.Count == 0)
        {
            return HealthCheckUnhealthy(failureStatus, name, $"No key pair with name '{options.KeyName}' found.");
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
