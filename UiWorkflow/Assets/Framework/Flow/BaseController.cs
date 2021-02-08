using System;

namespace Framework.Flow
{
    public abstract class BaseController
    {
        public ActionContext Context { get; set; }
        
        public string Name { get; }

        protected BaseController()
        {
            Name = GetType().Name.Replace("Controller", "");
        }

        protected IActionResult GoToPath(AppPath appPath)
        {
            return new RouteAction(appPath);
        }

        protected IActionResult GoToPage(string controller, string action)
        {
            var appPath = PathBuilder
                .Controller(controller)
                .Action(action)
                .Build();

            return GoToPath(appPath);
        }

        protected IActionResult GoToAction(string action)
        {
            var appPath = PathBuilder
                .Controller(this)
                .Action(action)
                .Build();

            return GoToPath(appPath);
        }

        protected IActionResult Ok => new OkAction();

        protected IActionResult Error(Exception ex)
        {
            return new ExceptionAction(ex);
        }
    }
}