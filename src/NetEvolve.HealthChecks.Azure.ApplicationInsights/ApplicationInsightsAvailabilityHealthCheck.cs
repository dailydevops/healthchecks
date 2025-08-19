namespace NetEvolve.HealthChecks.Azure.ApplicationInsights;

using System;
using System.Collections.Generic;
using System.Globalization;
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
