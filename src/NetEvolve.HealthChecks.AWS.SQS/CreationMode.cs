namespace NetEvolve.HealthChecks.AWS.SQS;

/// <summary>
/// Specifies the creation mode for the AWS SQS health check client.
/// </summary>
public enum CreationMode
{
    /// <summary>
    /// Use basic authentication for client creation.
    /// </summary>
    BasicAuthentication = 0,
}
