namespace NetEvolve.HealthChecks.Tests.Integration.Elasticsearch.Cluster.ContainerCluster;

using System;
using System.Collections.Generic;

public interface IContainerCluster : IAsyncInitializer, IAsyncDisposable
{
    IEnumerable<string> ConnectionStrings { get; }

    string Username { get; }

    string Password { get; }
}
