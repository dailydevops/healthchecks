namespace NetEvolve.HealthChecks.DB2.Devart;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class DB2DevartConfigure
    : IConfigureNamedOptions<DB2DevartOptions>,
        IPostConfigureOptions<DB2DevartOptions>,
        IValidateOptions<DB2DevartOptions>
{
    private readonly IConfiguration _configuration;

    public DB2DevartConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, DB2DevartOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:DB2:{name}", options);
    }

    public void Configure(DB2DevartOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, DB2DevartOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = DB2DevartHealthCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, DB2DevartOptions options)
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
