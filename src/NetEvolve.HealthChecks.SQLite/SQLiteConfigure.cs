namespace NetEvolve.HealthChecks.SQLite;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class SQLiteConfigure
    : IConfigureNamedOptions<SQLiteOptions>,
        IPostConfigureOptions<SQLiteOptions>,
        IValidateOptions<SQLiteOptions>
{
    private readonly IConfiguration _configuration;

    public SQLiteConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, SQLiteOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:SQLite:{name}", options);
    }

    public void Configure(SQLiteOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, SQLiteOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = SQLiteHealthCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, SQLiteOptions options)
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
