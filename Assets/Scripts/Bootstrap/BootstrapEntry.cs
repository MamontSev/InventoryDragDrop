using Inventory.SaveData;
using Inventory.SceneControl;

using UnityEngine;

using Zenject;

namespace Inventory.Bootstrap
{
	public class BootstrapEntry:MonoBehaviour
	{
		private ISceneControlService _sceneControlService;
		private ISavedDataService _savedDataService;
		[Inject]
		private void Construct(
			ISceneControlService _sceneControlService ,
			ISavedDataService _savedDataService 
		)
		{
			this._sceneControlService = _sceneControlService;
			this._savedDataService = _savedDataService;
		}

		private void Start()
		{
			_savedDataService.LoadData(_sceneControlService.LoadLevel);
		}
	}
}


