﻿namespace NetEvolve.HealthChecks.SqlEdge;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class SqlEdgeConfigure
    : IConfigureNamedOptions<SqlEdgeOptions>,
        IPostConfigureOptions<SqlEdgeOptions>,
        IValidateOptions<SqlEdgeOptions>
{
    private readonly IConfiguration _configuration;

    public SqlEdgeConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, SqlEdgeOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:SqlEdge:{name}", options);
    }

    public void Configure(SqlEdgeOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, SqlEdgeOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = SqlEdgeCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, SqlEdgeOptions options)
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
