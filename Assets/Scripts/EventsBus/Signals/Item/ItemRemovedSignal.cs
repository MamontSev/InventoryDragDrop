

using static Inventory.Data.Static.Enums;

namespace Inventory.Events.Signals
{
	public class ItemRemovedSignal:IEventBusSignal
	{
		public readonly ItemType ItemType;
		public readonly int ItemKey;
		public ItemRemovedSignal( ItemType itemType , int itemKey )
		{
			ItemType = itemType;
			ItemKey = itemKey;
		}
	}
}

