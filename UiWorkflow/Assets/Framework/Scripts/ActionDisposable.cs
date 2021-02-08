using System;

namespace Framework
{
    public class ActionDisposable : IDisposable
    {
        private Action _handler;

        public ActionDisposable(Action handler)
        {
            _handler = handler;
        }

        public void Dispose()
        {
            _handler();
            _handler = null;
        }
    }
}