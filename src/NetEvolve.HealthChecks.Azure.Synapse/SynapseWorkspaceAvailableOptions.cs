namespace NetEvolve.HealthChecks.Azure.Synapse;

using System;

/// <summary>
/// Options for the <see cref="SynapseWorkspaceAvailableHealthCheck"/>.
/// </summary>
public sealed record SynapseWorkspaceAvailableOptions : ISynapseOptions
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    public SynapseClientCreationMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the workspace uri.
    /// </summary>
    public Uri? WorkspaceUri { get; set; }
}