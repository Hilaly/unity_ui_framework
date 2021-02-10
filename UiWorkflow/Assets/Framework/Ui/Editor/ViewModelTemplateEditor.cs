using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Ui.Adapters;
using UnityEditor;
using UnityEngine;

namespace Framework.Ui.Editor
{
    [CustomEditor(typeof(ViewModelTemplate))]
    public class ViewModelTemplateEditor : UnityEditor.Editor
    {
        private ViewModelTemplate targetScript;

        private bool _propertiesFoldout;
        private readonly List<bool> _propertiesFoldouts = new List<bool>();

        private bool _eventsFoldout;
        private readonly List<bool> _eventsFoldouts = new List<bool>();

        #region Caches

        #region View Model Properties

        private List<ViewModelsGenerator.ComponentPropertyDefine> _allViewPropertiesCache;
        private List<ViewModelsGenerator.ComponentPropertyDefine> _prefabViewPropertiesCache;
        private List<ViewModelsGenerator.ComponentPropertyDefine> GetAllViewProperties()
        {
            return _allViewPropertiesCache ?? (_allViewPropertiesCache = ViewModelsGenerator.CollectAllViewProperties());
        }
        private List<ViewModelsGenerator.ComponentPropertyDefine> GetNeededViewProperties()
        {
            if(ViewModelEditor.IsFullMode)
                return _allViewPropertiesCache ?? (_allViewPropertiesCache = ViewModelsGenerator.CollectAllViewProperties());
            if (_prefabViewPropertiesCache == null)
            {
                var existComponents = new HashSet<Type>(targetScript.gameObject.GetComponentsInChildren<Component>()
                    .Select(x => x.GetType())) {typeof(GameObject)};
                
                _prefabViewPropertiesCache = GetAllViewProperties()
                    .Where(x => existComponents.Contains(x.ComponentType))
                    .ToList();
            }

            return _prefabViewPropertiesCache;
        }

        #endregion

        #region Model Properties

        private List<ViewModelsGenerator.ComponentPropertyDefine> _allModelPropertiesCache;

        private ViewModelsGenerator.ComponentPropertyDefine[] GetAllModelProperties(Type baseType)
        {
            return (_allModelPropertiesCache ??
                    (_allModelPropertiesCache = ViewModelsGenerator.CollectAllModelProperties()))
                .Where(x => baseType.IsAssignableFrom(x.PropertyType))
                .ToArray();
        }

        #endregion

        #region Adapters

        private List<Type> _allAdapters;

        private List<Type> GetAllAdapters()
        {
            return _allAdapters ?? (_allAdapters = ViewModelsGenerator.CollectAdapters());
        }

        private List<Type> GetAdapters(Type targetType)
        {
            return GetAllAdapters()
                .Where(x => x == null || x.GetMethod("Convert")?.ReturnType == targetType && targetType != null)
                .ToList();
        }
        
        #endregion
        
        #endregion

        private void OnEnable()
        {
            _allModelPropertiesCache = null;
            _allAdapters = null;
            _allViewPropertiesCache = null;
            _prefabViewPropertiesCache = null;
            
            targetScript = (ViewModelTemplate) target;
            if (targetScript.ViewModelName.IsNullOrEmpty())
            {
                targetScript.ViewModelName = $"{targetScript.name}ViewModel";
                EditorUtility.SetDirty(targetScript);
            }
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

                EditorGUILayout.Space();

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

                EditorGUILayout.Space();

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
            var allProperties = GetNeededViewProperties();
            var allStrProperties = allProperties.Select(x => x.DisplayText).ToArray();
            var selectedProperty = Array.IndexOf(allStrProperties, desc.ViewProperty);
            var newSelected = EditorGUILayout.Popup("ViewProperty", selectedProperty, allStrProperties);
            if (newSelected != selectedProperty)
            {
                desc.ViewProperty = newSelected >= 0 ? allStrProperties[newSelected] : null;
                EditorUtility.SetDirty(targetScript);
            }

            //Adapter
            //TODO: draw adapter parameters
            var temp = GUI.enabled;
            var viewProp = newSelected >= 0 ? allProperties[newSelected] : null;
            GUI.enabled = viewProp != null && temp;
            var allAdapters = GetAdapters(viewProp?.PropertyType);
            var allStrAdapters = allAdapters.Select(x => x?.Name ?? "--").ToArray();
            var selectedAdapter = Array.IndexOf(allStrAdapters, desc.Adapter);
            var newAdapter = EditorGUILayout.Popup("Adapter", selectedAdapter, allStrAdapters);
            var newAdapterValue = newAdapter >= 0 ? allStrAdapters[newAdapter] : null;
            if (desc.Adapter != newAdapterValue)
            {
                desc.Adapter = newAdapter > 0 ? newAdapterValue : null;
                desc.ModelProperties.Clear();
                EditorUtility.SetDirty(targetScript);
            }
            GUI.enabled = temp;
            
            //Model properties
            temp = GUI.enabled;
            var adapter = newAdapter >= 0 ? allAdapters[newAdapter] : null;
            GUI.enabled = (adapter != null || viewProp != null) && temp;
            if (adapter == null)
            {
                var targetCount = 1;
                if (desc.ModelProperties.Count > targetCount) 
                    desc.ModelProperties.RemoveRange(targetCount, desc.ModelProperties.Count - targetCount);
                while (desc.ModelProperties.Count < targetCount)
                    desc.ModelProperties.Add(string.Empty);

                var argType = viewProp?.PropertyType;
                for (var i = 0; i < desc.ModelProperties.Count; ++i)
                {
                    var tempModelPropertyValue = DrawSelectModelProperty(argType, desc.ModelProperties[i]);
                    if (tempModelPropertyValue != desc.ModelProperties[i])
                    {
                        desc.ModelProperties[i] = tempModelPropertyValue;
                        EditorUtility.SetDirty(targetScript);
                    }
                }
            }
            else
            {
                var adapterInfo = adapter.GetAttribute<AdapterAttribute>(false);
                if (adapterInfo.ArgumentsCount >= 0)
                {
                    var targetCount = adapterInfo.ArgumentsCount;
                    if (desc.ModelProperties.Count > targetCount) 
                        desc.ModelProperties.RemoveRange(targetCount, desc.ModelProperties.Count - targetCount);
                    while (desc.ModelProperties.Count < targetCount)
                        desc.ModelProperties.Add(string.Empty);

                    var argType = adapterInfo.SourceType;
                    for (var i = 0; i < desc.ModelProperties.Count; ++i)
                    {
                        var tempModelPropertyValue = DrawSelectModelProperty(argType, desc.ModelProperties[i]);
                        if (tempModelPropertyValue != desc.ModelProperties[i])
                        {
                            desc.ModelProperties[i] = tempModelPropertyValue;
                            EditorUtility.SetDirty(targetScript);
                        }
                    }
                }
                else
                {
                    var argType = adapterInfo.SourceType;
                    for (var i = 0; i < desc.ModelProperties.Count; ++i)
                    {
                        var tempModelPropertyValue = DrawSelectModelProperty(argType, desc.ModelProperties[i]);
                        if (tempModelPropertyValue != desc.ModelProperties[i])
                        {
                            desc.ModelProperties[i] = tempModelPropertyValue;
                            EditorUtility.SetDirty(targetScript);
                        }
                    }

                    EditorGUILayout.BeginHorizontal();
                    
                    if (GUILayout.Button("Add"))
                    {
                        desc.ModelProperties.Add(string.Empty);
                        EditorUtility.SetDirty(targetScript);
                    }

                    var buttonEnabled = desc.ModelProperties.Count > 0;
                    var tempAdd = GUI.enabled;
                    GUI.enabled = buttonEnabled && tempAdd && temp;
                    if (GUILayout.Button("Remove"))
                    {
                        desc.ModelProperties.RemoveAt(desc.ModelProperties.Count - 1);
                        EditorUtility.SetDirty(targetScript);
                    }
                    GUI.enabled = tempAdd;
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
            GUI.enabled = temp;

            //TODO: other properties
            
            EditorGUILayout.EndVertical();
        }

        string DrawSelectModelProperty(Type baseType, string oldValue)
        {
            if (baseType == null)
                return string.Empty;

            var allModels = GetAllModelProperties(baseType);
            var allStrModels = allModels
                .Select(x => $"{x.ComponentType.Name}/{x.PropertyInfo.Name}:{x.PropertyType.Name}")
                .ToArray();
            var selectedModel = Array.IndexOf(allStrModels, oldValue);
            var newModel = EditorGUILayout.Popup(selectedModel, allStrModels);
            var newModelValue = newModel >= 0 ? allStrModels[newModel] : string.Empty;

            return newModelValue;
        }

        private void Regenerate()
        {
            ViewModelsGenerator.GeneratePrefab(targetScript, ViewModelEditor.PathToGenerate);
            AssetDatabase.Refresh();
        }
    }
}