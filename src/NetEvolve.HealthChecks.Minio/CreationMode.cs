namespace NetEvolve.HealthChecks.Minio;

/// <summary>
/// Specifies the creation mode for the Minio health check client.
/// </summary>
public enum CreationMode
{
    /// <summary>
    /// Use basic authentication for client creation.
    /// </summary>
    BasicAuthentication = 0,
}
