namespace NetEvolve.HealthChecks.SQLite.Legacy;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class SQLiteLegacyConfigure
    : IConfigureNamedOptions<SQLiteLegacyOptions>,
        IPostConfigureOptions<SQLiteLegacyOptions>,
        IValidateOptions<SQLiteLegacyOptions>
{
    private readonly IConfiguration _configuration;

    public SQLiteLegacyConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, SQLiteLegacyOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:SQLite:{name}", options);
    }

    public void Configure(SQLiteLegacyOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, SQLiteLegacyOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = SQLiteLegacyHealthCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, SQLiteLegacyOptions options)
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
