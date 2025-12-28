namespace Host.Console.Framework48.Pretty;

public enum PrettySection
{
    LoadAssembly,
    ExecuteFunction
}

public record PrettyAssembly(string FullName, string? Location);

public record PrettyPlugin(string FullPath, string Name, string Version);


public abstract record PrettyPluginSection(PrettySection Section);

public record PrettyPluginFunctionExecutionSection(
    string LoadFunctionType,
    IReadOnlyList<PrettyAssembly> LoadedAssemblies,
    IReadOnlyList<PrettyResolvedAssembly> ResolvedAssemblies) : PrettyPluginSection(PrettySection.ExecuteFunction);
    
public record PrettyPluginLoadAssemblySection(
    string LoadFunctionType,
    PrettyAssembly Assembly,
    IReadOnlyList<PrettyAssembly> LoadedAssemblies,
    IReadOnlyList<PrettyResolvedAssembly> ResolvedAssemblies): PrettyPluginSection(PrettySection.LoadAssembly);
    
public record PrettyPluginExecutionTest(
    PrettyPlugin Plugin,
    IReadOnlyList<PrettyPluginSection> Sections);
    
public record PrettyResolvedAssembly(
    string LoadFunction,
    PrettyAssembly? RequestingAssembly,
    PrettyAssembly Assembly);