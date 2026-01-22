namespace NetEvolve.HealthChecks.QuestDB;

/// <summary>
/// Represents configuration options for the QuestDB health check.
/// </summary>
/// <remarks>
/// This record provides configuration for the health check that verifies connectivity and proper functioning of a QuestDB vector database.
/// </remarks>
public sealed record QuestDBOptions
{
    /// <summary>
    /// The HTTP status endpoint URI to check, e.g. "http://localhost:9000/status".
    /// </summary>
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string StatusUri { get; set; } = default!;
#pragma warning restore CA1056 // URI-like properties should not be strings

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    /// <remarks>
    /// The minimum value is -1 (Timeout.Infinite). Default value is 100 milliseconds.
    /// </remarks>
    public int Timeout { get; set; } = 100;
}
