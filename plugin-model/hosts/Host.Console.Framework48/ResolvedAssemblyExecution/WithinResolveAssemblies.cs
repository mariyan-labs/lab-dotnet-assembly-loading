using System.Reflection;
using Host.Console.Framework48.Shared;

namespace Host.Console.Framework48.ResolvedAssemblyExecution;

/*
{
    resolve: ResolveOptions.Custom(func: () => {});
    resolve: ResolveOptions.Config()
    "resolve": {
        "loadFunctionType": "LoadFrom",
        "coreDirectory": "location"
    }
    
      "resolve": {
        "loadFunctionType": "LoadFrom",
        "coreDirectory": "location"
    }
}
 */

/*
 ResolvedAssembly.ResolveFromCustom()
 */

public static class ResolveAssemblyStrategies
{
    extension(ResolvedAssembly)
    {
        public static ResolvedAssembly ResolveFromCustom(
            ResolveOptionsCustom customResolveOptions,
            ResolveEventArgs args)
        {
            return customResolveOptions.ResolveAssemblyFunction.Invoke(args);
        }
    }
}
public static class WithinResolveAssemblies
{
    public static WithinResolveAssembliesResult<TResult> Run<TResult>(
        Func<TResult> function,
        WithinResolveAssembliesOptions options)
    {
        var loadedAssemblies = List<Assembly>.Empty();
        var resolvedAssemblies = List<ResolvedAssembly>.Empty();
        var errors = List<Error>.Empty();
    
        var assemblyLoadHandler = new AssemblyLoadEventHandler((sender, args) =>
        {
            var loaded = args.LoadedAssembly;
            loadedAssemblies.Add(loaded);
        });
        
        var resolveOptions = options.ResolveOptions;
        
        var assemblyResolveHandler = new ResolveEventHandler((sender, args) =>
        {
            if (resolveOptions is ResolveOptionsCustom customResolveOptions)
            {
                var resolvedAssemblyFromCustom = customResolveOptions.ResolveAssemblyFunction.Invoke(args);
                
                resolvedAssemblies.Add(resolvedAssemblyFromCustom);
                return resolvedAssemblyFromCustom.Assembly;
            }
            
            if(resolveOptions is not ResolveOptionsConfig configResolveOptions)
            {
                return null;
            }
            
            var parsedAssemblyName = new AssemblyName(args.Name);
            var assemblyName = parsedAssemblyName.Name;
            var resolveFunctionType = configResolveOptions.ResolveFunctionType;
            var searchDirectory = configResolveOptions.AssemblySearchDirectory;

            if (searchDirectory is not null)
            {
                var path = Path.Combine(searchDirectory, assemblyName + ".dll");
                
                if (!File.Exists(path))
                {
                    errors.Add(Error.ResolveAssemblyNotFound(assemblyName: assemblyName, path: path));
                    return null;
                }
                
                var resolvedAssemblyFromConfig = Assembly.LoadFromPath(path: path, loadFunctionType: resolveFunctionType);
                
                resolvedAssemblies.Add(new ResolvedAssembly(
                    RequestingAssembly: args.RequestingAssembly,
                    ResolveFunction: resolveFunctionType,
                    Assembly: resolvedAssemblyFromConfig));
                
                return resolvedAssemblyFromConfig;
            }
            
            var requestingAssembly = args.RequestingAssembly;

            if (requestingAssembly is null)
            {
                errors.Add(Error.NoRequestingAssembly(assemblyName: assemblyName));
                
                return null;
            }

            var requestingAssemblyDirectory = Path.GetDirectoryName(requestingAssembly.Location)!;
            
            var assemblyPath = Path.Combine(requestingAssemblyDirectory, assemblyName + ".dll");
            
            if (!File.Exists(assemblyPath))
            {
                errors.Add(Error.ResolveAssemblyNotFound(assemblyName: assemblyName, path: assemblyPath));
                return null;
            }
            
            var assembly = Assembly.LoadFromPath(path: assemblyPath, loadFunctionType: resolveFunctionType);
            
            resolvedAssemblies.Add(new ResolvedAssembly(
                RequestingAssembly: args.RequestingAssembly,
                ResolveFunction: resolveFunctionType,
                Assembly: assembly));
            
            return assembly;
        });
    
        AppDomain.CurrentDomain.AssemblyLoad += assemblyLoadHandler;
        AppDomain.CurrentDomain.AssemblyResolve += assemblyResolveHandler;
        

        try
        {
            var result = function();
            
            return WithinResolveAssembliesResult<TResult>.From(
                result: result,
                loadedAssemblies: loadedAssemblies,
                resolvedAssemblies: resolvedAssemblies,
                errors: errors
            );
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyLoad -= assemblyLoadHandler;
            AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolveHandler;
        }
    }

    
    extension<T>(WithinResolveAssembliesResult<T>)
    {
        private static WithinResolveAssembliesResult<T> From(
            T result,
            IReadOnlyList<Assembly>? loadedAssemblies = null,
            IReadOnlyList<ResolvedAssembly>? resolvedAssemblies = null,
            IReadOnlyList<Error>? errors = null
        )
        {
            return new WithinResolveAssembliesResult<T>(
                Result: result,
                LoadedAssemblies: loadedAssemblies ?? List<Assembly>.Empty(),
                ResolvedAssemblies: resolvedAssemblies ?? List<ResolvedAssembly>.Empty(),
                Errors: errors ?? List<Error>.Empty()
            );
        }
    }
    
}