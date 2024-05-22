namespace NetEvolve.HealthChecks.Azure.Queues;

using System;
using global::Azure.Storage.Queues;

/// <summary>
/// Options for the <see cref="QueueClientAvailableHealthCheck"/>.
/// </summary>
public sealed class QueueClientAvailableOptions : IQueueOptions
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    public QueueClientCreationMode Mode { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against database.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the name of the container.
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
