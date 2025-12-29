namespace NetEvolve.HealthChecks.Azure.Kusto;

using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class KustoAvailableConfigure
    : IConfigureNamedOptions<KustoAvailableOptions>,
        IValidateOptions<KustoAvailableOptions>
{
    private readonly IConfiguration _configuration;

    public KustoAvailableConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, KustoAvailableOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureKusto:{name}", options);
    }

    public void Configure(KustoAvailableOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, KustoAvailableOptions options)
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

        if (string.IsNullOrWhiteSpace(options.ConnectionString) && options.ClusterUri is null)
        {
            return Fail("Either ConnectionString or ClusterUri must be provided.");
        }

        if (options.ClusterUri?.IsAbsoluteUri == false)
        {
            return Fail("The ClusterUri must be an absolute URI.");
        }

        return Success;
    }
}
