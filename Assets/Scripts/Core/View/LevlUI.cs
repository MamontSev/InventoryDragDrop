using Cysharp.Threading.Tasks;

using Inventory.Core.Model;
using Inventory.Core.View.Panel;

using UnityEngine;

using Zenject;

namespace Inventory.Core.View
{
	public class LevlUI:MonoBehaviour
	{
		private DiContainer _diContainer;
		private InventoryView _inventoryView;
		private MessagePanel _messagePanel;
		private AddItemSimulator _addItemSimulator;
		private UsePanel _usePanel;
		[Inject]
		private void Construct
		(
			DiContainer _diContainer,
			InventoryView _inventoryView,
			MessagePanel _messagePanel,
			AddItemSimulator _addItemSimulator,
			UsePanel _usePanel
		)
		{
			this._diContainer = _diContainer;
			this._inventoryView = _inventoryView;
			this._messagePanel = _messagePanel;
			this._addItemSimulator = _addItemSimulator;
			this._usePanel = _usePanel;
		}


		public async UniTask Init()
		{
			InventoryViewModel vm = _diContainer.Instantiate<InventoryViewModel>();
			_inventoryView.Bind(vm);

			await _inventoryView.Show();
			UniTask task1 = _addItemSimulator.Show();
			UniTask task2 = _usePanel.Show();

			await UniTask.WhenAll(task1, task2);
		}


		public async UniTask ShowMessage(string message)
		{
			await _messagePanel.Show(message);
		}

		
	}
}
