using Host.Console.Framework48.ResolvedAssemblyExecution;

namespace Host.Console.Framework48.Tests;

public static class PluginExecutionTestOptionsCreation
{
    extension(PluginExecutionTestOptions)
    {
        public static PluginExecutionTestOptions From(
            AssemblyLoadFunctionType loadPluginDllFunctionType,
            ResolveOptions resolveOptions 
            )
        {
            return new PluginExecutionTestOptions(
                LoadPluginDllFunctionType: loadPluginDllFunctionType,
                WithinResolveAssembliesOptions: WithinResolveAssembliesOptions.From(resolveOptions)
            );
        }
    }
}