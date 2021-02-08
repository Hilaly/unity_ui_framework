//Generated from model, dont change code, all changes can be lost if it will be regenerated
using UnityWeld.Binding;
using Framework.Flow;

namespace Demo.Scripts
{
	[Binding]
	public partial class TutorialViewModel : Framework.Ui.ViewModelMonoBehaviour
	{
		#region Private fields


		#endregion

		#region Bindings properties


		#endregion

		#region Bindings methods

		[Binding] public void OnTap()
		{
			AppRouter.Run($"Game/Run");
		}

		#endregion

		#region Models methods


		#endregion
	}
}
