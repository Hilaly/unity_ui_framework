namespace Framework.Flow
{
    public class ActionContext
    {
        public IAppRouter Router { get; }
        public AppPath Route { get; }

        public ActionContext(IAppRouter router, AppPath route)
        {
            Route = route;
            Router = router;
        }
    }
}