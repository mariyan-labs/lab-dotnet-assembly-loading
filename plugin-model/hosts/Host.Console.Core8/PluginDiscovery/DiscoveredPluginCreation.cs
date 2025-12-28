using Newtonsoft.Json;
using static Host.Console.Core8.PluginDiscovery.DirectoryLookup; 

namespace Host.Console.Core8.PluginDiscovery;

public static class DiscoveredPluginCreation
{
    extension(DiscoveredPlugin)
    {
        public static IReadOnlyList<DiscoveredPlugin> AllFromSolutionDirectory()
        {
            var root = FindSolutionDirectory();

            var pluginsDirectory = Path.Combine(root, "output");

            return Directory.EnumerateDirectories(pluginsDirectory)
                .Select(DiscoveredPlugin.FromPluginDirectory)
                .ToList();
        }
    }
    
    extension(DiscoveredPlugin)
    {
        private static DiscoveredPlugin FromPluginDirectory(string directory)
        {
            var manifest = Path.Combine(directory, "addin.json");
            var json = File.ReadAllText(manifest);
            var pluginManifest = JsonConvert.DeserializeObject<PluginManifest>(json) ?? throw new InvalidOperationException("Invalid manifest");
            var assemblyPath = Path.Combine(directory, pluginManifest.AssemblyName());
            
            return new DiscoveredPlugin(assemblyPath, pluginManifest);
        }
    }
}