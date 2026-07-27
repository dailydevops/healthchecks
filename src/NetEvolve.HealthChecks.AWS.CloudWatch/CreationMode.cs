namespace NetEvolve.HealthChecks.AWS.CloudWatch;

/// <summary>
/// Specifies the creation mode for the AWS CloudWatch health check client.
/// </summary>
public enum CreationMode
{
    /// <summary>
    /// Use basic authentication for client creation.
    /// </summary>
    BasicAuthentication = 0,
}
