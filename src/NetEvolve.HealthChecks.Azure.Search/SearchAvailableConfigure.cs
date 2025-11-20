namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using System.Threading;
using global::Azure.Search.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class SearchAvailableConfigure
    : IConfigureNamedOptions<SearchAvailableOptions>,
        IValidateOptions<SearchAvailableOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public SearchAvailableConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, SearchAvailableOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureSearchIndex:{name}", options);
    }

    public void Configure(SearchAvailableOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, SearchAvailableOptions options)
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
            ClientCreationMode.ServiceProvider => ValidateModeServiceProvider(),
            ClientCreationMode.DefaultAzureCredentials => ValidateModeDefaultAzureCredentials(options),
            ClientCreationMode.AzureKeyCredential => ValidateModeAzureKeyCredential(options),
            _ => Fail($"The mode `{mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeAzureKeyCredential(SearchAvailableOptions options)
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(ClientCreationMode.AzureKeyCredential)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(ClientCreationMode.AzureKeyCredential)}` mode."
            );
        }

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return Fail(
                $"The api key cannot be null or whitespace when using `{nameof(ClientCreationMode.AzureKeyCredential)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(SearchAvailableOptions options)
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(ClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(ClientCreationMode.DefaultAzureCredentials)}` mode."
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
