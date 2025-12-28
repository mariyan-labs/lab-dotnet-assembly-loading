using Geometry;
using Host.Contracts;

namespace PluginA;

public class PluginA : IPlugin
{
    public PluginResult Execute(PluginContext context)
    {
        var point = Point.FromXYZ(3, 4, 5);
        Console.WriteLine($"Hello from PluginA v1: {point}");

        
        return PluginResult.Success;
    }
}
