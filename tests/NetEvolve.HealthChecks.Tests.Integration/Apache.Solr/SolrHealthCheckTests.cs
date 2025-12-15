namespace NetEvolve.HealthChecks.Tests.Integration.Apache.Solr;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Apache.Solr;
using SolrNet;

[TestGroup($"{nameof(Apache)}.{nameof(Solr)}")]
[TestGroup("Z04TestGroup")]
[ClassDataSource<SolrContainer>(Shared = SharedType.PerClass)]
public class SolrHealthCheckTests : HealthCheckTestBase
{
    private readonly SolrContainer _database;

    public SolrHealthCheckTests(SolrContainer database) => _database = database;
}
