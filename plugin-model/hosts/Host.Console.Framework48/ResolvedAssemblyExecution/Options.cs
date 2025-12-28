namespace Host.Console.Framework48.ResolvedAssemblyExecution;

public static class Options
{
    extension(ResolveOptions)
    {
        public static ResolveOptions Custom(ResolveAssemblyFunction resolveAssemblyFunction)
        {
            return new ResolveOptionsCustom(resolveAssemblyFunction);
        }
        
        public static ResolveOptions Config(
            AssemblyLoadFunctionType resolveFunctionType,
            string? assemblySearchDirectory = null
            )
        {
            return new ResolveOptionsConfig(resolveFunctionType, assemblySearchDirectory);
        }
    }
    
    extension(WithinResolveAssembliesOptions)
    {
        public static WithinResolveAssembliesOptions From(
            ResolveOptions resolveOptions
            )
        {
            return new WithinResolveAssembliesOptions(
                ResolveOptions: resolveOptions
            );
        }
    }
}