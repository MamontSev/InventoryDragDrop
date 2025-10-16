using Cysharp.Threading.Tasks;

using Inventory.Core;
using Inventory.Core.Model;
using Inventory.Core.View;

using UnityEngine;

using Zenject;

namespace Inventory.Bootstrap
{
	public class BootstrapLevel:MonoBehaviour
	{
		private LevlUI _levlUI;
		private LevelState _levelState; 
		[Inject]
		private void Construct( LevlUI _levlUI, LevelState _levelState )
		{
			this._levlUI = _levlUI;
			this._levelState = _levelState;
		}

		private void Start()
		{
			Init();
		}
		private async UniTaskVoid Init()
		{
			_levelState.SetToPaused();

			await UniTask.Yield();
			await _levlUI.Init();

			_levelState.SetToGameLoop();
		}
	}
}


