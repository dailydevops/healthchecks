namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Tables;

using System;
using System.Diagnostics.CodeAnalysis;
using global::Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Tables;
using Xunit;

public sealed class TableClientAvailableConfigureTests
{
    [Fact]
    public void Configue_OnlyOptions_ThrowsArgumentException()
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

    [Theory]
    [MemberData(nameof(GetValidateTestCases))]
    public void Validate_Theory_Expected(
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
        Assert.Equal(expectedResult, result.Succeeded);
        Assert.Equal(expectedMessage, result.FailureMessage);
    }

    public static TheoryData<
        bool,
        string?,
        string?,
        TableClientAvailableOptions
    > GetValidateTestCases()
    {
        var data = new TheoryData<bool, string?, string?, TableClientAvailableOptions>
        {
            { false, "The name cannot be null or whitespace.", null, null! },
            { false, "The name cannot be null or whitespace.", "\t", null! },
            { false, "The option cannot be null.", "name", null! },
            {
                false,
                "The timeout cannot be less than infinite (-1).",
                "name",
                new TableClientAvailableOptions { Timeout = -2 }
            },
            {
                false,
                "The container name cannot be null or whitespace.",
                "name",
                new TableClientAvailableOptions()
            },
            {
                false,
                "The mode `13` is not supported.",
                "name",
                new TableClientAvailableOptions
                {
                    Mode = (TableClientCreationMode)13,
                    TableName = "test",
                }
            },
            // Mode: ServiceProvider
            {
                false,
                $"No service of type `{nameof(TableServiceClient)}` registered. Please execute `builder.AddAzureClients()`.",
                "name",
                new TableClientAvailableOptions
                {
                    Mode = TableClientCreationMode.ServiceProvider,
                    TableName = "test",
                }
            },
            // Mode: DefaultAzureCredentials
            {
                false,
                "The service url cannot be null when using `DefaultAzureCredentials` mode.",
                "name",
                new TableClientAvailableOptions
                {
                    Mode = TableClientCreationMode.DefaultAzureCredentials,
                    TableName = "test",
                }
            },
            {
                false,
                "The service url must be an absolute url when using `DefaultAzureCredentials` mode.",
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            },
            {
                true,
                null,
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                }
            },
            // Mode: ConnectionString
            {
                false,
                "The connection string cannot be null or whitespace when using `ConnectionString` mode.",
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.ConnectionString,
                }
            },
            {
                true,
                null,
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.ConnectionString,
                    ConnectionString = "connectionString",
                }
            },
            // Mode: SharedKey
            {
                false,
                "The service url cannot be null when using `SharedKey` mode.",
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.SharedKey,
                }
            },
            {
                false,
                "The service url must be an absolute url when using `SharedKey` mode.",
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.SharedKey,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            },
            {
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
            },
            {
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
            },
            {
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
            },
            // Mode: AzureSasCredential
            {
                false,
                "The service url cannot be null when using `AzureSasCredential` mode.",
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.AzureSasCredential,
                }
            },
            {
                false,
                "The service url must be an absolute url when using `AzureSasCredential` mode.",
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            },
            {
                false,
                "The sas query token cannot be null or whitespace when using `AzureSasCredential` mode.",
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute", UriKind.Absolute),
                }
            },
            {
                true,
                null,
                "name",
                new TableClientAvailableOptions
                {
                    TableName = "test",
                    Mode = TableClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute?query=test", UriKind.Absolute),
                }
            },
        };

        return data;
    }
}
