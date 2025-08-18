namespace NetEvolve.HealthChecks.AWS.EC2;

/// <summary>
/// Specifies the creation mode for the AWS EC2 health check client.
/// </summary>
public enum CreationMode
{
    /// <summary>
    /// Use basic authentication for client creation.
    /// </summary>
    BasicAuthentication = 0,
}