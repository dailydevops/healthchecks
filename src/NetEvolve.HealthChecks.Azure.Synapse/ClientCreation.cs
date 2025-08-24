namespace NetEvolve.HealthChecks.Azure.Synapse;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
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
                // For connection string mode, we extract the workspace URI from the connection string
                // and use DefaultAzureCredential for authentication
                var workspaceUri = ExtractWorkspaceUriFromConnectionString(options.ConnectionString);
                var credential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new ArtifactsClient(workspaceUri, credential);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
    }

    private static Uri ExtractWorkspaceUriFromConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
        }

        // Simple connection string parsing for Synapse
        // Expected format: "Endpoint=https://myworkspace.dev.azuresynapse.net;..."
        var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        var endpointPart = parts.FirstOrDefault(p => p.Trim().StartsWith("Endpoint=", StringComparison.OrdinalIgnoreCase));
        
        if (endpointPart is null)
        {
            throw new ArgumentException("Connection string must contain an 'Endpoint=' parameter.", nameof(connectionString));
        }

        var endpointValue = endpointPart.Split('=', 2)[1];
        return new Uri(endpointValue);
    }
}