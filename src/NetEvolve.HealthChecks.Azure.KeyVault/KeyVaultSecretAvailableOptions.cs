namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using System.ComponentModel.DataAnnotations;
using global::Azure.Security.KeyVault.Secrets;

/// <summary>
/// Options for the Azure Key Vault Secret availability health check.
/// </summary>
public sealed record KeyVaultSecretAvailableOptions : IKeyVaultOptions
{
    /// <inheritdoc cref="IKeyVaultOptions.Mode"/>
    [Required]
    public KeyVaultClientCreationMode Mode { get; set; }

    /// <inheritdoc cref="IKeyVaultOptions.VaultUri"/>
    [Required]
    public Uri? VaultUri { get; set; }

    /// <inheritdoc cref="IKeyVaultOptions.TenantId"/>
    public string? TenantId { get; set; }

    /// <inheritdoc cref="IKeyVaultOptions.ClientId"/>
    public string? ClientId { get; set; }

    /// <inheritdoc cref="IKeyVaultOptions.ClientSecret"/>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <inheritdoc cref="IKeyVaultOptions.ConfigureClientOptions"/>
    public Action<SecretClientOptions>? ConfigureClientOptions { get; set; }
}
