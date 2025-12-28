using Geometry;
using Host.Console.Framework48.PluginDiscovery;
using Host.Console.Framework48.ResolvedAssemblyExecution;
using Host.Console.Framework48.Shared;
using Host.Console.Framework48.Tests;
using Host.Contracts;
using Host.Console.Framework48.Formatting;
using Host.Console.Framework48.Pretty;

ForceLoadCoreAssemblies();

var pluginContext = new PluginContext(Document: new Document());

var tests = DiscoveredPlugin.AllFromSolutionDirectory()
    .Select(plugin =>
        {
            var options = PluginExecutionTestOptions.From(
                loadPluginDllFunctionType: AssemblyLoadFunctionType.LoadFrom,
                resolveOptions: ResolveOptions.Config(
                    resolveFunctionType: AssemblyLoadFunctionType.LoadFrom,
                    assemblySearchDirectory: Path.GetDirectoryName(plugin.AssemblyPath)
                ));
            
            return PluginExecutionTest.Run(plugin: plugin, pluginContext: pluginContext, options: options);
        }
    )
    .ToList();


tests
    .Select(test => test.Prettify())
    .ForEach(test =>
    {
        test.Html()
            .WriteTo($"{test.Plugin.Name}_{test.Plugin.Version}.html");
    });

static void ForceLoadCoreAssemblies()
{
    var pointType = typeof(Point);
    var pluginType = typeof(IPlugin);
}