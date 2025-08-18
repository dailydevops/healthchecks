namespace NetEvolve.HealthChecks.Azure.CosmosDB;

/// <summary>
/// Specifies the mode for creating a CosmosDB client.
/// </summary>
public enum CosmosDbClientCreationMode
{
    /// <summary>
    /// Use connection string to create the client.
    /// </summary>
    ConnectionString,

    /// <summary>
    /// Use service endpoint with default Azure credentials.
    /// </summary>
    DefaultAzureCredentials,

    /// <summary>
    /// Use service endpoint with account key authentication.
    /// </summary>
    AccountKey,

    /// <summary>
    /// Use service endpoint with Azure Active Directory (token credential).
    /// </summary>
    ServicePrincipal
}