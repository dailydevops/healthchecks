namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Search;

using System;
using global::Azure.Search.Documents.Indexes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Search;

[TestGroup($"{nameof(Azure)}.{nameof(Search)}")]
public sealed class SearchServiceAvailableConfigureTests
{
    [Test]
    public void Configure_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new SearchServiceAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new SearchServiceAvailableOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [MethodDataSource(nameof(GetValidateTestCases))]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        SearchServiceAvailableOptions options
    )
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new SearchServiceAvailableConfigure(
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

    public static IEnumerable<Func<(bool, string?, string?, SearchServiceAvailableOptions)>> GetValidateTestCases()
    {
        yield return () => (false, "The name cannot be null or whitespace.", null, null!);
        yield return () => (false, "The name cannot be null or whitespace.", "\t", null!);
        yield return () => (false, "The option cannot be null.", "name", null!);
        yield return () =>
            (
                false,
                "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.",
                "name",
                new SearchServiceAvailableOptions { Timeout = -2 }
            );
        yield return () =>
            (
                false,
                "The mode `13` is not supported.",
                "name",
                new SearchServiceAvailableOptions { Mode = (SearchIndexClientCreationMode)13 }
            );

        // Model: ServiceProvider
        yield return () =>
            (
                false,
                $"No service of type `{nameof(SearchIndexClient)}` registered. Please execute `builder.AddAzureClients()`.",
                "name",
                new SearchServiceAvailableOptions { Mode = SearchIndexClientCreationMode.ServiceProvider }
            );

        // Mode: DefaultAzureCredentials
        yield return () =>
            (
                false,
                "The service url cannot be null when using `DefaultAzureCredentials` mode.",
                "name",
                new SearchServiceAvailableOptions { Mode = SearchIndexClientCreationMode.DefaultAzureCredentials }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `DefaultAzureCredentials` mode.",
                "name",
                new SearchServiceAvailableOptions
                {
                    Mode = SearchIndexClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new SearchServiceAvailableOptions
                {
                    Mode = SearchIndexClientCreationMode.DefaultAzureCredentials,
                    ServiceUri = new Uri("https://example.search.windows.net", UriKind.Absolute),
                }
            );

        // Mode: AzureKeyCredential
        yield return () =>
            (
                false,
                "The service url cannot be null when using `AzureKeyCredential` mode.",
                "name",
                new SearchServiceAvailableOptions { Mode = SearchIndexClientCreationMode.AzureKeyCredential }
            );
        yield return () =>
            (
                false,
                "The service url must be an absolute url when using `AzureKeyCredential` mode.",
                "name",
                new SearchServiceAvailableOptions
                {
                    Mode = SearchIndexClientCreationMode.AzureKeyCredential,
                    ServiceUri = new Uri("/relative", UriKind.Relative),
                }
            );
        yield return () =>
            (
                false,
                "The api key cannot be null or whitespace when using `AzureKeyCredential` mode.",
                "name",
                new SearchServiceAvailableOptions
                {
                    Mode = SearchIndexClientCreationMode.AzureKeyCredential,
                    ServiceUri = new Uri("https://example.search.windows.net", UriKind.Absolute),
                    ApiKey = null,
                }
            );
        yield return () =>
            (
                true,
                null,
                "name",
                new SearchServiceAvailableOptions
                {
                    Mode = SearchIndexClientCreationMode.AzureKeyCredential,
                    ServiceUri = new Uri("https://example.search.windows.net", UriKind.Absolute),
                    ApiKey = "test-key",
                }
            );
    }
}
