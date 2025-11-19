namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Search;

using System;
using global::Azure.Search.Documents;
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
                new SearchIndexAvailableOptions { IndexName = null }
            );
        yield return () =>
            (
                false,
                "The mode `13` is not supported.",
                "name",
                new SearchIndexAvailableOptions { Mode = (SearchIndexClientCreationMode)13, IndexName = "test" }
            );

        // Model: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(SearchClient)}` registered. Please execute `builder.AddAzureClients()`.",
                "name",
                new SearchIndexAvailableOptions
                {
                    Mode = SearchIndexClientCreationMode.ServiceProvider,
                    IndexName = "test",
                }
            );

        // Mode: DefaultAzureCredentials
        yield return () =>
            (
                false,
                "The service url cannot be null when using `DefaultAzureCredentials` mode.",
                "name",
                new SearchIndexAvailableOptions
                {
                    Mode = SearchIndexClientCreationMode.DefaultAzureCredentials,
                    IndexName = "test",
                }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `DefaultAzureCredentials` mode.",
                "name",
                new SearchIndexAvailableOptions
                {
                    Mode = SearchIndexClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                    IndexName = "test",
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new SearchIndexAvailableOptions
                {
                    Mode = SearchIndexClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("https://example.search.windows.net", UriKind.Absolute),
                    IndexName = "test",
                }
            );

        // Mode: AzureKeyCredential
        yield return () =>
            (
                false,
                "The service url cannot be null when using `AzureKeyCredential` mode.",
                "name",
                new SearchIndexAvailableOptions
                {
                    Mode = SearchIndexClientCreationMode.AzureKeyCredential,
                    IndexName = "test",
                }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `AzureKeyCredential` mode.",
                "name",
                new SearchIndexAvailableOptions
                {
                    Mode = SearchIndexClientCreationMode.AzureKeyCredential,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                    IndexName = "test",
                }
            );
        yield return () =>
            (
                false,
                "The api key cannot be null or whitespace when using `AzureKeyCredential` mode.",
                "name",
                new SearchIndexAvailableOptions
                {
                    Mode = SearchIndexClientCreationMode.AzureKeyCredential,
                    ServiceUri = new Uri("https://example.search.windows.net", UriKind.Absolute),
                    ApiKey = null,
                    IndexName = "test",
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new SearchIndexAvailableOptions
                {
                    Mode = SearchIndexClientCreationMode.AzureKeyCredential,
                    ServiceUri = new Uri("https://example.search.windows.net", UriKind.Absolute),
                    ApiKey = "test-key",
                    IndexName = "test",
                }
            );
    }
}
