using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Flow
{
    public class PathBuilder
    {
        private string _controller;
        private string _action;
        private readonly Dictionary<string, object> _args = new Dictionary<string, object>();
        
        private PathBuilder()
        {}

        public static PathBuilder Controller(BaseController c)
        {
            return Controller(c.Name);
        }

        public static PathBuilder Controller<T>() where T : BaseController
        {
            return Controller(typeof(T).Name.Replace("Controller", ""));
        }

        public static PathBuilder Controller(Type type)
        {
            return Controller(type.Name.Replace("Controller", ""));
        }

        public static PathBuilder Controller(string controller)
        {
            return new PathBuilder() {_controller = controller};
        }

        public PathBuilder Action(string name)
        {
            _action = name;
            return this;
        }

        public PathBuilder AddArg(string name, object value)
        {
            _args[name] = value;
            return this;
        }

        public override string ToString()
        {
            if (_args.Count > 0)
                return $"/{_controller}/{_action}?{string.Join("&", _args.Select(x => $"{x.Key}={x.Value}"))}";
            return $"/{_controller}/{_action}";
        }

        public AppPath Build()
        {
            return new AppPath(ToString());
        }
    }
}