using System;

namespace Inventory.Data.Static
{
	public static class Enums
	{
		public enum ItemBehaviour
		{
			Stackable = 0,
			NonStackable = 1
		}
		public enum ItemType
		{
			Amulet = 0,
			Ring = 1,
			Rune = 2,
			Stone = 3
		}

		public enum Amulet
		{						    
			Amulet1,
			Amulet2,
			Amulet3,
			Amulet4
		}

		public enum Ring
		{
			Ring1,
			Ring2,
			Ring3,
			Ring4
		}
		public enum Rune
		{
			Rune1,
			Rune2,
			Rune3,
			Rune4
		}

		public enum Stone
		{
			Stone1,
			Stone2,
			Stone3,
			Stone4
		}


		public static Type GetItemEnumType( ItemType itemType ) => itemType switch
		{
			ItemType.Amulet => typeof(Amulet),
			ItemType.Ring => typeof(Ring),
			ItemType.Rune => typeof(Rune),
			ItemType.Stone => typeof(Stone),
			_ => throw new NotImplementedException(),
		};

	}
}
