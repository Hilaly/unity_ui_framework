using Framework.Flow;
using Framework.Ui;

namespace Demo.Scripts
{
    //We can generate many view models from single model
    [ViewModelMethod("GameViewModel", "OnTap", "Game/Tap")]
    [ViewModelMethod("TutorialViewModel", "OnTap", "Game/Run")]
    public class GameModel : BaseModel<GameModel>
    {
        [ViewModelProperty("GameViewModel", "WinsCount")]
        public int TapsToWin;
        //Many properties from one property
        [ViewModelProperty("GameViewModel", "Taps")]
        [ViewModelProperty("ResultViewModel", "Taps")]
        public int CurrentTapsCount;
        [ViewModelProperty("GameViewModel", "Level")]
        [ViewModelProperty("ResultViewModel", "Level")]
        public int Level;
    }
    
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
    }

    [ViewModelMethod("ResultViewModel", "OnNextLevelClicked", "")]
    public class LevelResultModel : BaseModel<LevelResultModel>
    {
        [ViewModelProperty("ResultViewModel", "IsWin")]
        public bool IsWin;
    }

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