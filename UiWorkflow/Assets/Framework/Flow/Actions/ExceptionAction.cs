using System;
using System.Threading.Tasks;

namespace Framework.Flow
{
    public class ExceptionAction : IActionResult
    {
        private readonly Exception _exception;

        public ExceptionAction(Exception exception)
        {
            _exception = exception;
        }

        public Task ExecuteAsync(ActionContext context)
        {
            return Task.FromException(_exception);
        }
    }
}