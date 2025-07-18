﻿namespace NetEvolve.HealthChecks.SqlServer.Devart;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class SqlServerDevartConfigure
    : IConfigureNamedOptions<SqlServerDevartOptions>,
        IPostConfigureOptions<SqlServerDevartOptions>,
        IValidateOptions<SqlServerDevartOptions>
{
    private readonly IConfiguration _configuration;

    public SqlServerDevartConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, SqlServerDevartOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:SqlServer:{name}", options);
    }

    public void Configure(SqlServerDevartOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, SqlServerDevartOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = SqlServerDevartHealthCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, SqlServerDevartOptions options)
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
