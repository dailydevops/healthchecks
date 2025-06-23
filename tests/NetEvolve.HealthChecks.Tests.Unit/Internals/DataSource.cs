namespace NetEvolve.HealthChecks.Tests.Unit.Internals;

public static class DataSource
{
    public static string?[] NullOrWhitespaces() => [null, string.Empty, "\t", "    "];

    public static int[] TimeoutsInvalid() => [-2, int.MinValue];

    public static int[] TimeoutsValid() => [-1, 0, int.MaxValue];
}
