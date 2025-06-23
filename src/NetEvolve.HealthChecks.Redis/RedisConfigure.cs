namespace NetEvolve.HealthChecks.Redis;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class RedisConfigure : IConfigureNamedOptions<RedisOptions>, IValidateOptions<RedisOptions>
{
    private readonly IConfiguration _configuration;

    public RedisConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, RedisOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:RedisDatabase:{name}", options);
    }

    public void Configure(RedisOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, RedisOptions options)
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

        if (options.Mode == ConnectionHandleMode.Create && string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail("The property ConnectionString cannot be null or whitespace.");
        }

        return Success;
    }
}
