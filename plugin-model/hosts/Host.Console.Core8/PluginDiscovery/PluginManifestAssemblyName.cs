namespace Host.Console.Core8.PluginDiscovery;

public static class PluginManifestAssemblyName
{
    extension(PluginManifest pluginManifest)
    {
        public string AssemblyName()
        {
            return $"{pluginManifest.Name}.dll";
        }
    }
}