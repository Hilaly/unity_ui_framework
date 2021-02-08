using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Framework.Flow;
using UnityEditor;
using UnityEngine;

namespace Framework.Ui
{
    public partial class ViewModelsGenerator
    {
        public static List<TypeDefine> GetAllViewModelsDefines()
        {
            var types = typeof(object).GetAllSubTypes()
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(IsInheritedFromModel)
                .ToList();

            var sources = new Dictionary<string, TypeDefine>();
            foreach (var type in types)
                PrepareDefines(type, sources);

            return sources.Values.ToList();
        }

        //TODO: create window and move calling of this method there
        [MenuItem("Framework/Ui/Generate ViewModels from models")]
        public static void RegenerateAllModel()
        {
            var sources = GetAllViewModelsDefines();

            var directory = Path.Combine("Assets", "Demo", "Scripts", "Generated");
            const string filenameTemplate = "{0}.cs";
            foreach (var filePath in new HashSet<string>(sources.Select(viewModel => viewModel.Name))
                .Select(fileName => Path.Combine(directory, string.Format(filenameTemplate, fileName))))
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            foreach (var source in sources)
            {
                var fileName = source.Name;
                var filePath = Path.Combine(directory, string.Format(filenameTemplate, fileName));
                using (var stream = File.AppendText(filePath))
                {
                    WriteData(source, stream);
                }
            }

            AssetDatabase.Refresh();
        }

        static bool IsInheritedFromModel(Type type)
        {
            return type.BaseType != null && type.BaseType.IsGenericType &&
                   type.BaseType.GetGenericTypeDefinition() == typeof(BaseModel<>) &&
                   type.BaseType.GetGenericArguments()[0] == type;
        }

        private static void PrepareDefines(Type type, Dictionary<string, TypeDefine> exists)
        {
            TypeDefine GetTypeDefine(string name, string namespaceName)
            {
                var fullName = $"{namespaceName}.{name}";
                lock (exists)
                {
                    if (!exists.TryGetValue(fullName, out var builderResult))
                        exists.Add(fullName, builderResult = new TypeDefine()
                        {
                            Name = name,
                            Namespace = namespaceName
                        });
                    return builderResult;
                }
            }

            foreach (var memInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attributes = memInfo.GetAttributes<ViewModelPropertyAttribute>(true);
                foreach (var attribute in attributes)
                {
                    var typeDefine = GetTypeDefine(attribute.ViewModel, attribute.Namespace ?? type.Namespace);
                    typeDefine.Properties.Add(new PropertyDefineFromProperty()
                    {
                        Attribute = attribute,
                        Model = type,
                        SourceProperty = memInfo
                    });
                }
            }

            foreach (var memInfo in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var attributes = memInfo.GetAttributes<ViewModelPropertyAttribute>(true);
                foreach (var attribute in attributes)
                {
                    var typeDefine = GetTypeDefine(attribute.ViewModel, attribute.Namespace ?? type.Namespace);
                    typeDefine.Properties.Add(new PropertyDefineFromField()
                    {
                        Attribute = attribute,
                        Model = type,
                        SourceField = memInfo
                    });
                }
            }

            foreach (var memInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                var attributes = memInfo.GetAttributes<ViewModelPropertyAttribute>(true);
                foreach (var attribute in attributes)
                {
                    var typeDefine = GetTypeDefine(attribute.ViewModel, attribute.Namespace ?? type.Namespace);
                    typeDefine.Properties.Add(new PropertyDefineFromMethod()
                    {
                        Attribute = attribute,
                        Model = type,
                        SourceMethod = memInfo
                    });
                }
            }

            foreach (var attribute in type.GetAttributes<ViewModelMethodAttribute>(true))
            {
                var typeDefine = GetTypeDefine(attribute.ViewModel, attribute.Namespace ?? type.Namespace);
                typeDefine.Methods.Add(new AppPathMethodDefine()
                {
                    Attribute = attribute,
                    Model = type
                });
            }
        }

        public class TypeDefine
        {
            public string Namespace;
            public string Name;

            public List<PropertyDefine> Properties = new List<PropertyDefine>();
            public List<AppPathMethodDefine> Methods = new List<AppPathMethodDefine>();

            public string GetCoolName()
            {
                if (Name.EndsWith("ViewModel"))
                    return Name;
                return $"{Name}ViewModel";
            }
        }

        public class AppPathMethodDefine
        {
            public Type Model;
            public ViewModelMethodAttribute Attribute;
        }

        public abstract class PropertyDefine
        {
            public Type Model;
            public ViewModelPropertyAttribute Attribute;
            public abstract MemberInfo Source { get; }

            public static string GetResultPropertyType(PropertyDefine propertyDefine)
            {
                Type sourceType;
                switch (propertyDefine)
                {
                    case PropertyDefineFromProperty prop:
                        sourceType = prop.SourceProperty.PropertyType;
                        break;
                    case PropertyDefineFromField field:
                        sourceType = field.SourceField.FieldType;
                        break;
                    case PropertyDefineFromMethod method:
                        sourceType = method.SourceMethod.ReturnType;
                        break;
                    default:
                        throw new Exception("Unsupported type");
                }

                if (sourceType.IsEnum)
                    return "int";
                return sourceType.FullName;
            }

            public virtual string GetCall()
            {
                return Source.Name;
            }
        }

        class PropertyDefineFromProperty : PropertyDefine
        {
            public override MemberInfo Source => SourceProperty;
            public PropertyInfo SourceProperty;
        }

        class PropertyDefineFromField : PropertyDefine
        {
            public override MemberInfo Source => SourceField;
            public FieldInfo SourceField;
        }

        class PropertyDefineFromMethod : PropertyDefine
        {
            public override MemberInfo Source => SourceMethod;
            public MethodInfo SourceMethod;

            public override string GetCall()
            {
                return $"{base.GetCall()}()";
            }
        }


        //TODO: use T4 to generate code files
        private static void WriteData(TypeDefine typeDefine, StreamWriter stream)
        {
            stream.WriteLine(
                "//Generated from model, dont change code, all changes can be lost if it will be regenerated");
            stream.WriteLine("using UnityWeld.Binding;");
            stream.WriteLine("using Framework.Flow;");
            stream.WriteLine();
            stream.WriteLine($"namespace {typeDefine.Namespace}");
            stream.WriteLine("{");
            stream.WriteLine("\t[Binding]");
            stream.WriteLine(
                $"\tpublic partial class {typeDefine.GetCoolName()} : Framework.Ui.ViewModelMonoBehaviour");
            var models = new HashSet<Type>(typeDefine.Properties.Select(x => x.Model));
            foreach (var model in models)
                stream.WriteLine($"\t\t, IModelObserver<{model.FullName}>");
            stream.WriteLine("\t{");
            stream.WriteLine("\t\t#region Private fields");
            stream.WriteLine();

            foreach (var propertyDefine in typeDefine.Properties)
            {
                stream.WriteLine(
                    $"\t\tprivate {PropertyDefine.GetResultPropertyType(propertyDefine)} _{propertyDefine.Attribute.PropertyName};");
            }

            stream.WriteLine();
            stream.WriteLine("\t\t#endregion");
            stream.WriteLine();
            stream.WriteLine("\t\t#region Bindings properties");
            stream.WriteLine();
            foreach (var propertyDefine in typeDefine.Properties)
            {
                stream.WriteLine(
                    $"\t\t[Binding] public {PropertyDefine.GetResultPropertyType(propertyDefine)} {propertyDefine.Attribute.PropertyName}");
                stream.WriteLine("\t\t{");
                stream.WriteLine($"\t\t\tget => _{propertyDefine.Attribute.PropertyName};");
                stream.WriteLine($"\t\t\tset");
                stream.WriteLine("\t\t\t{");
                stream.WriteLine($"\t\t\t\tif(value == _{propertyDefine.Attribute.PropertyName}) return;");
                stream.WriteLine($"\t\t\t\t_{propertyDefine.Attribute.PropertyName} = value;");
                stream.WriteLine($"\t\t\t\tOnPropertyChanged(nameof({propertyDefine.Attribute.PropertyName}));");
                stream.WriteLine("\t\t\t}");
                stream.WriteLine("\t\t}");
            }

            stream.WriteLine();
            stream.WriteLine("\t\t#endregion");
            stream.WriteLine();
            stream.WriteLine("\t\t#region Bindings methods");
            stream.WriteLine();
            foreach (var methodDefine in typeDefine.Methods)
            {
                stream.WriteLine($"\t\t[Binding] public void {methodDefine.Attribute.MethodName}()");
                stream.WriteLine("\t\t{");
                stream.WriteLine(
                    $"\t\t\tAppRouter.Run($\"{methodDefine.Attribute.AppPath}\");");
                stream.WriteLine("\t\t}");
            }

            stream.WriteLine();
            stream.WriteLine("\t\t#endregion");
            stream.WriteLine();
            stream.WriteLine("\t\t#region Models methods");
            stream.WriteLine();
            foreach (var model in models)
            {
                stream.WriteLine(
                    $"\t\tvoid IModelObserver<{model.FullName}>.ModelChanged({model.FullName} model)");
                stream.WriteLine("\t\t{");
                foreach (var propertyDefine in typeDefine.Properties)
                {
                    if (propertyDefine.Model != model)
                        continue;
                    stream.WriteLine(
                        $"\t\t\t{propertyDefine.Attribute.PropertyName} = ({PropertyDefine.GetResultPropertyType(propertyDefine)})model.{propertyDefine.GetCall()};");
                }

                stream.WriteLine("\t\t}");
                stream.WriteLine("\t\t#if USE_ZENJECT");
                stream.WriteLine("\t\t[Inject]");
                stream.WriteLine("\t\t#endif");
                stream.WriteLine(
                    $"\t\tpublic void InjectModel({model.FullName} model)");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t\tAddOnDispose(model.Subscribe(this));");
                stream.WriteLine($"\t\t\t((IModelObserver<{model.FullName}>) this).ModelChanged(model);");
                stream.WriteLine("\t\t}");
            }

            stream.WriteLine();
            stream.WriteLine("\t\t#endregion");
            stream.WriteLine("\t}");
            stream.WriteLine("}");
        }
    }
}