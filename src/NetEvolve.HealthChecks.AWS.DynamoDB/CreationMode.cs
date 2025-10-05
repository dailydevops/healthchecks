namespace NetEvolve.HealthChecks.AWS.DynamoDB;

/// <summary>
/// Specifies the creation mode for the AWS DynamoDB health check client.
/// </summary>
public enum CreationMode
{
    /// <summary>
    /// Use basic authentication for client creation.
    /// </summary>
    BasicAuthentication = 0,
}
