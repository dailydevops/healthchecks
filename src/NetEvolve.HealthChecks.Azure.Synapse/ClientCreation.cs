namespace NetEvolve.HealthChecks.Azure.Synapse;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using global::Azure.Analytics.Synapse.Artifacts;
using global::Azure.Identity;
using Microsoft.Extensions.DependencyInjection;

internal class ClientCreation
{
    private ConcurrentDictionary<string, ArtifactsClient>? _artifactsClients;

    internal ArtifactsClient GetArtifactsClient<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, ISynapseOptions
    {
        if (options.Mode == SynapseClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<ArtifactsClient>();
        }

        _artifactsClients ??= new ConcurrentDictionary<string, ArtifactsClient>(StringComparer.OrdinalIgnoreCase);

        return _artifactsClients.GetOrAdd(name, _ => CreateArtifactsClient(options, serviceProvider));
    }

    internal static ArtifactsClient CreateArtifactsClient<TOptions>(
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, ISynapseOptions
    {
        switch (options.Mode)
        {
            case SynapseClientCreationMode.DefaultAzureCredentials:
                var tokenCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new ArtifactsClient(options.WorkspaceUri, tokenCredential);
            case SynapseClientCreationMode.ConnectionString:
                // Note: For connection string mode, we'll need to parse the connection string to extract workspace URI
                // and use DefaultAzureCredential for authentication
                var credential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new ArtifactsClient(options.WorkspaceUri, credential);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
    }
}