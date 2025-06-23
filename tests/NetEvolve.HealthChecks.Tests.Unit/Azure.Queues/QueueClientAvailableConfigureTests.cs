namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Queues;

using System;
using global::Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Queues;

[TestGroup($"{nameof(Azure)}.{nameof(Queues)}")]
public sealed class QueueClientAvailableConfigureTests
{
    [Test]
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

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
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
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsEqualTo(expectedResult);
            _ = await Assert.That(result.FailureMessage).IsEqualTo(expectedMessage);
        }
    }

    public static IEnumerable<Func<(bool, string?, string?, QueueClientAvailableOptions)>> GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null!);
        yield return () => (false, "The name cannot be null or whitespace.", "\t", null!);
        yield return () => (false, "The option cannot be null.", "name", null!);
        yield return () =>
            (
                false,
                "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.",
                "name",
                new QueueClientAvailableOptions { Timeout = -2 }
            );
        yield return () =>
            (false, "The queue name cannot be null or whitespace.", "name", new QueueClientAvailableOptions());
        yield return () =>
            (
                false,
                "The mode `13` is not supported.",
                "name",
                new QueueClientAvailableOptions { Mode = (QueueClientCreationMode)13, QueueName = "test" }
            );

        // Mode: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(QueueServiceClient)}` registered. Please execute `builder.AddAzureClients()`.",
                "name",
                new QueueClientAvailableOptions { Mode = QueueClientCreationMode.ServiceProvider, QueueName = "test" }
            );

        // Mode: DefaultAzureCredentials
        yield return () =>
            (
                false,
                "The service url cannot be null when using `DefaultAzureCredentials` mode.",
                "name",
                new QueueClientAvailableOptions
                {
                    Mode = QueueClientCreationMode.DefaultAzureCredentials,
                    QueueName = "test",
                }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `DefaultAzureCredentials` mode.",
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                }
            );

        // Mode: ConnectionString
        yield return () =>
            (
                false,
                "The connection string cannot be null or whitespace when using `ConnectionString` mode.",
                "name",
                new QueueClientAvailableOptions { QueueName = "test", Mode = QueueClientCreationMode.ConnectionString }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.ConnectionString,
                    ConnectionString = "connectionString",
                }
            );

        // Mode: SharedKey
        yield return () =>
            (
                false,
                "The service url cannot be null when using `SharedKey` mode.",
                "name",
                new QueueClientAvailableOptions { QueueName = "test", Mode = QueueClientCreationMode.SharedKey }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `SharedKey` mode.",
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.SharedKey,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
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
            );
        yield return () =>
            (
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
            );
        yield return () =>
            (
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
            );

        // Mode: AzureSasCredential
        yield return () =>
            (
                false,
                "The service url cannot be null when using `AzureSasCredential` mode.",
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.AzureSasCredential,
                }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `AzureSasCredential` mode.",
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                false,
                "The sas query token cannot be null or whitespace when using `AzureSasCredential` mode.",
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute", UriKind.Absolute),
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new QueueClientAvailableOptions
                {
                    QueueName = "test",
                    Mode = QueueClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute?query=test", UriKind.Absolute),
                }
            );
    }
}
