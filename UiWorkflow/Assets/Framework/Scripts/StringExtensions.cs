using System;

namespace Framework
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        public static bool NotNullOrEmpty(this string source)
        {
            return !string.IsNullOrEmpty(source);
        }

        public static string[] Split(this string source, string separator, StringSplitOptions options)
        {
            return source.Split(new[] {separator}, options);
        }
    }
}