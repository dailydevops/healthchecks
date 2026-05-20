namespace NetEvolve.HealthChecks.Tests.Integration.Firebird;

using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.FirebirdSql;

public sealed class FirebirdDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly FirebirdSqlContainer _database = new FirebirdSqlBuilder(
        /*dockerimage*/"jacobalberty/firebird:v4.0.2"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        Environment.SetEnvironmentVariable("USERNAME", ToAsciiUsername(), EnvironmentVariableTarget.Process);
        await _database.StartAsync().ConfigureAwait(false);
    }

    private static string ToAsciiUsername()
    {
        // Normalisierung: zerlegt "ü" in "u" + Combining Diaeresis
        var normalized = Environment
            .UserName.Replace("ß", "ss", StringComparison.Ordinal)
            .Normalize(NormalizationForm.FormD);

        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            // Nur Basis-Buchstaben behalten, Combining-Zeichen (Umlaute etc.) weglassen
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                _ = sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
        // ü → u, ö → o, ä → a, ß bleibt ß (kein Basisbuchstabe)
    }
}
