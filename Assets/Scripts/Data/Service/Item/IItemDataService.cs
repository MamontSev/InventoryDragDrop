using Inventory.Data.Static;

using UnityEngine;

namespace Inventory.Data.Service
{
	public interface IItemDataService
	{
		Enums.ItemBehaviour GetBehaviourType( Enums.ItemType itemType );
		string GetDescription( Enums.ItemType itemType , int index );
		Sprite GetIcon( Enums.ItemType itemType , int index );
		string GetName( Enums.ItemType itemType , int index );
	}
}