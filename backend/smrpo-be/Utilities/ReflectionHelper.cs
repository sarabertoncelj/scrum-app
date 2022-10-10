using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace smrpo_be.Utilities
{
    public static class ReflectionHelper
    {
        public static IReadOnlyCollection<T> GetInstancesDerivedFrom<T>(Assembly assembly)
        {
            var result = new List<T>();
            var assemblies = GetAssemblyMap(Assembly.Load("smrpo-be"), fullName => fullName.StartsWith("smrpo-be"));

            // load automapper configurations from relevant assemblies
            foreach (var item in assemblies)
            {
                result.AddRange(item.Value.GetTypes()
                      .Where(t => typeof(T).IsAssignableFrom(t))
                      .Select(t => (T)Activator.CreateInstance(t)));
            }

            return result;
        }

        internal static Dictionary<string, Assembly> GetAssemblyMap(Assembly assembly, Func<string, bool> assemblyNamePredicate, Dictionary<string, Assembly> assemblyMap = null)
        {
            if (assemblyMap == null)
            {
                assemblyMap = new Dictionary<string, Assembly> { { assembly.FullName, assembly } };
            }
            AssemblyName[] assemblyNames = assembly.GetReferencedAssemblies()
                                           .Where(assemblyName => assemblyNamePredicate(assemblyName.FullName)).ToArray();

            foreach (AssemblyName assemblyName in assemblyNames)
            {
                if (assemblyMap.ContainsKey(assemblyName.FullName))
                {
                    continue;
                }

                Assembly loadedAssembly = Assembly.Load(assemblyName);
                assemblyMap.Add(loadedAssembly.FullName, loadedAssembly);

                // assignment not really needed, just for readability
                assemblyMap = GetAssemblyMap(loadedAssembly, assemblyNamePredicate, assemblyMap);
            }
            return assemblyMap;
        }
    }
}
