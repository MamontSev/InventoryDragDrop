using System;
using System.Collections.Generic;

using Inventory.SaveData;

using Newtonsoft.Json.Linq;

using static Inventory.Data.Static.Enums;

namespace Inventory.State
{
	public interface IInventoryStorageObject
	{
		int GetItemsCount( ItemType type , int key );
		void SetItemsCount( ItemType type , int key , int count );
	}

	public class InventoryStorageObject:IStorageObject, IInventoryStorageObject
	{
		public InventoryStorageObject( List<string> _keyList , ISavedDataService _savedService )
		{
			foreach( ItemType type in Enum.GetValues(typeof(ItemType)) )
			{
				Dictionary<int , int> temp = new();
				Type EnumType = GetItemEnumType(type);
				foreach( var enumType in Enum.GetValues(EnumType) )
				{
					temp.Add((int)enumType,0);
				}

				_itemsDict.Add(type , temp);
			}
			_savedService.InitBaseStorageObj(this);
		}


		public string GetKey() => "InventoryStorageObject";
		public Dictionary<string , object> GetSavedData()
		{
			Dictionary<string , object> data = new();
			data.Add(SavedObjNames.ItemsDict.ToString() , _itemsDict);
			return data;
		}
		public void SetSavedData( Dictionary<string , object> data )
		{
			foreach( KeyValuePair<string , object> obj in data )
			{
				if( obj.Key == SavedObjNames.ItemsDict.ToString() )
				{
					Dictionary<string , Dictionary<int , int>> temp = JObject.FromObject(obj.Value).ToObject<Dictionary<string , Dictionary<int , int>>>();
					foreach( KeyValuePair<string , Dictionary<int , int>> kv in temp )
					{
						if( Enum.TryParse<ItemType>(kv.Key , out ItemType type) )
						{
							foreach( var item in kv.Value )
							{
								_itemsDict[type][item.Key] = item.Value;
							}
						}
					}
				}
			}
		}


		private readonly Dictionary<ItemType , Dictionary<int,int>> _itemsDict = new();

		#region IHeroInventoryStorageObject

		public int GetItemsCount( ItemType type,int key )
			=> _itemsDict[type][key];
		public void SetItemsCount( ItemType type , int key, int count ) 
			=> _itemsDict[type][key] = count;


		#endregion
		enum SavedObjNames
		{
			ItemsDict
		}

		

	}
}

