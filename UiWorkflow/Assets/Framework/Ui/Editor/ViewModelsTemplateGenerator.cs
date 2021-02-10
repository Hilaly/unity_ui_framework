using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Framework.Flow;
using Framework.Ui.Adapters;
using UnityEngine;

namespace Framework.Ui.Editor
{
    public partial class ViewModelsGenerator
    {
        private static readonly HashSet<Type> hiddenTypes = new HashSet<System.Type>()
        {
            typeof(ViewModelTemplate),
            typeof (UnityWeld.Binding.AbstractMemberBinding),
            typeof (UnityWeld.Binding.OneWayPropertyBinding),
            typeof (UnityWeld.Binding.TwoWayPropertyBinding)
        };
        
        public class ComponentPropertyDefine
        {
            public Type ComponentType;
            public Type PropertyType;
            public PropertyInfo PropertyInfo;
            public string DisplayText => $"{ComponentType.FullName}/{PropertyInfo.Name} : {PropertyType.Name}";
        }

        private static List<Type> CollectAllModelTypes()
        {
            var types = typeof(object).GetAllSubTypes()
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(IsInheritedFromModel)
                .ToList();
            return types;
        }

        public static List<ComponentPropertyDefine> CollectAllModelProperties()
        {
            var result = new List<ComponentPropertyDefine>();
            var allTypes = CollectAllModelTypes();

            foreach (var type in allTypes)
            {
                if(hiddenTypes.Contains(type))
                    continue;
                
                if(type.FullName.Contains("UnityWeld"))
                    continue;
                
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var propertyInfo in properties)
                {
                    if(propertyInfo.GetGetMethod(false) == null || propertyInfo.GetSetMethod(false) == null)
                        continue;
                    if(propertyInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0)
                        continue;
                    result.Add(new ComponentPropertyDefine()
                    {
                        ComponentType = type,
                        PropertyType = propertyInfo.PropertyType,
                        PropertyInfo = propertyInfo
                    });
                }
            }

            result.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText, StringComparison.Ordinal));

            return result;
        }

        public static List<ComponentPropertyDefine> CollectAllViewProperties()
        {
            var result = new List<ComponentPropertyDefine>();
            
            var allTypes = typeof(Component).GetAllSubTypes();
            allTypes.Add(typeof(GameObject));

            foreach (var type in allTypes)
            {
                if(hiddenTypes.Contains(type))
                    continue;
                
                if(type.FullName.Contains("UnityWeld"))
                    continue;
                
                //TODO: add filter types
                
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var propertyInfo in properties)
                {
                    if(propertyInfo.GetGetMethod(false) == null || propertyInfo.GetSetMethod(false) == null)
                        continue;
                    if(propertyInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0)
                        continue;
                    result.Add(new ComponentPropertyDefine()
                    {
                        ComponentType = type,
                        PropertyType = propertyInfo.PropertyType,
                        PropertyInfo = propertyInfo
                    });
                }
            }

            result.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText, StringComparison.Ordinal));

            return result;
        }

        public static List<Type> CollectAdapters()
        {
            var result = typeof(object).GetAllSubTypes()
                .Where(x => !x.IsAbstract && x.GetAttributes<AdapterAttribute>(false).Length > 0)
                .OrderBy(x => x.Name)
                .ToList();
            result.Insert(0, null);
            return result;
        }
    }
}