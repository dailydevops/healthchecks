namespace NetEvolve.HealthChecks.Azure.Synapse;

using System;
using System.Linq;
using System.Threading;
using global::Azure.Analytics.Synapse.Artifacts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class SynapseWorkspaceAvailableConfigure
    : IConfigureNamedOptions<SynapseWorkspaceAvailableOptions>,
        IValidateOptions<SynapseWorkspaceAvailableOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public SynapseWorkspaceAvailableConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, SynapseWorkspaceAvailableOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureSynapse:{name}", options);
    }

    public void Configure(SynapseWorkspaceAvailableOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, SynapseWorkspaceAvailableOptions options)
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

        var mode = options.Mode;

        return options.Mode switch
        {
            SynapseClientCreationMode.ServiceProvider => ValidateModeServiceProvider(),
            SynapseClientCreationMode.ConnectionString => ValidateModeConnectionString(options),
            SynapseClientCreationMode.DefaultAzureCredentials => ValidateModeDefaultAzureCredentials(options),
            _ => Fail($"The mode `{mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(SynapseWorkspaceAvailableOptions options)
    {
        if (options.WorkspaceUri is null)
        {
            return Fail(
                $"The workspace uri cannot be null when using `{nameof(SynapseClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        if (!options.WorkspaceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The workspace uri must be an absolute uri when using `{nameof(SynapseClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeConnectionString(SynapseWorkspaceAvailableOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail(
                $"The connection string cannot be null or whitespace when using `{nameof(SynapseClientCreationMode.ConnectionString)}` mode."
            );
        }

        // Validate that connection string contains an Endpoint parameter
        try
        {
            var parts = options.ConnectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var endpointPart = parts.FirstOrDefault(p => p.Trim().StartsWith("Endpoint=", StringComparison.OrdinalIgnoreCase));
            
            if (endpointPart is null)
            {
                return Fail(
                    "The connection string must contain an 'Endpoint=' parameter when using ConnectionString mode."
                );
            }

            var endpointValue = endpointPart.Split('=', 2)[1];
            if (!Uri.TryCreate(endpointValue, UriKind.Absolute, out _))
            {
                return Fail(
                    "The Endpoint in the connection string must be a valid absolute URI."
                );
            }
        }
        catch (Exception)
        {
            return Fail(
                "Invalid connection string format. Expected format: 'Endpoint=https://myworkspace.dev.azuresynapse.net;'"
            );
        }

        return Success;
    }

    private ValidateOptionsResult ValidateModeServiceProvider()
    {
        if (_serviceProvider.GetService<ArtifactsClient>() is null)
        {
            return Fail(
                $"No service of type `{nameof(ArtifactsClient)}` registered. Please execute `builder.AddAzureClients()`."
            );
        }

        return Success;
    }
}