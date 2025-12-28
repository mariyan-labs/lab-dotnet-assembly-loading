using Host.Console.Framework48.PluginDiscovery;

namespace Host.Console.Framework48.Pretty;

public static class PrettyPluginConstruction
{
    extension(PrettyPlugin)
    {
        public static PrettyPlugin FromDiscoveredPlugin(DiscoveredPlugin plugin)
        {
            return new PrettyPlugin(plugin.AssemblyPath, plugin.Manifest.Name, plugin.Manifest.Version);
        }
    }
}