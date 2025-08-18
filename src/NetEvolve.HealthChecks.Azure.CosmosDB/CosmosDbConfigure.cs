namespace NetEvolve.HealthChecks.Azure.CosmosDB;

using System;
using System.Threading;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class CosmosDbConfigure
    : IConfigureNamedOptions<CosmosDbOptions>,
        IValidateOptions<CosmosDbOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public CosmosDbConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, CosmosDbOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:CosmosDb:{name}", options);
    }

    public void Configure(CosmosDbOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, CosmosDbOptions options)
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

        var mode = options.Mode ?? CosmosDbClientCreationMode.ConnectionString;

        return mode switch
        {
            CosmosDbClientCreationMode.ConnectionString => ValidateModeConnectionString(options),
            CosmosDbClientCreationMode.DefaultAzureCredentials => ValidateModeDefaultAzureCredentials(options),
            CosmosDbClientCreationMode.AccountKey => ValidateModeAccountKey(options),
            CosmosDbClientCreationMode.ServicePrincipal => ValidateModeServicePrincipal(options),
            _ => Fail($"The mode `{mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeConnectionString(CosmosDbOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail(
                $"The connection string cannot be null or whitespace when using `{nameof(CosmosDbClientCreationMode.ConnectionString)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(CosmosDbOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ServiceEndpoint))
        {
            return Fail(
                $"The service endpoint cannot be null or whitespace when using `{nameof(CosmosDbClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeAccountKey(CosmosDbOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ServiceEndpoint))
        {
            return Fail(
                $"The service endpoint cannot be null or whitespace when using `{nameof(CosmosDbClientCreationMode.AccountKey)}` mode."
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

    private static ValidateOptionsResult ValidateModeServicePrincipal(CosmosDbOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ServiceEndpoint))
        {
            return Fail(
                $"The service endpoint cannot be null or whitespace when using `{nameof(CosmosDbClientCreationMode.ServicePrincipal)}` mode."
            );
        }

        return Success;
    }
}