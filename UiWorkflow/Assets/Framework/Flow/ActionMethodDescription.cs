using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Framework.Flow
{
    class ActionMethodDescription
    {
        public MethodInfo Info { get; }
        public string Name => Info.Name;
        public ActionMethodArg[] Args { get; }

        public ActionMethodDescription(MethodInfo info)
        {
            Info = info;
            Args = info.GetParameters().Select(x => new ActionMethodArg(x)).ToArray();
        }

        public override string ToString()
        {
            return $"{Name}({string.Join(",", Args.Select(x => x.ToString()))})";
        }

        #region Match

        public bool IsMatch(AppPath info)
        {
            return Name == info.Action && IsMatch(info.Args);
        }

        bool IsMatch(Dictionary<string, string> parameters)
        {
            foreach (var infoArg in parameters)
                if (Args.FirstOrDefault(a => a.Name == infoArg.Key) == null)
                    return false;

            foreach (var methodArg in Args)
            {
                if (methodArg.IsNullable)
                    continue;

                if (!parameters.TryGetValue(methodArg.Name, out _))
                    return false;
            }

            return true;
        }

        #endregion

        #region Invoke

        public async Task<IActionResult> Invoke(BaseController controller, AppPath info)
        {
            var args = new object[Args.Length];
            for (var i = 0; i < Args.Length; ++i)
            {
                var arg = Args[i];
                if (info.Args.TryGetValue(arg.Name, out var strValue))
                    args[i] = arg.Parse(strValue);
                else
                    args[i] = null;
            }

            return await Invoke(controller, args);
        }

        //TODO: remove dynamic to use on mobile platforms
        async Task<IActionResult> Invoke(BaseController controller, object[] args)
        {
            IActionResult r;

            if (Info.IsAsyncMethod())
                r = await (dynamic) Info.Invoke(controller, args);
            else if (Info.ReturnType.IsGenericType &&
                     Info.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                r = await (dynamic) Info.Invoke(controller, args);
            else if (Info.ReturnType == typeof(Task))
            {
                await (Task) Info.Invoke(controller, args);
                r = new OkAction();
            }
            else if (Info.ReturnType == typeof(void))
            {
                Info.Invoke(controller, args);
                r = new OkAction();
            }
            else
                r = (IActionResult) Info.Invoke(controller, args);

            return r;
        }

        #endregion
    }
}