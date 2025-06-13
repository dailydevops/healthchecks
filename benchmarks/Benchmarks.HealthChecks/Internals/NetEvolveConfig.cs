namespace Benchmarks.HealthChecks.Internals;

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

internal sealed class NetEvolveConfig : ManualConfig
{
    public NetEvolveConfig()
    {
        _ = AddJob(Job.Default)
            .AddColumnProvider(DefaultColumnProviders.Instance)
            .AddColumn(BaselineRatioColumn.RatioMean, BaselineAllocationRatioColumn.RatioMean)
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddExporter(MarkdownExporter.GitHub)
            .AddLogger(ConsoleLogger.Default);

        SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend);
    }
}
