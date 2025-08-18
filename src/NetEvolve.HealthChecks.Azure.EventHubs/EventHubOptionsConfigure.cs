namespace NetEvolve.HealthChecks.Azure.EventHubs;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class EventHubOptionsConfigure
    : ConfigurableOptionsBase<EventHubOptions, EventHubOptionsConfigure>
{
    public EventHubOptionsConfigure(IConfiguration configuration)
        : base(configuration) { }

    protected override string SectionName => "HealthChecks:AzureEventHub";

    protected override void Configure(EventHubOptions options, string name) =>
        options.Mode ??= ClientCreationMode.ServiceProvider;

    protected override ValidateOptionsResult? Validate(string? name, EventHubOptions options)
    {
        var validationResult = EventHubsOptionsBase.InternalValidate(name, options);
        if (validationResult is not null)
        {
            return validationResult;
        }

        if (
            options.Mode is not ClientCreationMode.ServiceProvider
            && string.IsNullOrWhiteSpace(options.EventHubName)
        )
        {
            return ValidateOptionsResult.Fail("The Event Hub name cannot be null or whitespace.");
        }

        return ValidateOptionsResult.Success;
    }
}