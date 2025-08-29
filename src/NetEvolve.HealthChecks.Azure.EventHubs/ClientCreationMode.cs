namespace NetEvolve.HealthChecks.Azure.EventHubs;

using System;
using global::Azure.Messaging.EventHubs;

/// <summary>
/// Describes the mode to create or retrieve a <see cref="EventHubProducerClient"/>.
/// </summary>
public enum ClientCreationMode
{
    /// <summary>
    /// The default mode. The <see cref="EventHubProducerClient"/> is loading the preregistered instance from the <see cref="IServiceProvider"/>.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// Provides a default set of Azure Active Directory (AAD) credentials for authenticating with Azure services.
    /// </summary>
    DefaultAzureCredentials,

    /// <summary>
    /// Gets or sets the connection string used to establish a connection to the Event Hub.
    /// </summary>
    ConnectionString,
}