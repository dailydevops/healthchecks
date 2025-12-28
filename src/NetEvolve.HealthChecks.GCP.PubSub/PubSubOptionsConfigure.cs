namespace NetEvolve.HealthChecks.GCP.PubSub;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class PubSubOptionsConfigure : IConfigureNamedOptions<PubSubOptions>, IValidateOptions<PubSubOptions>
{
    private readonly IConfiguration _configuration;

    public PubSubOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, PubSubOptions options)
    {
        ArgumentNullException.ThrowIfNull(name);

        _configuration.Bind($"HealthChecks:GCP:PubSub:{name}", options);
    }

    public void Configure(PubSubOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, PubSubOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(options.ProjectName))
        {
            return Fail("The GCP project name must be provided.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return Fail("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
        }

        return Success;
    }
}
