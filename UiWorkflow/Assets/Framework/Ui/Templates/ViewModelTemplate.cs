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
        public string _name;
        public List<ViewModelPropertyDescription> properties = new List<ViewModelPropertyDescription>();
        public List<ViewModelEventDescription> events = new List<ViewModelEventDescription>();

        public string ViewModelName
        {
            get => _name;
            set => _name = value;
        }
    }
}