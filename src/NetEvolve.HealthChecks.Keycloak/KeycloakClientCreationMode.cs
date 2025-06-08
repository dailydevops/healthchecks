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
    Internal = 1,
}
