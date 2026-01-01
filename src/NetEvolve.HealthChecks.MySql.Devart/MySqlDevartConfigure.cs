namespace NetEvolve.HealthChecks.MySql.Devart;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class MySqlDevartConfigure
    : IConfigureNamedOptions<MySqlDevartOptions>,
        IPostConfigureOptions<MySqlDevartOptions>,
        IValidateOptions<MySqlDevartOptions>
{
    private readonly IConfiguration _configuration;

    public MySqlDevartConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, MySqlDevartOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:MySql:{name}", options);
    }

    public void Configure(MySqlDevartOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, MySqlDevartOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = MySqlDevartHealthCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, MySqlDevartOptions options)
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
