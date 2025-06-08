namespace NetEvolve.HealthChecks.Keycloak;

using global::Keycloak.Net;

/// <summary>
/// Options for <see cref="KeycloakHealthCheck"/>
/// </summary>
public sealed record KeycloakOptions
{
    /// <summary>
    /// Gets or sets the mode used to create a client instance.
    /// </summary>
    public KeycloakClientCreationMode Mode { get; set; } = KeycloakClientCreationMode.ServiceProvider;

    /// <summary>
    /// Gets or sets the key used to resolve the <see cref="KeycloakClient"/> from the service provider.
    /// </summary>
    /// <remarks>
    /// When specified, the health check will resolve the <see cref="KeycloakClient"/> using <c>IServiceProvider.GetRequiredKeyedService</c>.
    /// <br/>
    /// When <see langword="null"/> or <see langword="empty"/>, the health check will resolve the <see cref="KeycloakClient"/> using <c>IServiceProvider.GetRequiredService</c>.
    /// </remarks>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Gets or sets the address to the Keycloak instance to check.
    /// </summary>
    public string? BaseAddress { get; set; }

    /// <summary>
    /// Gets or sets the username for authenticating with the client.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for authenticating with the client.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the timeout to use when connecting and executing tasks against the service.
    /// </summary>
    public int Timeout { get; set; } = 100;

    /// <summary>
    /// Gets or sets the command to execute against the service.
    /// Returns <see langword="true"/> if successful, <see langword="false"/> otherwise.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    public Func<KeycloakClient, CancellationToken, Task<bool>> CommandAsync { get; set; } =
        KeycloakHealthCheck.DefaultCommandAsync;
}
