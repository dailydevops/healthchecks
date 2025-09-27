namespace NetEvolve.HealthChecks.Azure.IotHub;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class IotHubAvailabilityOptionsConfigure
    : IConfigureNamedOptions<IotHubAvailabilityOptions>,
        IValidateOptions<IotHubAvailabilityOptions>
{
    private readonly IConfiguration _configuration;

    public IotHubAvailabilityOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, IotHubAvailabilityOptions options) =>
        _configuration.Bind($"HealthChecks:AzureIotHubAvailability:{name}", options);

    public void Configure(IotHubAvailabilityOptions options) => Configure(null, options);

    public ValidateOptionsResult Validate(string? name, IotHubAvailabilityOptions options) =>
        IotHubOptionsBase.InternalValidate(name, options) ?? Success;
}
