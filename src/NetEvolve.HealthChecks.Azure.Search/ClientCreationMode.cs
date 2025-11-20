namespace NetEvolve.HealthChecks.Azure.Search;

/// <summary>
/// Represents the mode to create a <c>SearchIndexClient</c> or <c>SearchClient</c>.
/// </summary>
public enum ClientCreationMode
{
    /// <summary>
    /// Retrieves the client from the <c>IServiceProvider</c>.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// Creates a new client with the <c>DefaultAzureCredential</c>.
    /// </summary>
    DefaultAzureCredentials = 1,

    /// <summary>
    /// Creates a new client with a connection string / endpoint and api key.
    /// </summary>
    AzureKeyCredential = 2,
}
