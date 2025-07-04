﻿namespace NetEvolve.HealthChecks.Tests.Unit.MySql;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.MySql;

[TestGroup(nameof(MySql))]
public sealed class MySqlOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new MySqlOptions { ConnectionString = "test" };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
