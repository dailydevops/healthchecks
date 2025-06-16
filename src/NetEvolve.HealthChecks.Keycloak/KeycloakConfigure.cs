namespace NetEvolve.HealthChecks.Keycloak;

using System.Threading;
using global::Keycloak.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class KeycloakConfigure : IConfigureNamedOptions<KeycloakOptions>, IValidateOptions<KeycloakOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakConfigure"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to bind configuration values.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance used to retrieve a client instance.</param>
    public KeycloakConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public void Configure(string? name, KeycloakOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Keycloak:{name}", options);
    }

    /// <inheritdoc />
    public void Configure(KeycloakOptions options) => Configure(Options.DefaultName, options);

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, KeycloakOptions options)
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
            KeycloakClientCreationMode.ServiceProvider => ValidateCreationModeServiceProvider(options),
            KeycloakClientCreationMode.UsernameAndPassword => ValidateCreationModeInternal(options),
            _ => Fail($"The mode `{options.Mode}` is not supported."),
        };
    }

    private ValidateOptionsResult ValidateCreationModeServiceProvider(KeycloakOptions options)
    {
        var client = options.KeyedService is null
            ? _serviceProvider.GetService<KeycloakClient>()
            : _serviceProvider.GetKeyedService<KeycloakClient>(options.KeyedService);

        if (client is null)
        {
            return Fail(
                $"No service of type `{nameof(KeycloakClient)}` registered. Please execute `services.AddSingleton(<client instance>)`."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateCreationModeInternal(KeycloakOptions options)
    {
        const string creationModeName = nameof(KeycloakClientCreationMode.UsernameAndPassword);

        if (string.IsNullOrWhiteSpace(options.BaseAddress))
        {
            return Fail(
                $"The base address cannot be null or whitespace when using the `{creationModeName}` client creation mode."
            );
        }

        if (options.Username is null)
        {
            return Fail($"The username cannot be null when using the `{creationModeName}` client creation mode.");
        }

        if (options.Password is null)
        {
            return Fail($"The password cannot be null when using the `{creationModeName}` client creation mode.");
        }

        return Success;
    }
}
