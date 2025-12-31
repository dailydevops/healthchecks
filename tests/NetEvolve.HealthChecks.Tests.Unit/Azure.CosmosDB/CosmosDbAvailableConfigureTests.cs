namespace NetEvolve.HealthChecks.Tests.Unit.Azure.CosmosDB;

using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.CosmosDB;

[TestGroup($"{nameof(Azure)}.{nameof(CosmosDB)}")]
public sealed class CosmosDbAvailableConfigureTests
{
    [Test]
    public void Configure_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new CosmosDbAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new CosmosDbAvailableOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        CosmosDbAvailableOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new CosmosDbAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsEqualTo(expectedResult);
            _ = await Assert.That(result.FailureMessage).IsEqualTo(expectedMessage);
        }
    }

    public static IEnumerable<Func<(bool, string?, string?, CosmosDbAvailableOptions)>> GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null!);
        yield return () => (false, "The name cannot be null or whitespace.", "\t", null!);
        yield return () => (false, "The option cannot be null.", "name", null!);
        yield return () =>
            (
                false,
                "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.",
                "name",
                new CosmosDbAvailableOptions { Timeout = -2 }
            );
        yield return () =>
            (
                false,
                "The mode `13` is not supported.",
                "name",
                new CosmosDbAvailableOptions { Mode = (CosmosDbClientCreationMode)13 }
            );

        // Mode: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(CosmosClient)}` registered. Please register the CosmosClient.",
                "name",
                new CosmosDbAvailableOptions { Mode = CosmosDbClientCreationMode.ServiceProvider }
            );

        // Mode: DefaultAzureCredentials
        yield return () =>
            (
                false,
                "The account endpoint cannot be null when using `DefaultAzureCredentials` mode.",
                "name",
                new CosmosDbAvailableOptions { Mode = CosmosDbClientCreationMode.DefaultAzureCredentials }
            );
        yield return () =>
            (
                false,
                "The account endpoint must be an absolute url when using `DefaultAzureCredentials` mode.",
                "name",
                new CosmosDbAvailableOptions
                {
                    Mode = CosmosDbClientCreationMode.DefaultAzureCredentials,
                    AccountEndpoint = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new CosmosDbAvailableOptions
                {
                    Mode = CosmosDbClientCreationMode.DefaultAzureCredentials,
                    AccountEndpoint = new Uri("https://localhost", UriKind.Absolute),
                }
            );

        // Mode: ConnectionString
        yield return () =>
            (
                false,
                "The connection string cannot be null or whitespace when using `ConnectionString` mode.",
                "name",
                new CosmosDbAvailableOptions { Mode = CosmosDbClientCreationMode.ConnectionString }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new CosmosDbAvailableOptions
                {
                    Mode = CosmosDbClientCreationMode.ConnectionString,
                    ConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=test",
                }
            );

        // Mode: AccountKey
        yield return () =>
            (
                false,
                "The account endpoint cannot be null when using `AccountKey` mode.",
                "name",
                new CosmosDbAvailableOptions { Mode = CosmosDbClientCreationMode.AccountKey }
            );
        yield return () =>
            (
                false,
                "The account endpoint must be an absolute url when using `AccountKey` mode.",
                "name",
                new CosmosDbAvailableOptions
                {
                    Mode = CosmosDbClientCreationMode.AccountKey,
                    AccountEndpoint = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                false,
                "The account key cannot be null or whitespace when using `AccountKey` mode.",
                "name",
                new CosmosDbAvailableOptions
                {
                    Mode = CosmosDbClientCreationMode.AccountKey,
                    AccountEndpoint = new Uri("https://localhost", UriKind.Absolute),
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new CosmosDbAvailableOptions
                {
                    Mode = CosmosDbClientCreationMode.AccountKey,
                    AccountEndpoint = new Uri("https://localhost", UriKind.Absolute),
                    AccountKey = "test",
                }
            );
    }
}
