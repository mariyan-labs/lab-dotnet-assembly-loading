using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using Geometry;
using Host.Console.Core8;
using Host.Contracts;
using Host.Console.Core8.PluginDiscovery;
using Host.Console.Core8.Shared;

var dummyPointToLoadAssembly = Point.FromXYZ(1, 2, 3);

var pluginContext = new PluginContext(Document: new Document());

DiscoveredPlugin
    .AllFromSolutionDirectory()
    .ForEach(discoveredPlugin =>
    {
        var context = PluginLoadContext.FromPlugin(discoveredPlugin);

        var assembly = context.LoadFromAssemblyPath(discoveredPlugin.AssemblyPath);

        var plugin = IPlugin.FromAssembly(assembly);

        plugin.Execute(pluginContext);

    });

PrintAssembliesFromAppDomainGroupedByContext();

return;


static string Serialize(object any)
{
    return JsonSerializer.Serialize(any, new JsonSerializerOptions()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase

    });
}

static void DumpConsole(object any)
{
    Console.WriteLine(Serialize(any));
}

static void PrintAssembliesFromAppDomainGroupedByContext()
{
    var output = AppDomain.CurrentDomain
        .GetAssemblies()
        .Select(assembly => new
        {
            FullName = assembly.FullName,
            Location = assembly.Location,
            Context = AssemblyLoadContext.GetLoadContext(assembly).ToString()
        })
        .GroupBy(x => x.Context)
        .Select(grouping =>
        {
            return new
            {
                Context = grouping.Key,
                Assemblies = grouping.Select(x => new
                {
                    x.FullName,
                    x.Location
                }).ToList()
            };
        });
    
    DumpConsole(output);
    File.WriteAllText(path: "output.json", Serialize(output));
    
}

static void PrintAssembliesFromAppDomain()
{
    var output = AppDomain.CurrentDomain
        .GetAssemblies()
        .Select(assembly => new
        {
            FullName = assembly.FullName,
            Location = assembly.Location,
            Context = AssemblyLoadContext.GetLoadContext(assembly).ToString()
        })
        .ToList();
    
    DumpConsole(output);
    
}

static void RunTypeComparisonTest(
    Type type,
    Assembly assembly
    )
{
    var typeByFullName = assembly
        .GetTypes()
        .SelectMany(type => type.GetInterfaces())
        .Single(assemblyType => assemblyType.FullName == type.FullName);
    
    var typeAssembly = type.Assembly;
    var typeByFullNameAssembly = typeByFullName.Assembly;
    
    var sameType = typeAssembly == typeByFullNameAssembly;
    
    var typeContext = AssemblyLoadContext.GetLoadContext(type.Assembly);
    var typeByFullNameContext = AssemblyLoadContext.GetLoadContext(typeByFullName.Assembly);
    
    var sameContext = typeContext == typeByFullNameContext;


    var output = new
    {
        SameType = sameType,
        SameContext = sameContext,
        SharedType = new
        {
            type.FullName,
            Assembly = new
            {
                type.Assembly.FullName,
                type.Assembly.Location
            },
            Context = AssemblyLoadContext.GetLoadContext(type.Assembly).ToString()
        },
        TypeFromAssembly = new
        {
            typeByFullName.FullName,
            Assembly = new
            {
                typeByFullName.Assembly.FullName,
                typeByFullName.Assembly.Location
            },
            Context = AssemblyLoadContext.GetLoadContext(typeByFullName.Assembly).ToString()
        }
    };
    
    DumpConsole(output);
    
}