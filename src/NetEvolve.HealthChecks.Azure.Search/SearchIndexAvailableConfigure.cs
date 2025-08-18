namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using System.Threading;
using global::Azure.Search.Documents.Indexes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class SearchIndexAvailableConfigure
    : IConfigureNamedOptions<SearchIndexAvailableOptions>,
        IValidateOptions<SearchIndexAvailableOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public SearchIndexAvailableConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, SearchIndexAvailableOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureSearchIndex:{name}", options);
    }

    public void Configure(SearchIndexAvailableOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, SearchIndexAvailableOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return Fail("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
        }

        if (string.IsNullOrWhiteSpace(options.IndexName))
        {
            return Fail("The index name cannot be null or whitespace.");
        }

        var mode = options.Mode;

        return options.Mode switch
        {
            SearchClientCreationMode.ServiceProvider => ValidateModeServiceProvider(),
            SearchClientCreationMode.ConnectionString => ValidateModeConnectionString(options),
            SearchClientCreationMode.DefaultAzureCredentials => ValidateModeDefaultAzureCredentials(options),
            SearchClientCreationMode.ApiKey => ValidateModeApiKey(options),
            _ => Fail($"The mode `{mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeApiKey(SearchIndexAvailableOptions options)
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(SearchClientCreationMode.ApiKey)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(SearchClientCreationMode.ApiKey)}` mode."
            );
        }

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return Fail(
                $"The API key cannot be null or whitespace when using `{nameof(SearchClientCreationMode.ApiKey)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(SearchIndexAvailableOptions options)
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(SearchClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(SearchClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeConnectionString(SearchIndexAvailableOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail(
                $"The connection string cannot be null or whitespace when using `{nameof(SearchClientCreationMode.ConnectionString)}` mode."
            );
        }

        return Success;
    }

    private ValidateOptionsResult ValidateModeServiceProvider()
    {
        if (_serviceProvider.GetService<SearchIndexClient>() is null)
        {
            return Fail(
                $"No service of type `{nameof(SearchIndexClient)}` registered. Please execute `builder.AddAzureClients()`."
            );
        }

        return Success;
    }
}