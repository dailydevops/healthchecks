﻿namespace NetEvolve.HealthChecks.DuckDB;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class DuckDBConfigure
    : IConfigureNamedOptions<DuckDBOptions>,
        IPostConfigureOptions<DuckDBOptions>,
        IValidateOptions<DuckDBOptions>
{
    private readonly IConfiguration _configuration;

    public DuckDBConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, DuckDBOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:DuckDB:{name}", options);
    }

    public void Configure(DuckDBOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, DuckDBOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = DuckDBHealthCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, DuckDBOptions options)
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
