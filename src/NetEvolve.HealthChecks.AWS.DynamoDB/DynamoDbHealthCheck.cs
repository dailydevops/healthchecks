namespace NetEvolve.HealthChecks.AWS.DynamoDB;

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(DynamoDbOptions))]
internal sealed partial class DynamoDbHealthCheck
{
    private static async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        DynamoDbOptions options,
        CancellationToken cancellationToken
    )
    {
        using var client = CreateClient(options);

        var (isTimelyResponse, response) = await client
            .DescribeTableAsync(new DescribeTableRequest { TableName = options.TableName }, cancellationToken)
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

    private static AmazonDynamoDBClient CreateClient(DynamoDbOptions options)
    {
        var config = new AmazonDynamoDBConfig { ServiceURL = options.ServiceUrl };

        var credentials = options.GetCredentials();

        return (credentials is not null, options.RegionEndpoint is not null) switch
        {
            (true, true) => new AmazonDynamoDBClient(credentials, options.RegionEndpoint),
            (true, false) => new AmazonDynamoDBClient(credentials, config),
            (false, true) => new AmazonDynamoDBClient(options.RegionEndpoint),
            _ => new AmazonDynamoDBClient(config),
        };
    }
}
