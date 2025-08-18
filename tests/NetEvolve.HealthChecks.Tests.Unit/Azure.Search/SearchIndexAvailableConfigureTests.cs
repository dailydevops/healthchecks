namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Search;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Azure.Search.Documents.Indexes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Search;

[TestGroup($"{nameof(Azure)}.{nameof(Search)}")]
public sealed class SearchIndexAvailableConfigureTests
{
    [Test]
    public void Configure_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new SearchIndexAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new SearchIndexAvailableOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        SearchIndexAvailableOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new SearchIndexAvailableConfigure(
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

    public static IEnumerable<Func<(bool, string?, string?, SearchIndexAvailableOptions)>> GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null!);
        yield return () => (false, "The name cannot be null or whitespace.", "\t", null!);
        yield return () => (false, "The option cannot be null.", "name", null!);
        yield return () =>
            (
                false,
                "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.",
                "name",
                new SearchIndexAvailableOptions { Timeout = -2 }
            );
        yield return () =>
            (
                false,
                "The index name cannot be null or whitespace.",
                "name",
                new SearchIndexAvailableOptions { Mode = SearchClientCreationMode.ApiKey }
            );
        yield return () =>
            (
                false,
                "The mode `13` is not supported.",
                "name",
                new SearchIndexAvailableOptions { Mode = (SearchClientCreationMode)13, IndexName = "test-index" }
            );

        // Mode: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(SearchIndexClient)}` registered. Please execute `builder.AddAzureClients()`.",
                "name",
                new SearchIndexAvailableOptions { Mode = SearchClientCreationMode.ServiceProvider, IndexName = "test-index" }
            );

        // Mode: ConnectionString - valid
        yield return () =>
            (
                true,
                null,
                "name",
                new SearchIndexAvailableOptions
                {
                    Mode = SearchClientCreationMode.ConnectionString,
                    ConnectionString = "Endpoint=https://test.search.windows.net;ApiKey=test-key",
                    IndexName = "test-index"
                }
            );

        // Mode: ConnectionString - invalid (null)
        yield return () =>
            (
                false,
                $"The connection string cannot be null or whitespace when using `{nameof(SearchClientCreationMode.ConnectionString)}` mode.",
                "name",
                new SearchIndexAvailableOptions { Mode = SearchClientCreationMode.ConnectionString, IndexName = "test-index" }
            );

        // Mode: DefaultAzureCredentials - valid
        yield return () =>
            (
                true,
                null,
                "name",
                new SearchIndexAvailableOptions
                {
                    Mode = SearchClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("https://test.search.windows.net"),
                    IndexName = "test-index"
                }
            );

        // Mode: DefaultAzureCredentials - invalid URI (null)
        yield return () =>
            (
                false,
                $"The service url cannot be null when using `{nameof(SearchClientCreationMode.DefaultAzureCredentials)}` mode.",
                "name",
                new SearchIndexAvailableOptions { Mode = SearchClientCreationMode.DefaultAzureCredentials, IndexName = "test-index" }
            );

        // Mode: ApiKey - valid
        yield return () =>
            (
                true,
                null,
                "name",
                new SearchIndexAvailableOptions
                {
                    Mode = SearchClientCreationMode.ApiKey,
                    ServiceUri = new Uri("https://test.search.windows.net"),
                    ApiKey = "test-key",
                    IndexName = "test-index"
                }
            );

        // Mode: ApiKey - invalid (null API key)
        yield return () =>
            (
                false,
                $"The API key cannot be null or whitespace when using `{nameof(SearchClientCreationMode.ApiKey)}` mode.",
                "name",
                new SearchIndexAvailableOptions
                {
                    Mode = SearchClientCreationMode.ApiKey,
                    ServiceUri = new Uri("https://test.search.windows.net"),
                    IndexName = "test-index"
                }
            );
    }
}