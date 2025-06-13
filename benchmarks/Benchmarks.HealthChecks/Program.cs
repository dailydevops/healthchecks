using BenchmarkDotNet.Running;
using Benchmarks.HealthChecks.Internals;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new NetEvolveConfig());
