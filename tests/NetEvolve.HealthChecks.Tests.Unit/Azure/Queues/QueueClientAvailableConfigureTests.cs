namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Queues;

using System;
using global::Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Queues;
using Xunit;

[TestGroup("AzureQueues")]
public sealed class QueueClientAvailableConfigureTests
{
    [Fact]
    public void Configue_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new QueueClientAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new QueueClientAvailableOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Theory]
    [MemberData(nameof(GetValidateTestCases))]
    public void Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        QueueClientAvailableOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new QueueClientAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.Equal(expectedResult, result.Succeeded);
        Assert.Equal(expectedMessage, result.FailureMessage);
    }

    public static TheoryData<bool, string?, string?, QueueClientAvailableOptions> GetValidateTestCases()
    {
        var data = new TheoryData<bool, string?, string?, QueueClientAvailableOptions>
        {
            { false, "The name cannot be null or whitespace.", null, null! },
            { false, "The name cannot be null or whitespace.", "\t", null! },
            { false, "The option cannot be null.", "name", null! },
            {
                false,
                "The timeout cannot be less than infinite (-1).",
                "name",
                new QueueClientAvailableOptions { Timeout = -2 }
            },
            { false, "The queue name cannot be null or whitespace.", "name", new QueueClientAvailableOptions() },
            {
                false,
                "The mode `13` is not supported.",
                "name",
                new QueueClientAvailableOptions { Mode = (QueueClientCreationMode)13, QueueName = "test" }
            },
            // Mode: ServiceProvider
            {
                false,
                $"No service of type `{nameof(QueueServiceClient)}` registered. Please execute `builder.AddAzureClients()`.",
                "name",
                new QueueClientAvailableOptions { Mode = QueueClientCreationMode.ServiceProvider, QueueName = "test" }
            },
            // Mode: DefaultAzureCredentials
            {
                false,
                "The service url cannot be null when using `DefaultAzureCredentials` mode.",
                "name",
                new QueueClientAvailableOptions
                {
                    Mode = QueueClientCreationMode.DefaultAzureCredentials,
                    QueueName = "test",
                }
            },
            {
                false,
                "The service url must be an absolute url when using `DefaultAzureCredentials` mode.",
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            },
            {
                true,
                null,
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                }
            },
            // Mode: ConnectionString
            {
                false,
                "The connection string cannot be null or whitespace when using `ConnectionString` mode.",
                "name",
                new QueueClientAvailableOptions { QueueName = "test", Mode = QueueClientCreationMode.ConnectionString }
            },
            {
                true,
                null,
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.ConnectionString,
                    ConnectionString = "connectionString",
                }
            },
            // Mode: SharedKey
            {
                false,
                "The service url cannot be null when using `SharedKey` mode.",
                "name",
                new QueueClientAvailableOptions { QueueName = "test", Mode = QueueClientCreationMode.SharedKey }
            },
            {
                false,
                "The service url must be an absolute url when using `SharedKey` mode.",
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.SharedKey,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            },
            {
                false,
                "The account name cannot be null or whitespace when using `SharedKey` mode.",
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.SharedKey,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                    AccountName = null,
                }
            },
            {
                false,
                "The account key cannot be null or whitespace when using `SharedKey` mode.",
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
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
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
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
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.AzureSasCredential,
                }
            },
            {
                false,
                "The service url must be an absolute url when using `AzureSasCredential` mode.",
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            },
            {
                false,
                "The sas query token cannot be null or whitespace when using `AzureSasCredential` mode.",
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute", UriKind.Absolute),
                }
            },
            {
                true,
                null,
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute?query=test", UriKind.Absolute),
                }
            },
        };

        return data;
    }
}
