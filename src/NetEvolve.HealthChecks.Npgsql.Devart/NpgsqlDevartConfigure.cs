namespace NetEvolve.HealthChecks.Npgsql.Devart;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class NpgsqlDevartConfigure
    : IConfigureNamedOptions<NpgsqlDevartOptions>,
        IPostConfigureOptions<NpgsqlDevartOptions>,
        IValidateOptions<NpgsqlDevartOptions>
{
    private readonly IConfiguration _configuration;

    public NpgsqlDevartConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, NpgsqlDevartOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Npgsql:{name}", options);
    }

    public void Configure(NpgsqlDevartOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, NpgsqlDevartOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = NpgsqlDevartHealthCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, NpgsqlDevartOptions options)
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
