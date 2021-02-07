using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Flow
{
    public class AppPath
    {
        public string Controller { get; }
        public string Action { get; }
        public Dictionary<string, string> Args { get; } = new Dictionary<string, string>();
        
        public AppPath(string route)
        {
            var parts = route.Split("?", StringSplitOptions.RemoveEmptyEntries);
            var routeParts = parts[0].Split("/", StringSplitOptions.RemoveEmptyEntries);
            Controller = routeParts[0];
            Action = routeParts[1];
            if (parts.Length > 1)
            {
                var argsPairs = parts[1].Split("&", StringSplitOptions.RemoveEmptyEntries);
                foreach (var strPair in argsPairs)
                {
                    var pair = strPair.Split("=", StringSplitOptions.RemoveEmptyEntries);
                    if (pair.Length > 1)
                        Args[pair[0]] = pair[1];
                    else
                        Args[pair[0]] = null;
                }
            }
        }

        public override string ToString()
        {
            if (Args.Count > 0)
                return $"{Controller}/{Action}?{string.Join("&", Args.Select(x => $"{x.Key}={x.Value}"))}";
            return $"{Controller}/{Action}";
        }
    }
}