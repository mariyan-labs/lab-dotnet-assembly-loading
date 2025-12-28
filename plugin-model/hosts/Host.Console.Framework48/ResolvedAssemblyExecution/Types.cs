using System.Reflection;

namespace Host.Console.Framework48.ResolvedAssemblyExecution;

public enum AssemblyLoadFunctionType
{
    LoadFrom,
    LoadFile,
    LoadBytes
}

public record ResolvedAssembly(
    Assembly? RequestingAssembly,
    AssemblyLoadFunctionType ResolveFunction,
    Assembly Assembly);

public record Error(string Message);

public record ResolveAssemblyNotFound(string Message, string AssemblyName, string Path) : Error(Message);

public record NoRequestingAssembly(string AssemblyName, string Message) : Error(Message);

public record WithinResolveAssembliesResult<T>(
    T Result,
    IReadOnlyList<Assembly> LoadedAssemblies,
    IReadOnlyList<ResolvedAssembly> ResolvedAssemblies,
    IReadOnlyList<Error> Errors
);

public abstract record ResolveOptions;

public record ResolveOptionsConfig(
    AssemblyLoadFunctionType ResolveFunctionType,
    string? AssemblySearchDirectory) : ResolveOptions;

public delegate ResolvedAssembly? ResolveAssemblyFunction(ResolveEventArgs args);

public record ResolveOptionsCustom(ResolveAssemblyFunction ResolveAssemblyFunction) : ResolveOptions;

public record WithinResolveAssembliesOptions(
    ResolveOptions ResolveOptions);