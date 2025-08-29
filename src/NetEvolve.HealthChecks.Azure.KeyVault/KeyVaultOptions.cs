namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using global::Azure.Security.KeyVault.Secrets;

/// <summary>
/// Options for the <see cref="KeyVaultHealthCheck"/>.
/// </summary>
public sealed record KeyVaultOptions
{
    /// <summary>
    /// Gets or sets the mode to create the client. Default is <see cref="KeyVaultClientCreationMode.ServiceProvider"/>.
    /// </summary>
    public KeyVaultClientCreationMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets the Azure Key Vault URI.
    /// </summary>
    public Uri? VaultUri { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when connecting and executing tasks against Azure Key Vault. Default is 100 milliseconds.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the lambda to configure the <see cref="SecretClientOptions"/>.
    /// </summary>
    public Action<SecretClientOptions>? ConfigureClientOptions { get; set; }
}
