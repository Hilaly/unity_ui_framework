using System;
using System.Threading.Tasks;

namespace Framework.Flow
{
    public interface IAppRouter
    {
        Task Run(AppPath appPath);
        Task Run(IActionResult actionResult);
        Task Run(Func<IActionResult> actionCall);
    }
}