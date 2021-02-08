using System;
using Framework.Flow;

namespace Demo.Scripts
{
    public class MainController : BaseController
    {
        private TestModel _model;

        public MainController(TestModel model)
        {
            _model = model;
        }

        public IActionResult ButtonClick(string newNick)
        {
            UnityEngine.Debug.Log(nameof(ButtonClick));
            _model.Nickname = newNick;
            _model.TriggerChange();
            return Ok;
        }
        
        public IActionResult Exception()
        {
            UnityEngine.Debug.Log(nameof(Exception));
            return Error(new NotImplementedException());
        }
    }
}