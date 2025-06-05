namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Tables;

using System;
using global::Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Tables;

[TestGroup($"{nameof(Azure)}.{nameof(Tables)}")]
public sealed class TableServiceAvailableConfigureTests
{
    [Test]
    public void Configue_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new TableServiceAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new TableServiceAvailableOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        TableServiceAvailableOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new TableServiceAvailableConfigure(
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

    public static IEnumerable<Func<(bool, string?, string?, TableServiceAvailableOptions)>> GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null!);
        yield return () => (false, "The name cannot be null or whitespace.", "\t", null!);
        yield return () => (false, "The option cannot be null.", "name", null!);
        yield return () =>
            (
                false,
                "The timeout cannot be less than infinite (-1).",
                "name",
                new TableServiceAvailableOptions { Timeout = -2 }
            );
        yield return () =>
            (
                false,
                "The mode `13` is not supported.",
                "name",
                new TableServiceAvailableOptions { Mode = (TableClientCreationMode)13 }
            );

        // Model: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(TableServiceClient)}` registered. Please execute `builder.AddAzureClients()`.",
                "name",
                new TableServiceAvailableOptions { Mode = TableClientCreationMode.ServiceProvider }
            );

        // Mode: DefaultAzureCredentials
        yield return () =>
            (
                false,
                "The service url cannot be null when using `DefaultAzureCredentials` mode.",
                "name",
                new TableServiceAvailableOptions { Mode = TableClientCreationMode.DefaultAzureCredentials }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `DefaultAzureCredentials` mode.",
                "name",
                new TableServiceAvailableOptions
                {
                    Mode = TableClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new TableServiceAvailableOptions
                {
                    Mode = TableClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                }
            );

        // Mode: ConnectionString
        yield return () =>
            (
                false,
                "The connection string cannot be null or whitespace when using `ConnectionString` mode.",
                "name",
                new TableServiceAvailableOptions { Mode = TableClientCreationMode.ConnectionString }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new TableServiceAvailableOptions
                {
                    Mode = TableClientCreationMode.ConnectionString,
                    ConnectionString = "connectionString",
                }
            );

        // Mode: SharedKey
        yield return () =>
            (
                false,
                "The service url cannot be null when using `SharedKey` mode.",
                "name",
                new TableServiceAvailableOptions { Mode = TableClientCreationMode.SharedKey }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `SharedKey` mode.",
                "name",
                new TableServiceAvailableOptions
                {
                    Mode = TableClientCreationMode.SharedKey,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                false,
                "The account name cannot be null or whitespace when using `SharedKey` mode.",
                "name",
                new TableServiceAvailableOptions
                {
                    Mode = TableClientCreationMode.SharedKey,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                    AccountName = null,
                }
            );
        yield return () =>
            (
                false,
                "The account key cannot be null or whitespace when using `SharedKey` mode.",
                "name",
                new TableServiceAvailableOptions
                {
                    Mode = TableClientCreationMode.SharedKey,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                    AccountName = "test",
                    AccountKey = null,
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new TableServiceAvailableOptions
                {
                    Mode = TableClientCreationMode.SharedKey,
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
                new TableServiceAvailableOptions { Mode = TableClientCreationMode.AzureSasCredential }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `AzureSasCredential` mode.",
                "name",
                new TableServiceAvailableOptions
                {
                    Mode = TableClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                false,
                "The sas query token cannot be null or whitespace when using `AzureSasCredential` mode.",
                "name",
                new TableServiceAvailableOptions
                {
                    Mode = TableClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute", UriKind.Absolute),
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new TableServiceAvailableOptions
                {
                    Mode = TableClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute?query=test", UriKind.Absolute),
                }
            );
    }
}
