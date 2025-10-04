namespace NetEvolve.HealthChecks.Keycloak;

using System;
using global::Keycloak.Net;

/// <summary>
/// Describes the mode used to create the <see cref="KeycloakClient"/>.
/// </summary>
public enum KeycloakClientCreationMode
{
    /// <summary>
    /// The <see cref="KeycloakClient"/> preregistered instance is retrieved from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <remarks>
    /// This is the default mode.
    /// </remarks>
    ServiceProvider = 0,

    /// <summary>
    /// The <see cref="KeycloakClient"/> instance is created using the <see cref="KeycloakOptions.BaseAddress"/>,
    /// the <see cref="KeycloakOptions.Username"/> and the <see cref="KeycloakOptions.Password"/>.
    /// </summary>
    UsernameAndPassword = 1,

    /// <summary>
    /// The <see cref="KeycloakClient"/> instance is created using the <see cref="KeycloakOptions.BaseAddress"/>
    /// and the <see cref="KeycloakOptions.ClientSecret"/>.
    /// </summary>
    ClientSecret = 2,

    /// <inheritdoc cref="UsernameAndPassword" />
    [Obsolete($"Use `{nameof(UsernameAndPassword)}` instead. This value will be removed in a future version.")]
    Internal = UsernameAndPassword,
}
