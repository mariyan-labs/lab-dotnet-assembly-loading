using Geometry;
using Host.Contracts;
using Serialization;

namespace PluginA;

public class PluginA : IPlugin
{
    public PluginResult Execute(PluginContext context)
    {
        Console.WriteLine($"Geometry assembly: {typeof(Point).Assembly}");
        var element = new Element("PluginA");
        context.Document.AddElement(element);
        var serialized = element.Serialize();
        
        var somePoint = Point.FromXY(1, 2);
        
        Console.WriteLine($"Serialized: {serialized}");
        Console.WriteLine($"Point: {somePoint}");
        
        return PluginResult.Success;
    }
}
