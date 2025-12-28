using Host.Console.Framework48.Tests;

namespace Host.Console.Framework48.Pretty;

public static class PrettyPluginExecutionTestConstruction
{
    extension(PrettyPluginExecutionTest)
    {
        public static PrettyPluginExecutionTest FromPluginExecutionSection(PluginExecutionTest test)
        {
            return new PrettyPluginExecutionTest(
                Plugin: PrettyPlugin.FromDiscoveredPlugin(test.Plugin),
                Sections:
                [
                    new PrettyPluginLoadAssemblySection(
                        LoadFunctionType: test.LoadFunctionType.ToString(),
                        Assembly: PrettyAssembly.FromAssembly(test.WithinAssemblyResultLoadingResult.Result),
                        LoadedAssemblies: test.WithinAssemblyResultLoadingResult.LoadedAssemblies
                            .Select(PrettyAssembly.FromAssembly).ToList(),
                        ResolvedAssemblies: test.WithinAssemblyResultLoadingResult.ResolvedAssemblies
                            .Select(PrettyResolvedAssembly.FromResolvedAssembly).ToList()
                    ),
                    new PrettyPluginFunctionExecutionSection(
                        LoadFunctionType: test.LoadFunctionType.ToString(),
                        LoadedAssemblies: test.WithinPluginExecutionResult.LoadedAssemblies
                            .Select(PrettyAssembly.FromAssembly).ToList(),
                        ResolvedAssemblies: test.WithinPluginExecutionResult.ResolvedAssemblies
                            .Select(PrettyResolvedAssembly.FromResolvedAssembly).ToList()
                    )
                ]
            );
        }
    }

    extension(PluginExecutionTest pluginExecutionTest)
    {
        public PrettyPluginExecutionTest Prettify()
        {
            return PrettyPluginExecutionTest.FromPluginExecutionSection(pluginExecutionTest);
        }
    }
}