using System;

namespace Framework.Ui.Adapters
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AdapterAttribute : Attribute
    {
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
        TTarget Convert(TSource valueIn);
    }
    
    [Adapter(typeof(int), typeof(string))]
    public class IntToStringAdapter : IAdapter<int, string>
    {
        public string Convert(int valueIn)
        {
            return valueIn.ToString();
        }
    }
}