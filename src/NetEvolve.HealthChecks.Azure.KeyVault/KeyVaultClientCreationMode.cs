namespace NetEvolve.HealthChecks.Azure.KeyVault;

/// <summary>
/// Represents the mode to create a <c>SecretClient</c>.
/// </summary>
public enum KeyVaultClientCreationMode
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
    /// Creates a new client with a <c>ClientSecretCredential</c>.
    /// </summary>
    ClientSecretCredential = 2,
}
