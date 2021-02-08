using System;
using Framework.Flow;

namespace Framework.Ui
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = true)]
    public class ViewModelPropertyAttribute : Attribute
    {
        public string ViewModel { get; }
        public string PropertyName { get; }
        public string Namespace { get; set; }

        public ViewModelPropertyAttribute(string viewModel, string propertyName)
        {
            ViewModel = viewModel;
            PropertyName = propertyName;
        }
    }
}