using Host.Console.Framework48.ResolvedAssemblyExecution;

namespace Host.Console.Framework48.Pretty;

public static class PrettyResolvedAssemblyConstruction
{
    extension(PrettyResolvedAssembly)
    {
        public static PrettyResolvedAssembly FromResolvedAssembly(ResolvedAssembly resolvedAssembly)
        {
            return new PrettyResolvedAssembly(
                LoadFunction: resolvedAssembly.ResolveFunction.ToString(),
                RequestingAssembly: resolvedAssembly.RequestingAssembly is null ? null : PrettyAssembly.FromAssembly(resolvedAssembly.RequestingAssembly),
                Assembly: PrettyAssembly.FromAssembly(resolvedAssembly.Assembly)
            );
        }
    }
}