using System.Reflection;
using Host.Console.Framework48.PluginDiscovery;
using Host.Console.Framework48.ResolvedAssemblyExecution;

namespace Host.Console.Framework48;

public static class CustomResolvers
{
    extension(ResolveAssemblyFunction)
    {
        public static ResolveAssemblyFunction WithVersionNormalized(DiscoveredPlugin plugin)
        {
            return args =>
            {
                var directory = Path.GetDirectoryName(plugin.AssemblyPath)!;
                var rawName = new AssemblyName(args.Name).Name;

                var possiblePath = Path.Combine(directory, rawName + ".dll");

                if (File.Exists(possiblePath))
                {
                    return new ResolvedAssembly(
                        RequestingAssembly: args.RequestingAssembly,
                        ResolveFunction: AssemblyLoadFunctionType.LoadFrom,
                        Assembly: Assembly.LoadFromPath(possiblePath, AssemblyLoadFunctionType.LoadFrom)
                    );
                }

                var newName = rawName.Split('_')[0];
                var normalizedPath = Path.Combine(directory, newName + ".dll");

                if (File.Exists(normalizedPath))
                {
                    return new ResolvedAssembly(
                        RequestingAssembly: args.RequestingAssembly,
                        ResolveFunction: AssemblyLoadFunctionType.LoadFrom,
                        Assembly: Assembly.LoadFromPath(possiblePath, AssemblyLoadFunctionType.LoadFrom)
                    );
                }

                return null;
            };
        }
    } }