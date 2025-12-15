namespace NetEvolve.HealthChecks.Tests.Integration.Apache.Solr;

using System;
using System.Threading.Tasks;
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

    [Test]
    public async Task AddSolr_WithOptionsServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddSolr(
                    nameof(AddSolr_WithOptionsServiceProvider_Healthy),
                    options => options.Timeout = 10000
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSolrNet<string>(_database.Url)
        );

    [Test]
    public async Task AddSolr_WithOptionsServiceProvider_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddSolr(
                    nameof(AddSolr_WithOptionsServiceProvider_Degraded),
                    options => options.Timeout = 0
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSolrNet(_database.Url)
        );

    [Test]
    public async Task AddSolr_WithOptionsCreate_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddSolr(
                    nameof(AddSolr_WithOptionsCreate_Healthy),
                    options =>
                    {
                        options.CreationMode = ClientCreationMode.Create;
                        options.BaseUrl = _database.Url;
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddSolr_WithOptionsCreate_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddSolr(
                    nameof(AddSolr_WithOptionsCreate_Degraded),
                    options =>
                    {
                        options.CreationMode = ClientCreationMode.Create;
                        options.BaseUrl = _database.Url;
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded
        );
}
