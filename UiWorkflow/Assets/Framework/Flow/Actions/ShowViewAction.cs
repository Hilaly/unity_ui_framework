using System.Threading.Tasks;
using Framework.Ui;

namespace Framework.Flow
{
    class ShowViewAction<TViewModel> : IActionResult
        where TViewModel : ViewModelMonoBehaviour
    {
        //Allow spawn few viewModels at same time
        public int Layer { get; set; }
        public string Name { get; set; }
        
        public Task ExecuteAsync(ActionContext context)
        {
            context.UiLayers.Show(context.ViewModelFactory.Build<TViewModel>(Name), Layer);
            return Task.CompletedTask;
        }
    }
}