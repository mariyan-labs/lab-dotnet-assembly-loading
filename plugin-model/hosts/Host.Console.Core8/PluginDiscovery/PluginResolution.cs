using System.Reflection;
using Host.Contracts;

namespace Host.Console.Core8.PluginDiscovery;

public static class PluginResolution
{
    extension(IPlugin)
    {
        public static IPlugin FromAssembly(Assembly assembly)
        {
            var pluginType = typeof(IPlugin);
            var type =  assembly.GetTypes()
                .FirstOrDefault(type => pluginType.IsAssignableFrom(type)) ?? throw new InvalidOperationException("No plugin type found");
            
            return (IPlugin) Activator.CreateInstance(type);
        }
    }
}