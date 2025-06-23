namespace NetEvolve.HealthChecks.Apache.Kafka;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class KafkaConfigure : IConfigureNamedOptions<KafkaOptions>, IValidateOptions<KafkaOptions>
{
    private readonly IConfiguration _configuration;

    public KafkaConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, KafkaOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Kafka:{name}", options);
    }

    public void Configure(KafkaOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, KafkaOptions options)
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
