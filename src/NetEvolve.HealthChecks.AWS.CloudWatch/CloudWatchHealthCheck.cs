namespace NetEvolve.HealthChecks.AWS.CloudWatch;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(CloudWatchOptions))]
internal sealed partial class CloudWatchHealthCheck
{
    private static async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        CloudWatchOptions options,
        CancellationToken cancellationToken
    )
    {
        using var client = CreateClient(options);

        var request = new DescribeAlarmsRequest { AlarmNames = [options.AlarmName] };

        var (isTimelyResponse, response) = await client
            .DescribeAlarmsAsync(request, cancellationToken)
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

        var metricAlarmFound =
            response.MetricAlarms?.Exists(x => string.Equals(x.AlarmName, options.AlarmName, StringComparison.Ordinal))
            ?? false;
        var compositeAlarmFound =
            response.CompositeAlarms?.Exists(x =>
                string.Equals(x.AlarmName, options.AlarmName, StringComparison.Ordinal)
            )
            ?? false;
        var found = metricAlarmFound || compositeAlarmFound;

        if (!found)
        {
            return HealthCheckUnhealthy(failureStatus, name, $"Alarm `{options.AlarmName}` not found.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    private static AmazonCloudWatchClient CreateClient(CloudWatchOptions options)
    {
        var config = new AmazonCloudWatchConfig { ServiceURL = options.ServiceUrl };

        var credentials = options.GetCredentials();

        return (credentials is not null, options.RegionEndpoint is not null) switch
        {
            (true, true) => new AmazonCloudWatchClient(credentials, options.RegionEndpoint),
            (true, false) => new AmazonCloudWatchClient(credentials, config),
            (false, true) => new AmazonCloudWatchClient(options.RegionEndpoint),
            _ => new AmazonCloudWatchClient(config),
        };
    }
}
