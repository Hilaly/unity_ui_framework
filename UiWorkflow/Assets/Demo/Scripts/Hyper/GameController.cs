using System;
using Framework.Flow;

namespace Demo.Scripts
{
    //We can generate many view models from single model

    class GameController : BaseController
    {
        private readonly GameModel _model;

        public GameController(GameModel model)
        {
            _model = model;
        }

        public IActionResult Run()
        {
            //TODO: show ui
            return Ok;
        }

        public IActionResult StartLevel(int level)
        {
            _model.Level = level;
            _model.TapsToWin = level;
            _model.CurrentTapsCount = 0;
            
            //Trigger update all subscribers after data changed
            _model.TriggerChange();
            //TODO: show ui
            return Ok;
        }

        public IActionResult Tap()
        {
            _model.CurrentTapsCount += 1;
            _model.TriggerChange();
            
            if (_model.CurrentTapsCount >= _model.TapsToWin)
                //We can use string call: this.Context.Router.Run($"LevelResult/Show?win=true");
                //TODO: think about simplify
                return GoToPath(PathBuilder.Controller<LevelResultController>()
                    .Action(nameof(LevelResultController.Show))
                    .AddArg("win", _model.CurrentTapsCount >= _model.TapsToWin).Build());
            return Ok;
        }

        public IActionResult LoadSavedGame()
        {
            return Error(new Exception());
        }
    }
}