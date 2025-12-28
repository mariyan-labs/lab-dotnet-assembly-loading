namespace Host.Console.Framework48.ResolvedAssemblyExecution;

public static class Errors
{
    extension(Error)
    {
        public static Error ResolveAssemblyNotFound(string assemblyName, string path)
        {
            return new ResolveAssemblyNotFound(
                Message: $"Assembly not found: {assemblyName} at {path}",
                AssemblyName: assemblyName,
                Path: path);
        }

        public static Error NoRequestingAssembly(string assemblyName)
        {
            return new NoRequestingAssembly(AssemblyName: assemblyName, Message: $"No requesting assembly found for {assemblyName}");
        }
    }
}