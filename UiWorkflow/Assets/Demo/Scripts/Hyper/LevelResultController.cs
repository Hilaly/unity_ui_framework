using Framework.Flow;

namespace Demo.Scripts
{
    class LevelResultController : BaseController
    {
        //We can use different models 
        private readonly GameModel _gameModel;
        private readonly LevelResultModel _model;

        public LevelResultController(GameModel gameModel, LevelResultModel model)
        {
            _gameModel = gameModel;
            _model = model;
        }

        public IActionResult Show(bool win)
        {
            _model.IsWin = true;
            _model.TriggerChange();
            
            //TODO: show ui
            return Ok;
        }
    }
}