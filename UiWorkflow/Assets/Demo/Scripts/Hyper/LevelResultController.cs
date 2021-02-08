using Framework.Flow;
using UnityEngine;

namespace Demo.Scripts
{
    class LevelResultController : BaseController
    {
        //We can use many different models 
        private readonly GameModel _gameModel;
        private readonly LevelResultModel _model;

        public LevelResultController(GameModel gameModel, LevelResultModel model)
        {
            _gameModel = gameModel;
            _model = model;
        }

        public IActionResult Show(bool win)
        {
            //Save match result to model ...
            _model.IsWin = true;
            _model.TriggerChange();
            //and to PlayerPrefs (to load on next application open) ...
            if (win)
                PlayerPrefs.SetInt("saved_level", _gameModel.Level + 1);

            //And show result screen
            return View<ResultViewModel>();
        }
    }
}