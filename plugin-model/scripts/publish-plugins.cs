#pragma warning disable IL2026
#pragma warning disable IL3050

#:package Newtonsoft.Json@13.0.4

using System.Diagnostics;
using Newtonsoft.Json;


var rootDirectory =
    Directory.GetParent(Directory.GetCurrentDirectory())?.FullName
    ?? throw new InvalidOperationException("Failed to get parent directory");

var pluginsOutputDirectory = Path.Combine(rootDirectory, "output");


CleanDirectory(pluginsOutputDirectory);

var pluginsPath = Path.Combine(rootDirectory, "plugins");

Log($"Plugin path: {pluginsPath}");

var plugins = EnumeratePlugins(pluginsPath).ToList();

Log($"Found plugins: {plugins.Count}");


Directory.CreateDirectory(pluginsOutputDirectory);

plugins.ForEach(plugin =>
{
    var relativeName = $"{plugin.Name}_{plugin.Version}";
    var outputDirectory = Path.Combine(pluginsOutputDirectory, relativeName);
    Directory.CreateDirectory(outputDirectory);
    var pluginProjectPath = plugin.ProjectFullPath;
    
    Log($"Building plugin {pluginProjectPath} to {outputDirectory}");

    Run("dotnet", $"build {pluginProjectPath} -c Release -o {outputDirectory}");

    RemoveNonDllFiles(outputDirectory);

    CopyManifest(plugin, outputDirectory);

    Log($"Plugin {plugin} built");

});

static void RemoveNonDllFiles(string directory)
{
    Directory.EnumerateFiles(directory)
        .Where(file => !file.EndsWith(".dll"))
        .ToList()
        .ForEach(file =>
        {
            try
            {
                File.Delete(file);
            }
            catch(Exception e)
            {
                Log($"Failed to delete file {file}: {e.Message}");
            }
        });
}


static void CopyManifest(Plugin plugin, string outputDirectory)
{
    try
    {
        var fileName = Path.GetFileName(plugin.ManifestPath);
        File.Copy(plugin.ManifestPath, Path.Combine(outputDirectory, fileName), overwrite: true);
    }
    catch(Exception e)
    {
        Log($"Failed to copy manifest {plugin.ManifestPath} to {outputDirectory}: {e.Message}");
    }
}

static IEnumerable<Plugin> EnumeratePlugins(string path)
{
    var pluginProjects = Directory.EnumerateFiles(
        path: path,
        searchPattern: "*.csproj",
        enumerationOptions: new EnumerationOptions { RecurseSubdirectories = true }
    );
    var addinFileName = "addin.json";

    return pluginProjects.SelectMany(pluginProjectPath =>
    {
        var name = Path.GetFileNameWithoutExtension(pluginProjectPath);

        var manifestFile = Directory.EnumerateFiles(
            Path.GetDirectoryName(pluginProjectPath) ?? throw new InvalidOperationException("Failed to get plugin directory"), addinFileName)
        .SingleOrDefault();

        if(manifestFile is not null)
        {
            var manifestFileContent = File.ReadAllText(manifestFile);
            var pluginManifest = JsonConvert.DeserializeObject<PluginManifest>(manifestFileContent) ?? throw new InvalidOperationException("Failed to deserialize plugin manifest");

            return [new Plugin(ProjectFullPath: pluginProjectPath, Version: pluginManifest.Version, Name: pluginManifest.Name, ManifestPath: manifestFile)];
        }
        return Enumerable.Empty<Plugin>();
    });
}


static void Log(string message)
{
    Console.WriteLine(message);
}

static void Run(string file, string args)
{
    var p = Process.Start(
        new ProcessStartInfo
        {
            FileName = file,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        }
    )!;
    p.WaitForExit();
    if (p.ExitCode != 0)
    {
        throw new Exception(p.StandardError.ReadToEnd());
    }
}


static void CleanDirectory(string path)
{
    Directory.EnumerateDirectories(path).ToList().ForEach(directory =>
    {
        try
        {
            Directory.Delete(directory, recursive: true);
        }
        catch(Exception e)
        {
            Log($"Failed to delete directory {directory}: {e.Message}");
        }
    });
}
record PluginManifest(
    string Name,
    string Version
);
record Plugin(string ProjectFullPath, string Version, string Name, string ManifestPath);
