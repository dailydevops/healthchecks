namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using global::Azure.Security.KeyVault.Secrets;

internal interface IKeyVaultOptions
{
    /// <summary>
    /// Gets the mode to create the client.
    /// </summary>
    KeyVaultClientCreationMode? Mode { get; }

    /// <summary>
    /// Gets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    int Timeout { get; }

    /// <summary>
    /// Gets the vault URI.
    /// </summary>
    Uri? VaultUri { get; }

    /// <summary>
    /// Gets the lambda to configure the <see cref="SecretClientOptions"/>.
    /// </summary>
    Action<SecretClientOptions>? ConfigureClientOptions { get; }
}
