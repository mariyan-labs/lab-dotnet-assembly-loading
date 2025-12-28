using System.Reflection;

namespace Host.Console.Framework48.Pretty;

public static class PrettyAssemblyConstruction
{
    extension(PrettyAssembly)
    {
        public static PrettyAssembly FromAssembly(Assembly assembly)
        {
            return new PrettyAssembly(assembly.FullName, assembly.Location);
        }
    }
}