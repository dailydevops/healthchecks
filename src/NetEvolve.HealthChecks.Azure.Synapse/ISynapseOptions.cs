namespace NetEvolve.HealthChecks.Azure.Synapse;

using System;

internal interface ISynapseOptions
{
    Uri? WorkspaceUri { get; }

    string? ConnectionString { get; }

    SynapseClientCreationMode? Mode { get; }

    int Timeout { get; }
}