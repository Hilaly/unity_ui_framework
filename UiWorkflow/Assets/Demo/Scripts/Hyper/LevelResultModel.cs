using Framework.Flow;
using Framework.Ui;

namespace Demo.Scripts
{
    public class LevelResultModel : BaseModel<LevelResultModel>
    {
        //Define property with binding attribute at ResultViewModel
        [ViewModelProperty("ResultViewModel", "IsWin")]
        public bool IsWin;
    }
}