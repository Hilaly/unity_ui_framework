using System;

namespace Framework.Ui
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ViewModelMethodAttribute : Attribute
    {
        public string ViewModel { get; }
        public string MethodName { get; }
        public string AppPath { get; }
        public string Namespace { get; set; }

        public ViewModelMethodAttribute(string viewModel, string methodName, string appPath)
        {
            ViewModel = viewModel;
            AppPath = appPath;
            MethodName = methodName;
        }
    }
}