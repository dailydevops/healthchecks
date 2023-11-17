namespace NetEvolve.HealthChecks.MySql;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class MySqlConfigure
    : IConfigureNamedOptions<MySqlOptions>,
        IPostConfigureOptions<MySqlOptions>,
        IValidateOptions<MySqlOptions>
{
    private readonly IConfiguration _configuration;

    public MySqlConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, MySqlOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:MySql:{name}", options);
    }

    public void Configure(MySqlOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, MySqlOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = MySqlCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, MySqlOptions options)
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
