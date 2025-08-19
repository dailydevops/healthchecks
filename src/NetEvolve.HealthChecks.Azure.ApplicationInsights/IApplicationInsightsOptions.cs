namespace NetEvolve.HealthChecks.Azure.ApplicationInsights;

using System;
using Microsoft.ApplicationInsights.Extensibility;

/// <summary>
/// Interface for Application Insights options.
/// </summary>
public interface IApplicationInsightsOptions
{
    /// <summary>
    /// Gets or sets the connection string for Application Insights.
    /// </summary>
    string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the instrumentation key for Application Insights.
    /// </summary>
    string? InstrumentationKey { get; set; }

    /// <summary>
    /// Gets or sets the creation mode for the TelemetryClient.
    /// </summary>
    ApplicationInsightsClientCreationMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets the timeout for the health check operation.
    /// </summary>
    int Timeout { get; set; }

    /// <summary>
    /// Gets or sets the lambda to configure the <see cref="TelemetryConfiguration"/>.
    /// </summary>
    Action<TelemetryConfiguration>? ConfigureConfiguration { get; set; }
}
