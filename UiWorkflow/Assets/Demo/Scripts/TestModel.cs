using Framework.Flow;
using Framework.Ui;

namespace Demo.Scripts
{
    [ViewModelMethod("TestViewModel", "OnButtonClick", "Main/ButtonClick?newNick={NewNickname}")]
    [ViewModelMethod("TestViewModel", "OnButtonClickException", "Main/"+nameof(MainController.Exception))]
    public class TestModel : BaseModel<TestModel>
    {
        [ViewModelProperty("TestViewModel", "Nickname", Namespace = "Demo.Scripts")]
        [ViewModelProperty("TestViewModel", "NewNickname", Namespace = "Demo.Scripts")]
        public string Nickname;
    }
}