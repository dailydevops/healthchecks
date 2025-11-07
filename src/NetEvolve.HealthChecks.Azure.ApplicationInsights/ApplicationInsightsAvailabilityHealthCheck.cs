namespace NetEvolve.HealthChecks.Azure.ApplicationInsights;

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(ApplicationInsightsAvailabilityOptions))]
internal sealed partial class ApplicationInsightsAvailabilityHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        ApplicationInsightsAvailabilityOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var telemetryClient = clientCreation.GetTelemetryClient(name, options, _serviceProvider);

        // Create a custom event to test connectivity
        var customEvent = new EventTelemetry("HealthCheck") { Properties = { { "source", "NetEvolve.HealthChecks" } } };
        customEvent.Properties.Add("HealthCheckName", name);
        customEvent.Properties.Add(
            "Timestamp",
            DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)
        );

        // Test the telemetry client by tracking an event
        telemetryClient.TrackEvent(customEvent);
        var (isTimelyResponse, _) = await telemetryClient
            .FlushAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }
}
