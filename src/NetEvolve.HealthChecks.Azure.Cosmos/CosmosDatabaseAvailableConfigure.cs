namespace NetEvolve.HealthChecks.Azure.Cosmos;

using System;
using System.Threading;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class CosmosDatabaseAvailableConfigure
    : IConfigureNamedOptions<CosmosDatabaseAvailableOptions>,
        IValidateOptions<CosmosDatabaseAvailableOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public CosmosDatabaseAvailableConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, CosmosDatabaseAvailableOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureCosmosDatabase:{name}", options);
    }

    public void Configure(CosmosDatabaseAvailableOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, CosmosDatabaseAvailableOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(options.DatabaseId))
        {
            return Fail("The database ID cannot be null or whitespace.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return Fail("The timeout cannot be less than infinite (-1).");
        }

        var mode = options.Mode;

        return options.Mode switch
        {
            CosmosClientCreationMode.ServiceProvider => ValidateModeServiceProvider(),
            CosmosClientCreationMode.ConnectionString => ValidateModeConnectionString(options),
            CosmosClientCreationMode.DefaultAzureCredentials => ValidateModeDefaultAzureCredentials(options),
            _ => Fail($"The mode `{mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(CosmosDatabaseAvailableOptions options)
    {
        if (options.EndpointUri is null)
        {
            return Fail(
                $"The endpoint URI cannot be null when using `{nameof(CosmosClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        if (!options.EndpointUri.IsAbsoluteUri)
        {
            return Fail(
                $"The endpoint URI must be an absolute URI when using `{nameof(CosmosClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeConnectionString(CosmosDatabaseAvailableOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail(
                $"The connection string cannot be null or whitespace when using `{nameof(CosmosClientCreationMode.ConnectionString)}` mode."
            );
        }

        return Success;
    }

    private ValidateOptionsResult ValidateModeServiceProvider()
    {
        if (_serviceProvider.GetService<CosmosClient>() is null)
        {
            return Fail(
                $"No service of type `{nameof(CosmosClient)}` registered. Please register a CosmosClient instance."
            );
        }

        return Success;
    }
}
