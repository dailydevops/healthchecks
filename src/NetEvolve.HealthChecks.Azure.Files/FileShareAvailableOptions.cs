namespace NetEvolve.HealthChecks.Azure.Files;

using System;
using global::Azure.Storage.Files.Shares;

/// <summary>
/// Options for the <see cref="FileShareAvailableHealthCheck"/>.
/// </summary>
public sealed record FileShareAvailableOptions : IFileOptions
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    public FileClientCreationMode? Mode { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against the service.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the name of the file share.
    /// </summary>
    public string? ShareName { get; set; }

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
    /// Gets or sets the lambda to configure the <see cref="ShareClientOptions"/>.
    /// </summary>
    public Action<ShareClientOptions>? ConfigureClientOptions { get; set; }
}
