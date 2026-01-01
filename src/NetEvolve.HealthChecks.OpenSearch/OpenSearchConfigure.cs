namespace NetEvolve.HealthChecks.OpenSearch;

using System;
using System.Threading;
using global::OpenSearch.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class OpenSearchConfigure
    : IConfigureNamedOptions<OpenSearchOptions>,
        IValidateOptions<OpenSearchOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchConfigure"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to bind configuration values.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance used to retrieve a client instance.</param>
    public OpenSearchConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public void Configure(string? name, OpenSearchOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:OpenSearch:{name}", options);
    }

    /// <inheritdoc />
    public void Configure(OpenSearchOptions options) => Configure(Options.DefaultName, options);

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, OpenSearchOptions options)
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
            OpenSearchClientCreationMode.ServiceProvider => ValidateCreationModeServiceProvider(options),
            OpenSearchClientCreationMode.UsernameAndPassword => ValidateCreationModeUsernameAndPassword(options),
            _ => Fail($"The mode `{options.Mode}` is not supported."),
        };
    }

    private ValidateOptionsResult ValidateCreationModeServiceProvider(OpenSearchOptions options)
    {
        var client = options.KeyedService is null
            ? _serviceProvider.GetService<OpenSearchClient>()
            : _serviceProvider.GetKeyedService<OpenSearchClient>(options.KeyedService);

        if (client is null)
        {
            return Fail(
                $"No service of type `{nameof(OpenSearchClient)}` registered. Please execute `services.AddSingleton<OpenSearchClient>()`."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateCreationModeUsernameAndPassword(OpenSearchOptions options)
    {
        const string creationModeName = nameof(OpenSearchClientCreationMode.UsernameAndPassword);

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
