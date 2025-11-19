namespace NetEvolve.HealthChecks.Azure.KeyVault;

/// <summary>
/// Specifies the mode to create the Key Vault client.
/// </summary>
public enum KeyVaultClientCreationMode
{
    /// <summary>
    /// Create the client using the service provider.
    /// </summary>
    ServiceProvider,

    /// <summary>
    /// Create the client using the default Azure credentials.
    /// </summary>
    DefaultAzureCredentials,
}
