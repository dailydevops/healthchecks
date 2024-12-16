namespace NetEvolve.HealthChecks.Azure.Blobs;

using System;
using global::Azure.Storage.Blobs;

/// <summary>
/// Options for the <see cref="BlobContainerAvailableHealthCheck"/>.
/// </summary>
public sealed class BlobContainerAvailableOptions : IBlobOptions
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    public BlobClientCreationMode Mode { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against database.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the name of the container.
    /// </summary>
    public string? ContainerName { get; set; }

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
    /// Gets or sets the lambda to configure the <see cref="BlobClientOptions"/>.
    /// </summary>
    public Action<BlobClientOptions>? ConfigureClientOptions { get; set; }
}
