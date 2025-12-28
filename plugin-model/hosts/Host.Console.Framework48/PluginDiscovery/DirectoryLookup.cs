namespace Host.Console.Framework48.PluginDiscovery;

public static class DirectoryLookup
{
    public static string FindSolutionDirectory()
    {
        var root = FindSolutionDirectory(Directory.GetCurrentDirectory());
        
        return root ?? throw new InvalidOperationException("Solution directory not found");
    }
    
    private static string? FindSolutionDirectory(string directory)
    {
        var directoryContainsSolution = Directory.EnumerateFiles(directory, "*.slnx").Any();

        if (directoryContainsSolution)
        {
            return directory;
        }
    
        var parent = Directory.GetParent(directory)?.FullName;
    
        return parent is null ? null : FindSolutionDirectory(parent);
    }
}