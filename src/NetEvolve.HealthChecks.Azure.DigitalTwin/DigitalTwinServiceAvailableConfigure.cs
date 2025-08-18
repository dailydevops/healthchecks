namespace NetEvolve.HealthChecks.Azure.DigitalTwin;

using System;
using System.Threading;
using global::Azure.DigitalTwins.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class DigitalTwinServiceAvailableConfigure
    : IConfigureNamedOptions<DigitalTwinServiceAvailableOptions>,
        IValidateOptions<DigitalTwinServiceAvailableOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public DigitalTwinServiceAvailableConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, DigitalTwinServiceAvailableOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureDigitalTwins:{name}", options);
    }

    public void Configure(DigitalTwinServiceAvailableOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, DigitalTwinServiceAvailableOptions options)
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
            DigitalTwinClientCreationMode.ServiceProvider => ValidateModeServiceProvider(),
            DigitalTwinClientCreationMode.DefaultAzureCredentials => ValidateModeDefaultAzureCredentials(options),
            _ => Fail($"The mode `{mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(DigitalTwinServiceAvailableOptions options)
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(DigitalTwinClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(DigitalTwinClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        return Success;
    }

    private ValidateOptionsResult ValidateModeServiceProvider()
    {
        if (_serviceProvider.GetService<DigitalTwinsClient>() is null)
        {
            return Fail(
                $"No service of type `{nameof(DigitalTwinsClient)}` registered. Please execute `builder.AddAzureClients()`."
            );
        }

        return Success;
    }
}