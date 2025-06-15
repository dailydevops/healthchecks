namespace NetEvolve.HealthChecks.Elasticsearch.Cluster;

using System;
using System.Linq;
using System.Threading;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class ElasticsearchClusterConfigure
    : IConfigureNamedOptions<ElasticsearchClusterOptions>,
        IValidateOptions<ElasticsearchClusterOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchClusterConfigure"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to bind configuration values.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance used to retrieve a client instance.</param>
    public ElasticsearchClusterConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public void Configure(string? name, ElasticsearchClusterOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:ElasticsearchCluster:{name}", options);
    }

    /// <inheritdoc />
    public void Configure(ElasticsearchClusterOptions options) => Configure(Options.DefaultName, options);

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, ElasticsearchClusterOptions options)
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
            ElasticsearchClusterClientCreationMode.ServiceProvider => ValidateCreationModeServiceProvider(options),
            ElasticsearchClusterClientCreationMode.Internal => ValidateCreationModeInternal(options),
            _ => Fail($"The mode `{options.Mode}` is not supported."),
        };
    }

    private ValidateOptionsResult ValidateCreationModeServiceProvider(ElasticsearchClusterOptions options)
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

    private static ValidateOptionsResult ValidateCreationModeInternal(ElasticsearchClusterOptions options)
    {
        const string creationModeName = nameof(ElasticsearchClusterClientCreationMode.Internal);

        if (options.ConnectionStrings?.Any() != true)
        {
            return Fail(
                $"The connection strings list cannot be null or empty when using the `{creationModeName}` client creation mode."
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
