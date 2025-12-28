using System.Reflection;
using Host.Console.Framework48.PluginDiscovery;
using Host.Console.Framework48.ResolvedAssemblyExecution;
using Host.Contracts;

namespace Host.Console.Framework48.Tests;

public record PluginExecutionTestOptions(
    AssemblyLoadFunctionType LoadPluginDllFunctionType,
    WithinResolveAssembliesOptions WithinResolveAssembliesOptions
);

public record PluginExecutionTest(
    DiscoveredPlugin Plugin,
    AssemblyLoadFunctionType LoadFunctionType,
    WithinResolveAssembliesResult<Assembly> WithinAssemblyResultLoadingResult,
    WithinResolveAssembliesResult<PluginResult> WithinPluginExecutionResult);