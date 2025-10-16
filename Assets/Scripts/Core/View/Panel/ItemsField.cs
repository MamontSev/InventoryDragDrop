using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Inventory.Data.Config;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Inventory.Core.View.Panel
{
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(GridLayoutGroup))]
	public class ItemsField:MonoBehaviour
	{
		class CellItem
		{
			public Transform Point;
			public int Index;
			public bool IsFree;
		}
		private RectTransform Field => GetComponent<RectTransform>();
		private GridLayoutGroup GridLayout => GetComponent<GridLayoutGroup>();


		[SerializeField]
		private GameObject ItemFonPrefab;

		private readonly List<CellItem> _items = new();


		public async UniTask Init( int _cellCountX , int _cellCountY )
		{
			Vector2 cellSize = GridLayout.cellSize;
			float cpacing = GridLayout.spacing.x;

			float width = (( cellSize.x + cpacing ) * _cellCountX ) + cpacing;
			float height = (( cellSize.y + cpacing ) * _cellCountY ) + cpacing;
			Field.sizeDelta = new Vector2(width , height);


			int k = 0;
			for( int i = 0; i < _cellCountX; i++ )
			{
				for( int j = 0; j < _cellCountY; j++ )
				{
					GameObject fon = GameObject.Instantiate(ItemFonPrefab,transform);
					_items.Add(new CellItem() { Point = fon.transform , Index = k , IsFree = true });
					await UniTask.Delay(50, cancellationToken: destroyCancellationToken);
					k++;
				}
			}

			GridLayout.enabled = false;
		}

		public (Transform point, int fieldIndex) GetFreePoint( )
		{
			var item = _items.First(x => x.IsFree);
			item.IsFree = false;
			return (item.Point,item.Index);
        }

		public (Transform point, int fieldIndex) ReturnPointAndGetNearest( Transform target, int index )
		{
			_items[index].IsFree = true;

			CellItem nearestCell = null;
			float minDist = float.MaxValue;
			foreach( CellItem item in _items )
			{
				if( !item.IsFree )
					continue;
				float dist = Vector3.Distance(target.position , item.Point.position);
				if( dist > minDist )
					continue;
				minDist = dist;
				nearestCell = item;
			}
			
			nearestCell.IsFree = false;
			return (nearestCell.Point, nearestCell.Index);
		}

		public void ReturnPoint( int index ) => _items[index].IsFree = true;

		public void ReturnAllPoints()
		{
			foreach( CellItem item in _items )
				item.IsFree = true;
		}
	}
}
