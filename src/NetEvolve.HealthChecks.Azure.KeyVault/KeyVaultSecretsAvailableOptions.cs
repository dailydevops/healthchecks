namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using global::Azure.Security.KeyVault.Secrets;

/// <summary>
/// Options for the <see cref="KeyVaultSecretsAvailableHealthCheck"/>.
/// </summary>
public sealed record KeyVaultSecretsAvailableOptions : IKeyVaultOptions
{
    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    public KeyVaultClientCreationMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the vault URI.
    /// </summary>
    public Uri? VaultUri { get; set; }

    /// <summary>
    /// Gets or sets the lambda to configure the <see cref="SecretClientOptions"/>.
    /// </summary>
    public Action<SecretClientOptions>? ConfigureClientOptions { get; set; }
}
