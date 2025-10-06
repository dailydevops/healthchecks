namespace NetEvolve.HealthChecks.Azure.IotHub;

using System;
using Microsoft.Azure.Devices;

/// <summary>
/// Describes the mode to create or retrieve a <see cref="RegistryManager"/> or <see cref="ServiceClient"/>.
/// </summary>
public enum ClientCreationMode
{
    /// <summary>
    /// The default mode. The client is loading the preregistered instance from the <see cref="IServiceProvider"/>.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// Provides a default set of Azure Active Directory (AAD) credentials for authenticating with Azure services.
    /// </summary>
    DefaultAzureCredentials,

    /// <summary>
    /// Gets or sets the connection string used to establish a connection to the IoT Hub.
    /// </summary>
    ConnectionString,
}
