namespace NetEvolve.HealthChecks.Firebird;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class FirebirdConfigure
    : IConfigureNamedOptions<FirebirdOptions>,
        IPostConfigureOptions<FirebirdOptions>,
        IValidateOptions<FirebirdOptions>
{
    private readonly IConfiguration _configuration;

    public FirebirdConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, FirebirdOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Firebird:{name}", options);
    }

    public void Configure(FirebirdOptions options) => Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, FirebirdOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.Command))
        {
            options.Command = FirebirdHealthCheck.DefaultCommand;
        }
    }

    public ValidateOptionsResult Validate(string? name, FirebirdOptions options)
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
