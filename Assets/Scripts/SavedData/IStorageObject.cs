using System.Collections.Generic;

namespace Inventory.SaveData
{
	public interface IStorageObject
	{
		string GetKey();
		Dictionary<string , object> GetSavedData();
		void SetSavedData( Dictionary<string , object> data);
	}

}
