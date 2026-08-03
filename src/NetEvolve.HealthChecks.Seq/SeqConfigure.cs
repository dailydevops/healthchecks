namespace NetEvolve.HealthChecks.Seq;

using System;
using System.Threading;
using global::Seq.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class SeqConfigure : IConfigureNamedOptions<SeqOptions>, IValidateOptions<SeqOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public SeqConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, SeqOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Seq:{name}", options);
    }

    public void Configure(SeqOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, SeqOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The options cannot be null.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return Fail("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
        }

        return options.Mode switch
        {
            SeqClientCreationMode.ServiceProvider => ValidateCreationModeServiceProvider(options),
            SeqClientCreationMode.ServerUrl => ValidateCreationModeServerUrl(options),
            _ => Fail($"The mode `{options.Mode}` is not supported."),
        };
    }

    private ValidateOptionsResult ValidateCreationModeServiceProvider(SeqOptions options)
    {
        var client = options.KeyedService is null
            ? _serviceProvider.GetService<SeqConnection>()
            : _serviceProvider.GetKeyedService<SeqConnection>(options.KeyedService);

        if (client is null)
        {
            return Fail(
                $"No service of type `{nameof(SeqConnection)}` registered. Please execute `services.AddSingleton(<client instance>)`."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateCreationModeServerUrl(SeqOptions options)
    {
        const string creationModeName = nameof(SeqClientCreationMode.ServerUrl);

        if (options.ServerUrl is null)
        {
            return Fail($"The server url cannot be null when using the `{creationModeName}` client creation mode.");
        }

        if (!options.ServerUrl.IsAbsoluteUri)
        {
            return Fail(
                $"The server url must be an absolute url when using the `{creationModeName}` client creation mode."
            );
        }

        return Success;
    }
}
