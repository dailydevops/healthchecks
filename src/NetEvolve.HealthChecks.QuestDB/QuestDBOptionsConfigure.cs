namespace NetEvolve.HealthChecks.QuestDB;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class QuestDBOptionsConfigure : IConfigureNamedOptions<QuestDBOptions>, IValidateOptions<QuestDBOptions>
{
    private readonly IConfiguration _configuration;

    public QuestDBOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, QuestDBOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:QuestDB:{name}", options);
    }

    public void Configure(QuestDBOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, QuestDBOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(options.StatusUri))
        {
            return Fail("The status uri cannot be null or whitespace.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return Fail("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
        }

        return Success;
    }
}
