using System;
using System.Collections.Generic;
using System.IO;
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

        public static void GeneratePrefab(ViewModelTemplate template, string directory)
        {
            Directory.CreateDirectory(directory);
            var fileName = template.ViewModelName;
            var filePath = Path.Combine(directory, string.Format(FilenameTemplate, fileName));
            UnityEngine.Debug.Log($"Generating {template.ViewModelName} -> {filePath}");
            using (var stream = File.CreateText(filePath))
            {
                WriteData(template, stream);
            }
        }

        private static void WriteData(ViewModelTemplate typeDefine, StreamWriter stream)
        {
            var allTypes = typeof(object).GetAllSubTypes();
            var allAdapters = CollectAdapters();
            stream.WriteLine(
                "//Generated from prefab, don't change code, all changes can be lost if it will be regenerated");
            stream.WriteLine();
            stream.WriteLine($"using UnityEngine;");
            stream.WriteLine();
            stream.WriteLine($"namespace {ViewModelEditor.Namespace}");
            stream.WriteLine("{");
            stream.WriteLine($"\tpublic partial class {typeDefine.ViewModelName} : UnityEngine.MonoBehaviour");
            stream.WriteLine("\t{");
            foreach (var propDef in typeDefine.properties)
            {
                var viewArgs = propDef.ViewProperty.Split(":", StringSplitOptions.RemoveEmptyEntries);
                var viewTypes = viewArgs[0].Split("/", StringSplitOptions.RemoveEmptyEntries);
                var viewComponentTypeName = viewTypes[0].Trim();
                var viewComponentPropertyName = viewTypes[1].Trim();
                var viewType = allTypes.Find(x => x.FullName == viewComponentTypeName);
                var viewProperty = viewType.GetProperty(viewComponentPropertyName);
                stream.WriteLine($"\t\t[SerializeField] {viewType.FullName} _{propDef.name};");
                if (propDef.Adapter.NotNullOrEmpty())
                {
                    var adapterArgs = $"/*Insert args for {propDef.Adapter} here*/";
                    var adapterType = allAdapters.Find(x => x?.Name == propDef.Adapter);
                    stream.WriteLine(
                        $"\t\t[SerializeField] {adapterType.FullName} _{propDef.name}Adapter = new {adapterType.FullName}({adapterArgs});");
                }
                stream.WriteLine($"\t\tpublic {viewProperty.PropertyType.FullName} {propDef.name}");
                stream.WriteLine($"\t\t{{");
                stream.WriteLine($"\t\t\tget => _{propDef.name}.{viewComponentPropertyName};");
                stream.WriteLine($"\t\t\tset {{ if(value != _{propDef.name}.{viewComponentPropertyName}) _{propDef.name}.{viewComponentPropertyName} = value; }}");
                stream.WriteLine($"\t\t}}");
            }
            stream.WriteLine();
            stream.WriteLine("\t}");
            stream.WriteLine("}");
        }
    }
}