using System;
using System.Collections.Generic;

using Inventory.Data.Config;

using UnityEngine;

using static Inventory.Data.Static.Enums;

namespace Inventory.Data.Service
{
	public class ItemDataService:IItemDataService
	{
		private readonly ItemIConfig _config;
		private readonly ItemBehaviourConfig _behaviourConfig;
		public ItemDataService( ItemIConfig _config , ItemBehaviourConfig _behaviourConfig )
		{
			this._config = _config;
			this._behaviourConfig = _behaviourConfig;

			InitBehaviourData();
			InitIconData();
			InitTextData();
		}

		public ItemBehaviour GetBehaviourType( ItemType itemType ) => _behaviourData[itemType];
		public Sprite GetIcon( ItemType itemType , int index ) => _iconData[itemType][index];
		public String GetName( ItemType itemType , int index ) => _textData[itemType][index].name;
		public String GetDescription( ItemType itemType , int index ) => _textData[itemType][index].description;


		private readonly Dictionary<ItemType , ItemBehaviour> _behaviourData = new();
		private void InitBehaviourData()
		{
			foreach( ItemType type in Enum.GetValues(typeof(ItemType)) )
			{
				_behaviourData.Add(type , _behaviourConfig.GetBehaviourType(type));
			}
		}

		private readonly Dictionary<ItemType , Dictionary<int , Sprite>> _iconData = new();
		private void InitIconData()
		{
			foreach( ItemType type in Enum.GetValues(typeof(ItemType)) )
			{
				_iconData.Add(type , new());
				Type EnumType = GetItemEnumType(type);
				foreach( var enumType in Enum.GetValues(EnumType) )
				{
					int index = (int)enumType;
					_iconData[type].Add(index , _config.GetIconSprite(type , index));
				}
			}
		}

		private readonly Dictionary<ItemType , Dictionary<int , (string name, string description)>> _textData = new();
		private void InitTextData()
		{
			foreach( ItemType type in Enum.GetValues(typeof(ItemType)) )
			{
				_textData.Add(type , new());
				Type EnumType = GetItemEnumType(type);
				foreach( var enumType in Enum.GetValues(EnumType) )
				{
					int index = (int)enumType;
					_textData[type].Add(index , (_config.GetName(type , index), _config.GetDescription(type , index)));
				}
			}
		}
	}
}
