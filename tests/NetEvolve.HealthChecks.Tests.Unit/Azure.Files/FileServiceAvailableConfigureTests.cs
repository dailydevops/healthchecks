namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Files;

using System;
using System.Collections.Generic;
using global::Azure.Storage.Files.Shares;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Files;

[TestGroup($"{nameof(Azure)}.{nameof(Files)}")]
public sealed class FileServiceAvailableConfigureTests
{
    [Test]
    public void Configue_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new FileServiceAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new FileServiceAvailableOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        FileServiceAvailableOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new FileServiceAvailableConfigure(
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

    public static IEnumerable<Func<(bool, string?, string?, FileServiceAvailableOptions)>> GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null!);
        yield return () => (false, "The option cannot be null.", "name", null!);
        yield return () =>
            (
                false,
                "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.",
                "name",
                new FileServiceAvailableOptions { Timeout = -2 }
            );
        yield return () =>
            (
                false,
                "The mode `13` is not supported.",
                "name",
                new FileServiceAvailableOptions { Mode = (FileClientCreationMode)13 }
            );

        // Mode: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(ShareServiceClient)}` registered. Please execute `builder.AddAzureClients()`.",
                "name",
                new FileServiceAvailableOptions { Mode = FileClientCreationMode.ServiceProvider }
            );

        // Mode: DefaultAzureCredentials
        yield return () =>
            (
                false,
                "The service url cannot be null when using `DefaultAzureCredentials` mode.",
                "name",
                new FileServiceAvailableOptions { Mode = FileClientCreationMode.DefaultAzureCredentials }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `DefaultAzureCredentials` mode.",
                "name",
                new FileServiceAvailableOptions
                {
                    Mode = FileClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new FileServiceAvailableOptions
                {
                    Mode = FileClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                }
            );

        // Mode: ConnectionString
        yield return () =>
            (
                false,
                "The connection string cannot be null or whitespace when using `ConnectionString` mode.",
                "name",
                new FileServiceAvailableOptions { Mode = FileClientCreationMode.ConnectionString }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new FileServiceAvailableOptions
                {
                    Mode = FileClientCreationMode.ConnectionString,
                    ConnectionString = "connectionString",
                }
            );

        // Mode: SharedKey
        yield return () =>
            (
                false,
                "The service url cannot be null when using `SharedKey` mode.",
                "name",
                new FileServiceAvailableOptions { Mode = FileClientCreationMode.SharedKey }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `SharedKey` mode.",
                "name",
                new FileServiceAvailableOptions
                {
                    Mode = FileClientCreationMode.SharedKey,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                false,
                "The account name cannot be null or whitespace when using `SharedKey` mode.",
                "name",
                new FileServiceAvailableOptions
                {
                    Mode = FileClientCreationMode.SharedKey,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                }
            );
        yield return () =>
            (
                false,
                "The account key cannot be null or whitespace when using `SharedKey` mode.",
                "name",
                new FileServiceAvailableOptions
                {
                    Mode = FileClientCreationMode.SharedKey,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                    AccountName = "test",
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new FileServiceAvailableOptions
                {
                    Mode = FileClientCreationMode.SharedKey,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                    AccountName = "test",
                    AccountKey = "test",
                }
            );

        // Mode: AzureSasCredential
        yield return () =>
            (
                false,
                "The service url cannot be null when using `AzureSasCredential` mode.",
                "name",
                new FileServiceAvailableOptions { Mode = FileClientCreationMode.AzureSasCredential }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `AzureSasCredential` mode.",
                "name",
                new FileServiceAvailableOptions
                {
                    Mode = FileClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                false,
                "The sas query token cannot be null or whitespace when using `AzureSasCredential` mode.",
                "name",
                new FileServiceAvailableOptions
                {
                    Mode = FileClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new FileServiceAvailableOptions
                {
                    Mode = FileClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://example.com?sas=token", UriKind.Absolute),
                }
            );
    }
}
