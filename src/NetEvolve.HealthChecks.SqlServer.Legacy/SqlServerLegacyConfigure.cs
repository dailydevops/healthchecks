﻿namespace NetEvolve.HealthChecks.SqlServer.Legacy;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class SqlServerLegacyConfigure
    : IConfigureNamedOptions<SqlServerLegacyOptions>,
        IPostConfigureOptions<SqlServerLegacyOptions>,
        IValidateOptions<SqlServerLegacyOptions>
{
    private readonly IConfiguration _configuration;

    public SqlServerLegacyConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, SqlServerLegacyOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:SqlServer:{name}", options);
    }

    public void Configure(SqlServerLegacyOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, SqlServerLegacyOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = SqlServerLegacyHealthCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, SqlServerLegacyOptions options)
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
