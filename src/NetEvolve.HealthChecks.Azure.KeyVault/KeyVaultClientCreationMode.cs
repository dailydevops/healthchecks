namespace NetEvolve.HealthChecks.Azure.KeyVault;

/// <summary>
/// Defines the mode for creating a Key Vault client.
/// </summary>
public enum KeyVaultClientCreationMode
{
    /// <summary>
    /// The client is created using the service provider. The client must be registered in the service provider.
    /// </summary>
    ServiceProvider,

    /// <summary>
    /// The client is created using the default Azure credentials.
    /// </summary>
    DefaultAzureCredentials,

    /// <summary>
    /// The client is created using a managed identity.
    /// </summary>
    ManagedIdentity,

    /// <summary>
    /// The client is created using a service principal.
    /// </summary>
    ServicePrincipal,
}
