//Generated from model, dont change code, all changes can be lost if it will be regenerated
using UnityWeld.Binding;
using Framework.Flow;

namespace Demo.Scripts
{
	[Binding]
	public partial class GameViewModel : Framework.Ui.ViewModelMonoBehaviour
		, IModelObserver<Demo.Scripts.GameModel>
	{
		#region Private fields

		private System.Int32 _WinsCount;
		private System.Int32 _Taps;
		private System.Int32 _Level;

		#endregion

		#region Bindings properties

		[Binding] public System.Int32 WinsCount
		{
			get => _WinsCount;
			set
			{
				if(value == _WinsCount) return;
				_WinsCount = value;
				OnPropertyChanged(nameof(WinsCount));
			}
		}
		[Binding] public System.Int32 Taps
		{
			get => _Taps;
			set
			{
				if(value == _Taps) return;
				_Taps = value;
				OnPropertyChanged(nameof(Taps));
			}
		}
		[Binding] public System.Int32 Level
		{
			get => _Level;
			set
			{
				if(value == _Level) return;
				_Level = value;
				OnPropertyChanged(nameof(Level));
			}
		}

		#endregion

		#region Bindings methods

		[Binding] public void OnTap()
		{
			AppRouter.Run($"Game/Tap");
		}

		#endregion

		#region Models methods

		void IModelObserver<Demo.Scripts.GameModel>.ModelChanged(Demo.Scripts.GameModel model)
		{
			WinsCount = (System.Int32)model.TapsToWin;
			Taps = (System.Int32)model.CurrentTapsCount;
			Level = (System.Int32)model.Level;
		}
		#if USE_ZENJECT
		[Inject]
		#endif
		public void InjectModel(Demo.Scripts.GameModel model)
		{
			AddOnDispose(model.Subscribe(this));
			((IModelObserver<Demo.Scripts.GameModel>) this).ModelChanged(model);
		}

		#endregion
	}
}
