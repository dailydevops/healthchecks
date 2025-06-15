namespace NetEvolve.HealthChecks.DB2;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class DB2Configure
    : IConfigureNamedOptions<DB2Options>,
        IPostConfigureOptions<DB2Options>,
        IValidateOptions<DB2Options>
{
    private readonly IConfiguration _configuration;

    public DB2Configure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, DB2Options options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:DB2:{name}", options);
    }

    public void Configure(DB2Options options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, DB2Options options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = DB2HealthCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, DB2Options options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail("The connection string cannot be null or whitespace.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return Fail("The timeout cannot be less than infinite (-1).");
        }

        return Success;
    }
}
