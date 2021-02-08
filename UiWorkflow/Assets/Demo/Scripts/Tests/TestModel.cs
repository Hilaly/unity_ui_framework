using Framework.Flow;
using Framework.Ui;

namespace Demo.Scripts
{
    [ViewModelMethod("TestViewModel", "OnButtonClick", "Test/ButtonClick?newNick={NewNickname}")]
    [ViewModelMethod("TestViewModel", "OnButtonClickException", "Test/"+nameof(TestController.Exception))]
    public class TestModel : BaseModel<TestModel>
    {
        [ViewModelProperty("TestViewModel", "Nickname", Namespace = "Demo.Scripts")]
        [ViewModelProperty("TestViewModel", "NewNickname", Namespace = "Demo.Scripts")]
        public string Nickname;
    }
}