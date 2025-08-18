namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using global::Azure.Security.KeyVault.Secrets;

/// <summary>
/// Describes the mode to create or retrieve a <see cref="SecretClient"/>.
/// </summary>
public enum KeyVaultClientCreationMode
{
    /// <summary>
    /// The default mode. The <see cref="SecretClient"/> is loading the preregistered instance from the <see cref="IServiceProvider"/>.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// Provides a default set of Azure Active Directory (AAD) credentials for authenticating with Azure services.
    /// </summary>
    DefaultAzureCredentials,

    /// <summary>
    /// Uses connection string authentication.
    /// </summary>
    ConnectionString,
}