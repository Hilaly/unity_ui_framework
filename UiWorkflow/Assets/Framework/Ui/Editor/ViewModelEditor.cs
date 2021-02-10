using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Ui.Editor
{
    public class ViewModelEditor
    {
        static readonly string prefViewModelGeneratorPath = "ViewModelGenerator_path";
        static readonly string prefViewModelGeneratorMode = "ViewModelGenerator_mode";
        
        public static string PathToGenerate
        {
            get => EditorPrefs.GetString(prefViewModelGeneratorPath, Path.Combine("Assets", "Scripts", "Generated"));
            set => EditorPrefs.SetString(prefViewModelGeneratorPath, value);
        }

        public static bool IsFullMode
        {
            get => EditorPrefs.GetBool(prefViewModelGeneratorMode, false);
            set => EditorPrefs.SetBool(prefViewModelGeneratorMode, value);
        }
        
        [PreferenceItem("View Models")]
        static void PreferenceItem()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.HelpBox(@"This is path, where new files with generated classes will locale",
                MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Directory for generated files", GUILayout.Width(200));
            PathToGenerate = EditorGUILayout.TextField(PathToGenerate, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            IsFullMode =
                EditorGUILayout.Toggle(
                    new GUIContent("Use full mode (can generate properties not from prefab)",
                        "Extended selection menu for bindings properties"), IsFullMode);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button(new GUIContent("Generate from code")))
            {
                RegenerateAllModel();
            }

            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck())
            {
            }
        }
        
        //TODO: create window and move calling of this method there
        [MenuItem("Framework/Ui/Generate ViewModels from models")]
        static void RegenerateAllModel()
        {
            ViewModelsGenerator.RegenerateAllModel(PathToGenerate);
        }
    }
}