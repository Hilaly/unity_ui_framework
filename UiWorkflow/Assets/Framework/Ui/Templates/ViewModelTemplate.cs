using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Framework.Ui
{
    [Serializable]
    public class ViewModelPropertyDescription
    {
        public string name;
    }
    
    public class ViewModelTemplate : MonoBehaviour
    {
        public string _name;
        public List<ViewModelPropertyDescription> properties;

        public string Name
        {
            get => _name;
            set => _name = value;
        }
    }
}