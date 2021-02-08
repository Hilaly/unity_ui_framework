using System.Threading.Tasks;

namespace Framework.Flow
{
    public class RouteAction : IActionResult
    {
        private readonly AppPath _route;

        public RouteAction(AppPath route)
        {
            _route = route;
        }

        public Task ExecuteAsync(ActionContext context)
        {
            return context.Router.Run(_route);
        }
    }
}