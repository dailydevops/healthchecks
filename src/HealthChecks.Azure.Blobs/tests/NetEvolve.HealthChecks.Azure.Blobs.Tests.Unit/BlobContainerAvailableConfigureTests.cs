namespace NetEvolve.HealthChecks.BlobContainerAvailable.Connector.Tests.Unit;

using global::Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Blobs;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

[ExcludeFromCodeCoverage]
[UnitTest]
public sealed class BlobContainerAvailableConfigureTests
{
    [Theory]
    [MemberData(nameof(GetValidateTestCases))]
    public void Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        BlobContainerAvailableOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new BlobContainerAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );

        // Act
        var result = configure.Validate(name, options);

        // Assert
        Assert.Equal(expectedResult, result.Succeeded);
        Assert.Equal(expectedMessage, result.FailureMessage);
    }

    public static TheoryData GetValidateTestCases()
    {
        var data = new TheoryData<bool, string?, string?, BlobContainerAvailableOptions>
        {
            { false, "The name cannot be null or whitespace.", null, null! },
            { false, "The name cannot be null or whitespace.", "\t", null! },
            { false, "The option cannot be null.", "name", null! },
            // Mode: ServiceProvider
            {
                false,
                $"No service of type `{nameof(BlobServiceClient)}` registered. Please execute `builder.AddAzureClients()`.",
                "name",
                new BlobContainerAvailableOptions { Mode = ClientCreationMode.ServiceProvider }
            },
            // Mode: DefaultAzureCredentials
            {
                false,
                "The service url cannot be null when using `DefaultAzureCredentials` mode.",
                "name",
                new BlobContainerAvailableOptions
                {
                    Mode = ClientCreationMode.DefaultAzureCredentials
                }
            },
            {
                false,
                "The service url must be an absolute url when using `DefaultAzureCredentials` mode.",
                "name",
                new BlobContainerAvailableOptions
                {
                    Mode = ClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("/relative", UriKind.Relative)
                }
            },
            // Mode: ConnectionString
            {
                false,
                "The connection string cannot be null or whitespace when using `ConnectionString` mode.",
                "name",
                new BlobContainerAvailableOptions { Mode = ClientCreationMode.ConnectionString }
            },
            {
                true,
                null,
                "name",
                new BlobContainerAvailableOptions
                {
                    Mode = ClientCreationMode.ConnectionString,
                    ConnectionString = "connectionString"
                }
            },
            // Mode: SharedKey
            {
                false,
                "The service url cannot be null when using `SharedKey` mode.",
                "name",
                new BlobContainerAvailableOptions { Mode = ClientCreationMode.SharedKey }
            },
            {
                false,
                "The service url must be an absolute url when using `SharedKey` mode.",
                "name",
                new BlobContainerAvailableOptions
                {
                    Mode = ClientCreationMode.SharedKey,
                    ServiceUri = new Uri("/relative", UriKind.Relative)
                }
            },
            // Mode: AzureSasCredential
            {
                false,
                "The service url cannot be null when using `AzureSasCredential` mode.",
                "name",
                new BlobContainerAvailableOptions { Mode = ClientCreationMode.AzureSasCredential }
            },
            {
                false,
                "The service url must be an absolute url when using `AzureSasCredential` mode.",
                "name",
                new BlobContainerAvailableOptions
                {
                    Mode = ClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("/relative", UriKind.Relative)
                }
            },
            {
                false,
                "The sas query token cannot be null or whitespace when using `AzureSasCredential` mode.",
                "name",
                new BlobContainerAvailableOptions
                {
                    Mode = ClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute", UriKind.Absolute)
                }
            }
        };

        return data;
    }
}
