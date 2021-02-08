using Framework.Ui;

namespace Framework.Flow
{
    public class ActionContext
    {
        public IAppRouter Router { get; }
        public AppPath Route { get; }
        public IViewModelFactory ViewModelFactory { get; set; }
        public UiLayersManager UiLayers { get; set; }

        public ActionContext(IAppRouter router, AppPath route)
        {
            Route = route;
            Router = router;
        }
    }
}