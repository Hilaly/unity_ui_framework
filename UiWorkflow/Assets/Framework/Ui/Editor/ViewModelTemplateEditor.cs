using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Ui.Adapters;
using UnityEditor;
using UnityEngine;

namespace Framework.Ui
{
    [CustomEditor(typeof(ViewModelTemplate))]
    public class ViewModelTemplateEditor : Editor
    {
        private ViewModelTemplate targetScript;
        
        private bool _propertiesFoldout = true;
        private List<bool> _propertiesFoldouts = new List<bool>();
        
        private bool _eventsFoldout = true;
        private List<bool> _eventsFoldouts = new List<bool>();

        #region Caches

        private List<ViewModelsGenerator.ComponentPropertyDefine> _allPropertiesCache;
        private List<ViewModelsGenerator.ComponentPropertyDefine> GetAllProperties()
        {
            return _allPropertiesCache ?? (_allPropertiesCache = ViewModelsGenerator.CollectAllViewProperties());
        }

        private List<Type> _allAdapters;

        private List<Type> GetAllAdapters()
        {
            return _allAdapters ?? (_allAdapters = ViewModelsGenerator.CollectAdapters());
        }

        private List<Type> GetAdapters(Type targetType)
        {
            return GetAllAdapters().Where(x => x.GetMethod("Convert")?.ReturnType == targetType && targetType != null)
                .ToList();
        }
        
        #endregion

        private void OnEnable()
        {
            targetScript = (ViewModelTemplate) target;
        }

        public override void OnInspectorGUI()
        {
            var enabled = GUI.enabled;
            
            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Exit play mode to make changes.", MessageType.Info);
                GUI.enabled = false;
            }

            var newViewModelName = EditorGUILayout.TextField("ViewModel name", targetScript.ViewModelName);
            if (newViewModelName != targetScript.ViewModelName)
            {
                targetScript.ViewModelName = newViewModelName;
                EditorUtility.SetDirty(targetScript);
            }

            EditorGUILayout.Space();

            #region Properties

            _propertiesFoldout = EditorGUILayout.Foldout(_propertiesFoldout, "Properties");
            if (_propertiesFoldout)
            {
                EditorGUI.indentLevel++;
                for (var index = 0; index < targetScript.properties.Count; index++)
                {
                    var propertyDescription = targetScript.properties[index];
                    while (_propertiesFoldouts.Count <= index)
                        _propertiesFoldouts.Add(true);
                    _propertiesFoldouts[index] = EditorGUILayout.Foldout(_propertiesFoldouts[index],
                        propertyDescription.name.NotNullOrEmpty() ? propertyDescription.name : "UnknownProperty");
                    if (_propertiesFoldouts[index])
                    {
                        EditorGUI.indentLevel++;
                        Draw(propertyDescription, index);
                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add property"))
                {
                    targetScript.properties.Add(new ViewModelPropertyDescription());
                    EditorUtility.SetDirty(targetScript);
                }

                var buttonEnabled = targetScript.properties.Count > 0;
                var temp = GUI.enabled;
                GUI.enabled = buttonEnabled && temp;
                if (GUILayout.Button("Remove property"))
                {
                    targetScript.properties.RemoveAt(targetScript.properties.Count - 1);
                    EditorUtility.SetDirty(targetScript);
                }
                GUI.enabled = temp;
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            #endregion

            EditorGUILayout.Space();

            #region Events

            _eventsFoldout = EditorGUILayout.Foldout(_eventsFoldout, "Events");
            if (_eventsFoldout)
            {
                EditorGUI.indentLevel++;
                for (var index = 0; index < targetScript.events.Count; index++)
                {
                    var eventDescription = targetScript.events[index];
                    while (_eventsFoldouts.Count <= index)
                        _eventsFoldouts.Add(true);
                    _eventsFoldouts[index] = EditorGUILayout.Foldout(_eventsFoldouts[index],
                        eventDescription.name.NotNullOrEmpty() ? eventDescription.name : "UnknownEvent");
                    if (_eventsFoldouts[index])
                    {
                        EditorGUI.indentLevel++;
                        Draw(eventDescription, index);
                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add event"))
                {
                    targetScript.events.Add(new ViewModelEventDescription());
                    EditorUtility.SetDirty(targetScript);
                }

                var buttonEnabled = targetScript.events.Count > 0;
                var temp = GUI.enabled;
                GUI.enabled = buttonEnabled && temp;
                if (GUILayout.Button("Remove event"))
                {
                    targetScript.events.RemoveAt(targetScript.events.Count - 1);
                    EditorUtility.SetDirty(targetScript);
                }
                GUI.enabled = temp;
                
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            #endregion

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Generate ViewModel"))
                Regenerate();
            
            GUI.enabled = enabled;
        }

        private void Draw(ViewModelEventDescription desc, int index)
        {
            EditorGUILayout.BeginVertical();
            
            var newName = EditorGUILayout.TextField("Name", desc.name);
            if (newName != desc.name)
            {
                desc.name = newName;
                EditorUtility.SetDirty(targetScript);
            }
            
            //TODO: other properties
            
            EditorGUILayout.EndVertical();
        }

        private void Draw(ViewModelPropertyDescription desc, int index)
        {
            EditorGUILayout.BeginVertical();
            
            //Name
            var newName = EditorGUILayout.TextField("Name", desc.name);
            if (newName != desc.name)
            {
                desc.name = newName;
                EditorUtility.SetDirty(targetScript);
            }

            //View property
            var allStrProperties = GetAllProperties().Select(x => x.DisplayText).ToArray();
            var selectedProperty = Array.IndexOf(allStrProperties, desc.ViewProperty);
            var newSelected = EditorGUILayout.Popup("ViewProperty", selectedProperty, allStrProperties);
            if (newSelected != selectedProperty)
            {
                desc.ViewProperty = newSelected >= 0 ? allStrProperties[newSelected] : null;
                EditorUtility.SetDirty(targetScript);
            }

            //Adapter
            var temp = GUI.enabled;
            var viewProp = newSelected >= 0 ? GetAllProperties()[newSelected] : null;
            GUI.enabled = viewProp != null && temp;
            var allAdapters = GetAdapters(viewProp?.PropertyType);
            var allStrAdapters = allAdapters.Select(x => x.Name).ToArray();
            var selectedAdapter = Array.IndexOf(allStrAdapters, desc.Adapter);
            var newAdapter = EditorGUILayout.Popup("Adapter", selectedAdapter, allStrAdapters);
            var newAdapterValue = newAdapter >= 0 ? allStrAdapters[newAdapter] : null;
            if (desc.Adapter != newAdapterValue)
            {
                desc.Adapter = newAdapterValue;
                EditorUtility.SetDirty(targetScript);
            }
            GUI.enabled = temp;

            //TODO: other properties
            
            EditorGUILayout.EndVertical();
        }

        private void Regenerate()
        {
            throw new NotImplementedException();
        }
    }
}