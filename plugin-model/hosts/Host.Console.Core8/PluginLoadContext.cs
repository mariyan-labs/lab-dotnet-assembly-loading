using System.Reflection;
using System.Runtime.Loader;
using Host.Console.Core8.PluginDiscovery;

namespace Host.Console.Core8;

public record AssemblyLoadedArgs(Assembly? Assembly, AssemblyName AssemblyName, string Path);

public class PluginLoadContext(string pluginPath, string name) : AssemblyLoadContext(isCollectible: true, name: name)
{
    public event EventHandler<AssemblyLoadedArgs>? AssemblyLoaded;
    private readonly AssemblyDependencyResolver _resolver = new(pluginPath);

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        
        if (assemblyPath is null)
        {
            return null;
        }
        
        var assembly = LoadFromAssemblyPath(assemblyPath);
        
        AssemblyLoaded?.Invoke(this, new AssemblyLoadedArgs(assembly, assemblyName, assemblyPath));
            
        return assembly;
    }
}

public static class PluginLoadContextCreation
{
    extension(PluginLoadContext)
    {
        public static PluginLoadContext FromPlugin(DiscoveredPlugin plugin)
        {
            return new PluginLoadContext(pluginPath: plugin.AssemblyPath, name: $"{plugin.Manifest.Name}_{plugin.Manifest.Version}");
        }
    }
}