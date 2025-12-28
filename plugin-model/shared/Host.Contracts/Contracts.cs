namespace Host.Contracts;

public class Element(string name)
{
    public string Name { get; set; } = name;
}

public class Document
{
    public List<Element> Elements { get; } = new();
    
    public void AddElement(Element element)
    {
        Elements.Add(element);
    }
}

public record PluginContext(Document Document);

public enum PluginResult
{
    Success,
    Failure
}

public interface IPlugin
{
    PluginResult Execute(PluginContext context);
}