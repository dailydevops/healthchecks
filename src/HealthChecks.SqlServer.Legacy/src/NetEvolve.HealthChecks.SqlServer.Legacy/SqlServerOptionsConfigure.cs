namespace NetEvolve.HealthChecks.SqlServer.Legacy;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using System.Threading;

using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class SqlServerOptionsConfigure
    : IConfigureNamedOptions<SqlServerOptions>,
        IPostConfigureOptions<SqlServerOptions>,
        IValidateOptions<SqlServerOptions>
{
    private readonly IConfiguration _configuration;

    public SqlServerOptionsConfigure(IConfiguration configuration) =>
        _configuration = configuration;

    public void Configure(string? name, SqlServerOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:{name}", options);
    }

    public void Configure(SqlServerOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string name, SqlServerOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = SqlServerCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, SqlServerOptions options)
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
