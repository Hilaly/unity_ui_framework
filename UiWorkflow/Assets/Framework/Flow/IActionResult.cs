using System.Threading.Tasks;

namespace Framework.Flow
{
    public interface IActionResult
    {
        Task ExecuteAsync(ActionContext context);
    }
}