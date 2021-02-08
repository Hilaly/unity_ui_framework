using Framework.Flow;
using UnityEngine;

namespace Demo.Scripts
{
    // it's better to use dependency injection
    public class Context : MonoBehaviour
    {
        [SerializeField] private TutorialViewModel _tutorialPrefab;
        [SerializeField] private GameViewModel _gamePrefab;
        [SerializeField] private ResultViewModel _resultPrefab;
        
        void Awake()
        {
            //We don't need all this code if we use dependency injection
            
            var appRoute = new AppRouter();

            var gameModel = new GameModel();
            var resultModel = new LevelResultModel();

            var gameController = new GameController(gameModel);
            var resController = new LevelResultController(gameModel, resultModel);

            appRoute.RegisterController(gameController);
            appRoute.RegisterController(resController);
            
            //TODO: register prefabs
        }
    }
}