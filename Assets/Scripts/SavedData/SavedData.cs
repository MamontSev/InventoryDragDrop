using System;
using System.Collections.Generic;

namespace Inventory.SaveData
{
	[Serializable]
	public class SavedData
	{
		public SavedData()
		{
		}

		public Dictionary<string , Dictionary<string , object>> BaseStorage = new();
	}
}
