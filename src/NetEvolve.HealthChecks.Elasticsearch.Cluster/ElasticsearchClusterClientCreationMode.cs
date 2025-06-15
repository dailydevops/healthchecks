namespace NetEvolve.HealthChecks.Elasticsearch.Cluster;

using System;
using Elastic.Clients.Elasticsearch;

/// <summary>
/// Describes the mode used to create the <see cref="ElasticsearchClient"/>.
/// </summary>
public enum ElasticsearchClusterClientCreationMode
{
    /// <summary>
    /// The <see cref="ElasticsearchClient"/> preregistered instance is retrieved from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <remarks>
    /// This is the default mode.
    /// </remarks>
    ServiceProvider = 0,

    /// <summary>
    /// The <see cref="ElasticsearchClient"/> instance is created using the <see cref="ElasticsearchClusterOptions.ConnectionStrings"/>,
    /// the <see cref="ElasticsearchClusterOptions.Username"/> and the <see cref="ElasticsearchClusterOptions.Password"/>.
    /// </summary>
    Internal = 1,
}
