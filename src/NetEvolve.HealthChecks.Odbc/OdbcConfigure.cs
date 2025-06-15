namespace NetEvolve.HealthChecks.Odbc;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class OdbcConfigure
    : IConfigureNamedOptions<OdbcOptions>,
        IPostConfigureOptions<OdbcOptions>,
        IValidateOptions<OdbcOptions>
{
    private readonly IConfiguration _configuration;

    public OdbcConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, OdbcOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Odbc:{name}", options);
    }

    public void Configure(OdbcOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, OdbcOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = OdbcHealthCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, OdbcOptions options)
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
