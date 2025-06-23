namespace NetEvolve.HealthChecks.Elasticsearch;

using System;
using System.Threading;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class ElasticsearchConfigure
    : IConfigureNamedOptions<ElasticsearchOptions>,
        IValidateOptions<ElasticsearchOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchConfigure"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to bind configuration values.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance used to retrieve a client instance.</param>
    public ElasticsearchConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public void Configure(string? name, ElasticsearchOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Elasticsearch:{name}", options);
    }

    /// <inheritdoc />
    public void Configure(ElasticsearchOptions options) => Configure(Options.DefaultName, options);

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, ElasticsearchOptions options)
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
            ElasticsearchClientCreationMode.ServiceProvider => ValidateCreationModeServiceProvider(options),
            ElasticsearchClientCreationMode.UsernameAndPassword => ValidateCreationModeUsernameAndPassword(options),
            _ => Fail($"The mode `{options.Mode}` is not supported."),
        };
    }

    private ValidateOptionsResult ValidateCreationModeServiceProvider(ElasticsearchOptions options)
    {
        var client = options.KeyedService is null
            ? _serviceProvider.GetService<ElasticsearchClient>()
            : _serviceProvider.GetKeyedService<ElasticsearchClient>(options.KeyedService);

        if (client is null)
        {
            return Fail(
                $"No service of type `{nameof(ElasticsearchClient)}` registered. Please execute `services.AddSingleton<ElasticsearchClient>()`."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateCreationModeUsernameAndPassword(ElasticsearchOptions options)
    {
        const string creationModeName = nameof(ElasticsearchClientCreationMode.UsernameAndPassword);

        if (!options.ConnectionStrings.Any())
        {
            return Fail(
                $"The connection strings list cannot be empty when using the `{creationModeName}` client creation mode."
            );
        }

        if (options.ConnectionStrings.Any(string.IsNullOrWhiteSpace))
        {
            return Fail(
                $"The connection strings list cannot contain a null or empty entry when using the `{creationModeName}` client creation mode."
            );
        }

        var usernameNullOrWhitespace = string.IsNullOrWhiteSpace(options.Username);
        var passwordNullOrWhitespace = string.IsNullOrWhiteSpace(options.Password);

        if (usernameNullOrWhitespace && !passwordNullOrWhitespace)
        {
            return Fail(
                $"The username cannot be null or whitespace when using the `{creationModeName}` client creation mode with a password."
            );
        }

        if (passwordNullOrWhitespace && !usernameNullOrWhitespace)
        {
            return Fail(
                $"The password cannot be null or whitespace when using the `{creationModeName}` client creation mode with a username."
            );
        }

        return Success;
    }
}
