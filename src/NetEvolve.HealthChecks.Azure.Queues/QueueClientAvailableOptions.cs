namespace NetEvolve.HealthChecks.Azure.Queues;

using System;
using global::Azure.Storage.Queues;

/// <summary>
/// Options for the <see cref="QueueClientAvailableHealthCheck"/>.
/// </summary>
public sealed record QueueClientAvailableOptions : IQueueOptions
{
    /// <summary>
    /// Gets or sets the key used to resolve the <c>QueueServiceClient</c> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <c>QueueServiceClient</c> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// When null or empty, the health check will resolve the <c>QueueServiceClient</c> using <c>IServiceProvider.GetRequiredService</c>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    public QueueClientCreationMode? Mode { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against database.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the name of the queue.
    /// </summary>
    public string? QueueName { get; set; }

    /// <summary>
    /// Gets or sets the service uri.
    /// </summary>
    public Uri? ServiceUri { get; set; }

    /// <summary>
    /// Gets or sets the account name.
    /// </summary>
    public string? AccountName { get; set; }

    /// <summary>
    /// Gets or sets the account key.
    /// </summary>
    public string? AccountKey { get; set; }

    /// <summary>
    /// Gets or sets the lambda to configure the <see cref="QueueClientOptions"/>.
    /// </summary>
    public Action<QueueClientOptions>? ConfigureClientOptions { get; set; }
}
