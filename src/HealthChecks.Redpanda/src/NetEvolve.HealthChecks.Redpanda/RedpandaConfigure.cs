namespace NetEvolve.HealthChecks.Redpanda;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class RedpandaConfigure
    : IConfigureNamedOptions<RedpandaOptions>,
        IValidateOptions<RedpandaOptions>
{
    private readonly IConfiguration _configuration;

    public RedpandaConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, RedpandaOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Redpanda:{name}", options);
    }

    public void Configure(RedpandaOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, RedpandaOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(options.Topic))
        {
            return Fail("The topic cannot be null or whitespace.");
        }

        if (options.Mode == ProducerHandleMode.Create)
        {
            var configuration = options.Configuration;
            if (configuration is null)
            {
                return Fail("The configuration cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(configuration.BootstrapServers))
            {
                return Fail("The property BootstrapServers cannot be null or whitespace.");
            }
        }

        return Success;
    }
}
