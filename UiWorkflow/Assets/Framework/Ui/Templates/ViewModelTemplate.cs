using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Ui
{
    [Serializable]
    public class ViewModelPropertyDescription
    {
        public string name;
        public string ViewProperty;
        public string Adapter;
        public List<string> ModelProperties = new List<string>();
    }

    [Serializable]
    public class ViewModelEventDescription
    {
        public string name;
    }
    
    public class ViewModelTemplate : MonoBehaviour
    {
        [SerializeField] string _viewModelName;
        public List<ViewModelPropertyDescription> properties = new List<ViewModelPropertyDescription>();
        public List<ViewModelEventDescription> events = new List<ViewModelEventDescription>();

        public string ViewModelName
        {
            get => _viewModelName;
            set => _viewModelName = value;
        }
    }
}