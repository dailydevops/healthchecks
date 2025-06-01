namespace NetEvolve.HealthChecks.Tests.Integration;

using System.ComponentModel;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using NetEvolve.Extensions.TUnit;
using PublicApiGenerator;

[TestGroup(nameof(HealthChecks))]
[TestGroup("Architecture")]
public class PublicApiTests
{
    private static readonly string[] _excludedAttributes =
    [
        typeof(InternalsVisibleToAttribute).FullName!,
        "System.Runtime.CompilerServices.IsByRefLikeAttribute",
        typeof(TargetFrameworkAttribute).FullName!,
        typeof(CLSCompliantAttribute).FullName!,
        typeof(AssemblyMetadataAttribute).FullName!,
        typeof(NeutralResourcesLanguageAttribute).FullName!,
        typeof(AttributeUsageAttribute).FullName!,
    ];

    [Test]
    [MethodDataSource(nameof(GetAssemblies))]
    public async Task PublicApi_HasNotChanged_Theory(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var types = assembly.GetTypes().Where(IsVisibleToIntelliSense).ToArray();

        var options = new ApiGeneratorOptions { ExcludeAttributes = _excludedAttributes, IncludeTypes = types };

        var publicApi = assembly.GeneratePublicApi(options);

        _ = await Verify(publicApi).IgnoreParametersForVerified().UseTypeName(assembly.GetName().Name);
    }

    public static Func<Assembly>[] GetAssemblies() =>
        Assembly
            .GetExecutingAssembly()!
            .GetReferencedAssemblies()
            .Where(a => a.Name?.StartsWith("NetEvolve.HealthChecks", StringComparison.OrdinalIgnoreCase) == true)
            .Select<AssemblyName, Func<Assembly>>(a => () => Assembly.Load(a))
            .ToArray();

    private static bool IsVisibleToIntelliSense(Type type)
    {
        var browsable = type.GetCustomAttribute<BrowsableAttribute>();
        if (browsable is null || browsable.Browsable)
        {
            return true;
        }

        var editorBrowsable = type.GetCustomAttribute<EditorBrowsableAttribute>();
        return editorBrowsable is null || editorBrowsable.State != EditorBrowsableState.Never;
    }
}
