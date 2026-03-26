namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using global::Azure.Security.KeyVault.Secrets;

/// <summary>
/// Options for the <see cref="KeyVaultSecretAvailableHealthCheck"/>.
/// </summary>
public sealed record KeyVaultSecretAvailableOptions
{
    /// <summary>
    /// Gets or sets the key used to resolve the <c>SecretClient</c> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <c>SecretClient</c> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// When null or empty, the health check will resolve the <c>SecretClient</c> using <c>IServiceProvider.GetRequiredService</c>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Gets or sets the vault URI.
    /// </summary>
    public Uri? VaultUri { get; set; }

    /// <summary>
    /// Gets or sets the tenant ID for <c>ClientSecretCredential</c> mode.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the client ID for <c>ClientSecretCredential</c> mode.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret for <c>ClientSecretCredential</c> mode.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the mode to create the client.
    /// </summary>
    public KeyVaultClientCreationMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for executing the healthcheck.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the lambda to configure the <see cref="SecretClientOptions"/>.
    /// </summary>
    public Action<SecretClientOptions>? ConfigureClientOptions { get; set; }
}
