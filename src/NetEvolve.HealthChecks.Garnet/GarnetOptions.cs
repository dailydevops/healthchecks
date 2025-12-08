namespace NetEvolve.HealthChecks.Garnet;

/// <summary>
/// Represents the options for a Garnet database.
/// </summary>
public sealed record GarnetOptions
{
    /// <summary>
    /// Gets or sets the hostname of the Garnet Server.
    /// </summary>
    public string? Hostname { get; set; }

    /// <summary>
    /// Gets or sets the connection handle mode for the Garnet database.
    /// </summary>
    public ConnectionHandleMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the network port number used for the connection.
    /// </summary>
    /// <remarks>
    /// Valid port numbers range from 0 to 65535.
    /// The default value is typically determined by the protocol in use.
    /// </remarks>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for the Garnet database operations. Default value is 100.
    /// </summary>
    public int Timeout { get; set; } = 100;
}
