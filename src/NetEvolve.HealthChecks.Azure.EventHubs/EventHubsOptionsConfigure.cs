namespace NetEvolve.HealthChecks.Azure.EventHubs;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal class EventHubsOptionsConfigure : IConfigureNamedOptions<EventHubsOptions>, IValidateOptions<EventHubsOptions>
{
    private readonly IConfiguration _configuration;

    public EventHubsOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, EventHubsOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureEventHubs:{name}", options);
    }

    public void Configure(EventHubsOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, EventHubsOptions options)
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

        if (options.Mode is null)
        {
            return Fail("The client creation mode cannot be null.");
        }
        else if (
            options.Mode is ClientCreationMode.DefaultAzureCredentials
            && string.IsNullOrWhiteSpace(options.FullyQualifiedNamespace)
        )
        {
            return Fail(
                "The fully qualified namespace cannot be null or whitespace when using DefaultAzureCredentials."
            );
        }
        else if (
            options.Mode is ClientCreationMode.ConnectionString
            && string.IsNullOrWhiteSpace(options.ConnectionString)
        )
        {
            return Fail("The connection string cannot be null or whitespace when using ConnectionString.");
        }

        if (string.IsNullOrWhiteSpace(options.EventHubName))
        {
            return Fail("The event hub name cannot be null or whitespace.");
        }

        return Success;
    }
}
