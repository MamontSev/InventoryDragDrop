using UnityEngine;

namespace Inventory.Data.Config
{
	[CreateAssetMenu(menuName = "Data/FieldConfig" , fileName = "FieldConfig.asset")]
	public class FieldConfig:ScriptableObject
	{
		[SerializeField]
		[Range(2,6)]
		private int _cellCountX = 5;
		public int CellCountX => _cellCountX;

		[SerializeField]										      
		[Range(2 , 6)]
		private int _cellCountY = 4;
		public int CellCountY => _cellCountY;
	}
}
