namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using System.Threading;
using global::Azure.Search.Documents;
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
            SearchIndexClientCreationMode.ServiceProvider => ValidateModeServiceProvider(),
            SearchIndexClientCreationMode.DefaultAzureCredentials => ValidateModeDefaultAzureCredentials(options),
            SearchIndexClientCreationMode.AzureKeyCredential => ValidateModeAzureKeyCredential(options),
            _ => Fail($"The mode `{mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeAzureKeyCredential(SearchIndexAvailableOptions options)
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(SearchIndexClientCreationMode.AzureKeyCredential)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(SearchIndexClientCreationMode.AzureKeyCredential)}` mode."
            );
        }

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return Fail(
                $"The api key cannot be null or whitespace when using `{nameof(SearchIndexClientCreationMode.AzureKeyCredential)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(SearchIndexAvailableOptions options)
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(SearchIndexClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(SearchIndexClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        return Success;
    }

    private ValidateOptionsResult ValidateModeServiceProvider()
    {
        if (_serviceProvider.GetService<SearchClient>() is null)
        {
            return Fail(
                $"No service of type `{nameof(SearchClient)}` registered. Please execute `builder.AddAzureClients()`."
            );
        }

        return Success;
    }
}
