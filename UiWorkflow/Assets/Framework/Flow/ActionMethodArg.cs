using System;
using System.Reflection;

namespace Framework.Flow
{
    class ActionMethodArg
    {
        public ParameterInfo Info { get; }
        public bool IsNullable => Info.ParameterType.IsNullable();
        public string Name => Info.Name;

        public ActionMethodArg(ParameterInfo info)
        {
            Info = info;
        }

        public object Parse(string value)
        {
            var t = Info.ParameterType;
            if (t == typeof(string))
                return value;

            if (t == typeof(byte) || typeof(byte?) == t)
                return byte.Parse(value);

            if (t == typeof(short) || typeof(short?) == t)
                return short.Parse(value);
            if (t == typeof(int) || typeof(int?) == t)
                return int.Parse(value);
            if (t == typeof(long) || typeof(long?) == t)
                return long.Parse(value);

            if (t == typeof(ushort) || typeof(ushort?) == t)
                return ushort.Parse(value);
            if (t == typeof(uint) || typeof(uint?) == t)
                return uint.Parse(value);
            if (t == typeof(ulong) || typeof(ulong?) == t)
                return ulong.Parse(value);

            if (t == typeof(float) || typeof(float?) == t)
                return float.Parse(value);
            if (t == typeof(double) || typeof(double?) == t)
                return double.Parse(value);

            if (t.IsEnum)
                return Enum.Parse(t, value, true);

            throw new InvalidCastException($"{t.FullName} is not supported type");
        }

        public override string ToString()
        {
            return $"{Info.ParameterType.Name}{(IsNullable ? "?" : string.Empty)} {Name}";
        }
    }
}