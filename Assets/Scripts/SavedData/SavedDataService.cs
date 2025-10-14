using System;
using System.Collections.Generic;

using Inventory.Events;
using Inventory.Events.Signals;
using Inventory.SaveData.SaveLoad;

using UnityEngine;

namespace Inventory.SaveData
{
	public class SavedDataService:ISavedDataService
	{
		private readonly ISaveLoadService _saveLoadService;
		private readonly IEventBusService _eventBusService;

		public SavedDataService( ISaveLoadService saveLoadServ, IEventBusService eventBusService )
		{
			_saveKey = "save1.json";
			_savedData = new();
			_saveLoadService = saveLoadServ;
			_eventBusService = eventBusService;
		}

		protected SavedData _savedData;

		
		private readonly string _saveKey;

		public bool IsDataLoaded
		{
			get;
			private set;
		} = false;

		public void LoadData( Action onComplete )
		{
			_saveLoadService.Load<SavedData>(_saveKey ,
			data =>
			{
				_savedData = data;
				complete();
			} ,
			() =>
			{
				// First Start
				complete();
			} ,
			s =>
			{
				// Fail Load data
				Debug.LogError("Fail Load data");
				complete();
			});

			void complete()
			{
				_eventBusService.Invoke(new SavedDataLoadedSignal());
				IsDataLoaded = true;
				onComplete?.Invoke();
				
			}
		}

		public void SaveGame()
		{
			SaveBaseStorageObjList();
			_saveLoadService.Save(_saveKey , _savedData);
		}
		

		private List<IStorageObject> _baseStorageObjList = new();
		public void InitBaseStorageObj( IStorageObject storageObject )
		{
			_baseStorageObjList.Add(storageObject);

			string key = storageObject.GetKey();
			if( _savedData.BaseStorage.ContainsKey(key) == false )
			{
				_savedData.BaseStorage[key] = storageObject.GetSavedData();
			}
			else
			{
				storageObject.SetSavedData(_savedData.BaseStorage[key]);
			}
		}
		private void SaveBaseStorageObjList()
		{
			foreach( var obj in _baseStorageObjList )
			{
				_savedData.BaseStorage[obj.GetKey()] = obj.GetSavedData();
			}
		}
	}
}
