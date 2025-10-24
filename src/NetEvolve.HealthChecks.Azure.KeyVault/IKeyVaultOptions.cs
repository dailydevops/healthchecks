namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using global::Azure.Security.KeyVault.Secrets;

/// <summary>
/// Options for Key Vault health checks.
/// </summary>
public interface IKeyVaultOptions
{
    /// <summary>
    /// Gets or sets the mode for creating the Key Vault client.
    /// </summary>
    KeyVaultClientCreationMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the Key Vault URI.
    /// </summary>
    Uri? VaultUri { get; set; }

    /// <summary>
    /// Gets or sets the tenant ID for service principal authentication.
    /// </summary>
    string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the client ID for service principal or managed identity authentication.
    /// </summary>
    string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret for service principal authentication.
    /// </summary>
    string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets an optional action to configure the <see cref="SecretClientOptions"/>.
    /// </summary>
    Action<SecretClientOptions>? ConfigureClientOptions { get; set; }
}
