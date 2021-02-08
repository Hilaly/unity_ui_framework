namespace Framework.Ui
{
    public class UiLayersManager
    {
        private readonly IViewModelFactory _viewModelFactory;

        private ViewModelMonoBehaviour _spawnedView;

        public UiLayersManager(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void Show(ViewModelMonoBehaviour uiLayer, int layer)
        {
            //TODO: use different layers for show popups, tutorial and other features
            
            //Don't spawn if screen didn't change
            if(_spawnedView == uiLayer)
                return;

            if (_spawnedView != null)
            {
                _viewModelFactory.Free(_spawnedView);
                _spawnedView = null;
            }

            _spawnedView = uiLayer;
        }
    }
}