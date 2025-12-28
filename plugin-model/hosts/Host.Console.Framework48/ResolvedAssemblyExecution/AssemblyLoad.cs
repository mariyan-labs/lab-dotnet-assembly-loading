using System.Reflection;

namespace Host.Console.Framework48.ResolvedAssemblyExecution;

public static class AssemblyLoad
{
    extension(Assembly)
    {
        public static Assembly LoadFromPath(string path, AssemblyLoadFunctionType loadFunctionType)
        {
            return loadFunctionType switch
            {
                AssemblyLoadFunctionType.LoadFrom => Assembly.LoadFrom(path),
                AssemblyLoadFunctionType.LoadFile => Assembly.LoadFile(path),
                AssemblyLoadFunctionType.LoadBytes => Assembly.Load(File.ReadAllBytes(path)),
                _ => throw new ArgumentOutOfRangeException(nameof(loadFunctionType), loadFunctionType, null)
            } ?? throw new InvalidOperationException("Failed to load assembly");
        }
    }
}