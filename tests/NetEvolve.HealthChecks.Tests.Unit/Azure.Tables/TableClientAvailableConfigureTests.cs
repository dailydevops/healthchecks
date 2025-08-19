namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Tables;

using System;
using global::Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Tables;

[TestGroup($"{nameof(Azure)}.{nameof(Tables)}")]
public sealed class TableClientAvailableConfigureTests
{
    [Test]
    public void Configure_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new TableClientAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new TableClientAvailableOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        TableClientAvailableOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new TableClientAvailableConfigure(
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

    public static IEnumerable<Func<(bool, string?, string?, TableClientAvailableOptions)>> GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null!);
        yield return () => (false, "The name cannot be null or whitespace.", "\t", null!);
        yield return () => (false, "The option cannot be null.", "name", null!);
        yield return () =>
            (
                false,
                "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.",
                "name",
                new TableClientAvailableOptions { Timeout = -2 }
            );
        yield return () =>
            (
                false,
                "The container name cannot be null or whitespace.",
                "name",
                new TableClientAvailableOptions { TableName = null! }
            );
        yield return () =>
            (
                false,
                "The container name cannot be null or whitespace.",
                "name",
                new TableClientAvailableOptions { TableName = "\t" }
            );

        // Mode: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(TableServiceClient)}` registered. Please execute `builder.AddAzureClients()`.",
                "name",
                new TableClientAvailableOptions { Mode = TableClientCreationMode.ServiceProvider, TableName = "test" }
            );

        // Mode: DefaultAzureCredentials
        yield return () =>
            (
                false,
                "The service url cannot be null when using `DefaultAzureCredentials` mode.",
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.DefaultAzureCredentials,
                }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `DefaultAzureCredentials` mode.",
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
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
                new TableClientAvailableOptions { TableName = "test", Mode = TableClientCreationMode.ConnectionString }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
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
                new TableClientAvailableOptions { TableName = "test", Mode = TableClientCreationMode.SharedKey }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `SharedKey` mode.",
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.SharedKey,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                false,
                "The account name cannot be null or whitespace when using `SharedKey` mode.",
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
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
                new TableClientAvailableOptions
                {
                    TableName = "test",
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
                new TableClientAvailableOptions
                {
                    TableName = "test",
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
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.AzureSasCredential,
                }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `AzureSasCredential` mode.",
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                false,
                "The sas query token cannot be null or whitespace when using `AzureSasCredential` mode.",
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute", UriKind.Absolute),
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute?query=test", UriKind.Absolute),
                }
            );
    }
}
