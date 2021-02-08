using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Framework.Flow
{
    public static class FlowExtensions
    {
        #region TODO Move to editor

        private static List<string> _routes;
        public static List<string> FindAllRoutes()
        {
            if (_routes == null)
            {
                _routes = typeof(BaseController).GetAllSubTypes()
                    .Where(t => !t.IsAbstract)
                    .SelectMany(type =>
                    {
                        var methods = CollectActionMethods(type);
                        return methods.Select(m => $"{type.Name}/{m}");
                    })
                    .ToList();
            }

            return _routes;
        }

        #endregion

        public static Task Run(this IAppRouter router, string path)
        {
            var appPath = new AppPath(path);
            return router.Run(appPath);
        }

        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool IsAsyncMethod(this MethodInfo method)
        {
            // Obtain the custom attribute for the method. 
            // The value returned contains the StateMachineType property. 
            // Null is returned if the attribute isn't present for the method. 
            var attrib = (AsyncStateMachineAttribute) method.GetCustomAttribute(typeof(AsyncStateMachineAttribute));

            return (attrib != null);
        }
        
        internal static List<ActionMethodDescription> CollectActionMethods(this Type t)
        {
            var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(x =>
                {
                    if (typeof(IActionResult).IsAssignableFrom(x.ReturnType))
                        return true;
                    if (x.IsAsyncMethod())
                    {
                        if (x.ReturnType.IsGenericType)
                        {
                            var ga = x.ReturnType.GetGenericArguments();
                            return ga.Length == 1 && typeof(IActionResult).IsAssignableFrom(ga[0]);
                        }

                        return x.ReturnType == typeof(Task);
                    }
                    else
                    {
                        if (x.ReturnType.IsGenericType)
                        {
                            var ga = x.ReturnType.GetGenericArguments();
                            return ga.Length == 1 && typeof(IActionResult).IsAssignableFrom(ga[0]);
                        }

                        return x.ReturnType == typeof(void)
                               || x.ReturnType == typeof(Task)
                               || typeof(IActionResult).IsAssignableFrom(x.ReturnType);
                    }
                })
                .ToList();

            return methods.ConvertAll(x => new ActionMethodDescription(x));
        }
    }
}