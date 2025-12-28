namespace Host.Console.Framework48.PluginDiscovery;

public record PluginManifest(string Name, string Version);

public record DiscoveredPlugin(
    string AssemblyPath,
    PluginManifest Manifest
);