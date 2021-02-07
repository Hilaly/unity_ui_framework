using System.Threading.Tasks;

namespace Framework.Flow
{
    public class OkAction : IActionResult
    {
        public Task ExecuteAsync(ActionContext context)
        {
            return Task.CompletedTask;
        }
    }
}