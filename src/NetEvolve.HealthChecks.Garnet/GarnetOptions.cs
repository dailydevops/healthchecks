namespace NetEvolve.HealthChecks.Garnet;

/// <summary>
/// Represents the options for a Garnet database.
/// </summary>
public sealed record GarnetOptions
{
    /// <summary>
    /// Gets or sets the connection string for the Garnet database.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the connection handle mode for the Garnet database.
    /// </summary>
    public ConnectionHandleMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for the Garnet database operations. Default value is 100.
    /// </summary>
    public int Timeout { get; set; } = 100;
}
