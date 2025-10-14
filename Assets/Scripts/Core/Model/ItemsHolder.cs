using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Inventory.Core.View.Panel;
using Inventory.Data.Service;
using Inventory.Events;
using Inventory.Events.Signals;
using Inventory.State;

using UnityEngine;

using static Inventory.Data.Static.Enums;

namespace Inventory.Core.Model
{
	public class ItemsHolder:IDisposable
	{
		private readonly IEventBusService _eventBusService;
		private readonly IInventoryStateService _state;
		private readonly IItemDataService _data;
		public ItemsHolder(
			IEventBusService _eventBusService ,
			IInventoryStateService _state ,
			IItemDataService _data
		)
		{
			this._eventBusService = _eventBusService;
			this._state = _state;
			this._data = _data;
			Subscribe();
		}

		private Dictionary<int , Item> _currItems = new();

		private IInventoryView _selfView;
		public void BindView( IInventoryView _selfView )
		{
			this._selfView = _selfView;
		}

		public void RemoveItem( int index )
		{
			MakeRemoveItem(index);
		}

		public bool HaveFreeCell => _currItems.Any(x => x.Value == null);

		public int GetItemsCountOnStart()
		{
			int itemsCount = 0;
			foreach( ItemType type in Enum.GetValues(typeof(ItemType)) )
			{
				Type EnumType = GetItemEnumType(type);
				foreach( var enumType in Enum.GetValues(EnumType) )
				{
					int key = (int)enumType;
					ItemBehaviour behaviour = _data.GetBehaviourType(type);
					int count = _state.GetItemCount(type , key);
					if( count == 0 )
						continue;

					if( behaviour == ItemBehaviour.Stackable )
						itemsCount++;
					else
						itemsCount += count;
				}
			}
			return itemsCount;
		}

		public void Init(int cellCount)
		{
			_currItems.Clear();
			for( int i = 0; i < cellCount; i++ )
			{
				_currItems.Add(i , null);
			}
			int index = 0;
			foreach( ItemType type in Enum.GetValues(typeof(ItemType)) )
			{
				Type EnumType = GetItemEnumType(type);
				foreach( var enumType in Enum.GetValues(EnumType) )
				{
					int key = (int)enumType;
					ItemBehaviour behaviour = _data.GetBehaviourType(type);
					int count = _state.GetItemCount(type , key);
					if( count == 0 )
						continue;

					if( behaviour == ItemBehaviour.Stackable )
						addItem(type , behaviour , key , count);
					else
						for( int i = 0; i < count; i++ )
							addItem(type , behaviour , key , 1);
				}
			}
			void addItem( ItemType itemType , ItemBehaviour behaviour , int key , int count )
			{
				_currItems[index] = new Item(itemType , behaviour , key , count);
				index++;
			}
		}

		public async UniTask FillField()
		{
			foreach( var kvPair in _currItems )
			{
				if( kvPair.Value == null )
					continue;
				AddItemToField(kvPair.Value , kvPair.Key);
				await UniTask.Delay(50);
			}
		}

		public Item GetItem( int index ) => _currItems[index];

		private void AddItemToField( Item item , int index )
		{
			Sprite icon = _data.GetIcon(item.Type , item.Key);
			string name = _data.GetName(item.Type , item.Key);
			bool needCount = _data.GetBehaviourType(item.Type) == ItemBehaviour.Stackable;
			_selfView.AddItem(icon , name , index , needCount , item.Count,(int)item.Type,item.Key);
		}
		private void MakeRemoveItem( int index )
		{
			ItemType itemType = _currItems[index].Type;
			int key = _currItems[index].Key;
			_state.RemoveItem(itemType , key);
			if( _currItems[index].Behaviour == ItemBehaviour.NonStackable )
			{
				remove();
				return;
			}

			int count = _state.GetItemCount(itemType , key);
			if( count > 0 )
			{
				_selfView.SetItemCount(index , count);
			}
			else
			{
				remove();
			}

			void remove()
			{
				_selfView.RemoveItem(index);
				_currItems[index] = null;
			}
		}

		private void OnItemAddedSignal( ItemAddedSignal signal )
		{
			ItemType itemType = signal.ItemType;
			int key = signal.ItemKey;
			ItemBehaviour behaviour = _data.GetBehaviourType(itemType);
			if( behaviour == ItemBehaviour.NonStackable )
			{
				add();
				return;
			}
			foreach( var kvPair in _currItems )
			{
				if( kvPair.Value == null )
					continue;
				if( kvPair.Value.Type == itemType && kvPair.Value.Key == key )
				{
					_selfView.SetItemCount(kvPair.Key , _state.GetItemCount(itemType , key));
					return;
				}
			}
			add();

			void add()
			{
				Item item = new Item(itemType , behaviour , key , 1);
				int index = _currItems.First(x => x.Value == null).Key;
				_currItems[index] = item;
				AddItemToField(item , index);
			}
		}

		private void Subscribe()
		{
			_eventBusService.Subscribe<ItemAddedSignal>(OnItemAddedSignal);
		}

		private void Unsubscribe()
		{
			_eventBusService.Unsubscribe<ItemAddedSignal>(OnItemAddedSignal);
		}

		public void Dispose()
		{
			Unsubscribe();
		}
	}
}

