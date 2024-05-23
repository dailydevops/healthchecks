namespace NetEvolve.HealthChecks.Redis;

/// <summary>
/// Specifies the mode of handling connections.
/// </summary>
public enum ConnectionHandleMode
{
    /// <summary>
    /// Specifies that the connection is handled by the service provider.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// Specifies that a new connection is created each time.
    /// </summary>
    Create
}
