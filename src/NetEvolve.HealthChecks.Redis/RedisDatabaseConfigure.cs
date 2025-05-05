namespace NetEvolve.HealthChecks.Redis;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class RedisDatabaseConfigure
    : IConfigureNamedOptions<RedisDatabaseOptions>,
        IValidateOptions<RedisDatabaseOptions>
{
    private readonly IConfiguration _configuration;

    public RedisDatabaseConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, RedisDatabaseOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:RedisDatabase:{name}", options);
    }

    public void Configure(RedisDatabaseOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, RedisDatabaseOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (options.Timeout < -1)
        {
            return Fail("The property Timeout cannot be less than -1.");
        }

        if (options.Mode == ConnectionHandleMode.Create && string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail("The property ConnectionString cannot be null or whitespace.");
        }

        return Success;
    }
}
