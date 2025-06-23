namespace NetEvolve.HealthChecks.Tests.Unit.ArangoDb;

using System;
using System.Threading.Tasks;
using ArangoDBNetStandard;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.ArangoDb;

[TestGroup(nameof(ArangoDb))]
public sealed class ArangoDbConfigureTests
{
    [Test]
    public void Configue_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ArangoDbConfigure(new ConfigurationBuilder().Build(), services.BuildServiceProvider());
        var options = new ArangoDbOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        ArangoDbOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ArangoDbConfigure(new ConfigurationBuilder().Build(), services.BuildServiceProvider());

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsEqualTo(expectedResult);
            _ = await Assert.That(result.FailureMessage).IsEqualTo(expectedMessage);
        }
    }

    public static IEnumerable<Func<(bool, string?, string?, ArangoDbOptions)>> GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null!);
        yield return () => (false, "The name cannot be null or whitespace.", "\t", null!);
        yield return () => (false, "The options cannot be null.", "name", null!);
        yield return () =>
            (
                false,
                "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.",
                "name",
                new ArangoDbOptions { Timeout = -2 }
            );
        yield return () =>
            (
                false,
                "The mode `-1` is not supported.",
                "name",
                new ArangoDbOptions { Mode = (ArangoDbClientCreationMode)(-1) }
            );

        // Mode: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(ArangoDBClient)}` registered. Please execute `services.AddSingleton<ArangoDBClient>()`.",
                "name",
                new ArangoDbOptions { Mode = ArangoDbClientCreationMode.ServiceProvider }
            );

        // Mode: Internal
        yield return () =>
            (
                false,
                "The transport address cannot be null or whitespace when using the `Internal` client creation mode.",
                "name",
                new ArangoDbOptions { Mode = ArangoDbClientCreationMode.Internal }
            );
        yield return () =>
            (
                false,
                "The username cannot be null when using the `Internal` client creation mode with a password.",
                "name",
                new ArangoDbOptions
                {
                    Mode = ArangoDbClientCreationMode.Internal,
                    TransportAddress = "transport-address",
                    Username = null,
                    Password = "password",
                }
            );
        yield return () =>
            (
                false,
                "The password cannot be null when using the `Internal` client creation mode with a username.",
                "name",
                new ArangoDbOptions
                {
                    Mode = ArangoDbClientCreationMode.Internal,
                    TransportAddress = "transport-address",
                    Username = "username",
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new ArangoDbOptions
                {
                    Mode = ArangoDbClientCreationMode.Internal,
                    TransportAddress = "transport-address",
                    Username = "username",
                    Password = "password",
                }
            );
    }
}
