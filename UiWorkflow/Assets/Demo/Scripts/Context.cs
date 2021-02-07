using Framework.Flow;
using UnityEngine;

namespace Demo.Scripts
{
    // it's better to use dependency injection
    public class Context : MonoBehaviour
    {
        [SerializeField] private TestViewModel _viewModel;
        
        void Awake()
        {
            var model = new TestModel() {Nickname = "This is placeholder text"};
            var controller = new MainController(model);
            var appRoute = new AppRouter();
            
            //We dont need it if use zenject, all controllers will be resolved with zenject
            appRoute.RegisterController(controller);

            _viewModel.InjectModel(model);
            _viewModel.InjectDependencies(appRoute);
        }
    }
}