namespace NetEvolve.HealthChecks.Dapr;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class DaprConfigure : IConfigureNamedOptions<DaprOptions>, IValidateOptions<DaprOptions>
{
    private readonly IConfiguration _configuration;

    public DaprConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, DaprOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:{name}", options);
    }

    public void Configure(DaprOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, DaprOptions options)
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
            return Fail("The timeout cannot be less than infinite (-1).");
        }

        return Success;
    }
}
