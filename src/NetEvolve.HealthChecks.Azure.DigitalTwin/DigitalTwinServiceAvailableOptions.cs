namespace NetEvolve.HealthChecks.Azure.DigitalTwin;

using System;
using global::Azure.DigitalTwins.Core;

/// <summary>
/// Options for the <see cref="DigitalTwinServiceAvailableHealthCheck"/>.
/// </summary>
public sealed record DigitalTwinServiceAvailableOptions : IDigitalTwinOptions
{
    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    public DigitalTwinClientCreationMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the service uri.
    /// </summary>
    public Uri? ServiceUri { get; set; }

    /// <summary>
    /// Gets or sets the lambda to configure the <see cref="DigitalTwinsClientOptions"/>.
    /// </summary>
    public Action<DigitalTwinsClientOptions>? ConfigureClientOptions { get; set; }
}