namespace NetEvolve.HealthChecks.Azure.EventHubs;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

internal sealed class EventHubOptionsConfigure
    : IConfigureNamedOptions<EventHubOptions>,
        IValidateOptions<EventHubOptions>
{
    private readonly IConfiguration _configuration;

    public EventHubOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, EventHubOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureEventHub:{name}", options);
    }

    public void Configure(EventHubOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, EventHubOptions options)
    {
        var validationResult = EventHubsOptionsBase.InternalValidate(name, options);
        if (validationResult is not null)
        {
            return validationResult;
        }

        if (options.Mode is not ClientCreationMode.ServiceProvider && string.IsNullOrWhiteSpace(options.EventHubName))
        {
            return ValidateOptionsResult.Fail("The Event Hub name cannot be null or whitespace.");
        }

        return ValidateOptionsResult.Success;
    }
}
