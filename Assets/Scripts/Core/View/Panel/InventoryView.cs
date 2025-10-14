using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Inventory.Core.Model;

using UnityEngine;

using Zenject;

namespace Inventory.Core.View.Panel
{
	public class InventoryView:MonoBehaviour, IInventoryView
	{
		private DiContainer _diContainer;
		private UsePanel _usePanel;
		private LevelState _levelState;
		[Inject]
		private void Construct
		(
			DiContainer _diContainer ,
			UsePanel _usePanel,
			LevelState _levelState
		)
		{
			this._diContainer = _diContainer;
			this._usePanel = _usePanel;
			this._levelState = _levelState;
		}


		[SerializeField]
		private ItemsField Field;
		[SerializeField]
		private Item ItemPrefab;
		[SerializeField]
		private ItemTooltip Tooltip;

		private Dictionary<int , Item> _items = new();

		private InventoryViewModel _selfVM;
		public void Bind( InventoryViewModel selfVM )
		{
			_selfVM = selfVM;
			_selfVM.OnBind(this);
			InitUsePanel();
		}

		public async UniTask Show()
		{
			await _selfVM.OnShow();
		}

		public bool HaveFreeCell => _selfVM.HaveFreeCell;

		public async UniTask InitField( int _cellCountX , int _cellCountY )
		{
			await Field.Init(_cellCountX , _cellCountY);
		}

		#region Item Public

		public void AddItem( Sprite iconSprite , string name , int index , bool needCount , int count, int type,int key )
		{
			Item item = _diContainer.InstantiatePrefabForComponent<Item>(ItemPrefab , Field.transform);
			_items[index] = item;
			(Transform point,int fieldIndex) = Field.GetFreePoint();
			item.transform.localPosition = point.localPosition;
			item.InitState(index, iconSprite, name, needCount);
			item.InitCallBack(OnPressedItem, OnDoublePressedItem, OnStartDragItem, OnEndDragItem);
			item.InitTooltip(Tooltip);
			item.FieldIndex = fieldIndex;
			item.SetCount(count);
			item.SelfType = (type,key);
			item.Show();
		}

		public async void RemoveItem( int index )
		{
			await _items[index].Hide();
			Field.ReturnPoint(_items[index].FieldIndex);
			Destroy(_items[index].gameObject);
			_items.Remove(index);
		}

		public void SetItemCount( int index , int count )
		{
			_items[index].SetCount(count);
		}

		public void SetSelected( int index , bool state )
		{
			_items[index].SetSelected(state);
		}

		public void SetUse( int index , bool state )
		{
			_items[index].SetUse(state);
		}

		#endregion

	

		#region Item CallBacks

		private void OnPressedItem( int index )
		{
			_selfVM.OnPressedSelect(index);
		}
		private void OnDoublePressedItem( int index )
		{
			_selfVM.OnPressedUseDoubleClick(index);
		}

		private void OnStartDragItem( int index )
		{
			_items[index].transform.SetSiblingIndex(Field.transform.childCount);
			Tooltip.Block();
		}
		private void OnEndDragItem( int index )
		{
			Tooltip.UnBlock();
			
			(Transform point, int fieldIndex) = Field.ReturnPointAndGetNearest(_items[index].transform, _items[index].FieldIndex);
			_items[index].MoveToPlace(point.position);
			_items[index].FieldIndex = fieldIndex;
		}

		#endregion

		#region Sort

		private async void SortItems()
		{
			if( _levelState.IsPlayimg == false )
				return;
			_levelState.SetToPaused();
			List<UniTask> _taskList = new();
			foreach( var item in _items.Values )
			{
				_taskList.Add(item.Hide());
			}
			await UniTask.WhenAll(_taskList);
			Field.ReturnAllPoints();

			_items = _items.OrderBy(x => x.Value.SelfType).ToDictionary(x => x.Key , pair => pair.Value);
			foreach( var item in _items.Values )
			{
				(Transform point, int fieldIndex) = Field.GetFreePoint();
				item.transform.localPosition = point.localPosition;
				item.FieldIndex = fieldIndex;
				item.Show();
				await UniTask.Delay(50);
			}
			_levelState.SetToGameLoop();
			await UniTask.Yield();

		}

		#endregion

		#region Use Panel

		private void InitUsePanel()
		{
			_usePanel.InitCallBack(_selfVM.OnPressedUse , _selfVM.OnPressedUse , _selfVM.OnPressedDrop, SortItems);
		}

		public void ShowUsePanel( bool isPutOn , bool isIakeOff )
		{
			_usePanel.ShowItemCont(isPutOn , isIakeOff);
		}
		public void SetItemDescription( string description )
		{
			_usePanel.SetDescriptionText(description);
		}

		public void HideUsePanel()
		{
			_usePanel.HideItemCont();
		}

		#endregion


	}
}
