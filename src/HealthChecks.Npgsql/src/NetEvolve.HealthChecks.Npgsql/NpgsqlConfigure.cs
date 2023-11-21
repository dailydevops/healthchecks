﻿namespace NetEvolve.HealthChecks.Npgsql;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class NpgsqlConfigure
    : IConfigureNamedOptions<NpgsqlOptions>,
        IPostConfigureOptions<NpgsqlOptions>,
        IValidateOptions<NpgsqlOptions>
{
    private readonly IConfiguration _configuration;

    public NpgsqlConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, NpgsqlOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:PostgreSql:{name}", options);
    }

    public void Configure(NpgsqlOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, NpgsqlOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = NpgsqlCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, NpgsqlOptions options)
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
