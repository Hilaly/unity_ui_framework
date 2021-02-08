//Generated from model, dont change code, all changes can be lost if it will be regenerated
using UnityWeld.Binding;
using Framework.Flow;

namespace Demo.Scripts
{
	[Binding]
	public partial class ResultViewModel : Framework.Ui.ViewModelMonoBehaviour
		, IModelObserver<Demo.Scripts.GameModel>
		, IModelObserver<Demo.Scripts.LevelResultModel>
	{
		#region Private fields

		private System.Int32 _Taps;
		private System.Int32 _Level;
		private System.Boolean _IsWin;

		#endregion

		#region Bindings properties

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
		[Binding] public System.Boolean IsWin
		{
			get => _IsWin;
			set
			{
				if(value == _IsWin) return;
				_IsWin = value;
				OnPropertyChanged(nameof(IsWin));
			}
		}

		#endregion

		#region Bindings methods

		[Binding] public void OnNextLevelClicked()
		{
			AppRouter.Run($"Game/StartLevel?level={Level + 1}");
		}

		#endregion

		#region Models methods

		void IModelObserver<Demo.Scripts.GameModel>.ModelChanged(Demo.Scripts.GameModel model)
		{
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
		void IModelObserver<Demo.Scripts.LevelResultModel>.ModelChanged(Demo.Scripts.LevelResultModel model)
		{
			IsWin = (System.Boolean)model.IsWin;
		}
		#if USE_ZENJECT
		[Inject]
		#endif
		public void InjectModel(Demo.Scripts.LevelResultModel model)
		{
			AddOnDispose(model.Subscribe(this));
			((IModelObserver<Demo.Scripts.LevelResultModel>) this).ModelChanged(model);
		}

		#endregion
	}
}
