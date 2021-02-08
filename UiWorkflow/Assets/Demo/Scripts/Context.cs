using Framework.Flow;
using Framework.Ui;
using UnityEngine;

namespace Demo.Scripts
{
    // it's better to use dependency injection
    public class Context : MonoBehaviour
    {
        [SerializeField] private string startPath;
        
        [SerializeField] private TutorialViewModel tutorialPrefab;
        [SerializeField] private GameViewModel gamePrefab;
        [SerializeField] private ResultViewModel resultPrefab;
        
        void Awake()
        {
            //We don't need all this code if we use dependency injection

            var viewModelsFactory = new InstancesViewModelFactory();
            var appRoute = new AppRouter(viewModelsFactory, new UiLayersManager(viewModelsFactory));

            var gameModel = new GameModel();
            var resultModel = new LevelResultModel();

            var gameController = new GameController(gameModel);
            var resController = new LevelResultController(gameModel, resultModel);

            {
                viewModelsFactory.Register(tutorialPrefab).InjectDependencies(appRoute);
            }
            {
                var inst = viewModelsFactory.Register(gamePrefab);
                inst.InjectDependencies(appRoute);
                inst.InjectModel(gameModel);
            }
            {
                var inst = viewModelsFactory.Register(resultPrefab);
                inst.InjectDependencies(appRoute);
                inst.InjectModel(gameModel);
                inst.InjectModel(resultModel);
            }

            appRoute.RegisterController(gameController);
            appRoute.RegisterController(resController);
            
            //TODO: register prefabs

            appRoute.Run(startPath);
        }
    }
}