namespace NetEvolve.HealthChecks.Apache.ActiveMq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class ActiveMqConfigure
    : IConfigureNamedOptions<ActiveMqOptions>,
        IValidateOptions<ActiveMqOptions>
{
    private readonly IConfiguration _configuration;

    public ActiveMqConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, ActiveMqOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:ActiveMq:{name}", options);
    }

    public void Configure(ActiveMqOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, ActiveMqOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(options.BrokerAddress))
        {
            return Fail("The broker address cannot be null or whitespace.");
        }

        return Success;
    }
}
