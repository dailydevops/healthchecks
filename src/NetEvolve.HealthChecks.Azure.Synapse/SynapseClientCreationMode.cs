namespace NetEvolve.HealthChecks.Azure.Synapse;

using System;
using global::Azure.Analytics.Synapse.Artifacts;
using global::Azure.Identity;

/// <summary>
/// Describes the mode used to create the <see cref="ArtifactsClient"/>.
/// </summary>
public enum SynapseClientCreationMode
{
    /// <summary>
    /// The default mode. The <see cref="ArtifactsClient"/> is loading the preregistered instance from the <see cref="IServiceProvider"/>.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// The <see cref="ArtifactsClient"/> is created using the <see cref="DefaultAzureCredential"/>.
    /// </summary>
    DefaultAzureCredentials = 1,

    /// <summary>
    /// The <see cref="ArtifactsClient"/> is created using the connection string.
    /// </summary>
    ConnectionString = 2,
}