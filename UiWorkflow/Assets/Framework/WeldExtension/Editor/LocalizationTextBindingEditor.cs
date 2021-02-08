using System;
using UnityEditor;
using UnityEngine;
using UnityWeld.Binding;
using UnityWeld.Binding.Internal;
using UnityWeld_Editor;

namespace Framework.WeldExtension.Editor
{
    
    [CustomEditor(typeof(LocalizationTextBinding))]
    public class LocalizationTextBindingEditor : BaseBindingEditor
    {
        private LocalizationTextBinding targetScript;
        private bool viewModelPropertyPrefabModified;
        private bool viewPropertyPrefabModified;

        private void OnEnable()
        {
            targetScript = (LocalizationTextBinding) target;
        }

        public override void OnInspectorGUI()
        {
            if (CannotModifyInPlayMode())
                GUI.enabled = false;

            var nF = EditorGUILayout.TextField("Format", targetScript.LocalizationKey);
            if (nF != targetScript.LocalizationKey)
            {
                targetScript.LocalizationKey = nF;
                EditorUtility.SetDirty(target);
            }

            UpdatePrefabModifiedProperties();
            FontStyle fontStyle = EditorStyles.label.fontStyle;
            EditorStyles.label.fontStyle = viewPropertyPrefabModified ? FontStyle.Bold : fontStyle;
            bool enabled = GUI.enabled;
            EditorGUILayout.Space();
            EditorStyles.label.fontStyle = viewModelPropertyPrefabModified ? FontStyle.Bold : fontStyle;
            Type adaptedViewPropertyType = typeof(object);

            for (var index = 0; index < targetScript.ViewModelProperties.Count; index++)
            {
                var index1 = index;
                ShowViewModelPropertyMenu(
                    new GUIContent("View-model property", "Property on the view-model to bind to."),
                    TypeResolver.FindBindableProperties((AbstractMemberBinding) targetScript),
                    updatedValue => targetScript.ViewModelProperties[index1] = updatedValue,
                    targetScript.ViewModelProperties[index1],
                    property => typeof(object).IsAssignableFrom(property.PropertyType));
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
                targetScript.ViewModelProperties.Add(string.Empty);
            if (targetScript.ViewModelProperties.Count > 0 && GUILayout.Button("Remove"))
                targetScript.ViewModelProperties.RemoveAt(targetScript.ViewModelProperties.Count - 1);
            EditorGUILayout.EndHorizontal();

            GUI.enabled = enabled;
            EditorStyles.label.fontStyle = fontStyle;
        }

        private void UpdatePrefabModifiedProperties()
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            iterator.Next(true);
            do
            {
                string name = iterator.name;
                if (!(name == "viewModelPropertyName"))
                {
                    if (name == "viewPropertyName")
                        viewPropertyPrefabModified = iterator.prefabOverride;
                }
                else
                    viewModelPropertyPrefabModified = iterator.prefabOverride;
            } while (iterator.Next(false));
        }
    }
}