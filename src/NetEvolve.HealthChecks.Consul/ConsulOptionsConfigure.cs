namespace NetEvolve.HealthChecks.Consul;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class ConsulOptionsConfigure : IConfigureNamedOptions<ConsulOptions>, IValidateOptions<ConsulOptions>
{
    private readonly IConfiguration _configuration;

    public ConsulOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, ConsulOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Consul:{name}", options);
    }

    public void Configure(ConsulOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, ConsulOptions options)
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

        return Success;
    }
}
