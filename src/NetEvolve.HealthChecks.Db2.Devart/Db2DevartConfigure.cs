namespace NetEvolve.HealthChecks.Db2.Devart;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class Db2DevartConfigure
    : IConfigureNamedOptions<Db2DevartOptions>,
        IPostConfigureOptions<Db2DevartOptions>,
        IValidateOptions<Db2DevartOptions>
{
    private readonly IConfiguration _configuration;

    public Db2DevartConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, Db2DevartOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:DB2:{name}", options);
    }

    public void Configure(Db2DevartOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, Db2DevartOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = Db2DevartHealthCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, Db2DevartOptions options)
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
            return Fail("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
        }

        return Success;
    }
}