namespace NetEvolve.HealthChecks.Azure.Kusto;

using System;
using System.Threading;
using Kusto.Data.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class KustoConfigure : IConfigureNamedOptions<KustoOptions>, IValidateOptions<KustoOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public KustoConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, KustoOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureKusto:{name}", options);
    }

    public void Configure(KustoOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, KustoOptions options)
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
            KustoClientCreationMode.ServiceProvider => ValidateModeServiceProvider(),
            KustoClientCreationMode.ConnectionString => ValidateModeConnectionString(options),
            KustoClientCreationMode.DefaultAzureCredentials => ValidateModeDefaultAzureCredentials(options),
            _ => Fail($"The mode `{mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(KustoOptions options)
    {
        if (options.ClusterUri is null)
        {
            return Fail(
                $"The cluster URI cannot be null when using `{nameof(KustoClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        if (!options.ClusterUri.IsAbsoluteUri)
        {
            return Fail(
                $"The cluster URI must be an absolute URI when using `{nameof(KustoClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeConnectionString(KustoOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail(
                $"The connection string cannot be null or whitespace when using `{nameof(KustoClientCreationMode.ConnectionString)}` mode."
            );
        }

        return Success;
    }

    private ValidateOptionsResult ValidateModeServiceProvider()
    {
        if (_serviceProvider.GetService<ICslQueryProvider>() is null)
        {
            return Fail(
                $"No service of type `{nameof(ICslQueryProvider)}` registered. Please register a Kusto client."
            );
        }

        return Success;
    }
}
