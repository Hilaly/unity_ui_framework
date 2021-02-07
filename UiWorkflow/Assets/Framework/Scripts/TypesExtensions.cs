using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Framework
{
    public static class TypesExtensions
    {
        public static List<Type> GetAllSubTypes(this Type aBaseClass)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var result = new HashSet<Type> {aBaseClass};
            foreach (var assembly in assemblies)
            {
                try
                {
                    var assemblyTypes = assembly.GetTypes();
                    var selectedTypes = assemblyTypes
                        .Where(typ => typ.IsSubclassOf(aBaseClass) || aBaseClass.IsAssignableFrom(typ)).ToArray();
                    foreach (var selectedType in selectedTypes)

                        result.Add(selectedType);
                }
                catch
                {
                    //Do nothing if we got to assembly that probably not from this project
                }
            }

            return result.ToList();
        }

        public static T[] GetAttributes<T>(this MemberInfo info, bool inherit) where T : Attribute
        {
            var attributes = info.GetCustomAttributes(typeof(T), inherit);
            var result = new T[attributes.Length];
            for (var i = 0; i < attributes.Length; ++i)
                result[i] = (T) attributes[i];
            return result;
        }
    }
}