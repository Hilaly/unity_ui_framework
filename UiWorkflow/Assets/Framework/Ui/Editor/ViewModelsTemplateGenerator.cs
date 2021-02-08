using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Framework.Ui.Adapters;
using UnityEngine;

namespace Framework.Ui
{
    public partial class ViewModelsGenerator
    {
        private static readonly HashSet<System.Type> hiddenTypes = new HashSet<System.Type>()
        {
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

        public static List<ComponentPropertyDefine> CollectAllViewProperties()
        {
            var result = new List<ComponentPropertyDefine>();
            
            var allTypes = typeof(MonoBehaviour).GetAllSubTypes();

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
            return typeof(object).GetAllSubTypes()
                .Where(x => !x.IsAbstract && x.GetAttributes<AdapterAttribute>(false).Length > 0)
                .ToList();
        }
    }
}