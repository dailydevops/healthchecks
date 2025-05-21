namespace NetEvolve.HealthChecks.Redis;

/// <summary>
/// Represents the options for a Redis database.
/// </summary>
public sealed record RedisDatabaseOptions
{
    /// <summary>
    /// Gets or sets the connection string for the Redis database.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the connection handle mode for the Redis database.
    /// </summary>
    public ConnectionHandleMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for the Redis database operations. Default value is 100.
    /// </summary>
    public int Timeout { get; set; } = 100;
}
