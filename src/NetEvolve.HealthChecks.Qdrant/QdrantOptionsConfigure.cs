namespace NetEvolve.HealthChecks.Qdrant;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class QdrantOptionsConfigure : IConfigureNamedOptions<QdrantOptions>, IValidateOptions<QdrantOptions>
{
    private readonly IConfiguration _configuration;

    public QdrantOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, QdrantOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Qdrant:{name}", options);
    }

    public void Configure(QdrantOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, QdrantOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return Fail("The timeout cannot be less than infinite (-1).");
        }

        return Success;
    }
}
