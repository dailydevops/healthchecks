namespace NetEvolve.HealthChecks.Azure.Queues;

using System;
using global::Azure.Identity;
using global::Azure.Storage.Queues;

/// <summary>
/// Describes the mode used to create the <see cref="QueueServiceClient"/>.
/// </summary>
public enum QueueClientCreationMode
{
    /// <summary>
    /// The default mode. The <see cref="QueueServiceClient"/> is loading the preregistered instance from the <see cref="IServiceProvider"/>.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// The <see cref="QueueServiceClient"/> is created using the <see cref="DefaultAzureCredential"/>.
    /// </summary>
    DefaultAzureCredentials = 1,

    /// <summary>
    /// The <see cref="QueueServiceClient"/> is created using the <see cref="QueueClientAvailableOptions.ConnectionString"/>.
    /// </summary>
    ConnectionString = 2,

    /// <summary>
    /// The <see cref="QueueServiceClient"/> is created using the <see cref="QueueClientAvailableOptions.AccountName"/>
    /// and <see cref="QueueClientAvailableOptions.AccountKey"/>. As well as the <see cref="QueueClientAvailableOptions.ServiceUri"/>.
    /// </summary>
    SharedKey = 3,

    /// <summary>
    /// The <see cref="QueueServiceClient"/> is created using the <see cref="QueueClientAvailableOptions.ServiceUri"/>.
    /// </summary>
    AzureSasCredential = 4,
}
