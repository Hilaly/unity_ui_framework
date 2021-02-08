using System;
using Framework.Flow;
using UnityEngine;

namespace Demo.Scripts
{
    class GameController : BaseController
    {
        private readonly GameModel _model;

        public GameController(GameModel model)
        {
            _model = model;
        }

        public IActionResult Run()
        {
            //Show game HUD
            return View<GameViewModel>();
        }

        public IActionResult StartLevel(int level)
        {
            _model.Level = level;
            _model.TapsToWin = level;
            _model.CurrentTapsCount = 0;
            
            //Trigger update all subscribers after data changed
            _model.TriggerChange();

            //Show start level UI (window with level description)
            return View<TutorialViewModel>();
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
                    .AddArg("win", _model.CurrentTapsCount >= _model.TapsToWin)
                    .Build());
            return Ok;
        }

        public IActionResult LoadSavedGame()
        {
            var lastPlayedLevel = PlayerPrefs.GetInt("saved_level", 1);
            return StartLevel(lastPlayedLevel);
        }
    }
}