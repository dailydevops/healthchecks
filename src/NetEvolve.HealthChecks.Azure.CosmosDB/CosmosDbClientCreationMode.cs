namespace NetEvolve.HealthChecks.Azure.CosmosDB;

/// <summary>
/// Represents the mode to create the CosmosDB client.
/// </summary>
public enum CosmosDbClientCreationMode
{
    /// <summary>
    /// Get the client from the service provider.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// Create the client with a connection string.
    /// </summary>
    ConnectionString = 1,

    /// <summary>
    /// Create the client with default Azure credentials.
    /// </summary>
    DefaultAzureCredentials = 2,

    /// <summary>
    /// Create the client with account endpoint and key.
    /// </summary>
    AccountKey = 3,
}
