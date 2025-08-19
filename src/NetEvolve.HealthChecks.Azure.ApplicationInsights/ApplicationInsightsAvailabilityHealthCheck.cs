namespace NetEvolve.HealthChecks.Azure.ApplicationInsights;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class ApplicationInsightsAvailabilityHealthCheck
    : ConfigurableHealthCheckBase<ApplicationInsightsAvailabilityOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public ApplicationInsightsAvailabilityHealthCheck(
        IOptionsMonitor<ApplicationInsightsAvailabilityOptions> optionsMonitor,
        IServiceProvider serviceProvider
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        ApplicationInsightsAvailabilityOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var telemetryClient = clientCreation.GetTelemetryClient(name, options, _serviceProvider);

        // Create a custom event to test connectivity
        var properties = new Dictionary<string, string>
        {
            { "HealthCheckName", name },
            { "Timestamp", DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
        };

        var customEvent = new EventTelemetry("HealthCheck") { Properties = { { "source", "NetEvolve.HealthChecks" } } };

        foreach (var property in properties)
        {
            customEvent.Properties[property.Key] = property.Value;
        }

        try
        {
            // Test the telemetry client by tracking an event
            var (isSuccessful, _) = await Task.Run(
                    () =>
                    {
                        telemetryClient.TrackEvent(customEvent);
                        telemetryClient.Flush();
                        return true;
                    },
                    cancellationToken
                )
                .WithTimeoutAsync(options.Timeout, cancellationToken)
                .ConfigureAwait(false);

            return HealthCheckState(isSuccessful, name);
        }
        catch (Exception)
        {
            return HealthCheckState(false, name);
        }
    }
}
