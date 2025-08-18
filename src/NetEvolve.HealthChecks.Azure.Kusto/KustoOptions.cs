namespace NetEvolve.HealthChecks.Azure.Kusto;

using System;
using Kusto.Data.Common;

/// <summary>
/// Options for the <see cref="KustoHealthCheck"/>.
/// </summary>
public sealed record KustoOptions : IKustoOptions
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the cluster URI.
    /// </summary>
    public Uri? ClusterUri { get; set; }

    /// <summary>
    /// Gets or sets the database name to check for availability.
    /// </summary>
    public string? DatabaseName { get; set; }

    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    public KustoClientCreationMode? Mode { get; set; }

    /// <summary>
    /// The timeout to use when connecting and executing tasks against Kusto.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the lambda to configure the <see cref="KustoConnectionStringBuilder"/>.
    /// </summary>
    public Action<KustoConnectionStringBuilder>? ConfigureConnectionStringBuilder { get; set; }
}
