using static Inventory.Data.Static.Enums;

namespace Inventory.Core.Model
{
	public class Item
	{
		private readonly ItemType _type;
		private readonly ItemBehaviour _behaviour;
		private readonly int _key;

		public Item
		(
			ItemType _type ,
			ItemBehaviour _behaviour ,
			int _key ,
			int _count
		)
		{
			this._type = _type;
			this._behaviour = _behaviour;
			this._key = _key;
			this.Count = _count;
		}

		public ItemType Type => _type;
		public ItemBehaviour Behaviour => _behaviour;
		public int Key => _key;
		public int Count;
	}
}

