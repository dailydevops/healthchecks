namespace NetEvolve.HealthChecks.AWS.S3;

/// <summary>
/// Specifies the creation mode for the AWS S3 health check client.
/// </summary>
public enum CreationMode
{
    /// <summary>
    /// Use basic authentication for client creation.
    /// </summary>
    BasicAuthentication = 0,
}
