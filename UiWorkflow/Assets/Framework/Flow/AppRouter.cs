using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Ui;
using UnityEngine;

namespace Framework.Flow
{
    class AppRouter : IAppRouter
    {
        class Route
        {
            public BaseController Controller { get; }
            public List<ActionMethodDescription> Methods { get; }

            public Route(BaseController controller)
            {
                Controller = controller;
                Methods = controller.GetType().CollectActionMethods();
            }
        }

        private readonly Dictionary<string, Route> _routes = new Dictionary<string, Route>();
        private readonly IViewModelFactory _viewsFactory;
        private readonly UiLayersManager _uiLayersManager;

        public AppRouter(IViewModelFactory viewsFactory, UiLayersManager uiLayersManager)
        {
            _viewsFactory = viewsFactory;
            _uiLayersManager = uiLayersManager;
        }

        public void RegisterController(BaseController controller)
        {
            var route = new Route(controller);
            _routes.Add(route.Controller.Name, route);
        }

        private async Task<IActionResult> GetAction(AppPath appPath, ActionContext actionContext)
        {
            if (!_routes.TryGetValue(appPath.Controller, out var route))
                throw new Exception($"{appPath.Controller}  is not registered");

            var matchedMethods = route.Methods
                .Where(x => x.IsMatch(appPath))
                .ToList();

            if (matchedMethods.Count == 0)
                throw new Exception($"Can not find any action='{appPath.Action}' on {route.Controller.GetType().Name}");
            if (matchedMethods.Count > 1)
                throw new Exception(
                    $"Can not find action='{appPath.Action}' on {route.Controller.GetType().Name}, valid methods are:\n{string.Join("\n", matchedMethods)}");

            var method = matchedMethods[0];

            var savedContext = route.Controller.Context;
            //Set context before execute
            route.Controller.Context = actionContext;
            var result = await method.Invoke(route.Controller, appPath);
            //Reset context after execution
            route.Controller.Context = savedContext;

            return result;
        }

        public async Task Run(IActionResult actionResult)
        {
            try
            {
                var context = MakeContext();
                await actionResult.ExecuteAsync(context);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public async Task Run(Func<IActionResult> actionCall)
        {
            try
            {
                var context = MakeContext();
                var actionResult = actionCall();
                await actionResult.ExecuteAsync(context);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public async Task Run(AppPath appPath)
        {
            try
            {
                Debug.Log($"Route to {appPath}");
                var context = MakeContext(appPath);
                var actionResult = await GetAction(appPath, context);
                await actionResult.ExecuteAsync(context);
                Debug.Log($"Routed to {appPath} SUCCESS");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogWarning($"Routed to {appPath} FAILED");
                throw;
            }
        }

        ActionContext MakeContext()
        {
            return MakeContext(null);
        }

        ActionContext MakeContext(AppPath appPath)
        {
            return new ActionContext(this, appPath)
            {
                UiLayers = _uiLayersManager,
                ViewModelFactory = _viewsFactory
            };
        }
    }
}