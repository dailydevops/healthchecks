namespace NetEvolve.HealthChecks.Meilisearch;

using System;
using System.Threading;
using Meilisearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class MeilisearchConfigure
    : IConfigureNamedOptions<MeilisearchOptions>,
        IValidateOptions<MeilisearchOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeilisearchConfigure"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to bind configuration values.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance used to retrieve a client instance.</param>
    public MeilisearchConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public void Configure(string? name, MeilisearchOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Meilisearch:{name}", options);
    }

    /// <inheritdoc />
    public void Configure(MeilisearchOptions options) => Configure(Options.DefaultName, options);

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, MeilisearchOptions options)
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
            return Fail(
                "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout."
            );
        }

        return options.Mode switch
        {
            MeilisearchClientCreationMode.ServiceProvider
                => ValidateCreationModeServiceProvider(options),
            MeilisearchClientCreationMode.Internal => ValidateCreationModeInternal(options),
            _
                => Fail($"The mode `{options.Mode}` is not supported."),
        };
    }

    private ValidateOptionsResult ValidateCreationModeServiceProvider(MeilisearchOptions options)
    {
        var client = options.KeyedService is null
            ? _serviceProvider.GetService<MeilisearchClient>()
            : _serviceProvider.GetKeyedService<MeilisearchClient>(options.KeyedService);

        if (client is null)
        {
            return Fail(
                $"No service of type `{nameof(MeilisearchClient)}` registered. Please execute `services.AddSingleton<MeilisearchClient>()`."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateCreationModeInternal(MeilisearchOptions options)
    {
        const string creationModeName = nameof(MeilisearchClientCreationMode.Internal);

        if (string.IsNullOrWhiteSpace(options.Host))
        {
            return Fail(
                $"The host cannot be null or whitespace when using the `{creationModeName}` client creation mode."
            );
        }

        return Success;
    }
}
