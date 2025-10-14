using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Inventory.Core.View.Panel
{
	public interface IInventoryView
	{
		void AddItem( Sprite iconSprite , string name , int index , bool needCount , int count, int type , int key );
		void HideUsePanel();
		UniTask InitField( int _cellCountX , int _cellCountY );
		void RemoveItem( int index );
		void SetItemCount( int index , int count );
		void SetItemDescription( string description );
		void SetSelected( int index , bool state );
		void SetUse( int index , bool state );
		void ShowUsePanel( bool isPutOn , bool isIakeOff );
	}
}
