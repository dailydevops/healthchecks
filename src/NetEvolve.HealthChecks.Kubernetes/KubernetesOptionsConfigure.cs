namespace NetEvolve.HealthChecks.Kubernetes;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class KubernetesOptionsConfigure
    : IConfigureNamedOptions<KubernetesOptions>,
        IValidateOptions<KubernetesOptions>
{
    private readonly IConfiguration _configuration;

    public KubernetesOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, KubernetesOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Kubernetes:{name}", options);
    }

    public void Configure(KubernetesOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, KubernetesOptions options)
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
            return Fail("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
        }

        return Success;
    }
}
