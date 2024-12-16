namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Queues;

using System;
using global::Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.HealthChecks.Azure.Queues;
using Xunit;

public sealed class QueueServiceAvailableConfigureTests
{
    [Fact]
    public void Configue_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new QueueServiceAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new QueueServiceAvailableOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Theory]
    [MemberData(nameof(GetValidateTestCases))]
    public void Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        QueueServiceAvailableOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new QueueServiceAvailableConfigure(
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
        QueueServiceAvailableOptions
    > GetValidateTestCases()
    {
        var data = new TheoryData<bool, string?, string?, QueueServiceAvailableOptions>
        {
            { false, "The name cannot be null or whitespace.", null, null! },
            { false, "The name cannot be null or whitespace.", "\t", null! },
            { false, "The option cannot be null.", "name", null! },
            {
                false,
                "The timeout cannot be less than infinite (-1).",
                "name",
                new QueueServiceAvailableOptions { Timeout = -2 }
            },
            {
                false,
                "The mode `13` is not supported.",
                "name",
                new QueueServiceAvailableOptions { Mode = (QueueClientCreationMode)13 }
            },
            // Mode: ServiceProvider
            {
                false,
                $"No service of type `{nameof(QueueServiceClient)}` registered. Please execute `builder.AddAzureClients()`.",
                "name",
                new QueueServiceAvailableOptions { Mode = QueueClientCreationMode.ServiceProvider }
            },
            // Mode: DefaultAzureCredentials
            {
                false,
                "The service url cannot be null when using `DefaultAzureCredentials` mode.",
                "name",
                new QueueServiceAvailableOptions
                {
                    Mode = QueueClientCreationMode.DefaultAzureCredentials,
                }
            },
            {
                false,
                "The service url must be an absolute url when using `DefaultAzureCredentials` mode.",
                "name",
                new QueueServiceAvailableOptions
                {
                    Mode = QueueClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            },
            {
                true,
                null,
                "name",
                new QueueServiceAvailableOptions
                {
                    Mode = QueueClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                }
            },
            // Mode: ConnectionString
            {
                false,
                "The connection string cannot be null or whitespace when using `ConnectionString` mode.",
                "name",
                new QueueServiceAvailableOptions { Mode = QueueClientCreationMode.ConnectionString }
            },
            {
                true,
                null,
                "name",
                new QueueServiceAvailableOptions
                {
                    Mode = QueueClientCreationMode.ConnectionString,
                    ConnectionString = "connectionString",
                }
            },
            // Mode: SharedKey
            {
                false,
                "The service url cannot be null when using `SharedKey` mode.",
                "name",
                new QueueServiceAvailableOptions { Mode = QueueClientCreationMode.SharedKey }
            },
            {
                false,
                "The service url must be an absolute url when using `SharedKey` mode.",
                "name",
                new QueueServiceAvailableOptions
                {
                    Mode = QueueClientCreationMode.SharedKey,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            },
            {
                false,
                "The account name cannot be null or whitespace when using `SharedKey` mode.",
                "name",
                new QueueServiceAvailableOptions
                {
                    Mode = QueueClientCreationMode.SharedKey,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                    AccountName = null,
                }
            },
            {
                false,
                "The account key cannot be null or whitespace when using `SharedKey` mode.",
                "name",
                new QueueServiceAvailableOptions
                {
                    Mode = QueueClientCreationMode.SharedKey,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                    AccountName = "test",
                    AccountKey = null,
                }
            },
            {
                true,
                null,
                "name",
                new QueueServiceAvailableOptions
                {
                    Mode = QueueClientCreationMode.SharedKey,
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
                new QueueServiceAvailableOptions
                {
                    Mode = QueueClientCreationMode.AzureSasCredential,
                }
            },
            {
                false,
                "The service url must be an absolute url when using `AzureSasCredential` mode.",
                "name",
                new QueueServiceAvailableOptions
                {
                    Mode = QueueClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            },
            {
                false,
                "The sas query token cannot be null or whitespace when using `AzureSasCredential` mode.",
                "name",
                new QueueServiceAvailableOptions
                {
                    Mode = QueueClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute", UriKind.Absolute),
                }
            },
            {
                true,
                null,
                "name",
                new QueueServiceAvailableOptions
                {
                    Mode = QueueClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute?query=test", UriKind.Absolute),
                }
            },
        };

        return data;
    }
}
