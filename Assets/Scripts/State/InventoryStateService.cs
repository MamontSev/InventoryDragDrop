using System;
using System.Collections.Generic;

using Inventory.Events;
using Inventory.Events.Signals;
using Inventory.SaveData;

using UnityEngine;

using static Inventory.Data.Static.Enums;

namespace Inventory.State
{
	public class InventoryStateService:IInventoryStateService
	{
		private readonly ISavedDataService _savedDataService;
		private readonly IEventBusService _eventBusService;

		public InventoryStateService
		(
			ISavedDataService _savedDataService ,
			IEventBusService _eventBusService
		)
		{
			this._savedDataService = _savedDataService;
			this._eventBusService = _eventBusService;

			if( this._savedDataService.IsDataLoaded )
			{
				InitStorageObject();
			}
			else
			{
				this._eventBusService.Subscribe<SavedDataLoadedSignal>(OnSavedDataLoadedSignal);
			}
		}

		private void OnSavedDataLoadedSignal( SavedDataLoadedSignal signal )
		{
			_eventBusService.Unsubscribe<SavedDataLoadedSignal>(OnSavedDataLoadedSignal);
			InitStorageObject();
		}
		private IInventoryStorageObject _savedObject;
		private void InitStorageObject()
		{
			List<string> keyList = new();

			_savedObject = new InventoryStorageObject(keyList , _savedDataService);
			_savedDataService.SaveGame();
		}

		public int GetItemCount( ItemType type , int key )
		{
			if( !IsCorrectItemKey(type , key) )
			{
				string s = $"GetItemCount -> Item key is not Correct {type} {key}";
				throw new Exception(s);
			}
			return _savedObject.GetItemsCount(type , key);
		}
		public void AddItem( ItemType type , int key )
		{
			if( !IsCorrectItemKey(type , key) )
			{
				Debug.LogError($"AddItem -> Item key is not Correct {type} {key}");
				return;
			}
			int newCount = GetItemCount(type , key) + 1;
			_savedObject.SetItemsCount(type , key , newCount);
			_savedDataService.SaveGame();
			_eventBusService.Invoke(new ItemAddedSignal(type, key));
		}

		public void RemoveItem( ItemType type , int key )
		{
			if( !IsCorrectItemKey(type , key) )
			{
				Debug.LogError($"RemoveItem -> Item key is not Correct {type} {key}");
				return;
			}
			int newCount = GetItemCount(type , key) - 1;
			if( newCount < 0 )
			{
				Debug.LogError($"newCount count < 0 - is{newCount}");
				newCount = 0;
			}
			_savedObject.SetItemsCount(type , key , newCount);
			_savedDataService.SaveGame();
			_eventBusService.Invoke(new ItemRemovedSignal(type , key));
		}

		private bool IsCorrectItemKey( ItemType type , int key )
		{
			Type EnumType = GetItemEnumType(type);
			return key < Enum.GetValues(EnumType).Length;
		}

	}
}
