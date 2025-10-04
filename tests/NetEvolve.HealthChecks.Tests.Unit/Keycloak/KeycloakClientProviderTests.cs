namespace NetEvolve.HealthChecks.Tests.Unit.Keycloak;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Keycloak;

[TestGroup(nameof(Keycloak))]
public sealed class KeycloakClientProviderTests
{
    [Test]
    [MethodDataSource(nameof(InvalidArgumentsTestData))]
    public void CreateClient_Theory_Expected(
        Type expectedException,
        KeycloakClientCreationMode mode,
        string? baseAddress,
        string? username,
        string? password,
        string? clientSecret
    )
    {
        var options = new KeycloakOptions
        {
            Mode = mode,
            BaseAddress = baseAddress,
            Username = username,
            Password = password,
            ClientSecret = clientSecret,
        };
        _ = Assert.Throws(expectedException, () => KeycloakClientProvider.CreateClient(options));
    }

    public static IEnumerable<
        Func<(Type, KeycloakClientCreationMode, string?, string?, string?, string?)>
    > InvalidArgumentsTestData()
    {
        yield return () =>
            (
                typeof(ArgumentOutOfRangeException),
                (KeycloakClientCreationMode)(-1),
                "base-address",
                "username",
                "password",
                null
            );
        yield return () =>
            (
                typeof(ArgumentOutOfRangeException),
                KeycloakClientCreationMode.ServiceProvider,
                "base-address",
                "username",
                "password",
                null
            );
        yield return () =>
            (
                typeof(ArgumentException),
                KeycloakClientCreationMode.UsernameAndPassword,
                null,
                "username",
                "password",
                null
            );
        yield return () =>
            (
                typeof(ArgumentNullException),
                KeycloakClientCreationMode.UsernameAndPassword,
                "base-address",
                null,
                "password",
                null
            );
        yield return () =>
            (
                typeof(ArgumentNullException),
                KeycloakClientCreationMode.UsernameAndPassword,
                "base-address",
                "username",
                null,
                null
            );
        yield return () =>
            (
                typeof(ArgumentException),
                KeycloakClientCreationMode.ClientSecret,
                null,
                null,
                null,
                "client-secret"
            );
        yield return () =>
            (
                typeof(ArgumentNullException),
                KeycloakClientCreationMode.ClientSecret,
                "base-address",
                null,
                null,
                null
            );
    }
}
