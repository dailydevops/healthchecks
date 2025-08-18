namespace NetEvolve.HealthChecks.Azure.ApplicationInsights;

using System;
using Microsoft.ApplicationInsights.Extensibility;

/// <summary>
/// Options for the <see cref="ApplicationInsightsAvailabilityHealthCheck"/>.
/// </summary>
public sealed record ApplicationInsightsAvailabilityOptions : IApplicationInsightsOptions
{
    /// <summary>
    /// Gets or sets the connection string for Application Insights.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the instrumentation key for Application Insights.
    /// </summary>
    public string? InstrumentationKey { get; set; }

    /// <summary>
    /// Gets or sets the creation mode for the TelemetryClient.
    /// </summary>
    public ApplicationInsightsClientCreationMode? Mode { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against Application Insights.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the lambda to configure the <see cref="TelemetryConfiguration"/>.
    /// </summary>
    public Action<TelemetryConfiguration>? ConfigureConfiguration { get; set; }
}