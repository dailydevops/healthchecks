namespace NetEvolve.HealthChecks.Tests.Unit.Keycloak;

using System;
using global::Keycloak.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Keycloak;

[TestGroup(nameof(Keycloak))]
public sealed class KeycloakConfigureTests
{
    [Test]
    public void Configue_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new KeycloakConfigure(new ConfigurationBuilder().Build(), services.BuildServiceProvider());
        var options = new NetEvolve.HealthChecks.Keycloak.KeycloakOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        NetEvolve.HealthChecks.Keycloak.KeycloakOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new KeycloakConfigure(new ConfigurationBuilder().Build(), services.BuildServiceProvider());

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsEqualTo(expectedResult);
            _ = await Assert.That(result.FailureMessage).IsEqualTo(expectedMessage);
        }
    }

    public static IEnumerable<
        Func<(bool, string?, string?, NetEvolve.HealthChecks.Keycloak.KeycloakOptions)>
    > GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null!);
        yield return () => (false, "The name cannot be null or whitespace.", "\t", null!);
        yield return () => (false, "The options cannot be null.", "name", null!);
        yield return () =>
            (
                false,
                "The timeout cannot be less than infinite (-1).",
                "name",
                new NetEvolve.HealthChecks.Keycloak.KeycloakOptions { Timeout = -2 }
            );
        yield return () =>
            (
                false,
                "The mode `-1` is not supported.",
                "name",
                new NetEvolve.HealthChecks.Keycloak.KeycloakOptions { Mode = (KeycloakClientCreationMode)(-1) }
            );

        // Mode: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(KeycloakClient)}` registered. Please execute `services.AddSingleton(<client instance>)`.",
                "name",
                new NetEvolve.HealthChecks.Keycloak.KeycloakOptions
                {
                    Mode = KeycloakClientCreationMode.ServiceProvider,
                }
            );

        // Mode: UsernameAndPassword
        yield return () =>
            (
                false,
                "The base address cannot be null or whitespace when using the `UsernameAndPassword` client creation mode.",
                "name",
                new NetEvolve.HealthChecks.Keycloak.KeycloakOptions
                {
                    Mode = KeycloakClientCreationMode.UsernameAndPassword,
                }
            );
        yield return () =>
            (
                false,
                "The username cannot be null when using the `UsernameAndPassword` client creation mode.",
                "name",
                new NetEvolve.HealthChecks.Keycloak.KeycloakOptions
                {
                    Mode = KeycloakClientCreationMode.UsernameAndPassword,
                    BaseAddress = "base-address",
                }
            );
        yield return () =>
            (
                false,
                "The password cannot be null when using the `UsernameAndPassword` client creation mode.",
                "name",
                new NetEvolve.HealthChecks.Keycloak.KeycloakOptions
                {
                    Mode = KeycloakClientCreationMode.UsernameAndPassword,
                    BaseAddress = "base-address",
                    Username = "username",
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new NetEvolve.HealthChecks.Keycloak.KeycloakOptions
                {
                    Mode = KeycloakClientCreationMode.UsernameAndPassword,
                    BaseAddress = "base-address",
                    Username = "username",
                    Password = "password",
                }
            );
    }
}
