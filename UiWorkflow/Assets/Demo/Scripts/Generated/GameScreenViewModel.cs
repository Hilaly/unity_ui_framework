//Generated from prefab, don't change code, all changes can be lost if it will be regenerated

using UnityEngine;

namespace Ui.Generated
{
	public partial class GameScreenViewModel : UnityEngine.MonoBehaviour
	{
		[SerializeField] UnityEngine.UI.Text _LevelText;
		[SerializeField] Framework.Ui.Adapters.LocalizeTextAdapter _LevelTextAdapter = new Framework.Ui.Adapters.LocalizeTextAdapter(/*Insert args for LocalizeTextAdapter here*/);
		public System.String LevelText
		{
			get => _LevelText.text;
			set { if(value != _LevelText.text) _LevelText.text = value; }
		}
		[SerializeField] UnityEngine.UI.Text _TapsText;
		[SerializeField] Framework.Ui.Adapters.LocalizeTextAdapter _TapsTextAdapter = new Framework.Ui.Adapters.LocalizeTextAdapter(/*Insert args for LocalizeTextAdapter here*/);
		public System.String TapsText
		{
			get => _TapsText.text;
			set { if(value != _TapsText.text) _TapsText.text = value; }
		}

	}
}
