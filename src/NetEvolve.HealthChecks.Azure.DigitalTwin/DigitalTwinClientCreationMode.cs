namespace NetEvolve.HealthChecks.Azure.DigitalTwin;

using System;
using global::Azure.DigitalTwins.Core;
using global::Azure.Identity;

/// <summary>
/// Describes the mode used to create the <see cref="DigitalTwinsClient"/>.
/// </summary>
public enum DigitalTwinClientCreationMode
{
    /// <summary>
    /// The default mode. The <see cref="DigitalTwinsClient"/> is loading the preregistered instance from the <see cref="IServiceProvider"/>.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// The <see cref="DigitalTwinsClient"/> is created using the <see cref="DefaultAzureCredential"/>.
    /// </summary>
    DefaultAzureCredentials = 1,
}