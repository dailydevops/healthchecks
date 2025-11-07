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

    public ValidateOptionsResult Validate(string? name, EventHubsOptions options) =>
        EventHubsOptions.InternalValidate(name, options) ?? Success;
}
