using Inventory.Data.Static;

namespace Inventory.State
{
	public interface IInventoryStateService
	{
		void AddItem( Enums.ItemType type , int key );
		int GetItemCount( Enums.ItemType type , int key );
		void RemoveItem( Enums.ItemType type , int key );
	}
}