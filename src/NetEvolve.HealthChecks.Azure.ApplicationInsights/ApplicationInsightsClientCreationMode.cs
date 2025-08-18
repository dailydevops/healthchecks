namespace NetEvolve.HealthChecks.Azure.ApplicationInsights;

/// <summary>
/// Represents the different modes for creating an Application Insights TelemetryClient.
/// </summary>
public enum ApplicationInsightsClientCreationMode
{
    /// <summary>
    /// Create client using connection string.
    /// </summary>
    ConnectionString,

    /// <summary>
    /// Create client using instrumentation key.
    /// </summary>
    InstrumentationKey,

    /// <summary>
    /// Use TelemetryClient from service provider.
    /// </summary>
    ServiceProvider,
}