namespace NetEvolve.HealthChecks.AWS.SNS;

/// <summary>
/// Specifies the creation mode for the AWS SNS health check client.
/// </summary>
public enum CreationMode
{
    /// <summary>
    /// Use basic authentication for client creation.
    /// </summary>
    BasicAuthentication = 0,
}
