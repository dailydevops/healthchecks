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
        string? password
    )
    {
        var options = new KeycloakOptions
        {
            Mode = mode,
            BaseAddress = baseAddress,
            Username = username,
            Password = password,
        };
        _ = Assert.Throws(expectedException, () => KeycloakClientProvider.CreateClient(options));
    }

    public static IEnumerable<
        Func<(Type, KeycloakClientCreationMode, string?, string?, string?)>
    > InvalidArgumentsTestData()
    {
        yield return () =>
            (
                typeof(ArgumentOutOfRangeException),
                (KeycloakClientCreationMode)(-1),
                "base-address",
                "username",
                "password"
            );
        yield return () =>
            (
                typeof(ArgumentOutOfRangeException),
                KeycloakClientCreationMode.ServiceProvider,
                "base-address",
                "username",
                "password"
            );
        yield return () =>
            (
                typeof(ArgumentNullException),
                KeycloakClientCreationMode.UsernameAndPassword,
                null,
                "username",
                "password"
            );
        yield return () =>
            (
                typeof(ArgumentNullException),
                KeycloakClientCreationMode.UsernameAndPassword,
                "base-address",
                null,
                "password"
            );
        yield return () =>
            (
                typeof(ArgumentNullException),
                KeycloakClientCreationMode.UsernameAndPassword,
                "base-address",
                "username",
                null
            );
    }
}
