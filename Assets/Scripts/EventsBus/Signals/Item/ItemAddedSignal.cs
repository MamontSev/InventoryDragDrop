

using static Inventory.Data.Static.Enums;

namespace Inventory.Events.Signals
{
	public class ItemAddedSignal:IEventBusSignal
	{
		public readonly ItemType ItemType;
		public readonly int ItemKey;
		public ItemAddedSignal( ItemType itemType, int itemKey )
		{
			ItemType = itemType;
			ItemKey = itemKey;
		}
	}
}

