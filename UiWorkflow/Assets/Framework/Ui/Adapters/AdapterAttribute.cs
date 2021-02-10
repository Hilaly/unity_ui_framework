using System;

namespace Framework.Ui.Adapters
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AdapterAttribute : Attribute
    {
        /// <summary>
        /// Set this if adapter must have count of attributes
        /// </summary>
        public int ArgumentsCount { get; set; } = -1;
        public Type SourceType { get; }
        public Type TargetType { get; }

        public AdapterAttribute(Type sourceType, Type targetType)
        {
            SourceType = sourceType;
            TargetType = targetType;
        }
    }

    public interface IAdapter
    {}
    
    public interface IAdapter<in TSource, out TTarget> : IAdapter
    {
        TTarget Convert(params TSource[] values);
    }
    
    [Adapter(typeof(int), typeof(string), ArgumentsCount = 1)]
    public class IntToStringAdapter : IAdapter<int, string>
    {
        public string Convert(params int[] values)
        {
            return values[0].ToString();
        }
    }

    [Adapter(typeof(object), typeof(string))]
    public class LocalizeTextAdapter : IAdapter<object, string>
    {
        public string LocaleKey;
        
        public string Convert(params object[] values)
        {
            return string.Format(LocaleKey, values);
        }
    }
}