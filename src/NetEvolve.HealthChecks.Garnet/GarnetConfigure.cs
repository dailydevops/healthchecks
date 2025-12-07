namespace NetEvolve.HealthChecks.Garnet;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class GarnetConfigure : IConfigureNamedOptions<GarnetOptions>, IValidateOptions<GarnetOptions>
{
    private readonly IConfiguration _configuration;

    public GarnetConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, GarnetOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:GarnetDatabase:{name}", options);
    }

    public void Configure(GarnetOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, GarnetOptions options)
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
