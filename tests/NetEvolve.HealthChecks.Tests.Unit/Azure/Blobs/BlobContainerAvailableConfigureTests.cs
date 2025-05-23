﻿namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Blobs;

using System;
using global::Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.Blobs;
using Xunit;

[TestGroup("AzureBlobs")]
public sealed class BlobContainerAvailableConfigureTests
{
    [Fact]
    public void Configue_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new BlobContainerAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new BlobContainerAvailableOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

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

    public static TheoryData<bool, string?, string?, BlobContainerAvailableOptions> GetValidateTestCases()
    {
        var data = new TheoryData<bool, string?, string?, BlobContainerAvailableOptions>
        {
            { false, "The name cannot be null or whitespace.", null, null! },
            { false, "The name cannot be null or whitespace.", "\t", null! },
            { false, "The option cannot be null.", "name", null! },
            {
                false,
                "The timeout cannot be less than infinite (-1).",
                "name",
                new BlobContainerAvailableOptions { Timeout = -2 }
            },
            { false, "The container name cannot be null or whitespace.", "name", new BlobContainerAvailableOptions() },
            {
                false,
                "The mode `13` is not supported.",
                "name",
                new BlobContainerAvailableOptions { Mode = (BlobClientCreationMode)13, ContainerName = "test" }
            },
            // Mode: ServiceProvider
            {
                false,
                $"No service of type `{nameof(BlobServiceClient)}` registered. Please execute `builder.AddAzureClients()`.",
                "name",
                new BlobContainerAvailableOptions
                {
                    Mode = BlobClientCreationMode.ServiceProvider,
                    ContainerName = "test",
                }
            },
            // Mode: DefaultAzureCredentials
            {
                false,
                "The service url cannot be null when using `DefaultAzureCredentials` mode.",
                "name",
                new BlobContainerAvailableOptions
                {
                    Mode = BlobClientCreationMode.DefaultAzureCredentials,
                    ContainerName = "test",
                }
            },
            {
                false,
                "The service url must be an absolute url when using `DefaultAzureCredentials` mode.",
                "name",
                new BlobContainerAvailableOptions
                {
                    ContainerName = "test",
                    Mode = BlobClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            },
            {
                true,
                null,
                "name",
                new BlobContainerAvailableOptions
                {
                    ContainerName = "test",
                    Mode = BlobClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                }
            },
            // Mode: ConnectionString
            {
                false,
                "The connection string cannot be null or whitespace when using `ConnectionString` mode.",
                "name",
                new BlobContainerAvailableOptions
                {
                    ContainerName = "test",
                    Mode = BlobClientCreationMode.ConnectionString,
                }
            },
            {
                true,
                null,
                "name",
                new BlobContainerAvailableOptions
                {
                    ContainerName = "test",
                    Mode = BlobClientCreationMode.ConnectionString,
                    ConnectionString = "connectionString",
                }
            },
            // Mode: SharedKey
            {
                false,
                "The service url cannot be null when using `SharedKey` mode.",
                "name",
                new BlobContainerAvailableOptions { ContainerName = "test", Mode = BlobClientCreationMode.SharedKey }
            },
            {
                false,
                "The service url must be an absolute url when using `SharedKey` mode.",
                "name",
                new BlobContainerAvailableOptions
                {
                    ContainerName = "test",
                    Mode = BlobClientCreationMode.SharedKey,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            },
            {
                false,
                "The account name cannot be null or whitespace when using `SharedKey` mode.",
                "name",
                new BlobContainerAvailableOptions
                {
                    ContainerName = "test",
                    Mode = BlobClientCreationMode.SharedKey,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                    AccountName = null,
                }
            },
            {
                false,
                "The account key cannot be null or whitespace when using `SharedKey` mode.",
                "name",
                new BlobContainerAvailableOptions
                {
                    ContainerName = "test",
                    Mode = BlobClientCreationMode.SharedKey,
                    ServiceUri = new Uri("https://example.com", UriKind.Absolute),
                    AccountName = "test",
                    AccountKey = null,
                }
            },
            {
                true,
                null,
                "name",
                new BlobContainerAvailableOptions
                {
                    ContainerName = "test",
                    Mode = BlobClientCreationMode.SharedKey,
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
                new BlobContainerAvailableOptions
                {
                    ContainerName = "test",
                    Mode = BlobClientCreationMode.AzureSasCredential,
                }
            },
            {
                false,
                "The service url must be an absolute url when using `AzureSasCredential` mode.",
                "name",
                new BlobContainerAvailableOptions
                {
                    ContainerName = "test",
                    Mode = BlobClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            },
            {
                false,
                "The sas query token cannot be null or whitespace when using `AzureSasCredential` mode.",
                "name",
                new BlobContainerAvailableOptions
                {
                    ContainerName = "test",
                    Mode = BlobClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute", UriKind.Absolute),
                }
            },
            {
                true,
                null,
                "name",
                new BlobContainerAvailableOptions
                {
                    ContainerName = "test",
                    Mode = BlobClientCreationMode.AzureSasCredential,
                    ServiceUri = new Uri("https://absolute?query=test", UriKind.Absolute),
                }
            },
        };

        return data;
    }
}
