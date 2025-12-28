using System.Reflection;
using Host.Console.Framework48.PluginDiscovery;
using Host.Console.Framework48.ResolvedAssemblyExecution;
using Host.Contracts;

namespace Host.Console.Framework48.Tests;

public static class PluginExecutionTestExecution
{
    extension(PluginExecutionTest)
    {
        public static PluginExecutionTest Run(
            DiscoveredPlugin plugin,
            PluginContext pluginContext,
            PluginExecutionTestOptions options
        )
        {
            var loadPluginDllFunctionType = options.LoadPluginDllFunctionType;
            
            var withinAssemblyLoadingResult = WithinResolveAssemblies.Run(
                function: () => Assembly.LoadFromPath(plugin.AssemblyPath, loadPluginDllFunctionType),
                options: options.WithinResolveAssembliesOptions
            );

            var pluginAssembly = withinAssemblyLoadingResult.Result;

            var resolvedPlugin = IPlugin.FromAssembly(pluginAssembly);

            var withinPluginExecutionResult = WithinResolveAssemblies.Run(
                function: () => resolvedPlugin.Execute(pluginContext),
                options: options.WithinResolveAssembliesOptions
            );
            
            return new PluginExecutionTest(
                plugin,
                loadPluginDllFunctionType,
                withinAssemblyLoadingResult,
                withinPluginExecutionResult
            );
        }
    }
}