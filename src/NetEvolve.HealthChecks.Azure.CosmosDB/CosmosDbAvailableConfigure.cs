namespace NetEvolve.HealthChecks.Azure.CosmosDB;

using System;
using System.Threading;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class CosmosDbAvailableConfigure
    : IConfigureNamedOptions<CosmosDbAvailableOptions>,
        IValidateOptions<CosmosDbAvailableOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public CosmosDbAvailableConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, CosmosDbAvailableOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureCosmosDb:{name}", options);
    }

    public void Configure(CosmosDbAvailableOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, CosmosDbAvailableOptions options)
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
            CosmosDbClientCreationMode.ServiceProvider => ValidateModeServiceProvider(),
            CosmosDbClientCreationMode.ConnectionString => ValidateModeConnectionString(options),
            CosmosDbClientCreationMode.DefaultAzureCredentials => ValidateModeDefaultAzureCredentials(options),
            CosmosDbClientCreationMode.AccountKey => ValidateModeAccountKey(options),
            _ => Fail($"The mode `{mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeAccountKey(CosmosDbAvailableOptions options)
    {
        if (options.AccountEndpoint is null)
        {
            return Fail(
                $"The account endpoint cannot be null when using `{nameof(CosmosDbClientCreationMode.AccountKey)}` mode."
            );
        }

        if (!options.AccountEndpoint.IsAbsoluteUri)
        {
            return Fail(
                $"The account endpoint must be an absolute url when using `{nameof(CosmosDbClientCreationMode.AccountKey)}` mode."
            );
        }

        if (string.IsNullOrWhiteSpace(options.AccountKey))
        {
            return Fail(
                $"The account key cannot be null or whitespace when using `{nameof(CosmosDbClientCreationMode.AccountKey)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(CosmosDbAvailableOptions options)
    {
        if (options.AccountEndpoint is null)
        {
            return Fail(
                $"The account endpoint cannot be null when using `{nameof(CosmosDbClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        if (!options.AccountEndpoint.IsAbsoluteUri)
        {
            return Fail(
                $"The account endpoint must be an absolute url when using `{nameof(CosmosDbClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeConnectionString(CosmosDbAvailableOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail(
                $"The connection string cannot be null or whitespace when using `{nameof(CosmosDbClientCreationMode.ConnectionString)}` mode."
            );
        }

        return Success;
    }

    private ValidateOptionsResult ValidateModeServiceProvider()
    {
        if (_serviceProvider.GetService<CosmosClient>() is null)
        {
            return Fail(
                $"No service of type `{nameof(CosmosClient)}` registered. Please register the CosmosClient."
            );
        }

        return Success;
    }
}
