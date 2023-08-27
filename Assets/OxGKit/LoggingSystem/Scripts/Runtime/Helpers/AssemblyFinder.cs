using System;
using System.Collections.Generic;
using System.Reflection;

namespace OxGKit.LoggingSystem
{
    public static class AssemblyFinder
    {
        public static List<Type> GetAssignableTypes(Type parentType)
        {
            List<Type> allTypes = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                var types = assembly.GetTypes();
                allTypes.AddRange(types);
            }

            List<Type> result = new List<Type>();
            for (int i = 0; i < allTypes.Count; i++)
            {
                Type type = allTypes[i];
                if (parentType.IsAssignableFrom(type))
                {
                    if (type.Name == parentType.Name) continue;
                    result.Add(type);
                }
            }
            return result;
        }

        public static T GetAttribute<T>(Type type) where T : Attribute
        {
            return (T)type.GetCustomAttribute(typeof(T), false);
        }
    }
}
