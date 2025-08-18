namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using System.Threading;
using global::Azure.Search.Documents.Indexes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class SearchServiceAvailableConfigure
    : IConfigureNamedOptions<SearchServiceAvailableOptions>,
        IValidateOptions<SearchServiceAvailableOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public SearchServiceAvailableConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, SearchServiceAvailableOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureSearchService:{name}", options);
    }

    public void Configure(SearchServiceAvailableOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, SearchServiceAvailableOptions options)
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

    private static ValidateOptionsResult ValidateModeApiKey(SearchServiceAvailableOptions options)
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

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(SearchServiceAvailableOptions options)
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

    private static ValidateOptionsResult ValidateModeConnectionString(SearchServiceAvailableOptions options)
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