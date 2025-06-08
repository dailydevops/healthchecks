namespace NetEvolve.HealthChecks.ArangoDb;

using System;
using System.Threading;
using ArangoDBNetStandard;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class ArangoDbConfigure : IConfigureNamedOptions<ArangoDbOptions>, IValidateOptions<ArangoDbOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArangoDbConfigure"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to bind configuration values.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance used to retrieve a client instance.</param>
    public ArangoDbConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public void Configure(string? name, ArangoDbOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:ArangoDb:{name}", options);
    }

    /// <inheritdoc />
    public void Configure(ArangoDbOptions options) => Configure(Options.DefaultName, options);

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, ArangoDbOptions options)
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
            return Fail("The timeout cannot be less than infinite (-1).");
        }

        return options.Mode switch
        {
            ArangoDbClientCreationMode.ServiceProvider => ValidateCreationModeServiceProvider(options),
            ArangoDbClientCreationMode.Internal => ValidateCreationModeInternal(options),
            _ => Fail($"The mode `{options.Mode}` is not supported."),
        };
    }

    private ValidateOptionsResult ValidateCreationModeServiceProvider(ArangoDbOptions options)
    {
        var client = options.KeyedService is null
            ? _serviceProvider.GetService<ArangoDBClient>()
            : _serviceProvider.GetKeyedService<ArangoDBClient>(options.KeyedService);

        if (client is null)
        {
            return Fail(
                $"No service of type `{nameof(ArangoDBClient)}` registered. Please execute `services.AddSingleton<ArangoDBClient>()`."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateCreationModeInternal(ArangoDbOptions options)
    {
        const string creationModeName = nameof(ArangoDbClientCreationMode.Internal);

        if (string.IsNullOrWhiteSpace(options.TransportAddress))
        {
            return Fail(
                $"The transport address cannot be null or whitespace when using the `{creationModeName}` client creation mode."
            );
        }

        if (options.Username is not null && options.Password is null)
        {
            return Fail(
                $"The password cannot be null when using the `{creationModeName}` client creation mode with a username."
            );
        }

        if (options.Username is null && options.Password is not null)
        {
            return Fail(
                $"The username cannot be null when using the `{creationModeName}` client creation mode with a password."
            );
        }

        return Success;
    }
}
