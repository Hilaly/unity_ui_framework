using Framework.Flow;
using Framework.Ui;

namespace Demo.Scripts
{
    [ViewModelMethod("ResultViewModel", "OnNextLevelClicked", "")]
    public class LevelResultModel : BaseModel<LevelResultModel>
    {
        [ViewModelProperty("ResultViewModel", "IsWin")]
        public bool IsWin;
    }
}