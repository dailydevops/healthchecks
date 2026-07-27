namespace NetEvolve.HealthChecks.Apache.RocketMQ;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

/// <summary>
/// Validation and configuration class for <see cref="RocketMQOptions"/>.
/// </summary>
internal sealed class RocketMQConfigure : IConfigureNamedOptions<RocketMQOptions>, IValidateOptions<RocketMQOptions>
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="RocketMQConfigure"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to bind configuration values.</param>
    public RocketMQConfigure(IConfiguration configuration) => _configuration = configuration;

    /// <inheritdoc />
    public void Configure(string? name, RocketMQOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:RocketMQ:{name}", options);
    }

    /// <inheritdoc />
    public void Configure(RocketMQOptions options) => Configure(Options.DefaultName, options);

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, RocketMQOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(options.Endpoint))
        {
            return Fail("The endpoint cannot be null or whitespace.");
        }

        if (string.IsNullOrWhiteSpace(options.Topic))
        {
            return Fail("The topic cannot be null or whitespace.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return Fail("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
        }

        return Success;
    }
}
