#pragma warning disable IL2026
#pragma warning disable IL3050


#:package ErrorOr@2.0.1
#:package Newtonsoft.Json@13.0.4

using System.Diagnostics;
using ErrorOr;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Immutable;
using static Types;
using static DotnetTools;
using static IOHelpers;
using static Rendering;
using static Pipelines;

WorkspacePaths.ResolveFromCurrentDirectory()
    .Then(RefreshOutputDirectory)
    .Then(FindPlugins)
    .Then(PublishPlugins)
    .Then(RenderOutput);


static class Pipelines
{
    public record RefreshOutput(WorkspacePaths Workspace, PartialResult<DeletedDirectory> RefreshedDirectories);
    public record FindPluginsOutput(PartialResult<Plugin> Plugins, WorkspacePaths Workspace,PartialResult<DeletedDirectory> RefreshedDirectories);
    
    public static ErrorOr<RefreshOutput> RefreshOutputDirectory(
        WorkspacePaths workspace
    )
    {
        return workspace.RefreshOutputDirectory()
            .Then(refreshedDirectories => new RefreshOutput(workspace, refreshedDirectories));
    }

    public static ErrorOr<FindPluginsOutput> FindPlugins(RefreshOutput refreshOutput)
    {
        return Plugin.FromWorkspace(refreshOutput.Workspace)
            .Then(plugins => new FindPluginsOutput(plugins, refreshOutput.Workspace, refreshOutput.RefreshedDirectories));
    }

    public static ErrorOr<Output> PublishPlugins(FindPluginsOutput input)
    {
        return PublishedPlugin.Run(
            input.Plugins.Items,
            input.Workspace
        )
        .Then(publishedPluginsResult =>
        {
            return Output.From(
                errors: [..publishedPluginsResult.Errors, ..input.Plugins.Errors, ..input.RefreshedDirectories.Errors ],
                plugins: publishedPluginsResult.Items
            );
        });
    }
}


static class Rendering
{
    public static ErrorOr<Success> RenderOutput(Output output)
    {
        try
        {
            var stringBuilder = new StringBuilder();
        
            stringBuilder.AppendLine($"Count of published plugins: {output.PublishedPlugins.Count}");
            
            output.PublishedPlugins.ForEach(publishedPlugin =>
            {
                var plugin = publishedPlugin.Plugin;
                stringBuilder.AppendLine($"Plugin: {plugin.Name} {plugin.Version} was published to {publishedPlugin.OutputDirectory}");
            });

            if (output.Errors.Any())
            {
                stringBuilder.AppendLine($"Errors: {output.Errors.Count}");
            }

            output.Errors.ForEach(error =>
            {
                stringBuilder.AppendLine($"Error: {error.Description}");
            });

            Console.WriteLine(stringBuilder.ToString());

            return Result.Success;
        }
        catch (Exception)
        {
            
            Console.WriteLine("Failed to render output");
            return Error.Fail("Failed to render output");
        }
        
    }
}

static class WorkspacePathsExtensions
{
    extension(WorkspacePaths workspace)
    {
        public static ErrorOr<WorkspacePaths> ResolveFromCurrentDirectory()
        {
            var rootDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;

            if(rootDirectory is null)
            {
                return Error.Fail("Failed to get parent directory");
            }

            var outputDirectory = Path.Combine(rootDirectory, "output");

            if (!Directory.Exists(outputDirectory))
            {
                return Error.Fail("Output directory does not exist");
            }

           var pluginsDirectory = Path.Combine(rootDirectory, "plugins");

           if (!Directory.Exists(pluginsDirectory))
           {
                return Error.Fail("Plugins directory does not exist");
           }

           return new WorkspacePaths(rootDirectory, outputDirectory, pluginsDirectory);
        }
    

        public ErrorOr<PartialResult<DeletedDirectory>> RefreshOutputDirectory()
        {
            try
            {
                var output = workspace.Output;
            
                var result = Directory.EnumerateDirectories(output)
                    .Aggregate(
                        seed: PartialResult<DeletedDirectory>.Empty(),
                        func: (result, directory) =>
                        {
                            try
                            {
                                Directory.Delete(directory, recursive: true);
                                return result.Add(new DeletedDirectory(directory));
                            }
                            catch(Exception e)
                            {
                                return result.AddError(Error.Fail($"Failed to delete directory {directory}: {e.Message}"));
                            }
                        }
                        
                    );

                Directory.CreateDirectory(output);

                return result;

            }
            catch(Exception e)
            {
                return Error.Fail($"Failed to refresh output directory: {e.Message}");
            }
     
        }
    }
}

static class Errors
{
    extension(Error)
    {
        public static Error Fail(string description)
        {
            return Error.Failure(description: description);
        }
    } 
}

static class PartialResults
{
    extension<T>(PartialResult<T> partialResult)
    {
        public static PartialResult<T> Empty() => new PartialResult<T>([], []);

        public PartialResult<T> Add(T item)
        {
            return partialResult with { Items = partialResult.Items.Add(item)};
        }

        public PartialResult<T> AddError(Error error)
        {
            return partialResult with { Errors = partialResult.Errors.Add(error)};
        }

        public PartialResult<T> Combine(ErrorOr<T> error)
        {
            return error
                .Match(
                    onValue: partialResult.Add,
                    onError: error => partialResult.AddError(error.Single())
                );
        }
    }
}

static class Plugins
{
    extension(Plugin)
    {
        public static ErrorOr<PartialResult<Plugin>> FromWorkspace(
            WorkspacePaths workspace
        )
        {
            try
            {
                var path = workspace.Plugins;

             var pluginProjects = Directory.EnumerateFiles(
                path: path,
                searchPattern: "*.csproj",
                enumerationOptions: new EnumerationOptions { RecurseSubdirectories = true }
            );

            var addinFileName = "addin.json";

            return pluginProjects
                .Aggregate(
                    seed: PartialResult<Plugin>.Empty(),
                    func: (result, pluginProjectPath) =>
                    {
                        var name = Path.GetFileNameWithoutExtension(pluginProjectPath);

                        var projectDirectory = Path.GetDirectoryName(pluginProjectPath);
                        
                        if(projectDirectory is null)
                        {
                            return result.AddError(Error.Fail($"Failed to get plugin directory for {pluginProjectPath}"));
                        }

                        var manifestFile = Directory.EnumerateFiles(projectDirectory, addinFileName).SingleOrDefault();

                        if(manifestFile is null)
                        {
                            return result;
                        }

                        var manifestFileContent = File.ReadAllText(manifestFile);
                        
                        var pluginManifest = JsonConvert.DeserializeObject<PluginManifest>(manifestFileContent);
                        
                        if(pluginManifest is null)
                        {
                            return result.AddError(Error.Fail($"Failed to deserialize plugin manifest for {pluginProjectPath}"));
                        }

                        return result.Add(new Plugin(
                            ProjectFullPath: pluginProjectPath,
                            Version: pluginManifest.Version,
                            Name: pluginManifest.Name,
                            ManifestPath: manifestFile));
                        
                    }
                );
            }
            catch(Exception e)
            {
                return Error.Fail($"Failed to enumerate plugins: {e.Message}");
            }
        }
    } 
}

static class DotnetTools
{
    public static ErrorOr<Success> RunDotNet(string arguments)
    {
        var process = Process.Start(
            new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            }
        );

        if(process is null)
        {
            return Error.Fail($"Failed to start process with arguments: {arguments}");
        }

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            return Error.Fail($"Failed to run dotnet with arguments: {arguments}. Error: {process.StandardError.ReadToEnd()}");
        }

        return Result.Success;
    }
}

static class PublishedPlugins
{
    extension(PublishedPlugin)
    {
        public static ErrorOr<PartialResult<PublishedPlugin>> Run(
            IEnumerable<Plugin> plugins,
            WorkspacePaths workspace
        )
        {
            try
            {
                return plugins
                .Aggregate(
                    seed: PartialResult<PublishedPlugin>.Empty(),
                    func: (result, plugin) =>
                    {
                        return result.Combine(PublishedPlugin.Run(plugin, workspace));
                    }
                );
            }
            catch (Exception)
            {
                return Error.Fail("Failed to publish plugins");
            }
        }

        public static ErrorOr<PublishedPlugin> Run(
            Plugin plugin,
            WorkspacePaths workspace
        )
        {
            var pluginsRootDirectory = workspace.Output;
            
            var relativeName = $"{plugin.Name}_{plugin.Version}";
            var pluginOutputDirectory = Path.Combine(pluginsRootDirectory, relativeName);
            var pluginProjectPath = plugin.ProjectFullPath;
            var arguments = $"build {pluginProjectPath} -c Release -o {pluginOutputDirectory}";

            return CreateDirectorySafe(pluginOutputDirectory)
                .Then(_ => RunDotNet(arguments))
                .Then(_ => DeleteNonDllFiles(pluginOutputDirectory))
                .Then(_ => CopyManifestTo(plugin, pluginOutputDirectory))
                .Then(_ =>
                {
                    return new PublishedPlugin(
                        Plugin: plugin,
                        OutputDirectory: pluginOutputDirectory,
                        ProjectPath: pluginProjectPath,
                        CliArguments: arguments
                    );
                });
         
        }
    }
}

static class IOHelpers
{
    public static ErrorOr<string> CreateDirectorySafe(string directory)
    {
        try
        {
            Directory.CreateDirectory(directory);
            return directory;
        }
        catch(Exception e)
        {
            return Error.Fail($"Failed to create directory {directory}: {e.Message}");
        }
    }

    public static ErrorOr<Success> CopyManifestTo(Plugin plugin, string outputDirectory)
    {
        try
        {
            var fileName = Path.GetFileName(plugin.ManifestPath);
            File.Copy(plugin.ManifestPath, Path.Combine(outputDirectory, fileName), overwrite: true);
            
            return Result.Success;
        }
        catch(Exception e)
        {
            return Error.Fail($"Failed to copy manifest file: {e.Message}");
        }
    }

    public static ErrorOr<PartialResult<DeletedFile>> DeleteNonDllFiles(
        string directory
    )
    {
        try
        {
            return Directory.EnumerateFiles(directory)
            .Where(file => !file.EndsWith(".dll"))
            .Aggregate(
                seed: PartialResult<DeletedFile>.Empty(),
                func: (result, file) =>
                {
                    try
                    {
                        File.Delete(file);
                        return result.Add(new DeletedFile(file));
                    }
                    catch(Exception e)
                    {
                        return result.AddError(Error.Fail($"Failed to delete file {file}: {e.Message}"));
                    }
                }
            );
        }
        catch(Exception e)
        {
            return Error.Fail($"Failed to delete files in {directory}: {e.Message}");
        }
    }
}


static class Outputs
{
    extension(Output)
    {
        public static Output From(
            ImmutableList<PublishedPlugin> plugins,
            ImmutableList<Error> errors
        )
        {
            return new Output(PublishedPlugins: plugins, Errors: errors);
        }
    }
}
static class Types
{
    public record WorkspacePaths(
        string Root,
        string Output,
        string Plugins
    );

    public record PluginManifest(string Name, string Version);
    
    public record Plugin(string ProjectFullPath, string Version, string Name, string ManifestPath);

    public record PartialResult<T>(ImmutableList<T> Items, ImmutableList<Error> Errors);

    public record DeletedDirectory(string Path);

    public record DeletedFile(string Path);

    public record PublishedPlugin(
        Plugin Plugin,
        string OutputDirectory,
        string ProjectPath,
        string CliArguments
    );

    public record Output(
        ImmutableList<Error> Errors,
        ImmutableList<PublishedPlugin> PublishedPlugins
    );

}


