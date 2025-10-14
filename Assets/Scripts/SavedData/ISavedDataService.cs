using System;

namespace Inventory.SaveData
{
	public interface ISavedDataService
	{
		bool IsDataLoaded
		{
			get;
		}
		void LoadData( Action onComplete );
		void SaveGame();
		void InitBaseStorageObj( IStorageObject storageObject );
	}
}

