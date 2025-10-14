using System;

using Cysharp.Threading.Tasks;

using Inventory.Core.View;
using Inventory.Core.View.Panel;
using Inventory.Data.Config;
using Inventory.Data.Service;
using Inventory.Events;
using Inventory.State;

using static Inventory.Data.Static.Enums;

namespace Inventory.Core.Model
{
	public class InventoryViewModel:IDisposable
	{
		private readonly IInventoryStateService _state;
		private readonly IItemDataService _data;
		private readonly FieldConfig _fieldConfig;
		private readonly LevlUI _levlUI;
		private readonly ItemsHolder _items;
		public InventoryViewModel
		(
			IInventoryStateService _state ,
			IItemDataService _data ,
			IEventBusService _eventBusService ,
			FieldConfig _fieldConfig ,
			LevlUI _levlUI
		)
		{
			this._state = _state;
			this._data = _data;
			this._fieldConfig = _fieldConfig;
			this._levlUI = _levlUI;
			_items = new ItemsHolder(_eventBusService, _state, _data);
		}

		private const int EMPTY = -1;

		private int _useItemIndex = EMPTY;
		private int _selectedItemIndex = EMPTY;

		private IInventoryView _selfView;
		public void OnBind( IInventoryView selfView )
		{
			_selfView = selfView;
			_items.BindView(_selfView);
		}

	
		public async UniTask OnShow()
		{
			int cellCountX = _fieldConfig.CellCountX;
			int cellCountY = _fieldConfig.CellCountY;
			int cellCount = _fieldConfig.CellCountX * _fieldConfig.CellCountY;
			int neededCellCount = _items.GetItemsCountOnStart();
			if( cellCount < neededCellCount )
			{
				cellCountX = 6;
				cellCountY = ( neededCellCount / cellCountX ) + 1;
				await _levlUI.ShowMessage($"Недостаточно ячеекв данных конфига FieldConfig для всех предметов из сохранения , поле будет увеличено.");
			}
			cellCount = cellCountX * cellCountY;
			await _selfView.InitField(cellCountX , cellCountY);

			_items.Init(cellCount);

			await _items.FillField();

		}

		public bool HaveFreeCell => _items.HaveFreeCell;

		private int _lastSelectItemIndex;
		public void OnPressedSelect( int index )
		{
			_lastSelectItemIndex = _selectedItemIndex;
			if( _selectedItemIndex == index )
			{
				_selfView.HideUsePanel();
				_selfView.SetSelected(index , false);
				_selectedItemIndex = EMPTY;
				return;
			}
			if( _selectedItemIndex != EMPTY )
				_selfView.SetSelected(_selectedItemIndex , false);

			_selectedItemIndex = index;
			_selfView.SetSelected(_selectedItemIndex , true);

			ShowUsePanel();
		}

		public void OnPressedUse()
		{
			if( _useItemIndex == _selectedItemIndex )
			{
				_selfView.SetUse(_selectedItemIndex , false);
				_useItemIndex = EMPTY;
				ShowUsePanel();
				return;
			}
			if( _useItemIndex != EMPTY )
				_selfView.SetUse(_useItemIndex , false);

			_useItemIndex = _selectedItemIndex;
			_selfView.SetUse(_useItemIndex , true);

			ShowUsePanel();
		}

		public void OnPressedUseDoubleClick( int index )
		{
			if( index != _selectedItemIndex )
			{
				_selectedItemIndex = _lastSelectItemIndex;
				_selfView.SetSelected(_selectedItemIndex , true);
			}
			OnPressedUse();
		}

		public void OnPressedDrop()
		{
			Item item = _items.GetItem(_selectedItemIndex);
			ItemType itemType = item.Type;
			int key = item.Key;
			ItemBehaviour behaviour = _data.GetBehaviourType(item.Type);
			_items.RemoveItem(_selectedItemIndex);
			
			if( behaviour == ItemBehaviour.NonStackable )
			{
				reset();
				return;
			}

			int count = _state.GetItemCount(itemType , key);
			if( count == 0 )
			{
				reset();
			}
			else
			{
				_selfView.SetUse(_selectedItemIndex , false);
				_useItemIndex = EMPTY;
				ShowUsePanel();
			}

			void reset()
			{
				if( _selectedItemIndex == _useItemIndex )
					_useItemIndex = EMPTY;
				_selectedItemIndex = EMPTY;
				_selfView.HideUsePanel();
			}
		}
		private void ShowUsePanel()
		{
			bool isUseCurrItem = _useItemIndex == _selectedItemIndex;
			_selfView.ShowUsePanel(!isUseCurrItem , isUseCurrItem);
			Item item = _items.GetItem(_selectedItemIndex);
			string itemDescription = _data.GetDescription(item.Type , item.Key);
			_selfView.SetItemDescription(itemDescription);
		}


		public void Dispose()
		{
			_items.Dispose();
		}
	}
}

