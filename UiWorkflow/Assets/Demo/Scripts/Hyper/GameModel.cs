using Framework.Flow;
using Framework.Ui;

namespace Demo.Scripts
{
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
}