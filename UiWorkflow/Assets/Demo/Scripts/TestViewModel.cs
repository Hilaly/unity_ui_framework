//Generated from model, dont change code, all changes can be lost if it will be regenerated
using UnityWeld.Binding;
using Framework.Flow;

namespace Demo.Scripts
{
	[Binding]
	public partial class TestViewModel : Framework.Ui.ViewModelMonoBehaviour
		, IModelObserver<Demo.Scripts.TestModel>
	{
		#region Private fields

		private System.String _Nickname;
		private System.String _NewNickname;

		#endregion

		#region Bindings properties

		[Binding] public System.String Nickname
		{
			get => _Nickname;
			set
			{
				if(value == _Nickname) return;
				_Nickname = value;
				OnPropertyChanged(nameof(Nickname));
			}
		}
		[Binding] public System.String NewNickname
		{
			get => _NewNickname;
			set
			{
				if(value == _NewNickname) return;
				_NewNickname = value;
				OnPropertyChanged(nameof(NewNickname));
			}
		}

		#endregion

		#region Bindings methods

		[Binding] public void OnButtonClick()
		{
			AppRouter.Run($"Main/ButtonClick?newNick={NewNickname}");
		}
		[Binding] public void OnButtonClickException()
		{
			AppRouter.Run($"Main/Exception");
		}

		#endregion

		#region Models methods

		void IModelObserver<Demo.Scripts.TestModel>.ModelChanged(Demo.Scripts.TestModel model)
		{
			Nickname = (System.String)model.Nickname;
			NewNickname = (System.String)model.Nickname;
		}
		public void InjectModel(Demo.Scripts.TestModel model)
		{
			AddOnDispose(model.Subscribe(this));
			((IModelObserver<TestModel>) this).ModelChanged(model);
		}

		#endregion
	}
}
